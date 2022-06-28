using S3D.FileFormats;

namespace S3D.UI.Views.Events {
    public sealed class UpdatePrimitiveTypeEventArgs : UpdateMeshPrimitiveEventArgs {
        public S3DFaceAttribs.PrimitiveType Type { get; set; }

        public UpdatePrimitiveTypeEventArgs() : base(MeshPrimitiveUpdateType.PrimitiveType) {
        }
    }
}
