using OpenTK.Mathematics;

namespace S3D.UI.OpenTKFramework.Types {
    public class MeshTriangle {
        private static readonly Color4 _DefaultGouraudShadingColor = new Color4(0.5f, 0.5f, 0.5f, 1.0f);

        public Vector3[] Vertices { get; } = new Vector3[3];

        public Vector2[] Texcoords { get; } = new Vector2[3];

        public Color4[] Colors { get; } = new Color4[3] {
            _DefaultGouraudShadingColor,
            _DefaultGouraudShadingColor,
            _DefaultGouraudShadingColor
        };
    }
}
