using S3D.FileFormats;
using S3D.PaletteManagement;
using S3D.TextureConverters;
using S3D.TextureManagement;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System;

namespace S3D.Converters {
    public abstract class S3DConverter : IDisposable {
        private bool _disposed;

        protected TextureManager TextureManager { get; private set; }
        protected PaletteManager PaletteManager { get; private set; }

        private S3DConverter() {
        }

        protected S3DConverter(TextureManager textureManager, PaletteManager paletteManager) {
            TextureManager = textureManager;
            PaletteManager = paletteManager;
        }

        public S3DObject[] ToS3DObjects() {
            string[] objectIDs = GetObjectIDs();
            S3DObject[] s3dObjects = new S3DObject[objectIDs.Length];

            for (int index = 0; index < s3dObjects.Length; index++) {
                string objectID = objectIDs[index];
                S3DObject s3dObject = new S3DObject();

                s3dObject.Name = objectID;

                int[] indices = GetIndices(objectID);
                int[] normalizedIndices = CalculateNormalizedIndices(indices);

                Vector3?[] vertexMap = GetVertexMap(objectID);
                Vector3[] normalizedVertices = new Vector3[normalizedIndices.Length];

                for (int i = 0; i < normalizedIndices.Length; i++) {
                    normalizedVertices[normalizedIndices[i]] = vertexMap[indices[i]].Value;
                }

                s3dObject.Vertices.AddRange(normalizedVertices);

                s3dObject.VertexNormals.AddRange(GetVertexNormals(objectID));

                int indicesIndex = 0;

                int faceCount = GetFaceCount(objectID);

                for (int faceIndex = 0; faceIndex < faceCount; faceIndex++) {
                    S3DFace s3dFace = new S3DFace();

                    Array.Copy(normalizedIndices,
                               indicesIndex,
                               s3dFace.Indices,
                               0,
                               s3dFace.Indices.Length);

                    indicesIndex += s3dFace.Indices.Length;

                    // Check for gouraud shading
                    if (TryGetFaceColors(objectID, faceIndex, out Color[] colors)) {
                        Array.Copy(colors, s3dFace.GouraudShadingColors, s3dFace.GouraudShadingColors.Length);

                        s3dFace.GouraudShadingNumber = s3dObject.AllocateGouraudShadingNumber();

                        s3dFace.FeatureFlags |= S3DFaceAttribs.FeatureFlags.UseGouraudShading;

                        // By default, choose mode 4. Later on, the user can change to different modes
                        s3dFace.ColorCalculationMode = S3DFaceAttribs.ColorCalculationMode.GouraudShading;
                    }

                    // Determine the primitive type
                    if (TryGetFaceTexturePath(objectID, faceIndex, out string texturePath)) {
                        TryGetFaceTextureVertices(objectID, faceIndex, out Vector2[] textureVertices);

                        TextureManager.MapTextureToFace(s3dFace, texturePath, textureVertices);

                        Picture picture = s3dFace.Picture;

                        s3dFace.FeatureFlags |= S3DFaceAttribs.FeatureFlags.UseTexture;

                        s3dFace.RenderFlags |= S3DFaceAttribs.RenderFlags.DisableEndCodes;
                        s3dFace.RenderFlags |= S3DFaceAttribs.RenderFlags.DisableTransparencyIndex;

                        if (picture.TextureFlags.HasFlag(TextureFlags.FlippedHorizontally)) {
                            s3dFace.TextureFlipFlags |= S3DFaceAttribs.TextureFlipFlags.H;
                        }

                        if (picture.TextureFlags.HasFlag(TextureFlags.FlippedVertically)) {
                            s3dFace.TextureFlipFlags |= S3DFaceAttribs.TextureFlipFlags.V;
                        }

                        switch (picture.Texture.VDP1Data.Type) {
                            case VDP1DataType.Indexed16:
                                s3dFace.TextureType = S3DFaceAttribs.TextureType.CLUT16;
                                break;
                            case VDP1DataType.Indexed64:
                                s3dFace.TextureType = S3DFaceAttribs.TextureType.ColorBank64;
                                break;
                            case VDP1DataType.Indexed128:
                                s3dFace.TextureType = S3DFaceAttribs.TextureType.ColorBank128;
                                break;
                            case VDP1DataType.Indexed256:
                                s3dFace.TextureType = S3DFaceAttribs.TextureType.ColorBank256;
                                break;
                            case VDP1DataType.RGB1555:
                                s3dFace.TextureType = S3DFaceAttribs.TextureType.RGB1555;
                                break;
                        }

                        // We will always use distorted sprites
                        s3dFace.PrimitiveType = S3DFaceAttribs.PrimitiveType.DistortedSprite;
                    } else {
                        if (!TryGetFaceBaseColor(objectID, faceIndex, out Color polygonColor)) {
                            polygonColor = Color.Magenta;
                        }

                        s3dFace.PrimitiveType = S3DFaceAttribs.PrimitiveType.Polygon;

                        s3dFace.RenderFlags |= S3DFaceAttribs.RenderFlags.DisableEndCodes;
                        s3dFace.RenderFlags |= S3DFaceAttribs.RenderFlags.DisableTransparencyIndex;
                    }

                    s3dFace.Normal = CalculateNormal(GetFaceVertices(objectID, faceIndex));

                    s3dObject.Faces.Add(s3dFace);
                }

                s3dObjects[index] = s3dObject;
            }

            return s3dObjects;
        }

        protected abstract string[] GetObjectIDs();

        protected abstract Vector3?[] GetVertexMap(string objectID);

        protected abstract Vector3[] GetVertexNormals(string objectID);

        protected abstract int GetFaceCount(string objectID);

        protected abstract int[] GetFaceIndices(string objectID, int faceIndex);

        protected abstract Vector3[] GetFaceVertices(string objectID, int faceIndex);

        protected abstract bool TryGetFaceColors(string objectID, int faceIndex, out Color[] colors);

        protected abstract bool TryGetFaceBaseColor(string objectID, int faceIndex, out Color color);

        protected abstract bool TryGetFaceTextureVertices(string objectID, int faceIndex, out Vector2[] uvs);

        protected abstract bool TryGetFaceTexturePath(string objectID, int faceIndex, out string texturePath);

        protected abstract void DisposeManaged();

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    DisposeManaged();
                }

                // TODO: Free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: Set large fields to null
                _disposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~S3DConverter()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        private int[] GetIndices(string objectID) {
            int faceCount = GetFaceCount(objectID);

            int[] indices = new int[4 * faceCount];

            for (int faceIndex = 0; faceIndex < faceCount; faceIndex++) {
                int[] faceIndices = GetFaceIndices(objectID, faceIndex);

                Array.Copy(faceIndices,
                           0,
                           indices,
                           faceIndex * faceIndices.Length,
                           faceIndices.Length);
            }

            return indices;
        }

        private int[] CalculateNormalizedIndices(int[] indices) {
            int[] normalizedIndices = new int[indices.Length];

            Array.Fill(normalizedIndices, -1);

            for (int i = 0, index = 0; i < indices.Length; i++) {
                if (normalizedIndices[i] >= 0) {
                    continue;
                }

                for (int j = 0; j < indices.Length; j++) {
                    if (indices[i] == indices[j]) {
                        normalizedIndices[j] = index;
                    }
                }

                index++;
            }

            return normalizedIndices.ToArray();
        }

        private static Vector3 CalculateNormal(Vector3[] vertices) {
            Vector3 uVector = Vector3.Normalize(vertices[3] - vertices[2]);
            Vector3 vVector = Vector3.Normalize(vertices[1] - vertices[2]);
            Vector3 normal = Vector3.Cross(uVector, vVector);

            return normal;
        }
    }
}
