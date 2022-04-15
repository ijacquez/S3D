using S3D.Extensions;
using S3D.TextureConverters;
using S3D.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System;

namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        private int _textureSlotNumber;

        private readonly List<TextureData> _textureDatas = new List<TextureData>();

        private readonly IDictionary<S3DFace, TextureData> _faceToTextureDataDict =
            new Dictionary<S3DFace, TextureData>();

        private readonly HashSet<Texture> _uniqueTextureHashSet = new HashSet<Texture>();
        private readonly List<ITexture> _uniqueTextures = new List<ITexture>();

        private readonly TextureConverterParameters _textureConverterParameters =
            new TextureConverterParameters();

        public string BasePath { get; }
        public PaletteManager PaletteManager { get; }

        private TextureManager() {
        }

        public TextureManager(string basePath, PaletteManager paletteManager) {
            // XXX: How do we expose the feature to set target width/height?
            _textureConverterParameters.TargetWidth = 64;
            _textureConverterParameters.TargetHeight = 64;
            _textureConverterParameters.DumpFile = true;

            BasePath = Path.GetFullPath(basePath);
            PaletteManager = paletteManager;
        }

        public IReadOnlyList<ITexture> UniqueTextures => _uniqueTextures;

        public ITexture MapToFace(S3DFace face, string textureFilePath, Vector2[] textureVertices) {
            // If the face already has a texture
            if (TryGetTexture(face, out ITexture texture)) {
                return texture;
            }

            var textureData = new TextureData();

            textureData.Texture = CreateTexture(textureFilePath, textureVertices);
            textureData.Palette = PaletteManager.GetOrAddPalette(textureData.Texture.VDP1Data.Palette);

            _textureDatas.Add(textureData);

            _faceToTextureDataDict.Add(face, textureData);

            UpdateUniqueTexturesCache(textureData);

            return textureData;
        }

        public bool ContainsTexture(S3DFace face) {
            return _faceToTextureDataDict.ContainsKey(face);
        }

        public bool TryGetTexture(S3DFace face, out ITexture texture) {
            texture = null;

            if (!ContainsTexture(face)) {
                return false;
            }

            TextureData textureData = _faceToTextureDataDict[face];

            texture = (ITexture)textureData;

            return true;
        }

        private Texture CreateTexture(string filePath, Vector2[] vertices) {
            Texture texture = new Texture();

            texture.FilePath = filePath;
            texture.Vertices = vertices;
            texture.SlotNumber = AllocateTextureSlotNumber();
            texture.VDP1Data = CreateVDP1Data(texture, _textureConverterParameters);

            return texture;
        }

        private int AllocateTextureSlotNumber() {
            return _textureSlotNumber++;
        }

        private TextureData FindTextureData(List<TextureData> textureDatas,
                                            Texture texture) {
            return _textureDatas.Find(PredicateTextureData);

            bool PredicateTextureData(TextureData x) {
                return (x.Texture == texture);
            }
        }

        private TextureData FindTextureData(List<TextureData> textureDatas,
                                            Texture texture,
                                            TextureFlags textureFlags) {
            return _textureDatas.Find(PredicateTextureData);

            bool PredicateTextureData(TextureData x) {
                return ((x.Texture == texture) &&
                        (x.TextureFlags == textureFlags));
            }
        }

        private bool TryGetTextureData(string textureFilePath, Vector2[] textureVertices,
                                       out List<TextureData> textureDatas) {
            textureDatas = _textureDatas.FindAll(PredicateTextureData);

            if ((textureDatas != null) && (textureDatas.Count > 0)) {
                return true;
            }

            return false;

            bool PredicateTextureData(TextureData x) {
                return (string.Equals(x.Texture.FilePath, textureFilePath) &&
                        EqualsTextureVertices(x.Texture.Vertices, textureVertices));
            }
        }

        private static IEnumerable<Vector2> SortTextureVertices(IEnumerable<Vector2> vertices) {
            return vertices.OrderBy((vertex) => vertex.X).ThenBy((vertex) => vertex.Y);
        }

        private static bool EqualsTextureVertices(Vector2[] a, Vector2[] b) {
            if (a.Length != b.Length) {
                return false;
            }

            if (a.Length == 0) {
                return false;
            }

            // XXX: Check if both are convex

            Vector2 centerA = TextureUtility.GetCenterPoint(a);
            Vector2 centerB = TextureUtility.GetCenterPoint(b);

            // Distance between A & B
            Vector2 centerDiff = centerB - centerA;
            Vector2 centerModulo = TextureUtility.ClampVector(centerDiff);
            Vector2 amountSubtract = (centerB - centerA) - centerModulo;

            var shiftedB = b.Select((textureVertex) => (textureVertex - amountSubtract));

            var sortedA = SortTextureVertices(a).ToArray();
            var sortedB = SortTextureVertices(shiftedB).ToArray();

            for (int i = 0; i < sortedA.Length; i++) {
                if (!sortedA[i].IsApproximately(sortedB[i])) {
                    return false;
                }
            }

            return true;
        }

        private static VDP1Data CreateVDP1Data(Texture texture,
                                               TextureConverterParameters textureConverterParameters) {
            Array.Copy(texture.Vertices,
                       textureConverterParameters.TextureVertices,
                       texture.Vertices.Length);

            return TextureConverter.ToTexture(texture.FilePath, textureConverterParameters);
        }

        private void UpdateUniqueTexturesCache(TextureData textureData) {
            Texture texture = textureData.Texture;

            if (!_uniqueTextureHashSet.Contains(texture)) {
                _uniqueTextureHashSet.Add(texture);

                _uniqueTextures.Add(textureData);
            }
        }
    }
}
