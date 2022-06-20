using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using S3D.UI.OpenTKFramework.Types;
using S3D.UI.MeshUtilities;
using System;
using S3D.UI.MathUtilities.Raycasting;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace S3D.UI.Views {
    public class MainView : View {
        // private readonly FileDialogView _openFileDialogView = new FileDialogView();
        private readonly MainMenuBarView _mainMenuBarView = new MainMenuBarView();
        // private readonly ModelView _modelView = new ModelView();

        private Model _model;
        private ModelRender _modelRender;
        private MeshWireRender _modelWireRender;

        private readonly FlyCamera _flyCamera = new FlyCamera();

        public Action OpenFile { get; set; }

        protected override void OnLoad() {
            string vertexShader = System.IO.File.ReadAllText("Shaders/shader.vert");
            string fragmentShader = System.IO.File.ReadAllText("Shaders/shader.frag");
            Shader shader = new Shader("Model", vertexShader, fragmentShader);

            ProjectManagement.ProjectSettings projectSettings =
                ProjectManagement.ProjectManager.Open("mgl1_settings.json");

            Mesh mesh = S3DMeshGenerator.Generate(projectSettings.Objects[0]);
            Mesh wireMesh = S3DWireMeshGenerator.Generate(projectSettings.Objects[0]);

            ProjectManagement.ProjectManager.Close(projectSettings);

            _model = new Model();
            _model.Meshes = new Mesh[] {
                mesh
            };
            _modelRender = new ModelRender(_model);
            _modelRender.SetShader(shader);

            _modelWireRender = new MeshWireRender(wireMesh);

            _mainMenuBarView.Load();
        }

        protected override void OnUnload() {
        }

        protected override void OnUpdateFrame() {
            if (!Window.IsFocused) {
                return;
            }

            _flyCamera.UpdateFrame();

            // Window.Camera.Position = new Vector3(0.90117437f, 9.167797f, 5.874802f);
            // Console.WriteLine(Window.Camera.Position);

            // Check if fly camera is NOT moving
            if (true && Window.Input.MouseState.IsButtonDown(MouseButton.Button1)) {
                Vector2 origin = new Vector2(Window.Input.MouseState.X,
                                             Window.Input.MouseState.Y);

                // XXX: This should be a collider instance attached in a model
                Mesh mesh = _model.Meshes[0];

                if (Window.Camera.Cast(origin, mesh, out RaycastHitInfo hitInfo)) {
                    Console.WriteLine($"Hit! {hitInfo.TriangleIndex}");

                    int i = (int)hitInfo.TriangleIndex;
                }
            }
        }

        protected override void OnRenderFrame() {
            _mainMenuBarView.RenderFrame();

            _modelRender.Render();
            _modelWireRender.Render();
        }

        private void MenuFileOpen() {
        }
    }
}
