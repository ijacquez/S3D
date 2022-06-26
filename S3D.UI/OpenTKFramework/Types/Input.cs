using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace S3D.UI.OpenTKFramework.Types {
    public class Input {
        private readonly NativeWindow _nativeWindow;

        public KeyboardState KeyboardState => _nativeWindow.KeyboardState;

        public MouseState MouseState => _nativeWindow.MouseState;

        public bool IsKeyboardFocused { get; private set; }

        public bool IsMouseFocused { get; private set; }

        private Input() {
        }

        public Input(NativeWindow nativeWindow) {
            _nativeWindow = nativeWindow;
        }

        public void ToggleKeyboardInput(bool active) {
            IsKeyboardFocused = active;
        }

        public void ToggleMouseInput(bool active) {
            IsMouseFocused = active;
        }

        public void ToggleInput(bool active) {
            ToggleKeyboardInput(active);
            ToggleMouseInput(active);
        }
    }
}
