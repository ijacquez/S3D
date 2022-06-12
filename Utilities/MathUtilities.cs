using System;
using System.Numerics;

namespace S3D.Utilities {
    public static class MathUtility {
        /// <summary>
        /// </summary>
        public static float Modulo(float a, float b) {
            return ((a < 0) ? (b + (a % b)) : (a % b));
        }

        /// <summary>
        /// </summary>
        public static float Lerp(float a, float b, float t) {
            float tClamped = Math.Clamp(t, 0.0f, 1.0f);

            return ((a * (1.0f - tClamped)) + (b * tClamped));
        }

        /// <summary>
        /// </summary>
        public static Vector2 Blerp(Vector2[] points, float tX, float tY) {
            //                    (1,1)
            //       t3--------t2
            //       |    C    |
            //     A o----o----o B
            //       |         |
            //       t0--------t1
            // (0,0)
            //   A=lerp(t0,t3,ty)
            //   B=lerp(t1,t2,ty)
            //   C=lerp(A,B,tx)

            float aX = Lerp(points[0].X, points[3].X, tY);
            float aY = Lerp(points[0].Y, points[3].Y, tY);

            float bX = Lerp(points[1].X, points[2].X, tY);
            float bY = Lerp(points[1].Y, points[2].Y, tY);

            float cX = Lerp(aX, bX, tX);
            float cY = Lerp(aY, bY, tX);

            return new Vector2(cX, cY);
        }
    }
}
