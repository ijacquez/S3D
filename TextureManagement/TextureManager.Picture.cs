namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        private class Picture : IPicture {
            public ITexture Texture { get; set; }

            public TextureFlags TextureFlags { get; set; }

            public IPalette Palette { get; set; }
        }
    }
}
