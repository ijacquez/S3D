using S3D.UI.OpenTKFramework.Utilities;
using System.Drawing.Imaging;
using System.Drawing;
using System;

namespace S3D.UI.OpenTKFramework.Types {
    // Disambiguate between System.drawing.Imaging.PixelFormat
    using OpenTK.Graphics.OpenGL4;

    public enum TextureCoord {
        S = TextureParameterName.TextureWrapS,
        T = TextureParameterName.TextureWrapT,
        R = TextureParameterName.TextureWrapR
    }

    public class Texture : IDisposable {
        public const SizedInternalFormat Srgb8Alpha8 = (SizedInternalFormat)All.Srgb8Alpha8;
        public const SizedInternalFormat RGB32F      = (SizedInternalFormat)All.Rgb32f;

        public const GetPName MaxTextureMaxAnisotropy = (GetPName)0x84FF;

        public static float MaxAniso { get; }

        static Texture() {
            MaxAniso = GL.GetFloat(MaxTextureMaxAnisotropy);
        }

        public string Name { get; }
        public int Handle { get; }
        public int Width { get; }
        public int Height { get; }
        public int MipmapLevels { get; }
        public SizedInternalFormat InternalFormat { get; }

        private Texture() {
        }

        public Texture(string name, Bitmap image, bool generateMipmaps, bool srgb) {
            Name = name;
            Width = image.Width;
            Height = image.Height;
            InternalFormat = srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;

            if (generateMipmaps) {
                // Calculate how many levels to generate for this texture
                MipmapLevels = (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2));
            } else {
                // There is only one level
                MipmapLevels = 1;
            }

            DebugUtility.CheckGLError("Clear");

            CreationUtility.CreateTexture(TextureTarget.Texture2D, Name, out int texture);
            Handle = texture;
            GL.TextureStorage2D(Handle, MipmapLevels, InternalFormat, Width, Height);
            DebugUtility.CheckGLError("Storage2d");

            BitmapData data = image.LockBits(new Rectangle(0, 0, Width, Height),
                                             ImageLockMode.ReadOnly, global::System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TextureSubImage2D(Handle, 0, 0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            DebugUtility.CheckGLError("SubImage");

            image.UnlockBits(data);

            if (generateMipmaps) GL.GenerateTextureMipmap(Handle);

            GL.TextureParameter(Handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            DebugUtility.CheckGLError("WrapS");
            GL.TextureParameter(Handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            DebugUtility.CheckGLError("WrapT");

            GL.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int)(generateMipmaps ? TextureMinFilter.Linear : TextureMinFilter.LinearMipmapLinear));
            GL.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            DebugUtility.CheckGLError("Filtering");

            GL.TextureParameter(Handle, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);

            // This is a bit weird to do here
            image.Dispose();
        }

        public Texture(string name, int handler, int width, int height, int mipmaplevels, SizedInternalFormat internalFormat) {
            Name = name;
            Handle = handler;
            Width = width;
            Height = height;
            MipmapLevels = mipmaplevels;
            InternalFormat = internalFormat;
        }

        public Texture(string name, int width, int height, IntPtr data, bool generateMipmaps = false, bool srgb = false) {
            Name = name;
            Width = width;
            Height = height;
            InternalFormat = srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;
            MipmapLevels = generateMipmaps == false ? 1 : (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2));

            CreationUtility.CreateTexture(TextureTarget.Texture2D, Name, out int texture);
            Handle = texture;
            GL.TextureStorage2D(Handle, MipmapLevels, InternalFormat, Width, Height);

            GL.TextureSubImage2D(Handle, 0, 0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, data);

            if (generateMipmaps) {
                GL.GenerateTextureMipmap(Handle);
            }

            SetWrap(TextureCoord.S, TextureWrapMode.Repeat);
            SetWrap(TextureCoord.T, TextureWrapMode.Repeat);

            GL.TextureParameter(Handle, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
        }

        public void Use(TextureUnit unit) {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void SetMinFilter(TextureMinFilter filter) {
            GL.TextureParameter(Handle, TextureParameterName.TextureMinFilter, (int)filter);
        }

        public void SetMagFilter(TextureMagFilter filter) {
            GL.TextureParameter(Handle, TextureParameterName.TextureMagFilter, (int)filter);
        }

        public void SetAnisotropy(float level) {
            const TextureParameterName TextureMaxAnisotropy = (TextureParameterName)0x84FE;

            GL.TextureParameter(Handle, TextureMaxAnisotropy, Math.Clamp(level, 1, MaxAniso));
        }

        public void SetLod(int @base, int min, int max) {
            GL.TextureParameter(Handle, TextureParameterName.TextureLodBias, @base);
            GL.TextureParameter(Handle, TextureParameterName.TextureMinLod, min);
            GL.TextureParameter(Handle, TextureParameterName.TextureMaxLod, max);
        }

        public void SetWrap(TextureCoord coord, TextureWrapMode mode) {
            GL.TextureParameter(Handle, (TextureParameterName)coord, (int)mode);
        }

        public void Dispose() {
            GL.DeleteTexture(Handle);
        }
    }
}
