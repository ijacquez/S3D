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

namespace S3D.UI.Views {
    public class PrimitivePanelView : View {
        private MeshPrimitive _showingMeshPrimitive;

        public PrimitivePanelView() {
        }

        public void ShowMeshPrimitive(MeshPrimitive meshPrimitive) {
            _showingMeshPrimitive = meshPrimitive;
        }

        public void HideMeshPrimitive() {
            _showingMeshPrimitive = null;
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

            if (_showingMeshPrimitive != null) {
                ImGui.Text("Vertices");

                var vertices = _showingMeshPrimitive.GetVertices();

                for (int v = 0; v < vertices.Length; v++) {
                    var vertex = vertices[v].ToNumerics();

                    ImGui.InputFloat3($"v{v}", ref vertex, format: "%.5f", ImGuiInputTextFlags.ReadOnly);
                }

                ImGui.Separator();

                ImGui.Text("Attributes");

                ImGui.Text("Sorting"); ImGui.SameLine();
                ImGui.BeginGroup();
                ImGui.RadioButton("Before", ref _s, 0); ImGui.SameLine();
                ImGui.RadioButton("Min",    ref _s, 1); ImGui.SameLine();
                ImGui.RadioButton("Center", ref _s, 2); ImGui.SameLine();
                ImGui.RadioButton("Max",    ref _s, 3);
                ImGui.EndGroup();

                ImGui.Text("Gouraud Shading");

                bool isGouraudShaded =
                    _showingMeshPrimitive.Flags.HasFlag(MeshPrimitiveFlags.GouraudShaded);
                string label = (isGouraudShaded) ? "Disable" : "Enable";

                if (ImGui.Checkbox(label, ref isGouraudShaded)) {
                    if (isGouraudShaded) {
                        _showingMeshPrimitive.Flags |= MeshPrimitiveFlags.GouraudShaded;
                    } else {
                        _showingMeshPrimitive.Flags &= ~MeshPrimitiveFlags.GouraudShaded;
                    }
                }

                if (isGouraudShaded) {
                    bool colorsChanged = false;

                    System.Numerics.Vector4 colorVector;

                    colorVector = _showingMeshPrimitive.Triangles[0].Colors[0].ToNumerics();
                    var v0 = new System.Numerics.Vector3(colorVector.X, colorVector.Y, colorVector.Z);
                    colorVector = _showingMeshPrimitive.Triangles[0].Colors[1].ToNumerics();
                    var v1 = new System.Numerics.Vector3(colorVector.X, colorVector.Y, colorVector.Z);
                    colorVector = _showingMeshPrimitive.Triangles[0].Colors[2].ToNumerics();
                    var v2 = new System.Numerics.Vector3(colorVector.X, colorVector.Y, colorVector.Z);

                    var colors = _showingMeshPrimitive.Triangles[0].Colors.ToArray();

                    if (ImGui.ColorEdit3("c0", ref v0)) {
                        colorsChanged = true;
                        colors[0] = new Color4(v0.X, v0.Y, v0.Z, 1.0f);
                    }

                    if (ImGui.ColorEdit3("c1", ref v1)) {
                        colorsChanged = true;
                        colors[1] = new Color4(v1.X, v1.Y, v1.Z, 1.0f);
                    }

                    if (ImGui.ColorEdit3("c2", ref v2)) {
                        colorsChanged = true;
                        colors[2] = new Color4(v2.X, v2.Y, v2.Z, 1.0f);
                    }

                    if (colorsChanged) {
                        _showingMeshPrimitive.SetGouraudShading(colors[0],
                                                                colors[1],
                                                                colors[2]);
                    }
                }
            }

            ImGui.End();
        }
    }
}
