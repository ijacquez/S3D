using System.Collections.Generic;
using System.Linq;
using System;
using Blake2Fast;

namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        private sealed class TextureCache {
            private class Entry {
                public ITexture Texture { get; set; }

                public byte[] Hash { get; set; }
            }

            private const int _HashDigestLength = 64;

            private int _textureSlotNumber;

            private readonly List<Entry> _uniqueEntries = new List<Entry>();

            public TextureCache() {
            }

            public IReadOnlyList<ITexture> UniqueTextures =>
                _uniqueEntries.Select((entry) => entry.Texture)
                              .ToList()
                              .AsReadOnly();

            public ITexture GetOrAddTexture(ITexture texture) {
                byte[] computedHash = Blake2b.ComputeHash(_HashDigestLength, texture.VDP1Data.Data);

                Console.WriteLine($"{Convert.ToHexString(computedHash)}");

                Entry entry = _uniqueEntries.Find(PredicateFindTexture);

                if (entry != null) {
                    return entry.Texture;
                }

                entry = new Entry();

                var baseTexture = (Texture)texture;

                baseTexture.SlotNumber = AllocateTextureSlotNumber();

                entry.Texture = texture;
                entry.Hash = computedHash;

                _uniqueEntries.Add(entry);

                return texture;

                bool PredicateFindTexture(Entry x) {
                    return x.Hash.SequenceEqual(computedHash);
                }
            }

            private int AllocateTextureSlotNumber() {
                return _textureSlotNumber++;
            }
        }
    }
}
