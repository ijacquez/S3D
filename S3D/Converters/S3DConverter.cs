using S3D.TextureManagement;
using System.Drawing;
using System.Numerics;
using System;
using S3D.TextureConverters;

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

            for (int i = 0; i < s3dObjects.Length; i++) {
                string objectID = objectIDs[i];
                S3DObject s3dObject = new S3DObject();

                s3dObject.Name = objectID;

                s3dObject.Vertices.AddRange(GetVertices(objectID));
                s3dObject.VertexNormals.AddRange(GetVertexNormals(objectID));

                for (int faceIndex = 0; faceIndex < GetFaceCount(objectID); faceIndex++) {
                    S3DFace s3dFace = new S3DFace();

                    Array.Copy(GetFaceIndices(objectID, faceIndex),
                               s3dFace.Indices,
                               s3dFace.Indices.Length);

                    // Check for gouraud shading
                    if (TryGetFaceColors(objectID, faceIndex, out Color[] colors)) {
                        Array.Copy(colors, s3dFace.GouraudShadingColors, s3dFace.GouraudShadingColors.Length);

                        s3dFace.GouraudingShadingNumber = s3dObject.AllocateGouraudShadingNumber();

                        s3dFace.FeatureFlags |= S3DFaceAttribs.FeatureFlags.UseGouraudShading;

                        // By default, choose mode 4. Later on, the user can change to different modes
                        s3dFace.ColorCalculationMode = S3DFaceAttribs.ColorCalculationMode.GouraudShading;
                    }

                    // Determine the primitive type
                    if (TryGetFaceTexturePath(objectID, faceIndex, out string texturePath)) {
                        TryGetFaceTextureVertices(objectID, faceIndex, out Vector2[] textureVertices);

                        ITexture texture = TextureManager.MapToFace(s3dFace, texturePath, textureVertices);

                        s3dFace.FeatureFlags |= S3DFaceAttribs.FeatureFlags.UseTexture;

                        s3dFace.RenderFlags |= S3DFaceAttribs.RenderFlags.DisableEndCodes;
                        s3dFace.RenderFlags |= S3DFaceAttribs.RenderFlags.DisableTransparencyIndex;

                        if (texture.IsHorizontallyFlipped) {
                            s3dFace.TextureFlipFlags |= S3DFaceAttribs.TextureFlipFlags.H;
                        }

                        if (texture.IsVerticallyFlipped) {
                            s3dFace.TextureFlipFlags |= S3DFaceAttribs.TextureFlipFlags.V;
                        }

                        switch (texture.VDP1Data.Type) {
                            case TextureType.Indexed16:
                                s3dFace.TextureType = S3DFaceAttribs.TextureType.CLUT16;
                                break;
                            case TextureType.Indexed64:
                                s3dFace.TextureType = S3DFaceAttribs.TextureType.ColorBank64;
                                break;
                            case TextureType.Indexed128:
                                s3dFace.TextureType = S3DFaceAttribs.TextureType.ColorBank128;
                                break;
                            case TextureType.Indexed256:
                                s3dFace.TextureType = S3DFaceAttribs.TextureType.ColorBank256;
                                break;
                            case TextureType.RGB1555:
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

                s3dObjects[i] = s3dObject;
            }

            return s3dObjects;
        }

        protected abstract string[] GetObjectIDs();

        protected abstract Vector3[] GetVertices(string objectID);

        protected abstract Vector3[] GetVertexNormals(string objectID);

        protected abstract int GetFaceCount(string objectID);

        protected abstract int[] GetFaceIndices(string objectID, int faceIndex);

        protected abstract Vector3[] GetFaceVertices(string objectID, int faceIndex);

        protected abstract bool TryGetFaceColors(string objectID, int faceIndex, out Color[] colors);

        protected abstract bool TryGetFaceBaseColor(string objectID, int faceIndex, out Color color);

        protected abstract bool TryGetFaceTextureVertices(string objectID, int faceIndex, out Vector2[] uvs);

        protected abstract bool TryGetFaceTexturePath(string objectID, int faceIndex, out string texturePath);

        private static Vector3 CalculateNormal(Vector3[] vertices) {
            Vector3 uVector = Vector3.Normalize(vertices[3] - vertices[2]);
            Vector3 vVector = Vector3.Normalize(vertices[1] - vertices[2]);
            Vector3 normal = Vector3.Cross(uVector, vVector);

            return normal;
        }

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
    }
}
