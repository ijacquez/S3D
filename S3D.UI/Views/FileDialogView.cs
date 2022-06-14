using ImGuiNET;
using OpenTK.Windowing.Common;

namespace S3D.UI.Views {
    public class FileDialogView : View {
        protected override void OnLoad() {
        }

        protected override void OnUnload() {
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            ImGui.Button("Hello");
        }
    }
}
