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

        public bool Cast(Vector2 origin, ICollider collider, out RaycastHitInfo hitInfo) {
            hitInfo = default(RaycastHitInfo);

            Vector2 windowHalfDim = 0.5f * Window.ClientSize;

            Matrix4 viewMatrix = GetViewMatrix();
            Matrix4 projectionMatrix = GetProjectionMatrix();
            Matrix4 mvp = viewMatrix * projectionMatrix;

            // List<uint> hitIndices = new List<uint>();

            float closestZ = float.PositiveInfinity;
            uint closestIndex = uint.MaxValue;

            Console.WriteLine("[H[2J");
            for (int i = 0; i < (collider.Vertices.Length / 3); i++) {
                Vector4 p1 = new Vector4(collider.Vertices[(i * 3) + 0], 1.0f);
                Vector4 p2 = new Vector4(collider.Vertices[(i * 3) + 1], 1.0f);
                Vector4 p3 = new Vector4(collider.Vertices[(i * 3) + 2], 1.0f);

                // Bring the triangle into clip space
                Vector4 tp1 = p1 * mvp;
                Vector4 tp2 = p2 * mvp;
                Vector4 tp3 = p3 * mvp;

                // Transform to NDC space
                Vector2 ndctp1 = tp1.Xy / tp1.W;
                Vector2 ndctp2 = tp2.Xy / tp2.W;
                Vector2 ndctp3 = tp3.Xy / tp3.W;

                // Transform to screen space
                Vector2 wctp1 = windowHalfDim * (ndctp1 + Vector2.One);
                Vector2 wctp2 = windowHalfDim * (ndctp2 + Vector2.One);
                Vector2 wctp3 = windowHalfDim * (ndctp3 + Vector2.One);

                // Flip (+Y is down)
                wctp1.Y = Window.ClientSize.Y - wctp1.Y;
                wctp2.Y = Window.ClientSize.Y - wctp2.Y;
                wctp3.Y = Window.ClientSize.Y - wctp3.Y;

                // -> Caveat: Need model matrix ---------------> Create class that contains Mesh, and have model just create the collider

                if (Triangle.PointInTriangle(origin, wctp1, wctp2, wctp3)) {
                    Vector4 n = new Vector4(collider.Normals[i]);
                    Vector4 tn = n * viewMatrix;

                    Vector4 mp1 = p1 * viewMatrix;

                    float t = Vector3.Dot(tn.Xyz, mp1.Xyz - Position) / Vector3.Dot(tn.Xyz, Forward);
                    Vector3 point = Position + (t * Forward);

                    Console.WriteLine($"{i}, {t}, {Forward}, {tn}, {point}");

                    // Console.WriteLine($"{origin}, {wctp1}, {wctp2}, {wctp3}");
                    //
                    // hitInfo.Collider = collider;
                    // hitInfo.TriangleIndex = (uint)i;
                    //
                    // hitIndices.Add((uint)i);
                    // return false;
                }
            }

            // if (hitIndices.Count == 0) {
            //     return false;
            // }

            // float closestZ = float.PositiveInfinity;
            // uint closestIndex = uint.MaxValue;

            // foreach (uint hitIndex in hitIndices) {
            //     if (
            // }

            // viewMatrix.ExtractTranslation

            // hitInfo.Collider = collider;
            // hitInfo.TriangleIndex = closestIndex;

            return false;
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
