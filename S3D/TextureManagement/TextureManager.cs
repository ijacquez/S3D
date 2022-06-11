using S3D.FileFormats;
using S3D.TextureConverters;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System;

namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        private int _textureSlotNumber;

        private readonly List<Picture> _pictures = new List<Picture>();
        // XXX: Should be replaced by texture cache
        private readonly List<ITexture> _uniqueTextures = new List<ITexture>();

        private readonly string _basePath;

        private readonly TextureConverterParameters _textureConverterParameters =
            new TextureConverterParameters();

        private readonly PaletteManager _paletteManager;


        private TextureManager() {
        }

        public TextureManager(string basePath, PaletteManager paletteManager) {
            // XXX: How do we expose the feature to set target width/height?
            _textureConverterParameters.TargetWidth = 64;
            _textureConverterParameters.TargetHeight = 64;
            _textureConverterParameters.DumpFile = true;

            _basePath = Path.GetFullPath(basePath);
            _paletteManager = paletteManager;
        }

        // XXX: Should be replaced by texture cache -- access texture cache indirectly
        public IReadOnlyList<ITexture> UniqueTextures => _uniqueTextures.AsReadOnly();

        public void MapTextureToFace(S3DFace face, string textureFilePath, Vector2[] textureVertices) {
            Picture picture = null;

            if (true) {
                picture = new Picture();

                picture.Texture = CreateTexture(textureFilePath, textureVertices);
                picture.Palette = _paletteManager.GetOrAddPalette(picture.Texture.VDP1Data.Palette);

                _pictures.Add(picture);
                _uniqueTextures.Add(picture.Texture);
            }

            face.Picture = picture;
        }

        private int AllocateTextureSlotNumber() {
            return _textureSlotNumber++;
        }

        private Texture CreateTexture(string filePath, Vector2[] vertices) {
            Texture texture = new Texture();

            texture.FilePath = filePath;
            texture.Vertices = vertices;
            texture.SlotNumber = AllocateTextureSlotNumber();
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

        // private Picture FindPicture(List<Picture> pictures,
        //                                     Texture texture) {
        //     return _pictures.Find(PredicatePicture);
        //
        //     bool PredicatePicture(Picture x) {
        //         return (x.Texture == texture);
        //     }
        // }

        // private Picture FindPicture(List<Picture> pictures,
        //                                     Texture texture,
        //                                     TextureFlags textureFlags) {
        //     return _pictures.Find(PredicatePicture);
        //
        //     bool PredicatePicture(Picture x) {
        //         return ((x.Texture == texture) &&
        //                 (x.TextureFlags == textureFlags));
        //     }
        // }

        // private bool TryGetPicture(string textureFilePath,
        //                                Vector2[] textureVertices,
        //                                out List<Picture> pictures) {
        //     pictures = _pictures.FindAll(PredicatePicture);
        //
        //     if ((pictures != null) && (pictures.Count > 0)) {
        //         return true;
        //     }
        //
        //     return false;
        //
        //     bool PredicatePicture(Picture x) {
        //         return (string.Equals(x.Texture.FilePath, textureFilePath) && true);
        //     }
        // }
    }
}
