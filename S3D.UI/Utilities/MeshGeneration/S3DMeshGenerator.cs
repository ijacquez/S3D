using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using S3D.FileFormats;
using S3D.UI.OpenTKFramework.Types;
using System.Collections.Generic;
using System.Drawing;
using System;

namespace S3D.UI.MeshUtilities {

    public static class S3DMeshGenerator {
        /// <summary>
        ///   Generate a <see cref="Mesh"/> from a <see cref="S3DObject"/>.
        /// </summary>
        public static Mesh Generate(S3DObject s3dObject) {
            var objectVertices = s3dObject.Vertices.AsReadOnly();

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> texcoords = new List<Vector2>();
            List<Color4> colors = new List<Color4>();
            List<Vector3> normals = new List<Vector3>();
            List<uint> indices = new List<uint>();

            uint indexCount = 0;

            foreach (S3DFace s3dFace in s3dObject.Faces) {
                uint first = s3dFace.Indices[0];
                uint last = s3dFace.Indices[3];

                // Check if it's a triangle. The first and last vertices_OLD will be
                // equal
                if (first == last) {
                    var a = objectVertices[(int)s3dFace.Indices[1]]; a.Y *= -1.0f; a.Z *= -1.0f;
                    var b = objectVertices[(int)s3dFace.Indices[2]]; b.Y *= -1.0f; b.Z *= -1.0f;
                    var c = objectVertices[(int)s3dFace.Indices[3]]; c.Y *= -1.0f; c.Z *= -1.0f;

                    var u1 = s3dFace.Picture.Texture.Vertices[2];
                    var u2 = s3dFace.Picture.Texture.Vertices[1];
                    var u3 = s3dFace.Picture.Texture.Vertices[3];

                    var a1 = new Vector3(a.X, a.Y, a.Z);
                    var b1 = new Vector3(b.X, b.Y, b.Z);
                    var c1 = new Vector3(c.X, c.Y, c.Z);

                    vertices.Add(a1);
                    vertices.Add(b1);
                    vertices.Add(c1);

                    texcoords.Add(new Vector2(u1.X, 1.0f - u1.Y));
                    texcoords.Add(new Vector2(u2.X, 1.0f - u2.Y));
                    texcoords.Add(new Vector2(u3.X, 1.0f - u3.Y));

                    colors.Add(Color.White);
                    colors.Add(Color.White);
                    colors.Add(Color.White);

                    // Console.WriteLine(s3dFace.Normal);
                    normals.Add(CalculateNormal(a1, b1, c1));

                    indices.Add(indexCount);

                    indexCount++;
                } else if (first != last) { // Otherwise, it's a quad
                    var a = objectVertices[(int)s3dFace.Indices[1]]; a.Y *= -1.0f; a.Z *= -1.0f;
                    var b = objectVertices[(int)s3dFace.Indices[2]]; b.Y *= -1.0f; b.Z *= -1.0f;
                    var c = objectVertices[(int)s3dFace.Indices[3]]; c.Y *= -1.0f; c.Z *= -1.0f;

                    var d = objectVertices[(int)s3dFace.Indices[1]]; d.Y *= -1.0f; d.Z *= -1.0f;
                    var e = objectVertices[(int)s3dFace.Indices[3]]; e.Y *= -1.0f; e.Z *= -1.0f;
                    var f = objectVertices[(int)s3dFace.Indices[0]]; f.Y *= -1.0f; f.Z *= -1.0f;

                    var u1 = s3dFace.Picture.Texture.Vertices[2];
                    var u2 = s3dFace.Picture.Texture.Vertices[1];
                    var u3 = s3dFace.Picture.Texture.Vertices[0];

                    var v1 = s3dFace.Picture.Texture.Vertices[2];
                    var v2 = s3dFace.Picture.Texture.Vertices[0];
                    var v3 = s3dFace.Picture.Texture.Vertices[3];

                    var a1 = new Vector3(a.X, a.Y, a.Z);
                    var b1 = new Vector3(b.X, b.Y, b.Z);
                    var c1 = new Vector3(c.X, c.Y, c.Z);
                    var d1 = new Vector3(d.X, d.Y, d.Z);
                    var e1 = new Vector3(e.X, e.Y, e.Z);
                    var f1 = new Vector3(f.X, f.Y, f.Z);

                    vertices.Add(a1);
                    vertices.Add(b1);
                    vertices.Add(c1);
                    vertices.Add(d1);
                    vertices.Add(e1);
                    vertices.Add(f1);

                    texcoords.Add(new Vector2(u1.X, 1.0f - u1.Y));
                    texcoords.Add(new Vector2(u2.X, 1.0f - u2.Y));
                    texcoords.Add(new Vector2(u3.X, 1.0f - u3.Y));

                    texcoords.Add(new Vector2(v1.X, 1.0f - v1.Y));
                    texcoords.Add(new Vector2(v2.X, 1.0f - v2.Y));
                    texcoords.Add(new Vector2(v3.X, 1.0f - v3.Y));

                    colors.Add(Color.White);
                    colors.Add(Color.White);
                    colors.Add(Color.White);

                    colors.Add(Color.White);
                    colors.Add(Color.White);
                    colors.Add(Color.White);

                    // Console.WriteLine(s3dFace.Normal);
                    normals.Add(CalculateNormal(a1, b1, c1));
                    normals.Add(CalculateNormal(d1, e1, f1));

                    indices.Add(indexCount);

                    indexCount++;

                    indices.Add(indexCount);

                    indexCount++;
                }
            }

            // XXX: Change texture name
            Texture texture = new Texture("mesh_generation",
                                          Bitmap.FromFile("map_d03_01.png") as Bitmap,
                                          generateMipmaps: false,
                                          srgb: true);

            texture.SetMinFilter(TextureMinFilter.Nearest);
            texture.SetMagFilter(TextureMagFilter.Nearest);

            Mesh mesh = new Mesh();

            mesh.Name = s3dObject.Name;
            mesh.Vertices = vertices.ToArray();
            mesh.Texcoords = texcoords.ToArray();
            mesh.Colors = colors.ToArray();
            mesh.Normals = normals.ToArray();
            mesh.Indices = indices.ToArray();
            mesh.Texture = texture;

            return mesh;
        }

        private static Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c) {
            var n = Vector3.Cross(c - a, b - a).Normalized();

            // Console.WriteLine(n);

            return n;
        }
    }
}
