using S3D.FileFormats;

namespace S3D.UI.Views.Events {
    public sealed class UpdateColorCalculationModeEventArgs : UpdateMeshPrimitiveEventArgs {
        public S3DFaceAttribs.ColorCalculationMode Mode { get; set; }

        public UpdateColorCalculationModeEventArgs() : base(MeshPrimitiveUpdateType.ColorCalculationMode) {
        }
    }
}
