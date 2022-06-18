using OpenTK.Mathematics;

namespace S3D.UI.MathUtilities.Raycasting {
    public struct RaycastHitInfo {
        public ICollider Collider { get; set; }

        public uint TriangleIndex { get; set; }
    }
}
