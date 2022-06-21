using System;
using System.Collections.Generic;

namespace S3D.UI.OpenTKFramework.Types {
    public class Mesh {
        private readonly List<MeshPrimitive> _primitives = new List<MeshPrimitive>();

        public string Name { get; set; } = string.Empty;

        public IReadOnlyList<MeshPrimitive> Primitives => _primitives.AsReadOnly();

        public int PrimitiveCount { get; private set; }

        public int TriangleCount { get; private set; }

        public Texture Texture { get; set; }

        public void AddPrimitive(MeshPrimitive primitive) {
            if (primitive == null) {
                throw new NullReferenceException();
            }

            PrimitiveCount++;
            TriangleCount += primitive.Triangles.Length;

            _primitives.Add(primitive);
        }
    }
}
