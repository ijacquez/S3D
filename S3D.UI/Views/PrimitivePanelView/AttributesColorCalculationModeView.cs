using ImGuiNET;
using OpenTK.Mathematics;
using S3D.FileFormats;
using S3D.UI.Views.Events;
using System.Drawing;
using System.Linq;
using System;

namespace S3D.UI.Views {
    public class AttributesColorCalculationModeView : View {
        private readonly FaceData _faceData;

        private readonly UpdateColorCalculationModeEventArgs _updateColorCalculationModeEventArgs =
            new UpdateColorCalculationModeEventArgs();
        private readonly UpdateGouraudShadingEventArgs _updateGouraudShadingEventArgs =
            new UpdateGouraudShadingEventArgs();

        public event EventHandler<UpdateMeshPrimitiveEventArgs> UpdateMeshPrimitive;

        private AttributesColorCalculationModeView() {
        }

        public AttributesColorCalculationModeView(FaceData faceData) {
            _faceData = faceData;
        }

        protected override void OnLoad() {
        }

        protected override void OnUnload() {
        }

        protected override void OnUpdateFrame() {
        }

        protected override void OnRenderFrame() {
            ImGui.Text("Color Calculation Mode");

            int oldIndex = (int)_faceData.Face.ColorCalculationMode;
            int newIndex = oldIndex;

            bool changedRadio = false;

            changedRadio = changedRadio || RadioButton(oldIndex, ref newIndex, "Replace", (int)S3DFaceAttribs.ColorCalculationMode.Replace);
            changedRadio = changedRadio || RadioButton(oldIndex, ref newIndex, "Shadow", (int)S3DFaceAttribs.ColorCalculationMode.Shadow);
            changedRadio = changedRadio || RadioButton(oldIndex, ref newIndex, "Half Luminance", (int)S3DFaceAttribs.ColorCalculationMode.HalfLuminance);
            changedRadio = changedRadio || RadioButton(oldIndex, ref newIndex, "Half Transparent", (int)S3DFaceAttribs.ColorCalculationMode.HalfTransparent);
            changedRadio = changedRadio || RadioButton(oldIndex, ref newIndex, "Gouraud Shading", (int)S3DFaceAttribs.ColorCalculationMode.GouraudShading);
            changedRadio = changedRadio || RadioButton(oldIndex, ref newIndex, "Gouraud Shading + Half Luminance", (int)S3DFaceAttribs.ColorCalculationMode.GouraudShadingAndHalfLuminance);
            changedRadio = changedRadio || RadioButton(oldIndex, ref newIndex, "Gouraud Shading + Half Transparent", (int)S3DFaceAttribs.ColorCalculationMode.GouraudShadingAndHalfTransparent);

            _updateColorCalculationModeEventArgs.Mode = (S3DFaceAttribs.ColorCalculationMode)newIndex;

            if (changedRadio) {
                InvokeUpdateColorCalculationModeEvent();
            }
        }

        private bool RadioButton(int oldIndex, ref int newIndex, string label, int indexCheckmark) {
            bool changedRadio = ImGui.RadioButton(label, ref newIndex, indexCheckmark);

            // XXX: We might have to expand this further into a switch statement
            //      per color calculation mode
            if ((indexCheckmark == (int)S3DFaceAttribs.ColorCalculationMode.GouraudShading) ||
                (indexCheckmark == (int)S3DFaceAttribs.ColorCalculationMode.GouraudShadingAndHalfLuminance) ||
                (indexCheckmark == (int)S3DFaceAttribs.ColorCalculationMode.GouraudShadingAndHalfTransparent)) {
                if (newIndex == indexCheckmark) {
                    Color[] colors = null;

                    bool changedColors = ColorEdit3x4(ref colors);

                    if (changedColors) {
                        _updateGouraudShadingEventArgs.IsEnabled = true;
                        _updateGouraudShadingEventArgs.Colors = colors;

                        InvokeUpdateGouraudShadingEvent();
                    }
                } else if (oldIndex == indexCheckmark) {
                    _updateGouraudShadingEventArgs.IsEnabled = false;
                    _updateGouraudShadingEventArgs.Colors = null;

                    InvokeUpdateGouraudShadingEvent();
                }
            }

            return changedRadio;
        }

        private bool ColorEdit3x4(ref Color[] outputColors) {
            outputColors = _faceData.Face.GouraudShadingColors.ToArray();

            var inputColors = new System.Numerics.Vector3[outputColors.Length];

            bool changedColors = false;

            ImGui.Indent();

            for (int i = 0; i < outputColors.Length; i++) {
                inputColors[i] = new System.Numerics.Vector3(outputColors[i].R / 255.0f,
                                                             outputColors[i].G / 255.0f,
                                                             outputColors[i].B / 255.0f);

                // Show all 4 input color widgets, unless the face is a
                // triangle. In which case, copy the first output color to
                // the third output color
                if (!_faceData.Face.IsTriangle || (i < 3)) {
                    string label = $"c{i}";

                    if (ImGui.ColorEdit3(label, ref inputColors[i], ImGuiColorEditFlags.Uint8)) {
                        changedColors = true;

                        int clampedR = (int)(MathHelper.Clamp(inputColors[i].X, 0.0f, 1.0f) * 255.0f);
                        int clampedG = (int)(MathHelper.Clamp(inputColors[i].Y, 0.0f, 1.0f) * 255.0f);
                        int clampedB = (int)(MathHelper.Clamp(inputColors[i].Z, 0.0f, 1.0f) * 255.0f);

                        outputColors[i] = Color.FromArgb(clampedR, clampedG, clampedB);
                    }
                } else {
                    outputColors[3] = outputColors[0];
                }
            }

            ImGui.Unindent();

            return changedColors;
        }

        private void InvokeUpdateColorCalculationModeEvent() {
            UpdateMeshPrimitive?.Invoke(this, _updateColorCalculationModeEventArgs);
        }

        private void InvokeUpdateGouraudShadingEvent() {
            UpdateMeshPrimitive?.Invoke(this, _updateGouraudShadingEventArgs);
        }
    }
}
