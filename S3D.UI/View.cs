using OpenTK.Windowing.Common;

namespace S3D.UI {
    public abstract class View {
        public bool IsVisible { get; private set; } = true;

        public View() {
        }

        public void Load() {
            OnLoad();
        }

        public void Unload() {
            OnUnload();
        }

        public void UpdateFrame(FrameEventArgs e) {
            if (IsVisible) {
                OnUpdateFrame(e);
            }
        }

        public void RenderFrame(FrameEventArgs e) {
            if (IsVisible) {
                OnRenderFrame(e);
            }
        }

        public void ToggleVisibility(bool active) {
            IsVisible = active;
        }

        protected abstract void OnLoad();

        protected abstract void OnUnload();

        protected abstract void OnUpdateFrame(FrameEventArgs e);

        protected abstract void OnRenderFrame(FrameEventArgs e);
    }
}
