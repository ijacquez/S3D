using S3D.FileFormats;
using S3D.UI.OpenTKFramework.Types;

namespace S3D.UI.Views {
    public class FaceData {
        public S3DObject Object { get; private set; }

        public S3DFace Face { get; private set; }

        public MeshPrimitive MeshPrimitive { get; private set; }

        public void SetData(S3DObject s3dObject, Mesh mesh, int index) {
            Object = s3dObject;
            Face = Object.Faces[index];
            MeshPrimitive = mesh.Primitives[index];
        }
    }
}
