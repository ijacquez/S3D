using S3D.FileFormats;

namespace S3D.UI.Views.Events {
    public sealed class UpdateRenderFlagsEventArgs : UpdateMeshPrimitiveEventArgs {
        public S3DFaceAttribs.RenderFlags Flags { get; set; }

        public UpdateRenderFlagsEventArgs() : base(MeshPrimitiveUpdateType.RenderFlags) {
        }
    }
}
