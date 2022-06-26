using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using S3D.FileFormats;
using S3D.UI.MathUtilities.Raycasting;
using S3D.UI.MeshUtilities;
using S3D.UI.OpenTKFramework.Extensions;
using S3D.UI.OpenTKFramework.Types;
using System.Collections.Generic;
using System.Linq;
using System;
using S3D.UI.Views.Events;

namespace S3D.UI.Views {
    public class PrimitivePanelView : View {
        private MeshPrimitive _meshPrimitive;

        private readonly UpdateGouraudShadingEventArgs _updateGouraudShadingEventArgs =
            new UpdateGouraudShadingEventArgs();

        public event EventHandler<UpdateMeshPrimitiveEventArgs> UpdateMeshPrimitive;

        public PrimitivePanelView() {
        }

        public void ShowMeshPrimitive(MeshPrimitive meshPrimitive) {
            _meshPrimitive = meshPrimitive;
        }

        public void HideMeshPrimitive() {
            _meshPrimitive = null;
        }

        protected override void OnLoad() {
        }

        protected override void OnUnload() {
        }

        protected override void OnUpdateFrame() {
        }

        private int _s;

        protected override void OnRenderFrame() {
            ImGui.ShowDemoWindow();

            ImGui.Begin("Primitive");

            if (_meshPrimitive != null) {
                ImGui.Text("Vertices");

                RenderVertices();

                ImGui.Separator();

                ImGui.Text("Attributes");

                ImGui.Text("Sorting");

                RenderAttributesSorting();

                ImGui.Text("Gouraud Shading");

                RenderAttributesGouraudShading();
            }

            ImGui.End();
        }

        private void RenderVertices() {
            var vertices = _meshPrimitive.GetVertices();

            for (int v = 0; v < vertices.Length; v++) {
                var vertex = vertices[v].ToNumerics();

                ImGui.InputFloat3($"v{v}", ref vertex, format: "%.5f", ImGuiInputTextFlags.ReadOnly);
            }
        }

        private void RenderAttributesSorting() {
            ImGui.SameLine();
            ImGui.BeginGroup();
            ImGui.RadioButton("Before", ref _s, 0);
            ImGui.SameLine();
            ImGui.RadioButton("Min", ref _s, 1);
            ImGui.SameLine();
            ImGui.RadioButton("Center", ref _s, 2);
            ImGui.SameLine();
            ImGui.RadioButton("Max", ref _s, 3);
            ImGui.EndGroup();
        }

        private void RenderAttributesGouraudShading() {
            bool isGouraudShaded = _meshPrimitive.Flags.HasFlag(MeshPrimitiveFlags.GouraudShaded);

            string checkboxLabel = (isGouraudShaded) ? "Disable" : "Enable";
            bool changedCheckbox = ImGui.Checkbox(checkboxLabel, ref isGouraudShaded);

            _updateGouraudShadingEventArgs.IsEnabled = isGouraudShaded;
            _updateGouraudShadingEventArgs.Colors = null;

            if (isGouraudShaded) {
                var outputColors = _meshPrimitive.GetGouraudShadingTable();
                var inputColors = new System.Numerics.Vector3[outputColors.Length];

                bool changedColors = false;

                for (int i = 0; i < outputColors.Length; i++) {
                    inputColors[i] = new System.Numerics.Vector3(outputColors[i].R,
                                                                 outputColors[i].G,
                                                                 outputColors[i].B);

                    if (ImGui.ColorEdit3($"c{i}", ref inputColors[i], ImGuiColorEditFlags.Uint8)) {
                        changedColors = true;

                        outputColors[i] = new Color4(MathHelper.Clamp(inputColors[i].X, 0.0f, 1.0f),
                                                     MathHelper.Clamp(inputColors[i].Y, 0.0f, 1.0f),
                                                     MathHelper.Clamp(inputColors[i].Z, 0.0f, 1.0f),
                                                     1.0f);
                    }
                }

                // If gouraud shading is enabled for the first time, or any
                // field is changed
                if (changedCheckbox || changedColors) {
                    _updateGouraudShadingEventArgs.Colors = outputColors;

                    InvokeUpdateGouraudShadingEvent();
                }
            } else if (changedCheckbox) {
                // If gouraud shading is disabled
                InvokeUpdateGouraudShadingEvent();
            }
        }

        private void InvokeUpdateGouraudShadingEvent() {
            _updateGouraudShadingEventArgs.MeshPrimitive = _meshPrimitive;

            UpdateMeshPrimitive?.Invoke(this, _updateGouraudShadingEventArgs);
        }
    }
}
