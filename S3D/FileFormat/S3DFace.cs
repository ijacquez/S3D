using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using S3D.TextureManagement;
using System.Drawing;
using System.Numerics;

namespace S3D.FileFormats {
    public class S3DFace {
        [JsonProperty]
        public uint[] Indices { get; private set; } = new uint[4];

        [JsonProperty]
        public Vector3 Normal { get; set; }

        [JsonProperty]
        public Color[] GouraudShadingColors { get; private set; } = new Color[4];

        [JsonProperty]
        public int GouraudShadingNumber { get; set; } = -1;

        [JsonProperty]
        public Picture Picture { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public S3DFaceAttribs.FeatureFlags FeatureFlags { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public S3DFaceAttribs.PrimitiveType PrimitiveType { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public S3DFaceAttribs.PlaneType PlaneType { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public S3DFaceAttribs.SortType SortType { get; set; } = S3DFaceAttribs.SortType.Center;

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public S3DFaceAttribs.RenderFlags RenderFlags { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public S3DFaceAttribs.ColorCalculationMode ColorCalculationMode { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public S3DFaceAttribs.TextureType TextureType { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public S3DFaceAttribs.TextureFlipFlags TextureFlipFlags { get; set; }

        [JsonIgnore]
        public bool IsTriangle => (Indices[0] == Indices[3]);

        [JsonIgnore]
        public bool IsLine => false;
    }
}
