using S3D.FileFormats;
using S3D.UI.OpenTKFramework.Types;
using System;

namespace S3D.UI.Views.Events {
    public abstract class UpdateMeshPrimitiveEventArgs : EventArgs {
        public MeshPrimitiveUpdateType UpdateType { get; set; }

        private UpdateMeshPrimitiveEventArgs() {
        }

        protected UpdateMeshPrimitiveEventArgs(MeshPrimitiveUpdateType updateType) {
            UpdateType = updateType;
        }
    }
}
