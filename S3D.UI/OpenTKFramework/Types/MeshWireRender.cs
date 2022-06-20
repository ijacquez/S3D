using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using S3D.UI.OpenTKFramework.Utilities;
using System.Collections.Generic;
using System.Linq;
using System;

namespace S3D.UI.OpenTKFramework.Types {
    public class MeshWireRender : IDisposable {
        private const string _VertexShaderSource = @"#version 330 core

uniform mat4 modelview_matrix;
uniform mat4 projection_matrix;

in vec3 in_position;

void main(void)
{
    gl_Position = projection_matrix * modelview_matrix * vec4(in_position, 1.0);
}";

        private const string _FragmentShaderSource = @"#version 330 core

void main()
{
    gl_FragColor = vec4(1.0, 1.0, 1.0, 1.0);
}";

        private const string _PositionAttribName          = "in_position";

        private const string _ModelViewMatrixUniformName  = "modelview_matrix";
        private const string _ProjectionMatrixUniformName = "projection_matrix";

        private int _vbo;
        private int _vao;

        public Mesh[] Meshes { get; }

        private readonly Shader _shader;
        private readonly int _arraysCount;

        static MeshWireRender() {
        }

        private MeshWireRender() {
        }

        public MeshWireRender(Mesh mesh) : this(new Mesh[] { mesh }) {
        }

        public MeshWireRender(IEnumerable<Mesh> meshes) {
            Meshes = meshes.ToArray();

            // XXX: Fix this
            Mesh mesh = Meshes[0];

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _arraysCount = mesh.LineVertices.Length / 3;

            Console.WriteLine(_arraysCount);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer,
                          mesh.LineVertices.Length * sizeof(float),
                          mesh.LineVertices,
                          BufferUsageHint.StaticDraw);

            _shader = new Shader("mesh_wire_render", _VertexShaderSource, _FragmentShaderSource);

            _shader.Bind();

            int positionLocation = _shader.GetAttribLocation(_PositionAttribName);

            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation,
                                   size: 3,
                                   VertexAttribPointerType.Float,
                                   normalized: false,
                                   stride: 3 * sizeof(float),
                                   offset: 0);
        }

        public void Render() {
            // XXX: Fix this
            Mesh mesh = Meshes[0];

            _shader.Bind();

            var modelViewMatrix = Window.Camera.GetViewMatrix();
            var projectionMatrix = Window.Camera.GetProjectionMatrix();

            _shader.SetMatrix4(_ModelViewMatrixUniformName, transpose: false, modelViewMatrix);
            _shader.SetMatrix4(_ProjectionMatrixUniformName, transpose: false, projectionMatrix);

            // GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.RasterizerDiscard);
            // Avoid Z-fighting?
            GL.Disable(EnableCap.DepthTest);

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.DrawArrays(PrimitiveType.Lines, 0, _arraysCount);
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

            if (_shader != null) {
                int currentProgram = GL.GetInteger(GetPName.CurrentProgram);

                if (currentProgram == _shader.Handler) {
                    GL.UseProgram(0);
                    _shader.Dispose();
                }
            }
        }
    }
}
