using Newtonsoft.Json;
using S3D.Types;

namespace S3D.PaletteManagement {
    public  class Palette {
        [JsonProperty]
        public RGB1555[] Colors { get; set; }

        [JsonProperty]
        public int SlotNumber { get; set; }
    }
}
