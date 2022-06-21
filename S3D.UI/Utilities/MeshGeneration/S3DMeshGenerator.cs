using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using S3D.FileFormats;
using S3D.UI.OpenTKFramework.Types;
using System.Drawing;

namespace S3D.UI.MeshUtilities {
    public static class S3DMeshGenerator {
        private static readonly Color4 _DefaultGouraudShadingColor = new Color4(0.5f, 0.5f, 0.5f, 1.0f);
        private static readonly Color4 _DefaultBaseColor           = new Color4(1.0f, 0.0f, 1.0f, 1.0f);

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
                uint first = s3dFace.Indices[0];
                uint last = s3dFace.Indices[3];

                MeshPrimitive meshPrimitive;

                // Check if it's p0 triangle. The first and last vertices_OLD will be
                // equal
                if (first == last) {
                    meshPrimitive = MeshPrimitive.CreateTriangle();

                    // XXX: Fix System.Numerics and OpenTK.Mathematics conflict
                    var p0 = objectVertices[(int)s3dFace.Indices[0]]; p0.Y *= -1.0f; p0.Z *= -1.0f;
                    var p1 = objectVertices[(int)s3dFace.Indices[1]]; p1.Y *= -1.0f; p1.Z *= -1.0f; // 1
                    var p2 = objectVertices[(int)s3dFace.Indices[2]]; p2.Y *= -1.0f; p2.Z *= -1.0f; // 2

                    var u0 = s3dFace.Picture.Texture.Vertices[0];
                    var u1 = s3dFace.Picture.Texture.Vertices[1];
                    var u2 = s3dFace.Picture.Texture.Vertices[2];

                    var p0v = new Vector3(p0.X, p0.Y, p0.Z);
                    var p1v = new Vector3(p1.X, p1.Y, p1.Z);
                    var p2v = new Vector3(p2.X, p2.Y, p2.Z);

                    var u0v = new Vector2(u0.X, 1.0f - u0.Y);
                    var u1v = new Vector2(u1.X, 1.0f - u1.Y);
                    var u2v = new Vector2(u2.X, 1.0f - u2.Y);

                    meshPrimitive.SetVertices(p0v, p1v, p2v);
                    meshPrimitive.SetTexcoords(u0v, u1v, u2v);

                    meshPrimitive.CalculateNormal();

                    meshPrimitive.BaseColor = _DefaultBaseColor;
                    meshPrimitive.SetGouraudShading(_DefaultGouraudShadingColor,
                                                    _DefaultGouraudShadingColor,
                                                    _DefaultGouraudShadingColor);

                    mesh.AddPrimitive(meshPrimitive);
                } else {
                    meshPrimitive = MeshPrimitive.CreateQuad();

                    // XXX: Fix System.Numerics and OpenTK.Mathematics conflict
                    var p0 = objectVertices[(int)s3dFace.Indices[0]]; p0.Y *= -1.0f; p0.Z *= -1.0f;
                    var p1 = objectVertices[(int)s3dFace.Indices[1]]; p1.Y *= -1.0f; p1.Z *= -1.0f; // 1
                    var p2 = objectVertices[(int)s3dFace.Indices[2]]; p2.Y *= -1.0f; p2.Z *= -1.0f; // 2
                    var p3 = objectVertices[(int)s3dFace.Indices[3]]; p3.Y *= -1.0f; p3.Z *= -1.0f; // 3

                    var u0 = s3dFace.Picture.Texture.Vertices[0];
                    var u1 = s3dFace.Picture.Texture.Vertices[1];
                    var u2 = s3dFace.Picture.Texture.Vertices[2];
                    var u3 = s3dFace.Picture.Texture.Vertices[3];

                    var p0v = new Vector3(p0.X, p0.Y, p0.Z);
                    var p1v = new Vector3(p1.X, p1.Y, p1.Z);
                    var p2v = new Vector3(p2.X, p2.Y, p2.Z);
                    var p3v = new Vector3(p3.X, p3.Y, p3.Z);

                    var u0v = new Vector2(u0.X, 1.0f - u0.Y);
                    var u1v = new Vector2(u1.X, 1.0f - u1.Y);
                    var u2v = new Vector2(u2.X, 1.0f - u2.Y);
                    var u3v = new Vector2(u3.X, 1.0f - u3.Y);

                    meshPrimitive.SetVertices(p0v, p1v, p2v, p3v);
                    meshPrimitive.SetTexcoords(u0v, u1v, u2v, u3v);

                    meshPrimitive.CalculateNormal();

                    meshPrimitive.BaseColor = _DefaultBaseColor;

                    meshPrimitive.SetGouraudShading(_DefaultGouraudShadingColor,
                                                    _DefaultGouraudShadingColor,
                                                    _DefaultGouraudShadingColor,
                                                    _DefaultGouraudShadingColor);
                }

                mesh.AddPrimitive(meshPrimitive);
            }

            return mesh;
        }
    }
}
