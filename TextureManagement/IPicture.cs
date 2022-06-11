namespace S3D.TextureManagement {
    public interface IPicture {
        ITexture Texture { get; }

        TextureFlags TextureFlags { get; set; }

        IPalette Palette { get; }
    }
}
