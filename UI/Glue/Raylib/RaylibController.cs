using S3D.UI.Glue.ImGui;

namespace S3D.UI.Glue.Raylib {
    using Raylib_cs;

    public class RaylibController {
        public int WindowWidth { get; }

        public int WindowHeight { get; }

        public string WindowTitle { get; }

        private RaylibController() {
        }

        public RaylibController(string windowTitle, int windowWidth, int windowHeight) {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            WindowTitle = windowTitle?.Trim();
        }

        public unsafe void Run(Window window) {
            Raylib.SetTraceLogCallback(&Logging.LogConsole);
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(WindowWidth, WindowHeight, WindowTitle);
            Raylib.SetTargetFPS(60);

            Raylib.InitAudioDevice();

            ImGuiController controller = new ImGuiController();

            controller.Load(WindowWidth, WindowHeight);
            window.Init();

            while (!Raylib.WindowShouldClose()) {
                // Update
                float dt = Raylib.GetFrameTime();

                // Feed the input events to our ImGui controller, which passes
                // them through to ImGui
                controller.Update(dt);
                window.Update(dt);

                // Draw
                Raylib.BeginDrawing(); {
                    window.Draw();
                    controller.Draw();
                } Raylib.EndDrawing();
            }

            // XXX: editor.Unload();
            controller.Dispose();
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }
    }
}
