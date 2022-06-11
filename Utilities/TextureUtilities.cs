using System.Linq;
using System.Numerics;

namespace S3D.Utilities {
    public static class TextureUtility {
        public static float ClampVectorComponent(float value) {
            return MathUtility.Modulo(value, 1.00000001f);
        }

        public static Vector2 ClampVector(Vector2 vector) {
            return new Vector2(ClampVectorComponent(vector.X),
                               ClampVectorComponent(vector.Y));
        }

        public static Vector2 GetCenterPoint(Vector2[] a) {
            return new Vector2(a.Sum((textureVertex) => textureVertex.X) * (1.0f / a.Length),
                               a.Sum((textureVertex) => textureVertex.Y) * (1.0f / a.Length));
        }
    }
}
