using S3D.Types;
using System.Collections.Generic;
using System.Linq;
using System;

namespace S3D.TextureManagement {
    public sealed partial class PaletteManager {
        private int _slotNumber;

        private readonly List<IPalette> _uniquePalettes = new List<IPalette>();

        public IReadOnlyList<IPalette> UniquePalettes => _uniquePalettes;

        public IPalette GetOrAddPalette(RGB1555[] paletteColors) {
            if (paletteColors == null) {
                return null;
            }

            IPalette existingPalette = FindPalette(paletteColors);

            if (existingPalette != null) {
                return existingPalette;
            }

            Palette palette = new() {
                Colors     = paletteColors.ToArray(),
                SlotNumber = AllocateSlotNumber()
            };

            _uniquePalettes.Add(palette);

            return palette;
        }

        private IPalette FindPalette(RGB1555[] paletteColors) {
            if (paletteColors == null) {
                throw new ArgumentNullException();
            }

            return _uniquePalettes.Find(Predicate);

            bool Predicate(IPalette x) {
                return x.Colors.SequenceEqual(paletteColors);
            }
        }

        private int AllocateSlotNumber() {
            return _slotNumber++;
        }
    }
}
