using S3D.FileFormats;
using S3D.PaletteManagement;
using S3D.TextureConverters;
using System.Collections.Generic;
using System.Numerics;

namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        private readonly List<Picture> _pictures = new List<Picture>();

        private readonly TextureConverterParameters _textureConverterParameters =
            new TextureConverterParameters();

        private readonly TextureCache _textureCache = new TextureCache();

        private readonly PaletteManager _paletteManager;

        private TextureManager() {
        }

        public TextureManager(PaletteManager paletteManager) {
            // XXX: How do we expose the feature to set target width/height?
            _textureConverterParameters.TargetWidth     = 24;
            _textureConverterParameters.TargetHeight    = 24;
            _textureConverterParameters.AllowDuplicates = false;
            _textureConverterParameters.DumpFile        = true;

            _paletteManager = paletteManager;
        }

        public IReadOnlyList<Texture> UniqueTextures => _textureCache.UniqueTextures;

        public void AddPicture(Picture picture) {
            if (picture == null) {
                return;
            }

            if (!_textureCache.ContainsTexture(picture.Texture)) {
                _textureCache.GetOrAddTexture(picture.Texture);

                _pictures.Add(picture);
            }
        }

        public void MapTextureToFace(S3DFace face, string textureFilePath, Vector2[] textureVertices) {
            var picture = new Picture();

            Texture texture = CreateTexture(textureFilePath, textureVertices);
            Texture cachedTexture = texture;

            if (!_textureConverterParameters.AllowDuplicates) {
                cachedTexture = _textureCache.GetOrAddTexture(texture);
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

        private static VDP1Data CreateVDP1Data(Texture texture, TextureConverterParameters parameters) {
            parameters.TextureVertices = texture.Vertices;

            return VDP1DataConverter.ToVDP1Data(texture.FilePath, parameters);
        }
    }
}
