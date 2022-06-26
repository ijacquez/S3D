using S3D.FileFormats;

namespace S3D.UI.Views.Events {
    public sealed class UpdateSortTypeEventArgs : UpdateMeshPrimitiveEventArgs {
        public S3DFaceAttribs.SortType Type { get; set; }

        public UpdateSortTypeEventArgs() : base(MeshPrimitiveUpdateType.SortType) {
        }
    }
}
