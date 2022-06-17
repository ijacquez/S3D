using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using S3D.UI.OpenTKFramework.Types;
using S3D.UI.MeshUtilities;
using System;

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

        protected override void OnUpdateFrame(FrameEventArgs e) {
            if (!Window.IsFocused) {
                return;
            }

            float dt = (float)e.Time;

            _flyCamera.UpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            _mainMenuBarView.RenderFrame(e);

            float dt = (float)e.Time;

            _modelRender.Render();
            _modelWireRender.Render();
        }

        private void MenuFileOpen() {
        }
    }
}
