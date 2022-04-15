using System.Numerics;
using S3D.TextureConverters;

namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        private class Texture {
            public string FilePath { get; set; }

            public Vector2[] Vertices { get; set; }

            public VDP1Data VDP1Data { get; set; }

            public int SlotNumber { get; set; }
        }
    }
}
