using OpenTK.Graphics.OpenGL4;

namespace S3D.UI.OpenTKFramework.Types {
    public struct AttribFieldInfo {
        public int Location { get; set; }

        public string Name { get; set; }

        public int Size { get; set; }

        public ActiveAttribType Type { get; set; }
    }
}
