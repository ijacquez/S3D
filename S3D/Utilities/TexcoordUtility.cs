using System.Numerics;

namespace S3D.Utilities {
    public static class TexcoordUtility {
        public static float ClampVectorComponent(float value) {
            return MathUtility.Modulo(value, 1.00000001f);
        }

        public static Vector2 ClampVector(Vector2 vector) {
            return new Vector2(ClampVectorComponent(vector.X),
                               ClampVectorComponent(vector.Y));
        }
    }
}
