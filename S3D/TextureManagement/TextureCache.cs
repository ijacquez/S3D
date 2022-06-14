using Blake2Fast;
using System.Collections.Generic;
using System.Linq;
using System;

namespace S3D.TextureManagement {
    public sealed class TextureCache {
        private class Entry {
            public Texture Texture { get; set; }

            public byte[] Hash { get; set; }
        }

        private const int _HashDigestLength = 64;

        private int _textureSlotNumber;

        private readonly List<Entry> _uniqueEntries = new List<Entry>();

        public TextureCache() {
        }

        /// <summary>
        /// </summary>
        public IReadOnlyList<Texture> UniqueTextures =>
            _uniqueEntries.Select((entry) => entry.Texture)
                          .ToList()
                          .AsReadOnly();

        /// <summary>
        /// </summary>
        public bool ContainsTexture(Texture texture) {
            byte[] computedHash = GenerateHashFromTexture(texture);

            Entry entry = GetEntry(computedHash);

            return (entry != null);
        }

        /// <summary>
        /// </summary>
        public Texture GetOrAddTexture(Texture texture) {
            byte[] computedHash = GenerateHashFromTexture(texture);

            Entry entry = GetEntry(computedHash);

            if (entry != null) {
                return entry.Texture;
            }

            entry = new Entry();

            entry.Texture = texture;
            entry.Hash = computedHash;

            if (entry.Texture.SlotNumber < 0) {
                entry.Texture.SlotNumber = AllocateTextureSlotNumber();
            } else {
                SetTextureSlotNumber(entry.Texture.SlotNumber);
            }

            _uniqueEntries.Add(entry);

            return texture;
        }

        private Entry GetEntry(byte[] hash) {
            return _uniqueEntries.Find(PredicateFindEntry);

            bool PredicateFindEntry(Entry x) {
                return x.Hash.SequenceEqual(hash);
            }
        }

        private void SetTextureSlotNumber(int slotNumber) {
            _textureSlotNumber = Math.Max(slotNumber, _textureSlotNumber);;
        }

        private int AllocateTextureSlotNumber() {
            return _textureSlotNumber++;
        }

        private static byte[] GenerateHashFromTexture(Texture texture) {
            return Blake2b.ComputeHash(_HashDigestLength, texture.VDP1Data.Data);
        }
    }
}
