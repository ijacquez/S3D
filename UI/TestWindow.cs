using ImGuiNET;
using Raylib_cs;

namespace S3D.UI {
    public class TestWindow : Window {
        public override void Init() {
        }

        public override void Update(float dt) {
        }

        public override void Draw() {
            Raylib.ClearBackground(Color.SKYBLUE);
            Raylib.DrawText("Hello, world!", 12, 12, 20, Color.BLACK);

            bool showDemoWindow = true;

            ImGui.ShowDemoWindow(ref showDemoWindow);
        }
    }
}
