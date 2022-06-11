using S3D.FileFormats;
using S3D.TextureConverters;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System;

namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        private readonly List<Picture> _pictures = new List<Picture>();

        private readonly string _basePath;

        private readonly TextureConverterParameters _textureConverterParameters =
            new TextureConverterParameters();

        private readonly TextureCache _textureCache = new TextureCache();

        private readonly PaletteManager _paletteManager;

        private TextureManager() {
        }

        public TextureManager(string basePath, PaletteManager paletteManager) {
            // XXX: How do we expose the feature to set target width/height?
            _textureConverterParameters.TargetWidth     = 24;
            _textureConverterParameters.TargetHeight    = 24;
            _textureConverterParameters.AllowDuplicates = false;
            _textureConverterParameters.DumpFile        = true;

            _basePath = Path.GetFullPath(basePath);
            _paletteManager = paletteManager;
        }

        public IReadOnlyList<ITexture> UniqueTextures => _textureCache.UniqueTextures;

        public void MapTextureToFace(S3DFace face, string textureFilePath, Vector2[] textureVertices) {
            var picture = new Picture();

            ITexture texture = CreateTexture(textureFilePath, textureVertices);
            ITexture cachedTexture = texture;

            if (!_textureConverterParameters.AllowDuplicates) {
                cachedTexture = _textureCache.GetOrAddTexture(texture);

                // XXX: Debug. Remove
                if (texture == cachedTexture) {
                    Console.WriteLine($"[1;32m     New texture: {cachedTexture.SlotNumber}[m");
                } else {
                    Console.WriteLine($"[1;31mExisting texture: {cachedTexture.SlotNumber}[m");
                }
            }

            picture.Texture = cachedTexture;
            picture.Palette = _paletteManager.GetOrAddPalette(cachedTexture.VDP1Data.Palette);

            _pictures.Add(picture);

            face.Picture = picture;
        }

        private Texture CreateTexture(string filePath, Vector2[] vertices) {
            Texture texture = new Texture();

            texture.FilePath = filePath;
            texture.Vertices = vertices;
            texture.VDP1Data = CreateVDP1Data(texture, _textureConverterParameters);

            return texture;
        }

        private static VDP1Data CreateVDP1Data(Texture texture,
                                               TextureConverterParameters textureConverterParameters) {
            Array.Copy(texture.Vertices,
                       textureConverterParameters.TextureVertices,
                       texture.Vertices.Length);

            return VDP1DataConverter.ToVDP1Data(texture.FilePath, textureConverterParameters);
        }
    }
}
