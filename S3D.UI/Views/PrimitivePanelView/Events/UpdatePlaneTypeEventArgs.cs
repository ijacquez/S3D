using S3D.FileFormats;

namespace S3D.UI.Views.Events {
    public sealed class UpdatePlaneTypeEventArgs : UpdateMeshPrimitiveEventArgs {
        public S3DFaceAttribs.PlaneType Type { get; set; }

        public UpdatePlaneTypeEventArgs() : base(MeshPrimitiveUpdateType.PlaneType) {
        }
    }
}
