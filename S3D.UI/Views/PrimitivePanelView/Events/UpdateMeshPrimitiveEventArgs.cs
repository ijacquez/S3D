using OpenTK.Mathematics;
using S3D.UI.OpenTKFramework.Types;
using System;

namespace S3D.UI.Views.Events {
    public abstract class UpdateMeshPrimitiveEventArgs : EventArgs {
        public MeshPrimitiveUpdateType UpdateType { get; set; }

        public MeshPrimitive MeshPrimitive { get; set; }

        private UpdateMeshPrimitiveEventArgs() {
        }

        protected UpdateMeshPrimitiveEventArgs(MeshPrimitiveUpdateType updateType) {
            UpdateType = updateType;
        }
    }

    public sealed class UpdateGouraudShadingEventArgs : UpdateMeshPrimitiveEventArgs {
        public bool IsEnabled { get; set; }

        public Color4[] Colors { get; set; }

        public UpdateGouraudShadingEventArgs() : base(MeshPrimitiveUpdateType.GouraudShading) {
        }
    }
}
