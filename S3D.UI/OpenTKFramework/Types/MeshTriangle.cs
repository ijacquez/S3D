using OpenTK.Mathematics;

namespace S3D.UI.OpenTKFramework.Types {
    public class MeshTriangle {
        public Vector3[] Vertices { get; } = new Vector3[3];

        public Vector2[] Texcoords { get; } = new Vector2[3];

        public Color4[] Colors { get; } = new Color4[3];
    }
}
