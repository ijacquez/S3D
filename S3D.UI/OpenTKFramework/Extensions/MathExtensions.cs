using System.Runtime.CompilerServices;

namespace S3D.UI.OpenTKFramework.Extensions {
    public static class MathExtensions {
        #region Vector3
        public static OpenTK.Mathematics.Vector3 FromNumerics(this System.Numerics.Vector3 v) =>
            Unsafe.As<System.Numerics.Vector3, OpenTK.Mathematics.Vector3>(ref v);

        public static System.Numerics.Vector3 ToNumerics(this OpenTK.Mathematics.Vector3 v) =>
            Unsafe.As<OpenTK.Mathematics.Vector3, System.Numerics.Vector3>(ref v);

        public static ref System.Numerics.Vector3 AsNumerics(ref this OpenTK.Mathematics.Vector3 v) =>
            ref Unsafe.As<OpenTK.Mathematics.Vector3, System.Numerics.Vector3>(ref v);

        #endregion

        #region Vector4

        public static OpenTK.Mathematics.Vector4 FromNumerics(this System.Numerics.Vector4 v) =>
            Unsafe.As<System.Numerics.Vector4, OpenTK.Mathematics.Vector4>(ref v);

        public static System.Numerics.Vector4 ToNumerics(this OpenTK.Mathematics.Vector4 v) =>
            Unsafe.As<OpenTK.Mathematics.Vector4, System.Numerics.Vector4>(ref v);

        public static ref System.Numerics.Vector4 AsNumerics(ref this OpenTK.Mathematics.Vector4 v) =>
            ref Unsafe.As<OpenTK.Mathematics.Vector4, System.Numerics.Vector4>(ref v);

        #endregion

        // public static OpenTK.Mathematics.Color4 FromNumerics(this System.Numerics.Vector4 v) =>
        //     Unsafe.As<System.Numerics.Vector4, OpenTK.Mathematics.Color4>(ref v);

        public static System.Numerics.Vector4 ToNumerics(this OpenTK.Mathematics.Color4 v) =>
            Unsafe.As<OpenTK.Mathematics.Color4, System.Numerics.Vector4>(ref v);

        public static ref System.Numerics.Vector4 AsNumerics(ref this OpenTK.Mathematics.Color4 v) =>
            ref Unsafe.As<OpenTK.Mathematics.Color4, System.Numerics.Vector4>(ref v);
    }
}
