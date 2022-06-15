using OpenTK.Mathematics;

namespace S3D.UI.OpenTKFramework.Types {
    public class Mesh {
        public string Name { get; set; } = string.Empty;

        public Vector3[] Vertices2 { get; set; }

        public float[] Vertices { get; set; }

        public float[] Normals { get; set; }

        public uint[] Indices { get; set; }
    }
}
