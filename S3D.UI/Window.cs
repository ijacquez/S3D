using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using S3D.UI.ImGuiGlue;
using S3D.UI.OpenTKFramework.Types;
using S3D.UI.OpenTKFramework.Utilities;
using System;

namespace S3D.UI {
    public static class Window {
        private static readonly Color4 _ClearColor = new Color4(0, 32, 48, 255);

        private static readonly ClearBufferMask _ClearBufferMask =
            ClearBufferMask.ColorBufferBit |
            ClearBufferMask.DepthBufferBit |
            ClearBufferMask.StencilBufferBit;

        public static Camera Camera { get; } = new Camera();

        public static Input Input { get; }

        public static string Title {
            get => _GameWindow.Title;
            set => _GameWindow.Title = value;
        }

        public static bool IsFocused => _GameWindow.IsFocused;

        public static bool IsVisible => _GameWindow.IsVisible;

        public static bool IsCursorGrabbed =>
            (_GameWindow.CursorState == CursorState.Grabbed);

        /// <summary>
        ///   Size of this window.
        /// </summary>
        public static Vector2 ClientSize => _GameWindow.ClientSize;

        private static readonly GameWindow _GameWindow;

        // XXX: Move!
        private static readonly ImGuiController _imGuiController;

        static Window() {
            var nativeWindowSettings = new NativeWindowSettings() {
                APIVersion = new Version(4, 5),
                Profile    = ContextProfile.Any
            };

            _GameWindow = new GameWindow(GameWindowSettings.Default, nativeWindowSettings);

            _GameWindow.Load += OnLoad;
            _GameWindow.Unload += OnUnload;
            _GameWindow.Resize += OnResize;
            _GameWindow.UpdateFrame += OnUpdateFrame;
            _GameWindow.RenderFrame += OnRenderFrame;
            _GameWindow.TextInput += OnTextInput;
            _GameWindow.MouseWheel += OnMouseWheel;

            // XXX: Move ImGui related stuff to its own view. This one requires
            //      that we supply the "client size"... how?
            _imGuiController = new ImGuiController(_GameWindow.ClientSize.X, _GameWindow.ClientSize.Y);

            Input = new Input(_GameWindow);
        }

        public static event Action Load;

        public static event Action Unload;

        public static event Action<FrameEventArgs> UpdateFrame;

        public static event Action<FrameEventArgs> RenderFrame;

        public static void Run() {
            _GameWindow.Run();
        }

        public static void GrabCursor() {
            _GameWindow.CursorState = CursorState.Grabbed;
        }

        public static void ReleaseCursor() {
            _GameWindow.CursorState = CursorState.Normal;
        }

        private static void OnLoad() {
            Load?.Invoke();
        }

        private static void OnUnload() {
            // XXX: Move ImGui related stuff to its own view
            _imGuiController.Dispose();

            Unload?.Invoke();
        }

        private static void OnResize(ResizeEventArgs e) {
            // Update the opengl viewport
            GL.Viewport(x: 0, y: 0, _GameWindow.ClientSize.X, _GameWindow.ClientSize.Y);

            // XXX: Move ImGui related stuff to its own view Tell ImGui of the
            //      new size
            _imGuiController.Resize(_GameWindow.ClientSize.X, _GameWindow.ClientSize.Y);
        }

        private static void OnUpdateFrame(FrameEventArgs e) {
            _imGuiController.Update(_GameWindow, (float)e.Time);

            UpdateFrame?.Invoke(e);
        }

        private static void OnRenderFrame(FrameEventArgs e) {
            GL.ClearColor(_ClearColor);
            GL.Clear(_ClearBufferMask);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            RenderFrame?.Invoke(e);

            // XXX: Move ImGui related stuff to its own view
            _imGuiController.Render();

            DebugUtility.CheckGLError("End of frame");

            _GameWindow.SwapBuffers();
        }

        private static void OnTextInput(TextInputEventArgs e) {
            // XXX: Move ImGui related stuff to its own view
            _imGuiController.PressChar((char)e.Unicode);
        }

        private static void OnMouseWheel(MouseWheelEventArgs e) {
            // XXX: Move ImGui related stuff to its own view
            _imGuiController.MouseScroll(e.Offset);
        }
    }
}
