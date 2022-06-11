using S3D.Extensions;
using S3D.TextureConverters;
using S3D.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System;
using Blake2Fast;

namespace S3D.TextureManagement {
    public sealed class TextureCache {
        // private readonly HashSet<Texture> _uniqueTextureHashSet = new HashSet<Texture>();
        private readonly List<ITexture> _uniqueTextures = new List<ITexture>();

        public TextureCache() {
        }

        // private void UpdateUniqueTexturesCache(TextureData textureData) {
        //     Texture texture = textureData.Texture;
        //
        //     if (!_uniqueTextureHashSet.Contains(texture)) {
        //         _uniqueTextureHashSet.Add(texture);
        //
        //         _uniqueTextures.Add(textureData);
        //     }
        // }
    }
}
