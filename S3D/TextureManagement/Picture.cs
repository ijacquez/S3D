using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using S3D.PaletteManagement;

namespace S3D.TextureManagement {
    public class Picture {
        [JsonProperty]
        public Texture Texture { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public TextureFlags TextureFlags { get; set; }

        [JsonProperty]
        public Palette Palette { get; set; }
    }
}
