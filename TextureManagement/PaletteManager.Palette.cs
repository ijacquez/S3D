using S3D.Types;

namespace S3D.TextureManagement {
    public sealed partial class PaletteManager {
        private class Palette : IPalette {
            public RGB1555[] Colors { get; set; }

            public int SlotNumber { get; set; }
        }
    }
}
