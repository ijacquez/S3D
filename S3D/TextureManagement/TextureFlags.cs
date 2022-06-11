using System;

namespace S3D.TextureManagement {
    [Flags]
    public enum TextureFlags {
        None                = 0,
        FlippedHorizontally = 1 << 0,
        FlippedVertically   = 1 << 1
    }
}
