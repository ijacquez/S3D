using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using S3D.FileFormats;
using S3D.UI.MathUtilities.Raycasting;
using S3D.UI.MeshUtilities;
using S3D.UI.OpenTKFramework.Extensions;
using S3D.UI.OpenTKFramework.Types;
using S3D.UI.Views.Events;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System;

namespace S3D.UI.Views {
    public class PrimitivePanelView : View {
        private readonly FaceData _faceData;

        private readonly AttributesColorCalculationModeView _attributesColorCalculationModeView;

        private readonly UpdateSortTypeEventArgs _updateSortTypeEventArgs =
            new UpdateSortTypeEventArgs();
        private readonly UpdatePlaneTypeEventArgs _updatePlaneTypeEventArgs =
            new UpdatePlaneTypeEventArgs();

        public event EventHandler<UpdateMeshPrimitiveEventArgs> UpdateMeshPrimitive;

        private PrimitivePanelView() {
        }

        public PrimitivePanelView(FaceData faceData) {
            _faceData = faceData;

            _attributesColorCalculationModeView = new AttributesColorCalculationModeView(faceData);
            _attributesColorCalculationModeView.UpdateMeshPrimitive += OnUpdateMeshPrimitive;
        }

        protected override void OnLoad() {
        }

        protected override void OnUnload() {
        }

        protected override void OnUpdateFrame() {
        }

        protected override void OnRenderFrame() {
            ImGui.ShowDemoWindow();

            ImGui.Begin("Primitive");

            RenderVertices();

            ImGui.Separator();

            RenderAttributes();

            ImGui.End();
        }

        private void OnUpdateMeshPrimitive(object sender, UpdateMeshPrimitiveEventArgs e) {
            UpdateMeshPrimitive?.Invoke(this, e);
        }

        private void RenderVertices() {
            ImGui.Text("Vertices");

            var p0 = _faceData.Object.Vertices[(int)_faceData.Face.Indices[0]];
            var p1 = _faceData.Object.Vertices[(int)_faceData.Face.Indices[1]];
            var p2 = _faceData.Object.Vertices[(int)_faceData.Face.Indices[2]];
            var p3 = _faceData.Object.Vertices[(int)_faceData.Face.Indices[3]];

            ImGui.Text("Type:"); ImGui.SameLine();

            if (_faceData.Face.IsTriangle) {
                ImGui.Text("Triangle");
            } else {
                ImGui.Text("Quad");
            }

            ImGui.InputFloat3($"v0", ref p0, format: "%.5f", ImGuiInputTextFlags.ReadOnly);
            ImGui.InputFloat3($"v1", ref p1, format: "%.5f", ImGuiInputTextFlags.ReadOnly);
            ImGui.InputFloat3($"v2", ref p2, format: "%.5f", ImGuiInputTextFlags.ReadOnly);

            if (!_faceData.Face.IsTriangle) {
                ImGui.InputFloat3($"v3", ref p3, format: "%.5f", ImGuiInputTextFlags.ReadOnly);
            }
        }

        private void RenderAttributes() {
            ImGui.Text("Attributes");
            RenderAttributesPlaneType();
            ImGui.Separator();
            RenderAttributesSortType();
            ImGui.Separator();
            RenderAttributesColorCalculationMode();
        }

        private void RenderAttributesSortType() {
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Sorting");

            int index = (int)_faceData.Face.SortType;

            bool changedRadio = false;

            ImGui.SameLine();
            ImGui.BeginGroup();
            changedRadio = changedRadio || ImGui.RadioButton("Before", ref index, (int)S3DFaceAttribs.SortType.Before);
            ImGui.SameLine();
            changedRadio = changedRadio || ImGui.RadioButton("Min", ref index, (int)S3DFaceAttribs.SortType.Min);
            ImGui.SameLine();
            changedRadio = changedRadio || ImGui.RadioButton("Max", ref index, (int)S3DFaceAttribs.SortType.Max);
            ImGui.SameLine();
            changedRadio = changedRadio || ImGui.RadioButton("Center", ref index, (int)S3DFaceAttribs.SortType.Center);
            ImGui.EndGroup();

            if (changedRadio) {
                _updateSortTypeEventArgs.Type = (S3DFaceAttribs.SortType)index;

                InvokeUpdateSortTypeEvent();
            }
        }

        private void RenderAttributesColorCalculationMode() {
            _attributesColorCalculationModeView.RenderFrame();
        }

        private void RenderAttributesPlaneType() {
            bool isCulling = (_faceData.Face.PlaneType == S3DFaceAttribs.PlaneType.Single);
            bool changedCheckbox = ImGui.Checkbox("Backface culling", ref isCulling);

            if (changedCheckbox) {
                _updatePlaneTypeEventArgs.Type =
                    (isCulling) ? S3DFaceAttribs.PlaneType.Single : S3DFaceAttribs.PlaneType.Dual;

                InvokeUpdatePlaneTypeEvent();
            }
        }

        private void InvokeUpdateSortTypeEvent() {
            UpdateMeshPrimitive?.Invoke(this, _updateSortTypeEventArgs);
        }

        private void InvokeUpdatePlaneTypeEvent() {
            UpdateMeshPrimitive?.Invoke(this, _updatePlaneTypeEventArgs);
        }
    }
}
