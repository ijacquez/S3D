using OpenTK.Graphics.OpenGL4;
using S3D.FileFormats;
using S3D.UI.OpenTKFramework.Types;
using System.Collections.Generic;
using System.Drawing;

namespace S3D.UI.MeshUtilities {
    public static class S3DMeshGenerator {
        /// <summary>
        ///   Generate a <see cref="Mesh"/> from a <see cref="S3DObject"/>.
        /// </summary>
        public static Mesh Generate(S3DObject s3dObject) {
            var objectVertices = s3dObject.Vertices.AsReadOnly();

            List<float> vertices = new List<float>();

            foreach (S3DFace s3dFace in s3dObject.Faces) {
                uint first = s3dFace.Indices[0];
                uint last = s3dFace.Indices[3];

                // Check if it's a triangle. The first and last vertices will be
                // equal
                if (first == last) {
                    var a = objectVertices[(int)s3dFace.Indices[1]]; a.Y *= -1.0f; a.Z *= -1.0f;
                    var b = objectVertices[(int)s3dFace.Indices[2]]; b.Y *= -1.0f; b.Z *= -1.0f;
                    var c = objectVertices[(int)s3dFace.Indices[3]]; c.Y *= -1.0f; c.Z *= -1.0f;

                    var u1 = s3dFace.Picture.Texture.Vertices[2];
                    var u2 = s3dFace.Picture.Texture.Vertices[1];
                    var u3 = s3dFace.Picture.Texture.Vertices[3];

                    vertices.Add(a.X); vertices.Add(a.Y); vertices.Add(a.Z); vertices.Add(u1.X); vertices.Add(1.0f - u1.Y);
                    vertices.Add(b.X); vertices.Add(b.Y); vertices.Add(b.Z); vertices.Add(u2.X); vertices.Add(1.0f - u2.Y);
                    vertices.Add(c.X); vertices.Add(c.Y); vertices.Add(c.Z); vertices.Add(u3.X); vertices.Add(1.0f - u3.Y);
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

                    vertices.Add(a.X); vertices.Add(a.Y); vertices.Add(a.Z); vertices.Add(u1.X); vertices.Add(1.0f - u1.Y);
                    vertices.Add(b.X); vertices.Add(b.Y); vertices.Add(b.Z); vertices.Add(u2.X); vertices.Add(1.0f - u2.Y);
                    vertices.Add(c.X); vertices.Add(c.Y); vertices.Add(c.Z); vertices.Add(u3.X); vertices.Add(1.0f - u3.Y);

                    vertices.Add(d.X); vertices.Add(d.Y); vertices.Add(d.Z); vertices.Add(v1.X); vertices.Add(1.0f - v1.Y);
                    vertices.Add(e.X); vertices.Add(e.Y); vertices.Add(e.Z); vertices.Add(v2.X); vertices.Add(1.0f - v2.Y);
                    vertices.Add(f.X); vertices.Add(f.Y); vertices.Add(f.Z); vertices.Add(v3.X); vertices.Add(1.0f - v3.Y);
                }
            }

            // XXX: Change texture name
            Texture texture = new Texture("mesh_generation", Bitmap.FromFile("map_d03_01.png") as Bitmap, generateMipmaps: false, srgb: true);

            texture.SetMinFilter(TextureMinFilter.Nearest);
            texture.SetMagFilter(TextureMagFilter.Nearest);

            Mesh mesh = new Mesh();

            mesh.Name = s3dObject.Name;
            mesh.Vertices = vertices.ToArray();
            mesh.Texture = texture;

            return mesh;
        }
    }
}
