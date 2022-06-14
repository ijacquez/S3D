using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;

namespace S3D.FileFormats {
    public class S3DObject {
        private int _gouraudShadingNumber;

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public List<Vector3> Vertices { get; } = new List<Vector3>();

        [JsonProperty]
        public List<Vector3> VertexNormals { get; } = new List<Vector3>();

        [JsonProperty]
        public List<S3DFace> Faces { get; } = new List<S3DFace>();

        [JsonIgnore]
        public int PictureCount =>
            Faces.Where((face) => face.FeatureFlags.HasFlag(S3DFaceAttribs.FeatureFlags.UseTexture))
                 .Count();

        [JsonIgnore]
        public int GouraudShadingCount =>
            Faces.Where((face) => face.FeatureFlags.HasFlag(S3DFaceAttribs.FeatureFlags.UseGouraudShading))
                 .Count();

        public int AllocateGouraudShadingNumber() {
            return _gouraudShadingNumber++;
        }
    }
}
