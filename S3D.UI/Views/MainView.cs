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
        private readonly ModelView _modelView = new ModelView();
        private readonly PrimitivePanelView _primitivePanelView = new PrimitivePanelView();

        // private MeshWireRender _modelWireRender;

        private readonly FlyCamera _flyCamera = new FlyCamera();
        private int _lastIndexClicked;

        public Action OpenFile { get; set; }

        protected override void OnLoad() {
            _mainMenuBarView.Load();
            _primitivePanelView.Load();

            _modelView.ClickMeshPrimitive -= OnClickedMeshPrimitive;
            _modelView.ClickMeshPrimitive += OnClickedMeshPrimitive;
            _modelView.Load();

            ProjectManagement.ProjectSettings projectSettings =
                ProjectManagement.ProjectManager.Open("mgl1_settings.json");

            // Mesh wireMesh = S3DWireMeshGenerator.Generate(projectSettings.Objects[0]);
            Model model = new Model();
            Mesh mesh = S3DMeshGenerator.Generate(projectSettings.Objects[0]);

            model.Meshes = new Mesh[] {
                mesh
            };

            // XXX: Remove
            foreach (var meshPrimitive in mesh.Primitives) {
                meshPrimitive.Flags |= MeshPrimitiveFlags.Textured;
            }

            _modelView.LoadModel(model);

            ProjectManagement.ProjectManager.Close(projectSettings);

            // _modelWireRender = new MeshWireRender(wireMesh);

            // mesh.Primitives[55].SetGouraudShading(Color4.Red, Color4.Green, Color4.Blue, Color4.White);
            // mesh.Primitives[55].Flags |= MeshPrimitiveFlags.GouraudShaded;

            // mesh.Primitives[53].SetGouraudShading(Color4.Red, Color4.Green, Color4.Blue, Color4.White);
            // mesh.Primitives[53].Flags |= MeshPrimitiveFlags.GouraudShaded;
        }

        private void OnClickedMeshPrimitive(object sender, ClickMeshPrimitiveEventArgs e) {
            if (_flyCamera.IsFlying) {
                return;
            }

            var meshPrimitive = e.Mesh.Primitives[e.Index];

            if (_lastIndexClicked >= 0) {
                var lastMeshPrimitive = e.Mesh.Primitives[_lastIndexClicked];

                lastMeshPrimitive.Flags &= ~MeshPrimitiveFlags.Selected;

                _primitivePanelView.HideMeshPrimitive();
            }

            // Select if not previously selected. Otherwise, deselect
            if (_lastIndexClicked == e.Index) {
                _lastIndexClicked = -1;
            } else {
                _lastIndexClicked = e.Index;

                meshPrimitive.Flags |= MeshPrimitiveFlags.Selected;

                _primitivePanelView.ShowMeshPrimitive(meshPrimitive);
            }
        }

        protected override void OnUnload() {
        }

        protected override void OnUpdateFrame() {
            if (!Window.IsFocused) {
                return;
            }

            _flyCamera.UpdateFrame();
            _modelView.UpdateFrame();
            _primitivePanelView.UpdateFrame();
        }

        protected override void OnRenderFrame() {
            _mainMenuBarView.RenderFrame();
            _modelView.RenderFrame();
            _primitivePanelView.RenderFrame();

            // _modelWireRender.Render();
        }

        private void MenuFileOpen() {
        }
    }
}
