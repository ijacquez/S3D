using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using S3D.FileFormats;
using S3D.UI.OpenTKFramework.Types;
using System.Drawing;

namespace S3D.UI.MeshUtilities {
    public static class S3DMeshGenerator {
        private static readonly Matrix3 _TransformRotation =
            new Matrix3(1.0f,  0.0f,  0.0f,
                        0.0f, -1.0f,  0.0f,
                        0.0f,  0.0f, -1.0f);

        /// <summary>
        ///   Generate p0 <see cref="Mesh"/> from p0 <see cref="S3DObject"/>.
        /// </summary>
        public static Mesh Generate(S3DObject s3dObject) {
            var objectVertices = s3dObject.Vertices.AsReadOnly();

            // XXX: Change texture name
            Texture texture = new Texture("mesh_generation",
                                          Bitmap.FromFile("map_d03_01.png") as Bitmap,
                                          generateMipmaps: false,
                                          srgb: true);

            texture.SetMinFilter(TextureMinFilter.Nearest);
            texture.SetMagFilter(TextureMagFilter.Nearest);

            Mesh mesh = new Mesh();

            mesh.Name = s3dObject.Name;
            mesh.Texture = texture;

            foreach (S3DFace s3dFace in s3dObject.Faces) {
                uint firstIndex = s3dFace.Indices[0];
                uint lastIndex = s3dFace.Indices[3];

                MeshPrimitive meshPrimitive;

                var p0 = TransformVertex(objectVertices[(int)s3dFace.Indices[0]]);
                var p1 = TransformVertex(objectVertices[(int)s3dFace.Indices[1]]);
                var p2 = TransformVertex(objectVertices[(int)s3dFace.Indices[2]]);
                var p3 = TransformVertex(objectVertices[(int)s3dFace.Indices[3]]);

                var u0 = TransformTexcoord(s3dFace.Picture.Texture.Vertices[0]);
                var u1 = TransformTexcoord(s3dFace.Picture.Texture.Vertices[1]);
                var u2 = TransformTexcoord(s3dFace.Picture.Texture.Vertices[2]);
                var u3 = TransformTexcoord(s3dFace.Picture.Texture.Vertices[3]);

                // First and last will be equal if it's a triangle
                if (firstIndex == lastIndex) {
                    meshPrimitive = MeshPrimitive.CreateTriangle();

                    meshPrimitive.SetVertices(p0, p1, p2);
                    meshPrimitive.SetTexcoords(u0, u1, u2);
                } else {
                    meshPrimitive = MeshPrimitive.CreateQuad();

                    meshPrimitive.SetVertices(p0, p1, p2, p3);
                    meshPrimitive.SetTexcoords(u0, u1, u2, u3);
                }

                meshPrimitive.CalculateNormal();

                mesh.AddPrimitive(meshPrimitive);
            }

            return mesh;
        }

        private static Vector2 TransformTexcoord(System.Numerics.Vector2 vector) {
            return new Vector2(vector.X, 1.0f - vector.Y);
        }

        private static Vector3 TransformVertex(System.Numerics.Vector3 vector) {
            Vector3 cVertex = new Vector3(vector.X, vector.Y, vector.Z);

            return cVertex * _TransformRotation;
        }
    }
}
