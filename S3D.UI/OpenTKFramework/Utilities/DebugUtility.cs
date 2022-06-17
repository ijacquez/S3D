using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

namespace S3D.UI.OpenTKFramework.Utilities {
    public static class DebugUtility {
        [Conditional("DEBUG")]
        public static void CheckGLError(string title) {
            var error = GL.GetError();
            if (error != ErrorCode.NoError) {
                // Debug.Print($"{title}: {error}");
                System.Console.WriteLine($"{title}: {error}");
            }
        }
    }
}
