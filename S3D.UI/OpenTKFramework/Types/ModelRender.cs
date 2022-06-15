using OpenTK.Graphics.OpenGL4;
using S3D.UI.OpenTKFramework.Utilities;
using System;

namespace S3D.UI.OpenTKFramework.Types {
    public class ModelRender : IDisposable {
        private const string _VertexAttribName            = "in_position";

        private const string _ModelViewMatrixUniformName  = "modelview_matrix";
        private const string _ProjectionMatrixUniformName = "projection_matrix";

        private int _vbo;
        private int _vao;
        private int _ebo;

        public static Camera Camera { get; set; }

        public Model Model { get; }

        public Shader Shader { get; private set; }

        private ModelRender() {
        }

        public ModelRender(Model model) {
            Model = model;

            // XXX: Fix this
            Mesh mesh = model.Meshes[0];

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Vertices.Length * sizeof(float), mesh.Vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.Indices.Length * sizeof(uint), mesh.Indices, BufferUsageHint.StaticDraw);
        }

        public void SetShader(Shader shader) {
            Shader = shader;

            if (Shader != null) {
                // XXX: Fix this
                Mesh mesh = Model.Meshes[0];

                int vertexLocation = Shader.GetAttribLocation(_VertexAttribName);

                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation,
                                       3,
                                       VertexAttribPointerType.Float,
                                       normalized: false,
                                       stride: 3 * sizeof(float),
                                       offset: 0);
            }
        }

        public void Render() {
            if (Camera == null) {
                return;
            }

            if (Shader != null) {
                Shader.Bind();

                var modelViewMatrix = Model.Transform * Camera.GetViewMatrix();
                var projectionMatrix = Camera.GetProjectionMatrix();

                Shader.SetMatrix4(_ModelViewMatrixUniformName, transpose: false, modelViewMatrix);
                Shader.SetMatrix4(_ProjectionMatrixUniformName, transpose: false, projectionMatrix);
            }

            // XXX: Debugging
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            // XXX: Fix this
            Mesh mesh = Model.Meshes[0];

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, mesh.Indices.Length, DrawElementsType.UnsignedInt, indices: 0);
            DebugUtility.CheckGLError("Render");

            // XXX: Debugging
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public void Dispose() {
            int boundVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int boundVBO = GL.GetInteger(GetPName.ArrayBufferBinding);
            int boundEBO = GL.GetInteger(GetPName.ElementArrayBufferBinding);

            if (boundVBO == _vbo) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

            if (_vao == boundVAO) {
                GL.BindVertexArray(0);
            }

            if (_ebo == boundEBO) {
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);

            if (Shader != null) {
                int currentProgram = GL.GetInteger(GetPName.CurrentProgram);

                if (currentProgram == Shader.Program) {
                    GL.UseProgram(0);
                    Shader.Dispose();
                }
            }
        }
    }
}
