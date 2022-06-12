using System;
using ImGuiNET;
using Raylib_cs;

namespace S3D.UI.Views {
    public class MainView : View {
        private readonly FileDialogView _openFileDialogView = new FileDialogView();
        private readonly MainMenuBarView _mainMenuBarView = new MainMenuBarView();
        private readonly ModelView _modelView = new ModelView();

        public Action OpenFile { get; set; }

        protected override void ViewInit() {
            _mainMenuBarView.Init();

            _openFileDialogView.Init();
            _openFileDialogView.Hide();

            _modelView.Init();
            _modelView.Show();

            _mainMenuBarView.MenuFileOpen -= MenuFileOpen;
            _mainMenuBarView.MenuFileOpen += MenuFileOpen;
            // _mainMenuBarView.MenuFileQuit += delegate { Console.Write("Quit"); };
        }

        protected override void ViewUpdate(float dt) {
            _mainMenuBarView.Update(dt);
            _openFileDialogView.Update(dt);
            _modelView.Update(dt);
        }

        protected override void ViewDraw() {
            _mainMenuBarView.Draw();
            _modelView.Draw();
        }

        private void MenuFileOpen() {
            _openFileDialogView.Show();
        }
    }
}
