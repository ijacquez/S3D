using OpenTK.Mathematics;
using S3D.UI.OpenTKFramework.Types;
using System;

namespace S3D.UI.Views {
    public class ClickMeshPrimitiveEventArgs : EventArgs {
        public bool MultiSelect { get; set; }

        public Mesh Mesh { get; set; }

        public MeshPrimitive MeshPrimitive { get; set; }

        public int Index { get; set; }

        public Vector3 Point { get; set; }
    }
}
