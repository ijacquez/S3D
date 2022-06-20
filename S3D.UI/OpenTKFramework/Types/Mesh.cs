using System;
using OpenTK.Mathematics;
using S3D.UI.MathUtilities.Raycasting;

namespace S3D.UI.OpenTKFramework.Types {
    // XXX: Move this collider
    public class Mesh : ICollider {
        private readonly bool[] _dirtyBuffer = new bool[(int)MeshBufferType.Count];

        /// <summary>
        ///   Name of this <see cref="Mesh"/>.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///   Triangle flags per primitive.
        /// </summary>
        public MeshTriangleFlags[] TriangleFlags { get; set; }

        /// <summary>
        ///   Three vertices per primitive.
        /// </summary>
        public Vector3[] Vertices { get; set; }

        /// <summary>
        ///   Per vertex texture coordinates. Three per primitive.
        /// </summary>
        public Vector2[] Texcoords { get; set; }

        /// <summary>
        ///   Per vertex gouraud shaded color. Three per primitive.
        /// </summary>
        public Color4[] GSColors { get; set; }

        /// <summary>
        ///   Buffer for base color. One per primitive.
        /// </summary>
        public Color4[] BaseColors { get; set; }

        /// <summary>
        ///   Buffer for normals. One per primitive.
        /// </summary>
        public Vector3[] Normals { get; set; }

        /// <summary>
        ///   Element index to <see cref="Vertices"/>,
        ///   <see cref="GSColors"/>, <see cref="BaseColors"/>, and
        ///   <see cref="Normals"/>.
        public uint[] Indices { get; set; }

        /// <summary>
        ///   Assigned <see cref="Texture"/>.
        /// </summary>
        public Texture Texture { get; set; }
        // XXX: Remove
        public float[] LineVertices { get; set; }

        /// <summary>
        ///   Determine if buffer of <paramref name="type"/> is dirty.
        /// </summary>
        public bool IsDirty(MeshBufferType type) {
            return _dirtyBuffer[(int)type];
        }

        /// <summary>
        ///   When the buffer of <paramref name="type"/> is modified, mark the
        ///   buffer as dirty.
        /// </summary>
        public void SetDirty(MeshBufferType type, bool dirty) {
            _dirtyBuffer[(int)type] = dirty;
        }

        public MeshTriangleFlags GetTriangleFlags(int index) {
            return TriangleFlags[index];
        }

        public void SetTriangleFlags(int index, MeshTriangleFlags triangleFlagsMask) {
            TriangleFlags[index] |= triangleFlagsMask;

            // This doesn't take into account if the mask includes
            // MeshTriangleFlags.Quadrangle
            if ((TriangleFlags[index] & MeshTriangleFlags.Quadrangle) != 0) {
                if ((TriangleFlags[index] & MeshTriangleFlags.QuadrangleNext) != 0) {
                    index++;
                } else {
                    index--;
                }

                TriangleFlags[index] |= triangleFlagsMask;
            }

            SetDirty(MeshBufferType.TriangleFlags, true);
        }

        public void ClearTriangleFlags(int index, MeshTriangleFlags triangleFlagsMask) {
            TriangleFlags[index] &= ~triangleFlagsMask;

            // This doesn't take into account if the mask includes
            // MeshTriangleFlags.Quadrangle
            if ((TriangleFlags[index] & MeshTriangleFlags.Quadrangle) != 0) {
                if ((TriangleFlags[index] & MeshTriangleFlags.QuadrangleNext) != 0) {
                    index++;
                } else {
                    index--;
                }

                TriangleFlags[index] &= ~triangleFlagsMask;
            }

            SetDirty(MeshBufferType.TriangleFlags, true);
        }

        public void SetBaseColor(int index, Color4 color) {
            BaseColors[index] = color;

            SetDirty(MeshBufferType.BaseColors, true);
        }

        public void SetGouraudShadingColor(int index,
                                           Color4 color1,
                                           Color4 color2,
                                           Color4 color3,
                                           Color4 color4) {
            MeshTriangleFlags triangleFlags = TriangleFlags[index];

            Console.WriteLine(triangleFlags);

            if ((triangleFlags & MeshTriangleFlags.Quadrangle) == 0) {
                GSColors[(index * 3) + 0] = color1;
                GSColors[(index * 3) + 1] = color2;
                GSColors[(index * 3) + 2] = color3;
            } else {
                Color4[][] switchedColors = new Color4[][] {
                    new Color4[] {
                        color3,
                        color1,
                        color4
                    },
                    new Color4[] {
                        color3,
                        color2,
                        color1
                    }
                };

                int offset = 0;

                if ((triangleFlags & MeshTriangleFlags.QuadranglePrev) != 0) {
                    offset--;
                } else {
                    Array.Reverse(switchedColors);

                    offset++;
                }

                GSColors[(index * 3) + 0] = switchedColors[0][0];
                GSColors[(index * 3) + 1] = switchedColors[0][1];
                GSColors[(index * 3) + 2] = switchedColors[0][2];

                GSColors[((index + offset) * 3) + 0] = switchedColors[1][0];
                GSColors[((index + offset) * 3) + 1] = switchedColors[1][1];
                GSColors[((index + offset) * 3) + 2] = switchedColors[1][2];
            }

            SetDirty(MeshBufferType.GouraudShadingColors, true);
        }
    }
}
