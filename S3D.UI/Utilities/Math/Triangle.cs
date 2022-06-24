using OpenTK.Mathematics;

namespace S3D.UI.MathUtilities {
    public static class Triangle {
        public static bool PointInTriangle(Vector3 point, Vector3 normal, Vector3 p0, Vector3 p1, Vector3 p2) {
            // p1        p0
            //  +--------+
            //   \ *    / * = point
            //    \    /
            //     \  /
            //      \/
            //      p2

            bool sameSide1 = LineSameSide(point, normal, p0, p1);
            bool sameSide2 = LineSameSide(point, normal, p1, p2);
            bool sameSide3 = LineSameSide(point, normal, p2, p0);

            return (sameSide1 && sameSide2 && sameSide3);
        }

        private static bool LineSameSide(Vector3 point, Vector3 normal, Vector3 p0, Vector3 p1) {
            Vector3 edge = p1 - p0;
            Vector3 pv = point - p0;
            Vector3 a = Vector3.Cross(pv, edge);

            return (Vector3.Dot(normal, a) >= 0.0f);
        }
    }
}
