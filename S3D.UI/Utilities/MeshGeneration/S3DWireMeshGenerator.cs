using S3D.FileFormats;
using S3D.UI.OpenTKFramework.Types;
using System.Collections.Generic;
using System.Numerics;
using System;

namespace S3D.UI.MeshUtilities {
    public static class S3DWireMeshGenerator {
        private struct Edge {
            public Vector3 Start { get; set; }

            public Vector3 End { get; set; }

            public Edge(Vector3 start, Vector3 end) {
                Start = start;
                End = end;
            }
        }

        /// <summary>
        ///   Generate a <see cref="Mesh"/> from a <see cref="S3DObject"/>.
        /// </summary>
        public static Mesh Generate(S3DObject s3dObject) {
            var objectVertices = s3dObject.Vertices.AsReadOnly();

            List<float> vertices = new List<float>();

            List<Edge> edges = new List<Edge>();

            foreach (S3DFace s3dFace in s3dObject.Faces) {
                uint first = s3dFace.Indices[0];
                uint last = s3dFace.Indices[3];

                // Check if it's a triangle. The first and last vertices will be
                // equal
                if (first == last) {
                    var a = objectVertices[(int)s3dFace.Indices[1]]; a.Y *= -1.0f; a.Z *= -1.0f;
                    var b = objectVertices[(int)s3dFace.Indices[2]]; b.Y *= -1.0f; b.Z *= -1.0f;
                    var c = objectVertices[(int)s3dFace.Indices[3]]; c.Y *= -1.0f; c.Z *= -1.0f;

                    edges.Add(new Edge(a, b));
                    edges.Add(new Edge(b, c));
                    edges.Add(new Edge(c, a));
                } else if (first != last) { // Otherwise, it's a quad
                    var a = objectVertices[(int)s3dFace.Indices[0]]; a.Y *= -1.0f; a.Z *= -1.0f;
                    var b = objectVertices[(int)s3dFace.Indices[1]]; b.Y *= -1.0f; b.Z *= -1.0f;
                    var c = objectVertices[(int)s3dFace.Indices[2]]; c.Y *= -1.0f; c.Z *= -1.0f;
                    var d = objectVertices[(int)s3dFace.Indices[3]]; d.Y *= -1.0f; d.Z *= -1.0f;

                    edges.Add(new Edge(a, b));
                    edges.Add(new Edge(b, c));
                    edges.Add(new Edge(c, d));
                    edges.Add(new Edge(d, a));
                }
            }

            foreach (Edge edge in edges) {
                vertices.Add(edge.Start.X); vertices.Add(edge.Start.Y); vertices.Add(edge.Start.Z);
                vertices.Add(edge.End.X); vertices.Add(edge.End.Y); vertices.Add(edge.End.Z);
            }

            Mesh mesh = new Mesh();

            mesh.Name = s3dObject.Name;
            mesh.Vertices = vertices.ToArray();

            return mesh;
        }
    }
}
