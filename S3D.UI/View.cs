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

        public void UpdateFrame() {
            if (IsVisible) {
                OnUpdateFrame();
            }
        }

        public void RenderFrame() {
            if (IsVisible) {
                OnRenderFrame();
            }
        }

        public void ToggleVisibility(bool active) {
            IsVisible = active;
        }

        protected abstract void OnLoad();

        protected abstract void OnUnload();

        protected abstract void OnUpdateFrame();

        protected abstract void OnRenderFrame();
    }
}
