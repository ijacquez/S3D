namespace S3D.UI.OpenTKFramework.Types {
    // Disambiguate between System.drawing.Imaging.PixelFormat
    using OpenTK.Graphics.OpenGL;

    public enum TextureCoord {
        S = TextureParameterName.TextureWrapS,
        T = TextureParameterName.TextureWrapT,
        R = TextureParameterName.TextureWrapR
    }
}
