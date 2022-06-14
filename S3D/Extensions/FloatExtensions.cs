using System;

namespace S3D.Extensions {
    public static class FloatExtensions {
        public static bool IsApproximately(this float value, float compare, float epsilon = 0.001f) {
            return Math.Abs(value - compare) <= epsilon;
        }
    }
}
