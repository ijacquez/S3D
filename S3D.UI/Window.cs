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

        // XXX: Move! Why?
        private static readonly ImGuiController _ImGuiController;

        static Window() {
            var nativeWindowSettings = new NativeWindowSettings() {
                APIVersion   = new Version(4, 5),
                Profile      = ContextProfile.Any,
                StartVisible = false
            };

            _GameWindow = new GameWindow(GameWindowSettings.Default, nativeWindowSettings);

            // XXX: Move ImGui related stuff to its own view. This one requires
            //      that we supply the "client size"... how?
            _ImGuiController = new ImGuiController(_GameWindow.ClientSize.X, _GameWindow.ClientSize.Y);

            _ImGuiController.Focus += OnImGuiFocus;

            Input = new Input(_GameWindow);

            _GameWindow.Load += OnLoad;
            _GameWindow.Unload += OnUnload;
            _GameWindow.Resize += OnResize;
            _GameWindow.UpdateFrame += OnUpdateFrame;
            _GameWindow.RenderFrame += OnRenderFrame;
            _GameWindow.TextInput += OnTextInput;
            _GameWindow.MouseWheel += OnMouseWheel;
        }

        public static event Action Load;

        public static event Action Unload;

        public static event Action UpdateFrame;

        public static event Action RenderFrame;

        public static event Action Resize;

        /// <summary>
        /// </summary>
        public static void Run() {
            _GameWindow.IsVisible = true;

            _GameWindow.Run();
        }

        /// <summary>
        /// </summary>
        public static void GrabCursor() {
            _GameWindow.CursorState = CursorState.Grabbed;
        }

        /// <summary>
        /// </summary>
        public static void ReleaseCursor() {
            _GameWindow.CursorState = CursorState.Normal;
        }

        /// <summary>
        /// </summary>
        public static void SetSize(int width, int height) {
            _GameWindow.Size = new Vector2i(width, height);
        }

        private static void OnLoad() {
            Load?.Invoke();
        }

        private static void OnUnload() {
            // XXX: Move ImGui related stuff to its own view
            _ImGuiController.Dispose();

            Unload?.Invoke();
        }

        private static void OnResize(ResizeEventArgs e) {
            // Update the opengl viewport
            GL.Viewport(x: 0, y: 0, _GameWindow.ClientSize.X, _GameWindow.ClientSize.Y);

            // XXX: Move ImGui related stuff to its own view Tell ImGui of the
            //      new size
            _ImGuiController.Resize(_GameWindow.ClientSize.X, _GameWindow.ClientSize.Y);

            Resize?.Invoke();
        }

        private static void OnUpdateFrame(FrameEventArgs e) {
            Time.UpdateFrame(e);

            _ImGuiController.Update(_GameWindow, Time.DeltaTime);

            UpdateFrame?.Invoke();
        }

        private static void OnRenderFrame(FrameEventArgs e) {
            GL.ClearColor(_ClearColor);
            GL.Clear(_ClearBufferMask);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            RenderFrame?.Invoke();

            // XXX: Move ImGui related stuff to its own view
            _ImGuiController.Render();

            DebugUtility.CheckGLError("End of frame");

            _GameWindow.SwapBuffers();
        }

        private static void OnTextInput(TextInputEventArgs e) {
            // XXX: Move ImGui related stuff to its own view
            _ImGuiController.PressChar((char)e.Unicode);
        }

        private static void OnMouseWheel(MouseWheelEventArgs e) {
            // XXX: Move ImGui related stuff to its own view
            _ImGuiController.MouseScroll(e.Offset);
        }

        private static void OnImGuiFocus(object sender, ImGuiFocusEventArgs e) {
            Input.ToggleInput(!e.IsFocused);
        }
    }
}
