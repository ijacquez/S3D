using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using S3D.UI.MathUtilities.Raycasting;
using S3D.UI.MeshUtilities;
using S3D.UI.OpenTKFramework.Types;
using System;

namespace S3D.UI.Views {
    public class MainView : View {
        // private readonly FileDialogView _openFileDialogView = new FileDialogView();
        private readonly MainMenuBarView _mainMenuBarView = new MainMenuBarView();
        // private readonly ModelView _modelView = new ModelView();

        private Model _model;
        private ModelRender _modelRender;
        // private MeshWireRender _modelWireRender;

        private readonly FlyCamera _flyCamera = new FlyCamera();

        private int _lastPrimitiveClicked = -1;

        public Action OpenFile { get; set; }

        protected override void OnLoad() {
            string vertexShader = System.IO.File.ReadAllText("Shaders/shader.vert");
            string fragmentShader = System.IO.File.ReadAllText("Shaders/shader.frag");
            Shader shader = new Shader("Model", vertexShader, fragmentShader);

            ProjectManagement.ProjectSettings projectSettings =
                ProjectManagement.ProjectManager.Open("mgl1_settings.json");

            Mesh mesh = S3DMeshGenerator.Generate(projectSettings.Objects[0]);
            // Mesh wireMesh = S3DWireMeshGenerator.Generate(projectSettings.Objects[0]);

            ProjectManagement.ProjectManager.Close(projectSettings);

            _model = new Model();
            _model.Meshes = new Mesh[] {
                mesh
            };
            _modelRender = new ModelRender(_model);
            _modelRender.SetShader(shader);

            // _modelWireRender = new MeshWireRender(wireMesh);

            _mainMenuBarView.Load();

            foreach (var meshPrimitive in mesh.Primitives) {
                meshPrimitive.Flags |= MeshTriangleFlags.Textured;
            }

            // mesh.Primitives[55].SetGouraudShading(Color4.Red, Color4.Green, Color4.Blue, Color4.White);
            // mesh.Primitives[55].Flags |= MeshTriangleFlags.GouraudShaded;

            // mesh.Primitives[53].SetGouraudShading(Color4.Red, Color4.Green, Color4.Blue, Color4.White);
            // mesh.Primitives[53].Flags |= MeshTriangleFlags.GouraudShaded;
        }

        protected override void OnUnload() {
        }

        protected override void OnUpdateFrame() {
            if (!Window.IsFocused) {
                return;
            }

            _flyCamera.UpdateFrame();

            if (!_flyCamera.IsFlying && Window.Input.MouseState.IsButtonPressed(MouseButton.Button1)) {
                Vector2 mousePoint = new Vector2(Window.Input.MouseState.X, Window.Input.MouseState.Y);

                Mesh mesh = _model.Meshes[0];

                if (Window.Camera.Cast(mousePoint, mesh, out RaycastHitInfo hitInfo)) {
                    int index = (int)hitInfo.PrimitiveIndex;

                    if (_lastPrimitiveClicked >= 0) {
                        mesh.Primitives[_lastPrimitiveClicked].Flags &= ~MeshTriangleFlags.Selected;
                    }

                    // Select if not previously selected. Otherwise, deselect
                    if (_lastPrimitiveClicked == index) {
                        _lastPrimitiveClicked = -1;
                    } else {
                        _lastPrimitiveClicked = index;

                        mesh.Primitives[index].Flags |= MeshTriangleFlags.Selected;
                    }
                }
            }
        }

        protected override void OnRenderFrame() {
            _mainMenuBarView.RenderFrame();

            _modelRender.Render();
            // _modelWireRender.Render();
        }

        private void MenuFileOpen() {
        }
    }
}
