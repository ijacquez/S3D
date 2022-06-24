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

        // private MeshWireRender _modelWireRender;

        private readonly FlyCamera _flyCamera = new FlyCamera();
        private int _lastIndexClicked;

        public Action OpenFile { get; set; }

        protected override void OnLoad() {
            _modelView.ClickedMeshPrimitive -= OnClickedMeshPrimitive;
            _modelView.ClickedMeshPrimitive += OnClickedMeshPrimitive;
            _modelView.Load();

            ProjectManagement.ProjectSettings projectSettings =
                ProjectManagement.ProjectManager.Open("mgl1_settings.json");

            // Mesh wireMesh = S3DWireMeshGenerator.Generate(projectSettings.Objects[0]);

            _modelView.DisplayObject(projectSettings.Objects[0]);

            ProjectManagement.ProjectManager.Close(projectSettings);

            // _modelWireRender = new MeshWireRender(wireMesh);

            _mainMenuBarView.Load();

            // mesh.Primitives[55].SetGouraudShading(Color4.Red, Color4.Green, Color4.Blue, Color4.White);
            // mesh.Primitives[55].Flags |= MeshTriangleFlags.GouraudShaded;

            // mesh.Primitives[53].SetGouraudShading(Color4.Red, Color4.Green, Color4.Blue, Color4.White);
            // mesh.Primitives[53].Flags |= MeshTriangleFlags.GouraudShaded;
        }

        private void OnClickedMeshPrimitive(object sender, ClickedMeshPrimitiveEventArgs e) {
            if (_flyCamera.IsFlying) {
                return;
            }

            var meshPrimitive = e.Mesh.Primitives[e.Index];

            if (_lastIndexClicked >= 0) {
                var lastMeshPrimitive = e.Mesh.Primitives[_lastIndexClicked];

                lastMeshPrimitive.Flags &= ~MeshTriangleFlags.Selected;
            }

            // Select if not previously selected. Otherwise, deselect
            if (_lastIndexClicked == e.Index) {
                _lastIndexClicked = -1;
            } else {
                _lastIndexClicked = e.Index;

                meshPrimitive.Flags |= MeshTriangleFlags.Selected;
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
        }

        protected override void OnRenderFrame() {
            _mainMenuBarView.RenderFrame();
            _modelView.RenderFrame();

            // _modelWireRender.Render();
        }

        private void MenuFileOpen() {
        }
    }
}
