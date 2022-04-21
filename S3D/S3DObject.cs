using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace S3D {
    public class S3DObject {
        private int _gouraudShadingNumber;

        public string Name { get; set; }

        public List<Vector3> Vertices { get; } = new List<Vector3>();

        public List<Vector3> VertexNormals { get; } = new List<Vector3>();

        public List<S3DFace> Faces { get; } = new List<S3DFace>();

        public int PictureCount =>
            Faces.Where((face) => face.FeatureFlags.HasFlag(S3DFaceAttribs.FeatureFlags.UseTexture))
                 .Count();

        public int GouraudShadingCount =>
            Faces.Where((face) => face.FeatureFlags.HasFlag(S3DFaceAttribs.FeatureFlags.UseGouraudShading))
                 .Count();

        public int AllocateGouraudShadingNumber() {
            return _gouraudShadingNumber++;
        }
    }
}
