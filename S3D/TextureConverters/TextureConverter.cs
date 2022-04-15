using KGySoft.Drawing.Imaging;
using KGySoft.Drawing;
using MiscUtil.Conversion;
using MiscUtil.IO;
using S3D.Types;
using S3D.Utilities;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Numerics;
using System;
using System.Linq;

namespace S3D.TextureConverters {
    public static class TextureConverter {
        private static int _TextureConversionIndex = 0;

        public static VDP1Data ToTexture(string fileName, TextureConverterParameters parameters) {
            Image image = Image.FromFile(fileName);

            var sanitizedParameters = new TextureConverterParameters(parameters);

            SanitizeTargetWidthAndHeight(image, sanitizedParameters);

            switch (image.PixelFormat) {
                case PixelFormat.Format4bppIndexed:
                    return ToFormat4BPPIndexed(image, sanitizedParameters);
                case PixelFormat.Format8bppIndexed:
                    return ToFormat8BPPIndexed(image, sanitizedParameters);
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppRgb555:
                    return ToFormat15BPPRGB(image, sanitizedParameters);
                default:
                    return ToFormatUnknown(image, sanitizedParameters);
            }
        }

        #region Input options sanization

        private static void SanitizeTargetWidthAndHeight(Image image, TextureConverterParameters parameters) {
            int targetWidth = parameters.TargetWidth;
            int targetHeight = parameters.TargetHeight;

            targetWidth = (targetWidth < 0) ? image.Width : targetWidth;
            targetHeight = (targetHeight < 0) ? image.Height : targetHeight;

            if ((targetWidth < 8) || (targetWidth > 504)) {
                throw new InvalidDataException("Width must be between 8 and 504 pixels");
            }

            if ((targetWidth % 8) != 0) {
                throw new InvalidDataException("Width must be a multiple of 8 pixels");
            }

            if (targetHeight == 0) {
                throw new InvalidDataException("Height must be at least 1 pixel");
            }

            if (targetHeight > 255) {
                throw new InvalidDataException("Height must be less than or equal to 255");
            }

            parameters.TargetWidth = targetWidth;
            parameters.TargetHeight = targetHeight;
        }

        #endregion

        private static VDP1Data ToFormatUnknown(Image image, TextureConverterParameters parameters) {
            using (Bitmap newBitmap = new Bitmap((Bitmap)image)) {
                Rectangle rect = new Rectangle(0, 0, newBitmap.Width, newBitmap.Height);

                using (Bitmap targetBitmap = newBitmap.Clone(rect, PixelFormat.Format16bppArgb1555)) {
                    return ToFormat15BPPRGB(targetBitmap, parameters);
                }
            }
        }

        private static VDP1Data ToFormatRGB(Image image, Vector2[] textureVertices) {
            // Quantize to ARGB1555 (A=MSB)
            throw new NotImplementedException();
        }

        private static VDP1Data ToFormat15BPPRGB(Image image, TextureConverterParameters parameters) {
            Bitmap bitmap = (Bitmap)image;

            Bitmap generatedBitmap = GenerateBitmap(bitmap, parameters);
            byte[] textureBytes = ConvertToSaturn15BPPIndexed(generatedBitmap, parameters);

            DumpIntermediateFiles(generatedBitmap, parameters);

            return new VDP1Data(TextureType.RGB1555,
                                parameters.TargetWidth,
                                parameters.TargetHeight,
                                textureBytes,
                                null);
        }

        private static VDP1Data ToFormat4BPPIndexed(Image image, TextureConverterParameters parameters) {
            Bitmap bitmap = (Bitmap)image;

            Bitmap generatedBitmap = GenerateBitmap(bitmap, parameters);
            byte[] textureBytes = ConvertToSaturn4BPPIndexed(generatedBitmap, parameters);
            RGB1555[] palette = ConvertToRGB1555Palette(ExtractPalette(generatedBitmap.Palette));

            DumpIntermediateFiles(generatedBitmap, parameters);

            return new VDP1Data(TextureType.Indexed16,
                                parameters.TargetWidth,
                                parameters.TargetHeight,
                                textureBytes,
                                palette);
        }

        private static VDP1Data ToFormat8BPPIndexed(Image image, TextureConverterParameters parameters) {
            Bitmap bitmap = (Bitmap)image;

            Bitmap generatedBitmap = GenerateBitmap(bitmap, parameters);
            byte[] textureBytes = ConvertToSaturn8BPPIndexed(generatedBitmap, parameters);
            RGB1555[] palette = ConvertToRGB1555Palette(ExtractPalette(generatedBitmap.Palette));

            DumpIntermediateFiles(generatedBitmap, parameters);

            TextureType textureType;

            if (palette.Length == 64) {
                textureType = TextureType.Indexed64;
            } else if (palette.Length == 128) {
                textureType = TextureType.Indexed128;
            } else {
                textureType = TextureType.Indexed256;
            }

            return new VDP1Data(textureType,
                                parameters.TargetWidth,
                                parameters.TargetHeight,
                                textureBytes,
                                palette);
        }

        #region Debugging

        private static void DumpIntermediateFiles(Bitmap writeBitmap, TextureConverterParameters parameters) {
            if (!parameters.DumpFile) {
                return;
            }

            writeBitmap.SaveAsPng($"out_{_TextureConversionIndex:D04}.png");

            _TextureConversionIndex++;
        }

        #endregion

        #region Saturn specific conversions

        private static byte[] ConvertToSaturn4BPPIndexed(Bitmap bitmap, TextureConverterParameters parameters) {
            int size = (parameters.TargetWidth * parameters.TargetHeight) / 2;
            byte[] bytes = new byte[size];

            using (var readableBitmapData = bitmap.GetReadableBitmapData()) {
                for (int y = 0; y < parameters.TargetHeight; y++) {
                    IReadableBitmapDataRow row = readableBitmapData[y];

                    for (int x = 0; x < (parameters.TargetWidth / 2); x++) {
                        int offset = x + (y * (parameters.TargetWidth / 2));

                        byte index0 = (byte)row.GetColorIndex((2 * x) + 1);
                        byte index1 = (byte)row.GetColorIndex(2 * x);

                        bytes[offset] = (byte)((index1 << 4) | index0);
                    }
                }
            }

            return bytes;
        }

        private static byte[] ConvertToSaturn8BPPIndexed(Bitmap bitmap, TextureConverterParameters parameters) {
            byte[] bytes = new byte[parameters.TargetWidth * parameters.TargetHeight];

            using (var readableBitmapData = bitmap.GetReadableBitmapData()) {
                for (int y = 0; y < parameters.TargetHeight; y++) {
                    IReadableBitmapDataRow row = readableBitmapData[y];

                    for (int x = 0; x < parameters.TargetWidth; x++) {
                        int offset = x + (y * parameters.TargetWidth);

                        bytes[offset] = (byte)row.GetColorIndex(x);
                    }
                }
            }

            return bytes;
        }

        private static byte[] ConvertToSaturn15BPPIndexed(Bitmap bitmap, TextureConverterParameters parameters) {
            byte[] bytes = new byte[parameters.TargetWidth * parameters.TargetHeight * 2];

            var memoryStream = new MemoryStream(bytes);
            var binaryWriter = new EndianBinaryWriter(EndianBitConverter.Big, memoryStream);

            using (var readableBitmapData = bitmap.GetReadableBitmapData()) {
                for (int y = 0; y < parameters.TargetHeight; y++) {
                    IReadableBitmapDataRow row = readableBitmapData[y];

                    for (int x = 0; x < parameters.TargetWidth; x++) {
                        Color color = row.GetColor(x);

                        binaryWriter.Write((RGB1555)color);
                    }
                }
            }

            memoryStream.Dispose();
            binaryWriter.Dispose();

            return bytes;
        }

        private static RGB1555[] ConvertToRGB1555Palette(Color[] colors) {
            return colors.Select((color) => (RGB1555)color).ToArray();
        }

        #endregion

        private static Color[] ExtractPalette(ColorPalette colorPalette) {
            int paletteColorCount = colorPalette.Entries.Length;

            if (paletteColorCount <= 16) {
                paletteColorCount = 16;
            } else if (paletteColorCount <= 64) {
                paletteColorCount = 64;
            } else if (paletteColorCount <= 128) {
                paletteColorCount = 128;
            } else {
                paletteColorCount = 256;
            }

            Color[] palette = new Color[paletteColorCount];

            Array.Copy(colorPalette.Entries, palette, Math.Min(paletteColorCount, colorPalette.Entries.Length));

            return palette;
        }

        #region Texture sampling

        private static int GetBitmapX(int width, float u) {
            float uClamped = TextureUtility.ClampVectorComponent(u);

            return ((int)(width * uClamped));
        }

        private static int GetBitmapY(int height, float v) {
            float vClamped = 1.0f - TextureUtility.ClampVectorComponent(v);

            return ((int)(height * vClamped));
        }

        private static Color SampleColor(IReadableBitmapData bitmapData, float u, float v) {
            int bitmapX = GetBitmapX(bitmapData.Width - 1, u);
            int bitmapY = GetBitmapY(bitmapData.Height - 1, v);

            return bitmapData[bitmapY].GetColor(bitmapX);
        }

        private static int SampleColorIndex(IReadableBitmapData bitmapData, float u, float v) {
            int bitmapX = GetBitmapX(bitmapData.Width - 1, u);
            int bitmapY = GetBitmapY(bitmapData.Height - 1, v);

            return bitmapData[bitmapY].GetColorIndex(bitmapX);
        }

        private static Bitmap GenerateBitmap(Bitmap bitmap, TextureConverterParameters parameters) {
            Bitmap writeBitmap = new Bitmap(parameters.TargetWidth,
                                            parameters.TargetHeight,
                                            bitmap.PixelFormat);

            if (IsBitmapIndexed(bitmap)) {
                writeBitmap.Palette = bitmap.Palette;
            }

            IReadableBitmapData inputReadableBitmapData =
                bitmap.GetReadableBitmapData();

            IWritableBitmapData writableBitmapData =
                writeBitmap.GetWritableBitmapData();

            for (int y = 0; y < parameters.TargetHeight; y++) {
                IWritableBitmapDataRow row =
                    writableBitmapData[(parameters.TargetHeight - 1) - y];

                // Take into account target heights 1 pixel
                float tY = y / (float)Math.Max(1, parameters.TargetHeight - 1);

                for (int x = 0; x < parameters.TargetWidth; x++) {
                    float tX = x / (float)(parameters.TargetWidth - 1);

                    Vector2 c = MathUtility.Blerp(parameters.TextureVertices, tX, tY);

                    if (IsBitmapIndexed(bitmap)) {
                        int colorIndex = SampleColorIndex(inputReadableBitmapData, c.X, c.Y);

                        row.SetColorIndex(x, colorIndex);
                    } else {
                        Color color = SampleColor(inputReadableBitmapData, c.X, c.Y);

                        row.SetColor(x, color);
                    }
                }
            }

            inputReadableBitmapData.Dispose();
            writableBitmapData.Dispose();

            return writeBitmap;
        }

        private static bool IsBitmapIndexed(Bitmap bitmap) {
            return ((bitmap.PixelFormat == PixelFormat.Indexed) ||
                    (bitmap.PixelFormat == PixelFormat.Format4bppIndexed) ||
                    (bitmap.PixelFormat == PixelFormat.Format8bppIndexed));
        }

        #endregion
    }
}
