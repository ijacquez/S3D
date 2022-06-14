using S3D.Types;
using System.Collections.Generic;
using System.Linq;
using System;

namespace S3D.PaletteManagement {
    public sealed partial class PaletteManager {
        private int _slotNumber;

        private readonly List<Palette> _uniquePalettes = new List<Palette>();

        public IReadOnlyList<Palette> UniquePalettes => _uniquePalettes;

        public Palette GetOrAddPalette(RGB1555[] paletteColors) {
            if (paletteColors == null) {
                return null;
            }

            Palette existingPalette = FindPalette(paletteColors);

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

        private Palette FindPalette(RGB1555[] paletteColors) {
            if (paletteColors == null) {
                throw new ArgumentNullException();
            }

            return _uniquePalettes.Find(Predicate);

            bool Predicate(Palette x) {
                return x.Colors.SequenceEqual(paletteColors);
            }
        }

        private int AllocateSlotNumber() {
            return _slotNumber++;
        }
    }
}
