using OpenTK.Mathematics;

namespace S3D.UI.MathUtilities.Raycasting {
    public struct RaycastHitInfo {
            public Vector3 Point { get; set; }

            public float Distance { get; set; }

            public uint PrimitiveIndex { get; set; }
    }
}
