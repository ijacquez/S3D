using System.Numerics;
using System.Collections.Generic;

namespace S3D {
    public class S3DObject {
        private int _gouraudShadingNumber;

        public string Name { get; set; }

        public List<Vector3> Vertices { get; } = new List<Vector3>();

        public List<Vector3> VertexNormals { get; } = new List<Vector3>();

        public List<S3DFace> Faces { get; } = new List<S3DFace>();

        public int AllocateGouraudShadingNumber() {
            return _gouraudShadingNumber++;
        }
    }
}
