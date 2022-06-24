using System;
using OpenTK.Mathematics;

namespace S3D.UI.OpenTKFramework.Types {
    public class MeshPrimitive {
        private static readonly Color4 _DefaultBaseColor = new Color4(1.0f, 0.0f, 1.0f, 1.0f);

        public MeshTriangleFlags Flags { get; set; }

        public MeshTriangle[] Triangles { get; }

        public Color4 BaseColor { get; set; } = _DefaultBaseColor;

        public Vector3 Normal { get; set; }

        private MeshPrimitive() {
        }

        private MeshPrimitive(int triangleCount) {
            Triangles = new MeshTriangle[triangleCount];

            for (int i = 0; i < triangleCount; i++) {
                Triangles[i] = new MeshTriangle();
            }

            if (triangleCount == 2) {
                Flags |= MeshTriangleFlags.Quadrangle;
            }
        }

        public static MeshPrimitive CreateTriangle() {
            var meshPrimitive = new MeshPrimitive(1);

            return meshPrimitive;
        }

        public static MeshPrimitive CreateQuad() {
            var meshPrimitive = new MeshPrimitive(2);

            return meshPrimitive;
        }

        public void SetVertices(Vector3 p0, Vector3 p1, Vector3 p2) {
            Triangles[0].Vertices[0] = p1;
            Triangles[0].Vertices[1] = p2;
            Triangles[0].Vertices[2] = p0;
        }

        public void SetTexcoords(Vector2 t0, Vector2 t1, Vector2 t2) {
            Triangles[0].Texcoords[0] = t2;
            Triangles[0].Texcoords[1] = t1;
            Triangles[0].Texcoords[2] = t0;
        }

        public void SetGouraudShading(Color4 c0, Color4 c1, Color4 c2) {
            Triangles[0].Colors[0] = c0;
            Triangles[0].Colors[1] = c1;
            Triangles[0].Colors[2] = c2;
        }

        public void SetVertices(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            Triangles[0].Vertices[0] = p1;
            Triangles[0].Vertices[1] = p2;
            Triangles[0].Vertices[2] = p3;

            Triangles[1].Vertices[0] = p1;
            Triangles[1].Vertices[1] = p3;
            Triangles[1].Vertices[2] = p0;
        }

        public void SetTexcoords(Vector2 t0, Vector2 t1, Vector2 t2, Vector2 t3) {
            Triangles[0].Texcoords[0] = t2;
            Triangles[0].Texcoords[1] = t1;
            Triangles[0].Texcoords[2] = t0;

            Triangles[1].Texcoords[0] = t2;
            Triangles[1].Texcoords[1] = t0;
            Triangles[1].Texcoords[2] = t3;
        }

        public void SetGouraudShading(Color4 c0, Color4 c1, Color4 c2, Color4 c3) {
            Triangles[0].Colors[0] = c2;
            Triangles[0].Colors[1] = c1;
            Triangles[0].Colors[2] = c0;

            Triangles[1].Colors[0] = c2;
            Triangles[1].Colors[1] = c0;
            Triangles[1].Colors[2] = c3;
        }

        public void CalculateNormal() {
            Vector3 a = Triangles[0].Vertices[0];
            Vector3 b = Triangles[0].Vertices[1];
            Vector3 c = Triangles[0].Vertices[2];

            Normal = Vector3.Cross(c - a, b - a).Normalized();

            Console.WriteLine($"{a}, {b}, {c} => {Normal}");
        }
    }
}
