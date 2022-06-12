namespace S3D.UI {
    public abstract class View : Window {
        public bool IsShowing { get; private set; } = true;

        public sealed override void Init() {
            ViewInit();
        }

        public sealed override void Update(float dt) {
            if (IsShowing) {
                ViewUpdate(dt);
            }
        }

        public sealed override void Draw() {
            if (IsShowing) {
                ViewDraw();
            }
        }

        public void Show() {
            IsShowing = true;
        }

        public void Hide() {
            IsShowing = false;
        }

        protected abstract void ViewInit();

        protected abstract void ViewUpdate(float dt);

        protected abstract void ViewDraw();
    }
}
