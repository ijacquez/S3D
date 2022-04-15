using System;

namespace S3D.TextureManagement {
    public sealed partial class TextureManager {
        [Flags]
        private enum TextureFlags {
            None                = 0,
            FlippedHorizontally = 1 << 0,
            FlippedVertically   = 1 << 1
        }
    }
}
