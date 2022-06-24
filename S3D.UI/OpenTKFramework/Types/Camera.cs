using OpenTK.Mathematics;
using S3D.UI.MathUtilities.Raycasting;
using S3D.UI.MathUtilities;
using System.Collections.Generic;
using System;

namespace S3D.UI.OpenTKFramework.Types {
    public class Camera {
        private sealed class RaycastHitInfoComparer : Comparer<RaycastHitInfo> {
            public override int Compare(RaycastHitInfo x, RaycastHitInfo y) {
                if (MathHelper.ApproximatelyEquivalent(x.Distance, y.Distance, _Threshold)) {
                    return 0;
                }

                return Math.Sign((x.Distance - y.Distance));
            }
        }

        // XXX: Move this
        private const float _Threshold = 0.001f;

        // Rotation around the X axis (radians)
        private float _pitch;

        // Rotation around the Y axis (radians). Without this, you would be
        // started rotated 90 degrees right
        private float _yaw = -MathHelper.PiOver2;

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;

        public Camera() {
            UpdateVectors();
        }

        public float DepthNear { get; set; }= 0.01f;

        public float DepthFar { get; set; } = 1000.0f;

        // The position of the camera
        public Vector3 Position { get; set; } = Vector3.Zero;

        // This is simply the aspect ratio of the viewport, used for the
        // projection matrix
        public float AspectRatio { private get; set; } = 1.7777f;

        public Vector3 Forward { get; private set; } = -Vector3.UnitZ;

        public Vector3 Up { get; private set; } = Vector3.UnitY;

        public Vector3 Right { get; private set; } = Vector3.UnitX;

        // We convert from degrees to radians as soon as the property is set to improve performance.
        public float Pitch {
            get => MathHelper.RadiansToDegrees(_pitch);

            set {
                // We clamp the pitch value between -89 and 89 to prevent the
                // camera from going upside down, and a bunch of weird "bugs"
                // when you are using euler angles for rotation. If you want to
                // read more about this you can try researching a topic called
                // gimbal lock
                var angle = MathHelper.Clamp(value, -89.9f, 89.9f);

                _pitch = MathHelper.DegreesToRadians(angle);

                UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to
        // improve performance
        public float Yaw {
            get => MathHelper.RadiansToDegrees(_yaw);

            set {
                _yaw = MathHelper.DegreesToRadians(value);

                UpdateVectors();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view.
        // This has been discussed more in depth in a previous tutorial, but in
        // this tutorial, you have also learned how we can use this to simulate
        // a zoom feature. We convert from degrees to radians as soon as the
        // property is set to improve performance
        public float Fov {
            get => MathHelper.RadiansToDegrees(_fov);

            set {
                var angle = MathHelper.Clamp(value, 1.0f, 90.0f);

                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        // Get the view matrix using the amazing LookAt function described more
        // in depth on the web tutorials
        public Matrix4 GetViewMatrix() {
            return Matrix4.LookAt(Position, Position + Forward, Up);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix() {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, DepthNear, DepthFar);
        }

        public Vector3 ConvertScreenToWorldspace(Vector2 screenPoint) {
            Vector3 ndcPoint = ConvertScreenToViewport(screenPoint);

            // XXX: Cache this iff view/projection matrices change
            var viewMatrix = GetViewMatrix();
            var projectionMatrix = GetProjectionMatrix();
            var mvp = projectionMatrix * viewMatrix;

            return Vector3.TransformPosition(ndcPoint, mvp.Inverted());
        }

        public Vector3 ConvertScreenToViewport(Vector2 screenPoint) {
            Vector2 midPoint = (2.0f * screenPoint) / Window.ClientSize;
            // Note the negation on Y-axis
            // Z-axis is -1 for near plane in NDC space
            return new Vector3(midPoint.X - 1.0f, 1.0f - midPoint.Y, -1.0f);
        }

        public bool Cast(Vector2 screenOrigin, Mesh mesh, out RaycastHitInfo hitInfo) {
            Vector3 viewportOrigin = Window.Camera.ConvertScreenToViewport(screenOrigin);

            return Cast(viewportOrigin, mesh, out hitInfo);
        }

        public bool Cast(Vector3 viewportPoint, Mesh mesh, out RaycastHitInfo hitInfo) {
            hitInfo = default(RaycastHitInfo);

            var viewMatrix = GetViewMatrix();
            var projectionMatrix = GetProjectionMatrix();

            // Convert the origin point to a direction (into the viewport: <0,0,1>) then
            // transform the ray from viewport to world space

            // From viewport to clip space
            Vector3 clipPointZ = new Vector3(viewportPoint.X, viewportPoint.Y, -1.0f);
            // From clip to view space
            Vector3 viewPoint3 = Vector3.TransformVector(clipPointZ, projectionMatrix.Inverted());
            // From view to world space
            Vector3 viewPointZ = new Vector3(viewPoint3.X, viewPoint3.Y, -1.0f);

            Vector3 worldPoint = Position;
            Vector3 worldDirection = Vector3.Normalize(Vector3.TransformVector(viewPointZ, viewMatrix.Inverted()));

            List<RaycastHitInfo> raycastHitInfos = new List<RaycastHitInfo>();

            for (int i = 0; i < mesh.PrimitiveCount; i++) {
                MeshPrimitive meshPrimitive = mesh.Primitives[i];

                for (int t = 0; t < meshPrimitive.Triangles.Length; t++) {
                    var vertices = meshPrimitive.Triangles[t].Vertices;

                    // Bring the triangle into world space
                    Vector3 p0 = vertices[0];
                    Vector3 p1 = vertices[1];
                    Vector3 p2 = vertices[2];

                    float nd = Vector3.Dot(meshPrimitive.Normal, worldDirection);

                    // Test if ray is parallel to triangle normal
                    if (MathF.Abs(nd) < _Threshold) {
                        break;
                    }

                    // Find t0
                    float d = -Vector3.Dot(meshPrimitive.Normal, p0);
                    float t0 = -(Vector3.Dot(meshPrimitive.Normal, worldPoint) + d) / nd;

                    if (t0 < 0.0f) {
                        break;
                    }

                    Vector3 point = worldPoint + (t0 * worldDirection);

                    if (Triangle.PointInTriangle(point, meshPrimitive.Normal, p0, p1, p2)) {
                        var raycastHitInfo = new RaycastHitInfo();

                        raycastHitInfo.PrimitiveIndex = (uint)i;

                        raycastHitInfo.Point = point;
                        raycastHitInfo.Distance = t0;

                        raycastHitInfos.Add(raycastHitInfo);
                    }
                }
            }

            if (raycastHitInfos.Count == 0) {
                return false;
            }

            raycastHitInfos.Sort(new RaycastHitInfoComparer());

            hitInfo = raycastHitInfos[0];

            return true;
        }

        private void UpdateVectors() {
            // First, the forward matrix is calculated using some basic trigonometry
            Vector3 forward = new Vector3() {
                X = MathF.Cos(_pitch) * MathF.Cos(_yaw),
                Y = MathF.Sin(_pitch),
                Z = MathF.Cos(_pitch) * MathF.Sin(_yaw)
            };

            // We need to make sure the vectors are all normalized, as otherwise
            // we would get some funky results
            Forward = Vector3.Normalize(forward);

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this
            // behaviour might not be what you need for all cameras so keep this
            // in mind if you do not want a FPS camera
            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
        }
    }
}
