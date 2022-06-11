namespace S3D.TextureManagement {
    public interface IPicture {
        public ITexture Texture { get; }

        public TextureFlags TextureFlags { get; set; }

        public IPalette Palette { get; }
    }
}
