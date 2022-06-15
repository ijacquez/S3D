using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using S3D.UI.OpenTKFramework.Types;
using System.Drawing;
using System;
using System.Collections.Generic;
using S3D.FileFormats;
using System.Linq;

namespace S3D.UI.Views {
    public class MainView : View {
        private readonly FileDialogView _openFileDialogView = new FileDialogView();
        private readonly MainMenuBarView _mainMenuBarView = new MainMenuBarView();
        // private readonly ModelView _modelView = new ModelView();

        private Model _model1;
        private ModelRender _modelRender1;

        private Model _model2;
        private ModelRender _modelRender2;

        private Camera _camera;

        public Action OpenFile { get; set; }

        float[] _vertices = {
            -0.5f, -0.5f, 0.0f, // Bottom-left vertex
             0.5f, -0.5f, 0.0f, // Bottom-right vertex
             0.0f,  0.5f, 0.0f  // Top vertex
        };

        uint[] _indices = {
            0, 1, 2
        };
        protected override void OnLoad() {
            _camera = new Camera();

            _camera.Fov = 90.0f;
            _camera.DepthNear = 0.01f;
            _camera.DepthFar = 1000.0f;
            _camera.Position = new Vector3(0, 10, 25);

            string vertexShader = System.IO.File.ReadAllText("Shaders/shader.vert");
            string fragmentShader = System.IO.File.ReadAllText("Shaders/shader.frag");

            Shader shader = new Shader("Foo", vertexShader, fragmentShader);

            ModelRender.Camera = _camera;

            Mesh mesh = new Mesh();

            ProjectManagement.ProjectSettings projectSettings = ProjectManagement.ProjectManager.Open("mgl1_settings.json");

            List<float> vertices = new List<float>();

            // var v = new List<System.Numerics.Vector3>();
            // for (int i = 0; i < _vertices.Length; i += 3) {
            //     v.Add(new System.Numerics.Vector3(_vertices[i],
            //                                       _vertices[i + 1],
            //                                       _vertices[i + 2]));
            // }

            foreach (System.Numerics.Vector3 vertex in projectSettings.Objects[0].Vertices) {
                Console.WriteLine(vertex / 10f);

                vertices.Add(vertex.X / 10f);
                vertices.Add(-vertex.Y / 10f);
                vertices.Add(vertex.Z / 10f);
            }

            Console.WriteLine(projectSettings.Objects[0].Vertices.Count);
            Console.WriteLine(vertices.Count / 3);

            List<uint> indices = new List<uint>();

            // List<uint> i2 = _indices.ToList();

            foreach (S3DFace face in projectSettings.Objects[0].Faces) {
            // foreach(uint ii in i2) {
            //     indices.Add(ii);

                Console.WriteLine(string.Join(", ", face.Indices));
                indices.Add(face.Indices[0]);
                indices.Add(face.Indices[1]);
                indices.Add(face.Indices[2]);

                indices.Add(face.Indices[0]);
                indices.Add(face.Indices[2]);
                indices.Add(face.Indices[3]);
            }

            Console.WriteLine(projectSettings.Objects[0].Faces.Count);
            Console.WriteLine(indices.Count / 6);

            ProjectManagement.ProjectManager.Close(projectSettings);

            // mesh.Vertices = _vertices;
            // mesh.Indices = _indices;
            mesh.Vertices = vertices.ToArray();
            mesh.Indices = indices.ToArray();

            _model1 = new Model();
            _model1.Meshes = new Mesh[] { mesh };
            _modelRender1 = new ModelRender(_model1);
            _modelRender1.SetShader(shader);

            _model2 = new Model();
            _model2.Meshes = new Mesh[] { mesh };
            _modelRender2 = new ModelRender(_model2);
            _modelRender2.SetShader(shader);

            _mainMenuBarView.Load();
            _openFileDialogView.Load();
            _openFileDialogView.Hide();
            // _openFileDialogView.Hide();

            // _modelView.Init();
            // _modelView.Show();

            // _mainMenuBarView.MenuFileOpen -= MenuFileOpen;
            // _mainMenuBarView.MenuFileOpen += MenuFileOpen;
            // _mainMenuBarView.MenuFileQuit += delegate { Console.Write("Quit"); };
        }

        protected override void OnUnload() {
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            _mainMenuBarView.RenderFrame(e);
            //_openFileDialogView.RenderFrame(e);
            // _modelView.Update(dt);

            float dt = (float)e.Time;

            _model1.Transform = _model1.Transform * Matrix4.CreateRotationY(dt * MathHelper.DegreesToRadians(20.0f));
            // _model1.Transform = _model1.Transform * Matrix4.CreateRotationX(dt * MathHelper.DegreesToRadians(20.0f));
            _modelRender1.Render();

            // _model2.Transform = _model2.Transform * Matrix4.CreateTranslation(dt * 0.5f, 0.0f, 0.0f);
            // _model2.Transform = _model2.Transform * Matrix4.CreateRotationZ(dt * MathHelper.DegreesToRadians(-20.0f));
            // _modelRender2.Render();
        }

        private void MenuFileOpen() {
            _openFileDialogView.Show();
        }
    }
}
