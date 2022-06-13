using S3D.FileFormats.IO;
using S3D.FileFormats;
using S3D.IO;
using S3D.PaletteManagement;
using S3D.TextureManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System;

namespace S3D {
    public static class S3DWriter {
        private class S3DObjectWriteContext {
            public S3DObject Object { get; private set; }

            /// <summary>
            ///   Offset to the object's pool of vertices.
            /// </summary>
            public IWriteReference VerticesReference { get; set; }

            /// <summary>
            ///   Offset to the object's pool of faces.
            /// </summary>
            public IWriteReference FacesReference { get; set; }

            /// <summary>
            ///   Offset to the object's pool of vertex normals.
            /// </summary>
            public IWriteReference VertexNormalsReference { get; set; }

            /// <summary>
            ///   Offset to the object's pool of face attributes.
            /// </summary>
            public IWriteReference FaceAttributesReference { get; set; }

            /// <summary>
            ///   Offset to the object's gouraud shading tables.
            /// </summary>
            public IWriteReference GouraudShadingTablesReference { get; set; }

            /// <summary>
            ///   Offset to the object's pool of picture structures.
            /// </summary>
            public IWriteReference PicturesReference { get; set; }

            private S3DObjectWriteContext() {
            }

            public S3DObjectWriteContext(S3DObject s3dObject) {
                Object = s3dObject;
            }
        }

        public static void WriteS3DFile(string outputFilePath, IEnumerable<S3DObject> objects, Context context) {
            using (var fileStream = File.Open(outputFilePath, FileMode.Create)) {
                WriteS3D(fileStream, objects, context);
            }
        }

        public static void WriteS3D(FileStream fileStream, IEnumerable<S3DObject> s3dObjects, Context context) {
            IWriteReference textureBaseReference;
            IWriteReference textureDataBaseReference;
            IWriteReference paletteBaseReference;
            IWriteReference paletteDataBaseReference;
            IWriteReference eofReference;

            // Specific context for writing
            S3DObjectWriteContext[] objectWriteContexts =
                s3dObjects.Select((s3dObject) => new S3DObjectWriteContext(s3dObject)).ToArray();

            using (S3DBinaryWriter binaryWriter = new S3DBinaryWriter(fileStream)) {
                // Header
                binaryWriter.WriteSignature();
                binaryWriter.WriteVersion();
                binaryWriter.WriteFlags(S3DFlags.None);
                binaryWriter.WriteUInt32((UInt16)objectWriteContexts.Length);

                textureBaseReference = binaryWriter.WriteDeferredReference();
                binaryWriter.WriteUInt32(context.TextureManager.UniqueTextures.Count);

                paletteBaseReference = binaryWriter.WriteDeferredReference();
                binaryWriter.WriteUInt32(context.PaletteManager.UniquePalettes.Count);

                textureDataBaseReference = binaryWriter.WriteDeferredReference();
                paletteDataBaseReference = binaryWriter.WriteDeferredReference();

                eofReference = binaryWriter.WriteDeferredReference();

                foreach (S3DObjectWriteContext objectWriteContext in objectWriteContexts) {
                    // XPDATA (extended)
                    WriteExtendedPolygonalStructure(binaryWriter, objectWriteContext);
                }

                foreach (S3DObjectWriteContext objectWriteContext in objectWriteContexts) {
                    // pntbl
                    WriteVerticesData(binaryWriter, objectWriteContext);
                    // pltbl
                    WriteFaceIndicesData(binaryWriter, objectWriteContext);
                    // attbl
                    WriteFaceAttributesData(binaryWriter, objectWriteContext, context);
                    // vntbl
                    WriteVectorNormalsData(binaryWriter, objectWriteContext);
                    // PICTUREs
                    WritePictureStructure(binaryWriter, objectWriteContext, context);
                    // Gouraud shading tables
                    WriteGouraudShadingTablesData(binaryWriter, objectWriteContext);
                }

                if (context.TextureManager.UniqueTextures.Count > 0) {
                    // TEXTUREs (global, shared)
                    binaryWriter.WriteReferenceOffset(textureBaseReference);
                    WriteTextureStructure(binaryWriter, context);

                    // Texture data (global, shared)
                    binaryWriter.WriteReferenceOffset(textureDataBaseReference);
                    WriteTextureData(binaryWriter, context);
                }

                if (context.PaletteManager.UniquePalettes.Count > 0) {
                    // PALETTEs (global, shared)
                    binaryWriter.WriteReferenceOffset(paletteBaseReference);
                    WritePaletteStructure(binaryWriter, context);

                    // Palette data (global, shared)
                    binaryWriter.WriteReferenceOffset(paletteDataBaseReference);
                    WritePaletteData(binaryWriter, context);
                }

                binaryWriter.WriteReferenceOffset(eofReference);
            }
        }

        private static void WritePaletteStructure(S3DBinaryWriter binaryWriter, Context context) {
            foreach (Palette palette in context.PaletteManager.UniquePalettes) {
                int colorCount = palette.Colors.Length;

                //   Uint16 Color 2B palette number
                UInt16 clutOrCRAMBit = (UInt16)((colorCount == 16) ? 0x0000 : 0x8000);

                binaryWriter.WriteUInt16(clutOrCRAMBit);
            }
        }

        private static void WritePaletteData(S3DBinaryWriter binaryWriter, Context context) {
            // Use this to chain palettes
            IWriteReference paletteChainReference = null;

            for (int i = 0; i < context.PaletteManager.UniquePalettes.Count; i++) {
                Palette palette = context.PaletteManager.UniquePalettes[i];

                if (paletteChainReference != null) {
                    binaryWriter.WriteReferenceOffset(paletteChainReference);
                }

                // If this is the last palette in the list, then set a flag too
                // denote the size
                if ((i + 1) == context.PaletteManager.UniquePalettes.Count) {
                    binaryWriter.WriteUInt32((uint)(0x80000000 | (palette.Colors.Length * sizeof(UInt16))));
                } else {
                    paletteChainReference = binaryWriter.WriteDeferredReference();
                }

                binaryWriter.WriteColors(palette.Colors);
            }
        }

        private static void WriteExtendedPolygonalStructure(S3DBinaryWriter binaryWriter, S3DObjectWriteContext objectWriteContext) {
            // XPDATA -> 24B
            //   POINT *   pntbl              4B offset
            objectWriteContext.VerticesReference = binaryWriter.WriteDeferredReference();
            //   Uint32    nbPoint            4B
            binaryWriter.WriteUInt32(objectWriteContext.Object.Vertices.Count);
            //   POLYGON * pltbl              4B offset
            objectWriteContext.FacesReference = binaryWriter.WriteDeferredReference();
            //   Uint32    nbPolygon          4B
            binaryWriter.WriteUInt32(objectWriteContext.Object.Faces.Count);
            //   ATTR *    attbl              4B offset
            objectWriteContext.FaceAttributesReference = binaryWriter.WriteDeferredReference();
            //   VECTOR *  vntbl              4B offset
            objectWriteContext.VertexNormalsReference = binaryWriter.WriteDeferredReference();

            // Extended -> 16B
            //   void *   PICTURE data        4B offset
            objectWriteContext.PicturesReference = binaryWriter.WriteDeferredReference();
            //   Uint32   PICTURE count       4B
            binaryWriter.WriteUInt32(objectWriteContext.Object.PictureCount);

            //   void *   Gouraud tables      4B offset
            objectWriteContext.GouraudShadingTablesReference = binaryWriter.WriteDeferredReference();

            //   Uint16   Gouraud tables size 4B
            binaryWriter.WriteUInt32(objectWriteContext.Object.GouraudShadingCount);
        }

        private static void WriteTextureData(S3DBinaryWriter binaryWriter, Context context) {
            // Use this to chain textures
            IWriteReference textureChainReference = null;

            for (int index = 0; index < context.TextureManager.UniqueTextures.Count; index++) {
                Texture texture = context.TextureManager.UniqueTextures[index];

                var textureReferences =
                    context.TextureWriteReferenceManager.GetDeferredReferences(texture);

                foreach (IWriteReference writeReference in textureReferences) {
                    binaryWriter.WriteReferenceOffset(writeReference);
                }

                if (textureChainReference != null) {
                    binaryWriter.WriteReferenceOffset(textureChainReference);
                }

                // If this is the last texture in the list, then set a flag too
                // denote the size
                if ((index + 1) == context.TextureManager.UniqueTextures.Count) {
                    binaryWriter.WriteUInt32((int)(0x80000000 | texture.VDP1Data.Data.Length));
                } else {
                    textureChainReference = binaryWriter.WriteDeferredReference();
                }

                binaryWriter.WriteVDP1Data(texture.VDP1Data);
            }
        }

        private static void WriteTextureStructure(S3DBinaryWriter binaryWriter, Context context) {
            foreach (Texture texture in context.TextureManager.UniqueTextures) {
                //   Uint16 Hsize  2B width
                binaryWriter.WriteUInt16((UInt16)texture.VDP1Data.Width);
                //   Uint16 Vsize  2B height
                binaryWriter.WriteUInt16((UInt16)texture.VDP1Data.Height);
                //   Uint16 CGadr  2B texture VRAM number
                binaryWriter.WriteUInt16(0);
                binaryWriter.WriteUInt16((UInt16)((texture.VDP1Data.Width << 5) | texture.VDP1Data.Height));
            }
        }

        private static void WritePictureStructure(S3DBinaryWriter binaryWriter, S3DObjectWriteContext objectWriteContext, Context context) {
            if (objectWriteContext.Object.PictureCount == 0) {
                return;
            }

            binaryWriter.WriteReferenceOffset(objectWriteContext.PicturesReference);

            foreach (S3DFace face in objectWriteContext.Object.Faces) {
                if (face.Picture != null) {
                    Picture picture = face.Picture;

                    // PICTURE -> 8B

                    //   Uint16 texno 2B texture number (in texture list)
                    binaryWriter.WriteUInt16((UInt16)picture.Texture.SlotNumber);

                    //   Uint16 cmode 2B color mode
                    binaryWriter.WriteUInt16(0); // XXX: Fix (COL_32K = 2 - 1)

                    //   Uint32 pcsrc 4B offset
                    IWriteReference textureDataReference =
                        binaryWriter.WriteDeferredReference();

                    context.TextureWriteReferenceManager.AddDeferredReference(picture.Texture, textureDataReference);
                }
            }
        }

        private static void WriteGouraudShadingTablesData(S3DBinaryWriter binaryWriter, S3DObjectWriteContext objectWriteContext) {
            if (objectWriteContext.Object.GouraudShadingCount == 0) {
                return;
            }

            binaryWriter.WriteReferenceOffset(objectWriteContext.GouraudShadingTablesReference);

            foreach (S3DFace face in objectWriteContext.Object.Faces) {
                if (face.FeatureFlags.HasFlag(S3DFaceAttribs.FeatureFlags.UseGouraudShading)) {
                    binaryWriter.WriteColors(face.GouraudShadingColors);
                }
            }
        }

        private static void WriteVectorNormalsData(S3DBinaryWriter binaryWriter, S3DObjectWriteContext objectWriteContext) {
            binaryWriter.WriteReferenceOffset(objectWriteContext.VertexNormalsReference);

            foreach (Vector3 normal in objectWriteContext.Object.VertexNormals) {
                binaryWriter.WriteVector(normal);
            }
        }

        private static void WriteFaceAttributesData(S3DBinaryWriter binaryWriter, S3DObjectWriteContext objectWriteContext, Context context) {
            binaryWriter.WriteReferenceOffset(objectWriteContext.FaceAttributesReference);

            foreach (S3DFace face in objectWriteContext.Object.Faces) {
                S3DFaceAttribStruct faceAttribStruct = CreateS3DFaceAttribsStruct(face);

                binaryWriter.WriteFaceAttributes(faceAttribStruct);
            }
        }

        private static void WriteFaceIndicesData(S3DBinaryWriter binaryWriter, S3DObjectWriteContext objectWriteContext) {
            binaryWriter.WriteReferenceOffset(objectWriteContext.FacesReference);

            foreach (S3DFace face in objectWriteContext.Object.Faces) {
                // Vertex normal (12B)
                binaryWriter.WriteVector(face.Normal);

                // Indices (8B)
                binaryWriter.WriteUInt16((UInt16)face.Indices[0]);
                binaryWriter.WriteUInt16((UInt16)face.Indices[1]);
                binaryWriter.WriteUInt16((UInt16)face.Indices[2]);
                binaryWriter.WriteUInt16((UInt16)face.Indices[3]);
            }
        }

        private static void WriteVerticesData(S3DBinaryWriter binaryWriter, S3DObjectWriteContext objectWriteContext) {
            binaryWriter.WriteReferenceOffset(objectWriteContext.VerticesReference);

            foreach (Vector3 vertex in objectWriteContext.Object.Vertices) {
                binaryWriter.WriteVector(vertex);
            }
        }

        // XXX: Refactor this
        private static S3DFaceAttribStruct CreateS3DFaceAttribsStruct(S3DFace face) {
            S3DFaceAttribStruct faceAttribStruct = new S3DFaceAttribStruct();

            faceAttribStruct.Flag = (byte)face.PlaneType;

            faceAttribStruct.Sort = (byte)face.SortType;
            faceAttribStruct.FeatureFlags = (byte)face.FeatureFlags;

            faceAttribStruct.RenderFlags = (UInt16)face.RenderFlags;
            faceAttribStruct.ColorCalculationMode = (UInt16)face.ColorCalculationMode;
            faceAttribStruct.TextureType = (UInt16)face.TextureType;

            if (face.FeatureFlags.HasFlag(S3DFaceAttribs.FeatureFlags.UseTexture)) {
                if (face.Picture != null) {
                    Picture picture = face.Picture;

                    faceAttribStruct.TextureNumber = (UInt16)picture.Texture.SlotNumber;

                    if (picture.Palette != null) {
                        faceAttribStruct.PaletteNumberOrRGB1555 = (UInt16)picture.Palette.SlotNumber;
                    }
                }
            } else {
                // XXX: Fix
                faceAttribStruct.PaletteNumberOrRGB1555 = 0x83C0;
            }

            if (face.FeatureFlags.HasFlag(S3DFaceAttribs.FeatureFlags.UseGouraudShading)) {
                faceAttribStruct.GouraudShadingNumber = (UInt16)face.GouraudShadingNumber;
            }

            faceAttribStruct.PrimitiveType = (UInt16)face.PrimitiveType;
            faceAttribStruct.Dir = (UInt16)face.TextureFlipFlags;

            return faceAttribStruct;
        }
    }
}
