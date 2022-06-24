using OpenTK.Windowing.Common;

namespace S3D.UI.OpenTKFramework.Types {
    public static class Time {
        public static float ElapsedTime { get; private set; }

        public static float DeltaTime { get; private set; }

        public static void UpdateFrame(FrameEventArgs e) {
            DeltaTime = (float)e.Time;
            ElapsedTime += DeltaTime;
        }
    }
}
