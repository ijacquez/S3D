using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using S3D.FileFormats;
using S3D.UI.MathUtilities.Raycasting;
using S3D.UI.MeshUtilities;
using S3D.UI.OpenTKFramework.Types;
using S3D.UI.Views.Events;
using System;

namespace S3D.UI.Views {
    public class MainView : View {
        // private readonly FileDialogView _openFileDialogView = new FileDialogView();
        private readonly MainMenuBarView _mainMenuBarView;
        private readonly ModelView _modelView;
        private readonly PrimitivePanelView _primitivePanelView;

        // private MeshWireRender _modelWireRender;

        private readonly FlyCamera _flyCamera = new FlyCamera();
        private int _lastIndexClicked;

        private readonly FaceData _faceData = new FaceData();

        public MainView() {
            _mainMenuBarView = new MainMenuBarView();
            _modelView = new ModelView();
            _primitivePanelView = new PrimitivePanelView(_faceData);
        }

        protected override void OnLoad() {
            _mainMenuBarView.Load();

            _primitivePanelView.UpdateMeshPrimitive -= OnUpdateMeshPrimitive;
            _primitivePanelView.UpdateMeshPrimitive += OnUpdateMeshPrimitive;
            _primitivePanelView.Load();

            _primitivePanelView.ToggleVisibility(false);

            _modelView.ClickMeshPrimitive -= OnClickMeshPrimitive;
            _modelView.ClickMeshPrimitive += OnClickMeshPrimitive;
            _modelView.Load();

            ProjectManagement.ProjectSettings projectSettings =
                ProjectManagement.ProjectManager.Open("mgl1_settings.json");

            var s3dObject = projectSettings.Objects[0];

            // XXX: Fix
            // Mesh wireMesh = S3DWireMeshGenerator.Generate(projectSettings.Objects[0]);

            Model model = new Model();
            Mesh mesh = S3DMeshGenerator.Generate(s3dObject);

            model.Objects = new S3DObject[] {
                s3dObject
            };

            model.Meshes = new Mesh[] {
                mesh
            };

            _modelView.LoadModel(model);

            // ProjectManagement.ProjectManager.Close(projectSettings);

            // XXX: Fix
            // _modelWireRender = new MeshWireRender(wireMesh);
        }

        private void OnUpdateMeshPrimitive(object sender, UpdateMeshPrimitiveEventArgs e) {
            switch (e.UpdateType) {
                case MeshPrimitiveUpdateType.SortType:
                    OnUpdateSortType(e);
                    break;
                case MeshPrimitiveUpdateType.PlaneType:
                    OnUpdatePlaneType(e);
                    break;
                case MeshPrimitiveUpdateType.ColorCalculationMode:
                    OnUpdateColorCalculationMode(e);
                    break;
                case MeshPrimitiveUpdateType.GouraudShading:
                    OnUpdateGouraudShading(e);
                    break;
            }
        }

        private void OnUpdateSortType(UpdateMeshPrimitiveEventArgs e) {
            var eventArgs = (UpdateSortTypeEventArgs)e;

            _faceData.Face.SortType = eventArgs.Type;
        }

        private void OnUpdatePlaneType(UpdateMeshPrimitiveEventArgs e) {
            var eventArgs = (UpdatePlaneTypeEventArgs)e;

            _faceData.Face.PlaneType = eventArgs.Type;
        }

        private void OnUpdateColorCalculationMode(UpdateMeshPrimitiveEventArgs e) {
            var eventArgs = (UpdateColorCalculationModeEventArgs)e;

            _faceData.Face.ColorCalculationMode = eventArgs.Mode;
        }

        private void OnUpdateGouraudShading(UpdateMeshPrimitiveEventArgs e) {
            var eventArgs = (UpdateGouraudShadingEventArgs)e;

            if (eventArgs.IsEnabled) {
                _faceData.Face.FeatureFlags |= FileFormats.S3DFaceAttribs.FeatureFlags.UseGouraudShading;

                if (_faceData.Face.GouraudShadingNumber < 0) {
                    // XXX: Allocating should also free to reuse unused numbers!
                    _faceData.Face.GouraudShadingNumber =
                        _faceData.Object.AllocateGouraudShadingNumber();
                }

                _faceData.Face.GouraudShadingColors[0] = eventArgs.Colors[0];
                _faceData.Face.GouraudShadingColors[1] = eventArgs.Colors[1];
                _faceData.Face.GouraudShadingColors[2] = eventArgs.Colors[2];
                _faceData.Face.GouraudShadingColors[3] = eventArgs.Colors[3];

                // XXX: This should not be in the [future] controller. Instead,
                //      it should be before (or after?) the controller handles
                //      this event
                _faceData.MeshPrimitive.Flags |= MeshPrimitiveFlags.GouraudShaded;

                if (_faceData.MeshPrimitive.Flags.HasFlag(MeshPrimitiveFlags.Quadrangle)) {
                    _faceData.MeshPrimitive.SetGouraudShading(eventArgs.Colors[0],
                                                              eventArgs.Colors[1],
                                                              eventArgs.Colors[2],
                                                              eventArgs.Colors[3]);
                } else {
                    _faceData.MeshPrimitive.SetGouraudShading(eventArgs.Colors[0],
                                                              eventArgs.Colors[1],
                                                              eventArgs.Colors[2]);
                }
            } else {
                _faceData.MeshPrimitive.Flags &= ~MeshPrimitiveFlags.GouraudShaded;
                _faceData.Face.FeatureFlags &= ~FileFormats.S3DFaceAttribs.FeatureFlags.UseGouraudShading;

                // XXX: Free properly
                _faceData.Face.GouraudShadingNumber = -1;
            }

            // XXX: WE NEED THIS!!!
            // s3dFace.Mode = S3DFaceAttribs.Mode.GouraudShading;
        }

        private void OnClickMeshPrimitive(object sender, ClickMeshPrimitiveEventArgs e) {
            if (_flyCamera.IsFlying) {
                return;
            }

            if (_lastIndexClicked >= 0) {
                _faceData.SetData(e.Object, e.Mesh, _lastIndexClicked);

                _faceData.MeshPrimitive.Flags &= ~MeshPrimitiveFlags.Selected;

                _primitivePanelView.ToggleVisibility(false);
            }

            // Select if not previously selected. Otherwise, deselect
            if (_lastIndexClicked == e.Index) {
                _lastIndexClicked = -1;
            } else {
                _lastIndexClicked = e.Index;

                _faceData.SetData(e.Object, e.Mesh, e.Index);

                _faceData.MeshPrimitive.Flags |= MeshPrimitiveFlags.Selected;

                _primitivePanelView.ToggleVisibility(true);
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
    }
}
