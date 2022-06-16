using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using S3D.Extensions;
using S3D.FileFormats;
using S3D.UI.OpenTKFramework.Types;
using S3D.UI.Utilities;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;

namespace S3D.UI.Views {
    public class MainView : View {
        // private readonly FileDialogView _openFileDialogView = new FileDialogView();
        private readonly MainMenuBarView _mainMenuBarView = new MainMenuBarView();
        // private readonly ModelView _modelView = new ModelView();

        private Model _model1;
        private ModelRender _modelRender1;

        private Model _model2;
        private ModelRender _modelRender2;

        private Camera _camera;

        public Action OpenFile { get; set; }

        protected override void OnLoad() {
            _camera = new Camera();
            _camera.Fov = 90.0f;
            _camera.DepthNear = 0.01f;
            _camera.DepthFar = 1000.0f;
            _camera.Position = new Vector3(0, 0, 10);

            ModelRender.Camera = _camera;

            string vertexShader = System.IO.File.ReadAllText("Shaders/shader.vert");
            string fragmentShader = System.IO.File.ReadAllText("Shaders/shader.frag");
            Shader shader = new Shader("Foo", vertexShader, fragmentShader);

            ProjectManagement.ProjectSettings projectSettings = ProjectManagement.ProjectManager.Open("mgl1_settings.json");

            Mesh mesh1 = S3DMeshGenerator.GenerateMesh(projectSettings.Objects[0]);

            ProjectManagement.ProjectManager.Close(projectSettings);

            _model1 = new Model();
            _model1.Meshes = new Mesh[] { mesh1 };
            _modelRender1 = new ModelRender(_model1);
            _modelRender1.SetShader(shader);

            _model2 = new Model();
            _model2.Meshes = new Mesh[] { mesh1 };
            _modelRender2 = new ModelRender(_model2);
            _modelRender2.SetShader(shader);

            _mainMenuBarView.Load();
        }

        protected override void OnUnload() {
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            _mainMenuBarView.RenderFrame(e);

            float dt = (float)e.Time;

            // _model1.Transform = _model1.Transform *
            //     Matrix4.CreateRotationZ(dt * MathHelper.DegreesToRadians(20.0f)) *
            //     Matrix4.CreateRotationY(dt * MathHelper.DegreesToRadians(20.0f)) *
            //     Matrix4.CreateRotationX(dt * MathHelper.DegreesToRadians(20.0f));
            // _model1.Transform = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(180.0f)) * Matrix4.CreateTranslation(0, -3, -3);
            _modelRender1.Render();

            // _model2.Transform = _model2.Transform * Matrix4.CreateTranslation(dt * 0.5f, 0.0f, 0.0f);
            // _model2.Transform = _model2.Transform * Matrix4.CreateRotationZ(dt * MathHelper.DegreesToRadians(-20.0f));
            // _modelRender2.Render();
        }

        private void MenuFileOpen() {
        }
    }
}
