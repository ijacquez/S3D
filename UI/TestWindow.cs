using S3D.UI.Views;
using ImGuiNET;
using Raylib_cs;

namespace S3D.UI {
    public class TestWindow : Window {
        private readonly MainView _mainView = new MainView();

        public override void Init() {
            _mainView.Init();
        }

        public override void Update(float dt) {
            _mainView.Update(dt);
        }

        public override void Draw() {
            Raylib.ClearBackground(Color.SKYBLUE);

            _mainView.Draw();
        }
    }
}
