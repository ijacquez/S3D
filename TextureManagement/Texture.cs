using Newtonsoft.Json;
using S3D.TextureConverters;
using System.Numerics;

namespace S3D.TextureManagement {
    public class Texture {
        [JsonProperty]
        public string FilePath { get; set; }

        [JsonProperty]
        public Vector2[] Vertices { get; set; }

        [JsonProperty]
        public VDP1Data VDP1Data { get; set; }

        [JsonProperty]
        public int SlotNumber { get; set; } = -1;
    }
}
