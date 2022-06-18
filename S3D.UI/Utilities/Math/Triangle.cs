using OpenTK.Mathematics;

namespace S3D.UI.MathUtilities {
    public static class Triangle {
        public static bool PointInTriangle(Vector2 point, Vector2 p1, Vector2 p2, Vector2 p3) {
            // p1        p2
            //  +--------+
            //   \ *    / * = point
            //    \    /
            //     \  /
            //      \/
            //      p3

            bool sameSide1 = LineSameSide(point, p1, p2, p3);
            bool sameSide2 = LineSameSide(point, p2, p1, p3);
            bool sameSide3 = LineSameSide(point, p3, p1, p2);

            return (sameSide1 && sameSide2 && sameSide3);
        }

        private static bool LineSameSide(Vector2 p1, Vector2 p2, Vector2 t1, Vector2 t2) {
            Vector2 t2t1 = (t2 - t1);
            Vector2 p1t1 = (p1 - t1);
            Vector2 p2t1 = (p2 - t1);

            Vector3 cp1 = Vector3.Cross(new Vector3(t2t1), new Vector3(p1t1));
            Vector3 cp2 = Vector3.Cross(new Vector3(t2t1), new Vector3(p2t1));

            return (Vector3.Dot(cp1, cp2) >= 0.0f);
        }
    }
}
