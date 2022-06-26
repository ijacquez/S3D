using System.Drawing;

namespace S3D.UI.Views.Events {
    public sealed class UpdateGouraudShadingEventArgs : UpdateMeshPrimitiveEventArgs {
        public bool IsEnabled { get; set; }

        public Color[] Colors { get; set; }

        public UpdateGouraudShadingEventArgs() : base(MeshPrimitiveUpdateType.GouraudShading) {
        }
    }
}
