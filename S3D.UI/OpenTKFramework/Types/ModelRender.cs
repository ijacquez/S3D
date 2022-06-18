using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using S3D.UI.OpenTKFramework.Utilities;
using System.Collections.Generic;
using System;

namespace S3D.UI.OpenTKFramework.Types {
    public class ModelRender : IDisposable {
        private const string _PositionAttribName          = "in_position";
        private const string _TexcoordAttribName          = "in_texcoord";
        private const string _ColorAttribName             = "in_color";

        private const string _ModelViewMatrixUniformName  = "modelview_matrix";
        private const string _ProjectionMatrixUniformName = "projection_matrix";

        private int _vaoHandle;

        private int _vboHandle;
        private int _vboSize;

        public Model Model { get; }

        public Shader Shader { get; private set; }

        static ModelRender() {
        }

        private ModelRender() {
        }

        public ModelRender(Model model) {
            Model = model;

            // XXX: Fix this
            Mesh mesh = model.Meshes[0];

            float[] vbo = GenerateVertexArrayBuffer(mesh);

            _vboSize = vbo.Length * sizeof(float);

            _vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(_vaoHandle);

            _vboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, _vboSize, vbo, BufferUsageHint.StaticDraw);
        }

        public void SetShader(Shader shader) {
            Shader = shader;

            if (Shader != null) {
                // XXX: Fix this
                Mesh mesh = Model.Meshes[0];

                int positionLocation = Shader.GetAttribLocation(_PositionAttribName);

                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation,
                                       3,
                                       VertexAttribPointerType.Float,
                                       normalized: false,
                                       stride: 8 * sizeof(float),
                                       offset: 0);
                DebugUtility.CheckGLError("SetShader: PositionAttribName");

                int texcoordLocation = Shader.GetAttribLocation(_TexcoordAttribName);

                GL.EnableVertexAttribArray(texcoordLocation);
                GL.VertexAttribPointer(texcoordLocation,
                                       2,
                                       VertexAttribPointerType.Float,
                                       normalized: false,
                                       stride: 8 * sizeof(float),
                                       offset: 3 * sizeof(float));
                DebugUtility.CheckGLError("SetShader: TexcoordAttribName");

                int colorLocation = Shader.GetAttribLocation(_ColorAttribName);
                DebugUtility.CheckGLError("SetShader: ColorAttribName3");

                Console.WriteLine(colorLocation);
                GL.EnableVertexAttribArray(colorLocation);
                DebugUtility.CheckGLError("SetShader: ColorAttribName1");
                GL.VertexAttribPointer(colorLocation,
                                       3,
                                       VertexAttribPointerType.Float,
                                       normalized: false,
                                       stride: 8 * sizeof(float),
                                       offset: 5 * sizeof(float));
                DebugUtility.CheckGLError("SetShader: ColorAttribName");
            }
        }

        public void Render() {
            Camera camera = Window.Camera;

            // XXX: Fix this
            Mesh mesh = Model.Meshes[0];

            mesh.Texture?.Use(TextureUnit.Texture0);

            if (Shader != null) {
                Shader.Bind();

                var modelViewMatrix = camera.GetViewMatrix();
                var projectionMatrix = camera.GetProjectionMatrix();

                Shader.SetMatrix4(_ModelViewMatrixUniformName, transpose: false, modelViewMatrix);
                Shader.SetMatrix4(_ProjectionMatrixUniformName, transpose: false, projectionMatrix);
            }

            // XXX: Debugging
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.RasterizerDiscard);

            GL.BindVertexArray(_vaoHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, (_vboSize / 8) * 2);
            DebugUtility.CheckGLError("ModelRender.Render");

            // XXX: Debugging
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public void Dispose() {
            int boundVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int boundVBO = GL.GetInteger(GetPName.ArrayBufferBinding);

            if (boundVBO == _vboHandle) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

            if (_vaoHandle == boundVAO) {
                GL.BindVertexArray(0);
            }

            GL.DeleteVertexArray(_vaoHandle);
            GL.DeleteBuffer(_vboHandle);

            if (Shader != null) {
                int currentProgram = GL.GetInteger(GetPName.CurrentProgram);

                if (currentProgram == Shader.Handler) {
                    GL.UseProgram(0);
                    Shader.Dispose();
                }
            }
        }

        private static float[] GenerateVertexArrayBuffer(Mesh mesh) {
            List<float> buffer = new List<float>();

            var vertices = mesh.Vertices;
            var texcoords = mesh.Texcoords;
            var colors = mesh.Colors;

            for (int i  = 0, v = 0; i < mesh.Indices.Length; i++) {
                buffer.Add(vertices[v].X); buffer.Add(vertices[v].Y); buffer.Add(vertices[v].Z); buffer.Add(texcoords[v].X); buffer.Add(texcoords[v].Y); buffer.Add(colors[v].R); buffer.Add(colors[v].G); buffer.Add(colors[v].B);
                v++;
                buffer.Add(vertices[v].X); buffer.Add(vertices[v].Y); buffer.Add(vertices[v].Z); buffer.Add(texcoords[v].X); buffer.Add(texcoords[v].Y); buffer.Add(colors[v].R); buffer.Add(colors[v].G); buffer.Add(colors[v].B);
                v++;
                buffer.Add(vertices[v].X); buffer.Add(vertices[v].Y); buffer.Add(vertices[v].Z); buffer.Add(texcoords[v].X); buffer.Add(texcoords[v].Y); buffer.Add(colors[v].R); buffer.Add(colors[v].G); buffer.Add(colors[v].B);
                v++;
            }

            return buffer.ToArray();
        }
    }
}
