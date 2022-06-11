using S3D.Types;
using System.Collections.Generic;
using System.Linq;
using System;

namespace S3D.TextureConverters {
    public class VDP1Data {
        public VDP1DataType Type { get; }

        public byte[] Data { get; }

        public int Width { get; }

        public int Height { get; }

        public RGB1555[] Palette { get; }

        private VDP1Data() {
        }

        public VDP1Data(VDP1DataType type, int width, int height, byte[] data, IEnumerable<RGB1555> palette) {
            Type = type;
            Width = width;
            Height = height;
            Data = data.ToArray();

            // If the texture type is indexed, copy (and pad if needed) the
            // palette
            if (type != VDP1DataType.RGB1555) {
                int paletteColorCount = GetPaletteColorCount(type);

                Palette = new RGB1555[paletteColorCount];

                RGB1555[] paletteArray = palette?.ToArray();

                Array.Copy(paletteArray, Palette, Math.Min(paletteColorCount, paletteArray.Length));
            }
        }

        private static int GetPaletteColorCount(VDP1DataType type) {
            switch (type) {
                case VDP1DataType.Indexed16:
                    return 16;
                case VDP1DataType.Indexed64:
                    return 64;
                case VDP1DataType.Indexed128:
                    return 128;
                case VDP1DataType.Indexed256:
                    return 256;
                case VDP1DataType.RGB1555:
                default:
                    return 0;
            }
        }
    }
}
