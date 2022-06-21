using System;

namespace S3D.UI.OpenTKFramework.Types {
    /// <summary>
    ///   Flags per primitive. These flags must be kept in sync with the
    ///   vertex/fragment shaders.
    /// </summary>
    [Flags]
    public enum MeshTriangleFlags : uint {
        None           = 0,
        Quadrangle     = 1U << 0,
        Textured       = 1U << 3,
        GouraudShaded  = 1U << 4,

        Selected      = 1U << 31
    }
}
