using OpenTK.Mathematics;

namespace S3D.UI.MathUtilities.Raycasting {
    public interface ICollider {
        public Vector3[] Vertices { get; }

        public Vector3[] Normals { get; }
    }
}
