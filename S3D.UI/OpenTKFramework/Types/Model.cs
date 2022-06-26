using OpenTK.Mathematics;
using S3D.FileFormats;

namespace S3D.UI.OpenTKFramework.Types {
    public class Model {
        public string Name { get; } = string.Empty;

        public S3DObject[] Objects { get; set; }

        public Mesh[] Meshes { get; set; }

        public Matrix4 Transform { get; set; } = Matrix4.Identity;
    }
}
