using S3D.TextureConverters;
using System.Numerics;

namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        private class TextureData : ITexture {
            string ITexture.FilePath => Texture?.FilePath;

            Vector2[] ITexture.Vertices => Texture?.Vertices;

            VDP1Data ITexture.VDP1Data => Texture?.VDP1Data;

            bool ITexture.IsHorizontallyFlipped =>
                TextureFlags.HasFlag(TextureFlags.FlippedHorizontally);

            bool ITexture.IsVerticallyFlipped =>
                TextureFlags.HasFlag(TextureFlags.FlippedVertically);

            int ITexture.SlotNumber => Texture?.SlotNumber ?? -1;

            IPalette ITexture.Palette => Palette;

            public Texture Texture { get; set; }

            public IPalette Palette { get; set; }

            public TextureFlags TextureFlags { get; set; }
        }
    }
}
