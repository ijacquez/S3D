namespace S3D.TextureManagement {
    public interface IPicture {
        public ITexture Texture { get; }

        public IPalette Palette { get; }
    }
}
