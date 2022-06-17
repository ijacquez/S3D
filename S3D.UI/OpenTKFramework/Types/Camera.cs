using OpenTK.Mathematics;
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

        public Vector3 Front { get; private set; } = -Vector3.UnitZ;

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
            return Matrix4.LookAt(Position, Position + Front, Up);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix() {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, DepthNear, DepthFar);
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
            Front = Vector3.Normalize(front);

            // Calculate both the right and the up vector using cross product.
            // Note that we are calculating the right from the global up; this
            // behaviour might not be what you need for all cameras so keep this
            // in mind if you do not want a FPS camera
            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}
