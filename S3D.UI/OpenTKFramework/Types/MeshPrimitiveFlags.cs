using System;

namespace S3D.UI.OpenTKFramework.Types {
    /// <summary>
    ///   Flags per primitive. These flags must be kept in sync with the
    ///   vertex/fragment shaders.
    /// </summary>
    [Flags]
    public enum MeshPrimitiveFlags : uint {
        None            = 0,
        Quadrangle      = 1U << 0,
        Textured        = 1U << 1,
        Wired           = 1U << 2,
        GouraudShaded   = 1U << 3,
        HalfLuminance   = 1U << 4,
        HalfTransparent = 1U << 5,
        Meshed          = 1U << 6,

        Selected        = 1U << 31
    }
}
