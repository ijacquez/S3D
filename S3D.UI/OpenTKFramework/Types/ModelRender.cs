using OpenTK.Graphics.OpenGL4;
using S3D.UI.OpenTKFramework.Utilities;
using System;

namespace S3D.UI.OpenTKFramework.Types {
    public class ModelRender : IDisposable {
        private const string _PositionAttribName          = "in_position";
        private const string _TexcoodAttribName           = "in_texcoord";

        private const string _ModelViewMatrixUniformName  = "modelview_matrix";
        private const string _ProjectionMatrixUniformName = "projection_matrix";

        private int _vbo;
        private int _vao;

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

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Vertices.Length * sizeof(float), mesh.Vertices, BufferUsageHint.StaticDraw);
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
                                       stride: 5 * sizeof(float),
                                       offset: 0);

                int texcoordLocation = Shader.GetAttribLocation(_TexcoodAttribName);

                GL.EnableVertexAttribArray(texcoordLocation);
                GL.VertexAttribPointer(texcoordLocation,
                                       2,
                                       VertexAttribPointerType.Float,
                                       normalized: false,
                                       stride: 5 * sizeof(float),
                                       offset: 3 * sizeof(float));
            }
        }

        public void Render() {
            Camera camera = Window.Camera;

            // XXX: Fix this
            Mesh mesh = Model.Meshes[0];

            mesh.Texture?.Use(TextureUnit.Texture0);

            if (Shader != null) {
                Shader.Bind();

                var modelViewMatrix = Model.Transform * camera.GetViewMatrix();
                var projectionMatrix = camera.GetProjectionMatrix();

                Shader.SetMatrix4(_ModelViewMatrixUniformName, transpose: false, modelViewMatrix);
                Shader.SetMatrix4(_ProjectionMatrixUniformName, transpose: false, projectionMatrix);
            }

            // XXX: Debugging
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.RasterizerDiscard);

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.DrawArrays(PrimitiveType.Triangles, 0, (mesh.Vertices.Length/5)*2);
            DebugUtility.CheckGLError("Render");

            // XXX: Debugging
            // GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        public void Dispose() {
            int boundVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int boundVBO = GL.GetInteger(GetPName.ArrayBufferBinding);

            if (boundVBO == _vbo) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

            if (_vao == boundVAO) {
                GL.BindVertexArray(0);
            }

            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);

            if (Shader != null) {
                int currentProgram = GL.GetInteger(GetPName.CurrentProgram);

                if (currentProgram == Shader.Handler) {
                    GL.UseProgram(0);
                    Shader.Dispose();
                }
            }
        }
    }
}
