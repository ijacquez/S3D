using S3D.TextureConverters;
using System.Numerics;

namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        private class Texture : ITexture {
            public string FilePath { get; set; }

            public Vector2[] Vertices { get; set; }

            public VDP1Data VDP1Data { get; set; }

            public int SlotNumber { get; set; }

            public bool IsHorizontallyFlipped =>
                TextureFlags.HasFlag(TextureFlags.FlippedHorizontally);

            public bool IsVerticallyFlipped =>
                TextureFlags.HasFlag(TextureFlags.FlippedVertically);

            public TextureFlags TextureFlags { get; set; }
        }
    }
}
