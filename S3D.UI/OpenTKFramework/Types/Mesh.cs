using OpenTK.Mathematics;
using S3D.UI.MathUtilities.Raycasting;

namespace S3D.UI.OpenTKFramework.Types {
    public class Mesh : ICollider {
        public string Name { get; set; } = string.Empty;

        public Vector3[] Vertices { get; set; }

        public Vector2[] Texcoords { get; set; }

        public Color4[] Colors { get; set; }

        public Vector3[] Normals { get; set; }

        public uint[] Indices { get; set; }

        // XXX: Remove
        public float[] Vertices_Remove { get; set; }

        public Texture Texture { get; set; }
    }
}
