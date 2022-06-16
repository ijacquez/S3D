using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using S3D.UI.ImGuiGlue;
using S3D.UI.OpenTKFramework.Utilities;
using System;

namespace S3D.UI {
    public class Window {
        private readonly GameWindow _gameWindow;

        private ImGuiController _imGuiController;

        private Window() {
        }

        public Window(string title, int width, int height) {
            var nativeWindowSettings = new NativeWindowSettings() {
                Title      = title,
                Size       = new Vector2i(width, height),
                APIVersion = new Version(4, 5),
                Profile    = ContextProfile.Any
            };

            _gameWindow = new GameWindow(GameWindowSettings.Default, nativeWindowSettings);

            _gameWindow.Load += OnLoad;
            _gameWindow.Unload += OnUnload;
            _gameWindow.Resize += OnResize;
            _gameWindow.UpdateFrame += OnUpdateFrame;
            _gameWindow.RenderFrame += OnRenderFrame;
            _gameWindow.TextInput += OnTextInput;
            _gameWindow.MouseWheel += OnMouseWheel;
        }

        public event Action Load;

        public event Action Unload;

        public event Action<FrameEventArgs> RenderFrame;

        public void Run() {
            _gameWindow.Run();
        }

        private void OnLoad() {
            _imGuiController = new ImGuiController(_gameWindow.ClientSize.X, _gameWindow.ClientSize.Y);

            Load?.Invoke();
        }

        private void OnUnload() {
            _imGuiController.Dispose();

            Unload?.Invoke();
        }

        private void OnResize(ResizeEventArgs e) {
            // Update the opengl viewport
            GL.Viewport(0, 0, _gameWindow.ClientSize.X, _gameWindow.ClientSize.Y);

            // Tell ImGui of the new size
            _imGuiController.WindowResized(_gameWindow.ClientSize.X, _gameWindow.ClientSize.Y);
        }

        private void OnUpdateFrame(FrameEventArgs e) {
            _imGuiController.Update(_gameWindow, (float)e.Time);
        }

        private void OnRenderFrame(FrameEventArgs e) {
            GL.ClearColor(new Color4(0, 32, 48, 255));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            RenderFrame?.Invoke(e);

            _imGuiController.Render();

            DebugUtility.CheckGLError("End of frame");

            _gameWindow.SwapBuffers();
        }

        private void OnTextInput(TextInputEventArgs e) {
            _imGuiController.PressChar((char)e.Unicode);
        }

        private void OnMouseWheel(MouseWheelEventArgs e) {
            _imGuiController.MouseScroll(e.Offset);
        }
    }
}
