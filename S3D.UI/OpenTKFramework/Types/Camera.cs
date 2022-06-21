using OpenTK.Mathematics;
using S3D.UI.MathUtilities.Raycasting;
using S3D.UI.MathUtilities;
using System.Collections.Generic;
using System;

namespace S3D.UI.OpenTKFramework.Types {
    public class Camera {
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

        public float DepthFar { get; set; } = 100.0f;

        // The position of the camera
        public Vector3 Position { get; set; }

        // This is simply the aspect ratio of the viewport, used for the
        // projection matrix
        public float AspectRatio { private get; set; } = 1.667f;

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
                var angle = MathHelper.Clamp(value, -89.0f, 89.0f);

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

        public Vector3 ScreenToWorldspace(Vector2 screenPoint) {
            Vector4 ndcPoint = new Vector4(new Vector3(screenPoint / Window.ClientSize), 1.0f);

            var projectionMatrix = GetProjectionMatrix().Inverted();
            var viewMatrix = GetViewMatrix().Inverted();

            var wordspacePoint = ndcPoint * projectionMatrix * viewMatrix;

            return new Vector3(wordspacePoint);
        }

        private class HitPrimitive {
            public Vector3 Point { get; set; }

            public uint PrimitiveIndex { get; set; }

            public Vector3[] TransformedVertices { get; } = new Vector3[3];
        }

        public bool Cast(Vector2 origin, Mesh mesh, out RaycastHitInfo hitInfo) {
            hitInfo = default(RaycastHitInfo);

            Vector2 windowHalfDim = 0.5f * Window.ClientSize;

            Matrix4 viewMatrix = GetViewMatrix();
            Matrix4 projectionMatrix = GetProjectionMatrix();
            Matrix4 mvp = viewMatrix * projectionMatrix;

            List<HitPrimitive> hitPrimitives = new List<HitPrimitive>();
            // float closestZ = float.PositiveInfinity;
            // uint closestIndex = uint.MaxValue;

            Console.WriteLine("[H[2J");

            for (int i = 0; i < mesh.PrimitiveCount; i++) {
                MeshPrimitive meshPrimitive = mesh.Primitives[i];

                for (int t = 0; t < meshPrimitive.Triangles.Length; t++) {
                    var vertices = meshPrimitive.Triangles[t].Vertices;

                    // Bring the triangle into clip space
                    Vector3 tp0 = Vector3.TransformPerspective(vertices[0], mvp);
                    Vector3 tp1 = Vector3.TransformPerspective(vertices[1], mvp);
                    Vector3 tp2 = Vector3.TransformPerspective(vertices[2], mvp);

                    // Transform to screen space
                    Vector2 wctp0 = windowHalfDim * (tp0.Xy + Vector2.One);
                    Vector2 wctp1 = windowHalfDim * (tp1.Xy + Vector2.One);
                    Vector2 wctp2 = windowHalfDim * (tp2.Xy + Vector2.One);

                    // Flip (+Y is down)
                    wctp0.Y = Window.ClientSize.Y - wctp0.Y;
                    wctp1.Y = Window.ClientSize.Y - wctp1.Y;
                    wctp2.Y = Window.ClientSize.Y - wctp2.Y;

                    if (Triangle.PointInTriangle(origin, wctp0, wctp1, wctp2)) {

                        Vector3 tn = Vector3.TransformNormal(meshPrimitive.Normal, viewMatrix);
                        Vector3 mp1 = Vector3.TransformVector(vertices[0], viewMatrix);

                        float t0 = Vector3.Dot(tn, mp1 - Position) / Vector3.Dot(tn, Forward);
                        Vector3 point = Position + (t0 * Forward);

                        if (!float.IsNaN(point.X) && !float.IsNaN(point.Y) && !float.IsNaN(point.Z)) {
                            var hitPrimitive = new HitPrimitive();

                            hitPrimitive.TransformedVertices[0] = tp0;
                            hitPrimitive.TransformedVertices[1] = tp1;
                            hitPrimitive.TransformedVertices[2] = tp2;

                            hitPrimitive.PrimitiveIndex = (uint)i;

                            hitPrimitive.Point = point;

                            hitPrimitives.Add(hitPrimitive);
                        }
                    }
                }
            }

            hitPrimitives.Sort(new PointComparer());

            foreach (var hitPrimitive in hitPrimitives) {
                Console.WriteLine($"{hitPrimitive.PrimitiveIndex}, {hitPrimitive.Point}");
            }

            if (hitPrimitives.Count == 0) {
                return false;
            }

            hitInfo.PrimitiveIndex = hitPrimitives[0].PrimitiveIndex;

            return true;
        }

        private sealed class PointComparer : Comparer<HitPrimitive> {
            public override int Compare(HitPrimitive x, HitPrimitive y) {
                Vector3 xp = x.Point - Window.Camera.Position;
                Vector3 yp = y.Point - Window.Camera.Position;

                float diff = xp.Length - yp.Length;

                return (diff < 0.0f) ? -1 : ((diff < 0.001f) ? 0 : 1);
                // return (diff < 0.0f) ? 1 : ((diff > 0.001f) ? -1 : 0);
            }
        }

        private void UpdateVectors() {
            // First, the front matrix is calculated using some basic trigonometry
            Vector3 front = new Vector3() {
                X = MathF.Cos(_pitch) * MathF.Cos(Yaw),
                Y = MathF.Sin(_pitch),
                Z = MathF.Cos(_pitch) * MathF.Sin(Yaw)
            };

            // We need to make sure the vectors are all normalized, as otherwise
            // we would get some funky results
            Forward = Vector3.Normalize(front);

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this
            // behaviour might not be what you need for all cameras so keep this
            // in mind if you do not want a FPS camera
            Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
        }
    }
}
