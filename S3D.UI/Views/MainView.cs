using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using S3D.UI.OpenTKFramework.Types;
using System.Drawing;
using System;

namespace S3D.UI.Views {
    public class MainView : View {
        private readonly FileDialogView _openFileDialogView = new FileDialogView();
        private readonly MainMenuBarView _mainMenuBarView = new MainMenuBarView();
        // private readonly ModelView _modelView = new ModelView();

        public Action OpenFile { get; set; }

        protected override void OnLoad() {
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            string vertexShader = System.IO.File.ReadAllText("Shaders/shader.vert");
            string fragmentShader = System.IO.File.ReadAllText("Shaders/shader.frag");
            _shader = new Shader("Foo", vertexShader, fragmentShader);
            _shader.UseShader();

            _mainMenuBarView.Load();
            _openFileDialogView.Load();
            _openFileDialogView.Hide();
            // _openFileDialogView.Hide();

            // _modelView.Init();
            // _modelView.Show();

            // _mainMenuBarView.MenuFileOpen -= MenuFileOpen;
            // _mainMenuBarView.MenuFileOpen += MenuFileOpen;
            // _mainMenuBarView.MenuFileQuit += delegate { Console.Write("Quit"); };
        }

        protected override void OnUnload() {
        }

        protected override void OnRenderFrame(FrameEventArgs e) {
            _mainMenuBarView.RenderFrame(e);
            //_openFileDialogView.RenderFrame(e);
            // _modelView.Update(dt);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            _shader.UseShader();
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        float[] _vertices = {
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
            0.5f, -0.5f, 0.0f, //Bottom-right vertex
            0.0f,  0.5f, 0.0f  //Top vertex
        };
        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private Shader _shader;

        private void MenuFileOpen() {
            _openFileDialogView.Show();
        }
    }
}
