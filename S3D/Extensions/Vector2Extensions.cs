using System.Numerics;

namespace S3D.Extensions {
    public static class Vector2Extensions {
        public static bool IsApproximately(this Vector2 value, Vector2 compare, float epsilon = 0.001f) {
            return (value.X.IsApproximately(compare.X, epsilon) &&
                    value.Y.IsApproximately(compare.Y, epsilon));
        }

        public static float Cross(Vector2 u, Vector2 v) {
            Vector2 uNormalized = Vector2.Normalize(u);
            Vector2 vNormalized = Vector2.Normalize(v);

            return ((uNormalized.X * vNormalized.Y) - (uNormalized.Y * vNormalized.X));
        }
    }
}
