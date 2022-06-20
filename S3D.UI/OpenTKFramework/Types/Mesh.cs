using OpenTK.Mathematics;
using S3D.UI.MathUtilities.Raycasting;

namespace S3D.UI.OpenTKFramework.Types {
    public class Mesh : ICollider {
        public string Name { get; set; } = string.Empty;

        public Vector3[] Vertices { get; set; }

        public Vector2[] Texcoords { get; set; }

        public Color4[] GSColors { get; set; }

        public Color4[] BaseColors { get; set; }

        public Vector3[] Normals { get; set; }

        public uint[] Indices { get; set; }

        public Texture Texture { get; set; }

        // XXX: Remove
        public float[] LineVertices { get; set; }
    }
}
