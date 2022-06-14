using OpenTK.Windowing.Common;

namespace S3D.UI {
    public abstract class View {
        public bool IsShowing { get; private set; } = true;

        public void Load() {
            OnLoad();
        }

        public void Unload() {
            OnUnload();
        }

        public void RenderFrame(FrameEventArgs e) {
            if (IsShowing) {
                OnRenderFrame(e);
            }
        }

        public void Show() {
            IsShowing = true;
        }

        public void Hide() {
            IsShowing = false;
        }

        protected abstract void OnLoad();

        protected abstract void OnUnload();

        protected abstract void OnRenderFrame(FrameEventArgs e);
    }
}
