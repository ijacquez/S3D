using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using S3D.UI.OpenTKFramework.Types;

namespace S3D.UI {
    public class FlyCamera {
        // XXX: Move these to a S3DSettings file
        private const float CameraSpeed           = 1.5f;
        private const float PitchMouseSensitivity = 20.0f;
        private const float YawMouseSensitivity   = 0.2f;

        private bool _mouseFirstMove;

        public void UpdateFrame() {
            var keyboardState = Window.Input.KeyboardState;
            var mouseState = Window.Input.MouseState;

            if (mouseState.IsButtonDown(MouseButton.Button2)) {
                Window.GrabCursor();

                if (keyboardState.IsKeyDown(Keys.W)) {
                    Window.Camera.Position += Window.Camera.Forward * CameraSpeed * Time.DeltaTime; // Forward
                }

                if (keyboardState.IsKeyDown(Keys.S)) {
                    Window.Camera.Position -= Window.Camera.Forward * CameraSpeed * Time.DeltaTime; // Backwards
                }

                if (keyboardState.IsKeyDown(Keys.A)) {
                    Window.Camera.Position -= Window.Camera.Right * CameraSpeed * Time.DeltaTime; // Left
                }

                if (keyboardState.IsKeyDown(Keys.D)) {
                    Window.Camera.Position += Window.Camera.Right * CameraSpeed * Time.DeltaTime; // Right
                }

                if (keyboardState.IsKeyDown(Keys.Space)) {
                    Window.Camera.Position += Window.Camera.Up * CameraSpeed * Time.DeltaTime; // Up
                }

                if (keyboardState.IsKeyDown(Keys.LeftShift)) {
                    Window.Camera.Position -= Window.Camera.Up * CameraSpeed * Time.DeltaTime; // Down
                }

                // Calculate the offset of the mouseState position
                // var deltaX = mouseState.X - _mouseLastPosition.X;
                // var deltaY = mouseState.Y - _mouseLastPosition.Y;
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                Vector2 delta = mouseState.Delta;
                Vector2 clientSize = Window.ClientSize;

                if (!_mouseFirstMove) {
                    _mouseFirstMove = true;

                    delta = Vector2.Zero;
                }

                float yaw = (delta.X / clientSize.X) * YawMouseSensitivity * 90.0f;
                float pitch = (-delta.Y / clientSize.Y) * PitchMouseSensitivity * 90.0f;

                Window.Camera.Yaw += Time.DeltaTime * yaw;
                Window.Camera.Pitch += Time.DeltaTime * pitch;
            } else {
                Window.ReleaseCursor();
            }
        }
    }
}
