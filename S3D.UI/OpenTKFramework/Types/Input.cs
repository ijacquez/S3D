using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace S3D.UI.OpenTKFramework.Types {
    public class Input {
        private NativeWindow NativeWindow { get; }

        public KeyboardState KeyboardState => NativeWindow.KeyboardState;

        public MouseState MouseState => NativeWindow.MouseState;

        private Input() {
        }

        public Input(NativeWindow nativeWindow) {
            NativeWindow = nativeWindow;
        }
    }
}
