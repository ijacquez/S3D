using OpenTK.Mathematics;
using S3D.UI.OpenTKFramework.Types;

namespace S3D.UI.MeshUtilities {
    public static class MeshGenerator {
        private static int _GenerationID = -1;

        public static Mesh GenerateTriangle(Vector3 p0, Vector3 p1, Vector3 p2) {
            _GenerationID++;

            Mesh mesh = new Mesh();

            mesh.Name = $"mesh_triangle_{_GenerationID}";

            MeshPrimitive meshPrimitive = MeshPrimitive.CreateTriangle();

            meshPrimitive.SetVertices(p0, p1, p2);

            meshPrimitive.CalculateNormal();

            mesh.AddPrimitive(meshPrimitive);

            return mesh;
        }

        public static Mesh GenerateQuad(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            _GenerationID++;

            Mesh mesh = new Mesh();

            mesh.Name = $"mesh_quad_{_GenerationID}";

            MeshPrimitive meshPrimitive = MeshPrimitive.CreateQuad();

            meshPrimitive.SetVertices(p0, p1, p2, p3);

            meshPrimitive.CalculateNormal();

            mesh.AddPrimitive(meshPrimitive);

            return mesh;
        }
    }
}
