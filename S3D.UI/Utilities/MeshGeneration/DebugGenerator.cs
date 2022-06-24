using OpenTK.Mathematics;
using S3D.UI.OpenTKFramework.Types;

namespace S3D.UI.MeshUtilities {
    public static class DebugMeshGenerator {
        private static int _GenerationID = -1;

        public static Mesh GenerateTriangle(Vector3 p0, Vector3 p1, Vector3 p2) {
            _GenerationID++;

            Mesh mesh = new Mesh();

            mesh.Name = $"debug_triangle_{_GenerationID}";

            MeshPrimitive meshPrimitive = MeshPrimitive.CreateTriangle();

            meshPrimitive.SetVertices(p0, p1, p2);

            meshPrimitive.CalculateNormal();

            mesh.AddPrimitive(meshPrimitive);

            return mesh;
        }
    }
}
