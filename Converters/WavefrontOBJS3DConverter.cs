using S3D.PaletteManagement;
using S3D.TextureManagement;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System;

namespace S3D.Converters {
    using JeremyAnsel.Media.WavefrontObj;

    public sealed class WavefrontOBJS3DConverter : S3DConverter {
        private readonly string _basePath;
        private readonly string _fileName;

        private readonly Stream _stream;
        private readonly ObjFile _objFile;

        private readonly IReadOnlyList<ObjMaterialFile> _objMaterialFiles =
            new List<ObjMaterialFile>();

        private readonly IDictionary<string, ObjMaterial> _objNameToMaterialDict =
            new Dictionary<string, ObjMaterial>();

        private readonly IDictionary<string, ObjGroup> _objNameToGroupDict =
            new Dictionary<string, ObjGroup>();

        public WavefrontOBJS3DConverter(TextureManager textureManager,
                                        PaletteManager paletteManager,
                                        string basePath,
                                        string fileName) : base(textureManager, paletteManager) {
            _fileName = fileName;
            _basePath = basePath;
            _stream = File.Open(Path.Combine(_basePath, _fileName), FileMode.Open);
            _objFile = ObjFile.FromStream(_stream);

            foreach (string materialLibrary in _objFile.MaterialLibraries) {
                string materialLibraryPath = GetFullPath(materialLibrary);

                ObjMaterialFile objMaterialFile = ObjMaterialFile.FromFile(materialLibraryPath);

                foreach (ObjMaterial objMaterial in objMaterialFile.Materials) {
                    _objNameToMaterialDict.Add(objMaterial.Name, objMaterial);
                }
            }

            if (_objFile.Groups.Count > 0) {
                foreach (ObjGroup objGroup in _objFile.Groups) {
                    _objNameToGroupDict.Add(objGroup.Name, objGroup);
                }
            } else {
                // When there are no groups, just use the file name and default
                // group
                _objNameToGroupDict.Add(_fileName, _objFile.DefaultGroup);
            }
        }

        protected override void DisposeManaged() {
            _stream.Dispose();
        }

        protected override string[] GetObjectIDs() {
            return _objNameToGroupDict.Keys.ToArray();
        }

        protected override Vector3?[] GetVertexMap(string objectID) {
            ObjGroup objGroup = _objNameToGroupDict[objectID];

            Vector3?[] vertexMap = new Vector3?[_objFile.Vertices.Count];

            // Vertices are stored in sequential order for all groups
            IList<ObjVertex> objVertices = _objFile.Vertices;

            foreach (ObjFace objFace in objGroup.Faces) {
                int[] vertexIndices = GetVertexIndices(objFace);

                foreach (int vertexIndex in vertexIndices) {
                    ObjVector4 position = objVertices[vertexIndex].Position;
                    Vector3 vertex = TransformVertex(Conversions.ToVector3(position));

                    vertexMap[vertexIndex] = vertex;
                }
            }

            return vertexMap;
        }

        protected override Vector3[] GetVertexNormals(string objectID) {
            ObjGroup objGroup = _objNameToGroupDict[objectID];

            Vector3[] vertexNormals = new Vector3[_objFile.VertexNormals.Count];

            // Vertices are stored in sequential order for all groups
            IList<ObjVector3> objVertexNormals = _objFile.VertexNormals;

            // Find what the max vertex index is and used that to determine the
            // number of vertices this object is using
            int maxVertexIndex = 0;

            foreach (ObjFace objFace in objGroup.Faces) {
                int[] vertexIndices = GetVertexNormalIndices(objFace);

                foreach (int vertexIndex in vertexIndices) {
                    maxVertexIndex = Math.Max(vertexIndex, maxVertexIndex);

                    Vector3 vertexNormal = Conversions.ToVector3(objVertexNormals[vertexIndex]);

                    vertexNormals[vertexIndex] = vertexNormal;
                }
            }

            Array.Resize(ref vertexNormals, maxVertexIndex + 1);

            return vertexNormals;
        }

        protected override int GetFaceCount(string objectID) {
            ObjGroup objGroup = _objNameToGroupDict[objectID];

            return objGroup.Faces.Count;
        }

        protected override int[] GetFaceIndices(string objectID, int faceIndex) {
            ObjGroup objGroup = _objNameToGroupDict[objectID];
            ObjFace objFace = objGroup.Faces[faceIndex];

            return GetVertexIndices(objFace);
        }

        protected override Vector3[] GetFaceVertices(string objectID, int faceIndex) {
            ObjGroup objGroup = _objNameToGroupDict[objectID];
            ObjFace objFace = objGroup.Faces[faceIndex];

            IList<ObjVertex> objVertices = _objFile.Vertices;

            int[] vertexIndices = GetVertexIndices(objFace);

            return vertexIndices.Select((vertexIndex) =>
                                        TransformVertex(Conversions.ToVector3(objVertices[vertexIndex].Position))).ToArray();
        }

        private static Vector3 TransformVertex(Vector3 vector) => vector;

        protected override bool TryGetFaceColors(string objectID, int faceIndex, out Color[] colors) {
            ObjGroup objGroup = _objNameToGroupDict[objectID];
            ObjFace objFace = objGroup.Faces[faceIndex];

            IList<ObjVertex> objVertices = _objFile.Vertices;

            colors = null;

            int[] vertexIndices = GetVertexIndices(objFace);

            Color[] convertedColors = new Color[4];
            int colorCount = 0;

            foreach (int vertexIndex in vertexIndices) {
                ObjVector4? color = objVertices[vertexIndex].Color;

                if (color.HasValue) {
                    convertedColors[colorCount] = Conversions.ToColor(color.Value);

                    colorCount++;
                }
            }

            if (colorCount == 0) {
                return false;
            }

            colors = convertedColors;

            return true;
        }

        protected override bool TryGetFaceBaseColor(string objectID, int faceIndex, out Color color) {
            ObjGroup objGroup = _objNameToGroupDict[objectID];
            ObjFace objFace = objGroup.Faces[faceIndex];

            color = default(Color);

            // Distinguish between not having a material and a missing material.
            // The latter should throw an exception
            if (string.IsNullOrEmpty(objFace.MaterialName)) {
                return false;
            }

            ObjMaterial objMaterial = _objNameToMaterialDict[objFace.MaterialName];

            if (objMaterial.AmbientColor.IsRGB) {
                color = Conversions.ToColor(objMaterial.AmbientColor.Color);

                return true;
            }

            return false;
        }

        protected override bool TryGetFaceTextureVertices(string objectID,
                                                          int faceIndex,
                                                          out Vector2[] textureVertices) {
            ObjGroup objGroup = _objNameToGroupDict[objectID];
            ObjFace objFace = objGroup.Faces[faceIndex];

            textureVertices = null;

            IList<ObjVector3> objTextureVertices = _objFile.TextureVertices;

            // Do we have any texture vertices?
            if (objTextureVertices?.Count == 0) {
                return false;
            }

            int[] uvIndices = GetVertexUVIndices(objFace);

            // If any are invalid indices, exit
            if (uvIndices.Any((index) => (index < 0))) {
                return false;
            }

            textureVertices = new Vector2[uvIndices.Length];

            for (int i = 0; i < uvIndices.Length; i++) {
                textureVertices[i] = Conversions.ToVector2(objTextureVertices[uvIndices[i]]);
            }

            return true;
        }

        protected override bool TryGetFaceTexturePath(string objectID, int faceIndex, out string texturePath) {
            ObjGroup objGroup = _objNameToGroupDict[objectID];
            ObjFace objFace = objGroup.Faces[faceIndex];

            texturePath = null;

            // Distinguish between not having a material and a missing material.
            // The latter should throw an exception
            if (string.IsNullOrEmpty(objFace.MaterialName)) {
                return false;
            }

            ObjMaterial objMaterial = _objNameToMaterialDict[objFace.MaterialName];

            if (objMaterial.DiffuseMap == null) {
                return false;
            }

            texturePath = GetFullPath(objMaterial.DiffuseMap.FileName);

            if (texturePath == null) {
                return false;
            }

            return true;
        }

        private ObjTriplet[] GetVertexTriplets(ObjFace objFace) {
            IList<ObjTriplet> objTriplets = objFace.Vertices;

            if (objTriplets.Count == 3) {
                objTriplets.Add(objTriplets[0]);
            } else if (objTriplets.Count != 4) {
                throw new Exception($"Face contains neither 3 or 4 vertices, but {objTriplets.Count} vertices");
            }

            return objTriplets.ToArray();
        }

        private int[] GetVertexIndices(ObjFace objFace) {
            // Value is 1-indexed
            return GetVertexTriplets(objFace).Select((objTriplet) => (objTriplet.Vertex - 1))
                                             .ToArray();
        }

        private int[] GetVertexNormalIndices(ObjFace objFace) {
            // Value is 1-indexed
            return GetVertexTriplets(objFace).Select((objTriplet) => (objTriplet.Normal - 1))
                                             .ToArray();
        }

        private int[] GetVertexUVIndices(ObjFace objFace) {
            // Value is 1-indexed
            return GetVertexTriplets(objFace).Select((objTriplet) => objTriplet.Texture - 1).Reverse()
                                             .ToArray();
        }

        private string GetFullPath(string fileName) {
            if (string.IsNullOrEmpty(fileName)) {
                return null;
            }

            if (Path.IsPathRooted(fileName)) {
                return fileName;
            }

            return Path.Combine(_basePath, fileName);
        }

        private static class Conversions {
            public static Vector2 ToVector2(ObjVector3 objVector3) {
                return new Vector2(objVector3.X, objVector3.Y);
            }

            public static Vector3 ToVector3(ObjVector4 objVector4) {
                return new Vector3(objVector4.X, objVector4.Y, objVector4.Z);
            }

            public static Vector3 ToVector3(ObjVector3 objVector3) {
                return new Vector3(objVector3.X, objVector3.Y, objVector3.Z);
            }

            public static Color ToColor(ObjVector3 objVector3) {
                return Color.FromArgb(255,
                                      (int)(255 * objVector3.X),
                                      (int)(255 * objVector3.Y),
                                      (int)(255 * objVector3.Z));
            }

            public static Color ToColor(ObjVector4 objVector4) {
                return Color.FromArgb(255,
                                      (int)(255 * objVector4.X),
                                      (int)(255 * objVector4.Y),
                                      (int)(255 * objVector4.Z));
            }
        }
    }
}
