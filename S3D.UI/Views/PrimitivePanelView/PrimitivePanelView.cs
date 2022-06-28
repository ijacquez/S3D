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

        private readonly UpdatePrimitiveTypeEventArgs _updatePrimitiveTypeEventArgs =
            new UpdatePrimitiveTypeEventArgs();
        private readonly UpdateSortTypeEventArgs _updateSortTypeEventArgs =
            new UpdateSortTypeEventArgs();
        private readonly UpdatePlaneTypeEventArgs _updatePlaneTypeEventArgs =
            new UpdatePlaneTypeEventArgs();
        private readonly UpdateRenderFlagsEventArgs _updateRenderFlagsEventArgs =
            new UpdateRenderFlagsEventArgs();

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

            RenderPrimitiveType();

            RenderVertices();

            ImGui.Separator();

            RenderAttributes();

            ImGui.Separator();

            RenderOptions();

            ImGui.End();
        }

        private void OnUpdateMeshPrimitive(object sender, UpdateMeshPrimitiveEventArgs e) {
            UpdateMeshPrimitive?.Invoke(this, e);
        }

        private void RenderPrimitiveType() {
            ImGui.Text("Primitive Type");

            int index = (int)_faceData.Face.PrimitiveType;

            bool changedRadio = false;

            changedRadio = changedRadio || ImGui.RadioButton("Polygon", ref index, (int)S3DFaceAttribs.PrimitiveType.Polygon);
            changedRadio = changedRadio || ImGui.RadioButton("Polyline", ref index, (int)S3DFaceAttribs.PrimitiveType.Polyline);
            changedRadio = changedRadio || ImGui.RadioButton("Distorted Sprite", ref index, (int)S3DFaceAttribs.PrimitiveType.DistortedSprite);
            changedRadio = changedRadio || ImGui.RadioButton("Line", ref index, (int)S3DFaceAttribs.PrimitiveType.Line);

            if (changedRadio) {
                _updatePrimitiveTypeEventArgs.Type = (S3DFaceAttribs.PrimitiveType)index;

                InvokeUpdatePrimitiveTypeEvent();
            }
        }

        private void RenderVertices() {
            if (_faceData.Face.IsTriangle) {
                ImGui.Text("Triangle");
            } else if (_faceData.Face.IsLine) {
                ImGui.Text("Line");
            } else {
                ImGui.Text("Quad");
            }

            ImGui.Spacing();

            ImGui.Text("Vertices");

            var p0 = _faceData.Object.Vertices[(int)_faceData.Face.Indices[0]];
            var p1 = _faceData.Object.Vertices[(int)_faceData.Face.Indices[1]];
            var p2 = _faceData.Object.Vertices[(int)_faceData.Face.Indices[2]];
            var p3 = _faceData.Object.Vertices[(int)_faceData.Face.Indices[3]];

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

        private void RenderOptions() {
            ImGui.Text("Render Options");

            bool isMeshed = _faceData.Face.RenderFlags.HasFlag(S3DFaceAttribs.RenderFlags.Mesh);

            S3DFaceAttribs.RenderFlags renderFlags = _faceData.Face.RenderFlags;

            bool changed = false;

            ImGui.BeginGroup();
            if (ImGui.Checkbox("Mesh", ref isMeshed)) {
                changed = true;

                if (isMeshed) {
                    renderFlags |= S3DFaceAttribs.RenderFlags.Mesh;
                } else {
                    renderFlags &= ~S3DFaceAttribs.RenderFlags.Mesh;
                }
            }
            ImGui.EndGroup();

            if (changed) {
                _updateRenderFlagsEventArgs.Flags = renderFlags;

                InvokeUpdateRenderFlagsEvent();
            }
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

        private void InvokeUpdatePrimitiveTypeEvent() {
            UpdateMeshPrimitive?.Invoke(this, _updatePrimitiveTypeEventArgs);
        }

        private void InvokeUpdateSortTypeEvent() {
            UpdateMeshPrimitive?.Invoke(this, _updateSortTypeEventArgs);
        }

        private void InvokeUpdatePlaneTypeEvent() {
            UpdateMeshPrimitive?.Invoke(this, _updatePlaneTypeEventArgs);
        }

        private void InvokeUpdateRenderFlagsEvent() {
            UpdateMeshPrimitive?.Invoke(this, _updateRenderFlagsEventArgs);
        }
    }
}
