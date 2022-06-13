using System;
using ImGuiNET;

namespace S3D.UI.Views {
    public class MainMenuBarView : View {
        public Action MenuFileOpen { get; set; }

        public Action MenuFileSave { get; set; }

        public Action MenuFileQuit { get; set; }

        public Action MenuAbout { get; set; }

        protected override void ViewInit() {
        }

        protected override void ViewUpdate(float dt) {
            if (ImGui.BeginMainMenuBar()) {
                if (ImGui.BeginMenu("File")) {
                    if (ImGui.MenuItem("Open", "Ctrl+O")) {
                        MenuFileOpen?.Invoke();
                    }

                    if (ImGui.MenuItem("Save", "Ctrl+O")) {
                        MenuFileSave?.Invoke();
                    }

                    if (ImGui.MenuItem("Quit", "Alt+F4")) {
                        MenuFileQuit?.Invoke();
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("About")) {
                    if (ImGui.MenuItem("About S3D")) {
                        MenuAbout?.Invoke();
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }

        protected override void ViewDraw() {
        }
    }
}