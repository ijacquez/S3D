using OpenTK.Core.Native;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using S3D.UI.OpenTKFramework.Utilities;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace S3D.UI.OpenTKFramework.Types {
    public class ModelRender : IDisposable {
        private const string _PositionAttribName          = "in_position";
        private const string _TexcoordAttribName          = "in_texcoord";
        private const string _GSColorAttribName           = "in_gscolor";
        private const string _BaseColorAttribName         = "in_basecolor";
        private const string _FlagsAttribName             = "in_flags";

        private const string _ElapsedTimeUniformName      = "time";
        private const string _ModelViewMatrixUniformName  = "modelview_matrix";
        private const string _ProjectionMatrixUniformName = "projection_matrix";

        private int _vaoHandle;

        // VBO structure
        //   Vertex   Texture
        //     float    float
        //   0        3
        //   -------- -------
        //   vx vy vz tx ty
        //
        //   Base color
        //    float
        //   5
        //   ----------
        //   r g b
        //
        //   Color
        //    float
        //   0
        //   --------
        //   gr gg gb
        //
        //   Flags
        //     uint
        //   0
        //   --------
        //   flags
        private int _vboHandle;
        private int _vboSize;
        private float[] _vertexTexcoordBuffer;
        private IntPtr _vertexTexcoordPtr;
        private float[] _gsColorBuffer;
        private IntPtr _gsColorPtr;
        private float[] _baseColorBuffer;
        private IntPtr _baseColorPtr;
        private uint[] _flagsBuffer;
        private IntPtr _flagsPtr;

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

            _vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(_vaoHandle);

            _vertexTexcoordBuffer = new float[mesh.Indices.Length * 3 * 5];
            _gsColorBuffer = new float[mesh.Indices.Length * 3 * 3];
            _baseColorBuffer = new float[mesh.Indices.Length * 3 * 3];
            _flagsBuffer = new uint[mesh.Indices.Length * 3];

            _vboSize = (_vertexTexcoordBuffer.Length * sizeof(float)) +
                       (_gsColorBuffer.Length * sizeof(float)) +
                       (_baseColorBuffer.Length * sizeof(float)) +
                       (_flagsBuffer.Length * sizeof(uint));

            _vertexTexcoordPtr = (IntPtr)0;
            _gsColorPtr        = _vertexTexcoordPtr + (_vertexTexcoordBuffer.Length * sizeof(float));
            _baseColorPtr      = _gsColorPtr + (_gsColorBuffer.Length * sizeof(float));
            _flagsPtr          = _baseColorPtr + (_baseColorBuffer.Length * sizeof(float));

            PopulateVertexTexcoordBuffer(mesh, _vertexTexcoordBuffer);
            PopulateGSColorBuffer(mesh, _gsColorBuffer);
            PopulateBaseColorBuffer(mesh, _baseColorBuffer);
            PopulateFlagsBuffer(mesh, _flagsBuffer);

            // The Mesh class should have a "IsDirty" for every type of buffer?
            //    Inside Render:
            //       if (mesh.IsDirty(MeshBufferType.Color)) {
            //          ProcessColors again
            //          Upload
            //       }
            //
            //       if (mesh.IsDirty(MeshBufferType.BaseColor)) {
            //          ProcessBaseColors again
            //          Upload
            //       }
            //
            // For this to work, you need to have a mesh.SetDirty(MeshBufferType) as well...
            //    This would be set in the view (think MainView/ModelView)

            // XXX: Figure out how to have textureless prims but still have gouraud shading
            //             Maybe specify a color in the postexcoord buffer

            int n = 101;
            _flagsBuffer[(n * 3)] &= ~(uint)1 << 0;
            _flagsBuffer[((n - 1) * 3) + 0] &= ~(uint)1 << 0;

            _flagsBuffer[(n * 3)] |= (uint)1 << 1;
            _flagsBuffer[((n - 1) * 3) + 0] |= (uint)1 << 1;

            // _flagsBuffer[(n * 3)] |= (uint)1 << 31;
            // _flagsBuffer[((n - 1) * 3) + 0] |= (uint)1 << 31;

            _gsColorBuffer[(n * 9) + 0] = 1.0f; _gsColorBuffer[(n * 9) + 1] = 0.0f; _gsColorBuffer[(n * 9) + 2] = 0.0f;
            _gsColorBuffer[(n * 9) + 3] = 0.0f; _gsColorBuffer[(n * 9) + 4] = 1.0f; _gsColorBuffer[(n * 9) + 5] = 0.0f;
            _gsColorBuffer[(n * 9) + 6] = 0.0f; _gsColorBuffer[(n * 9) + 7] = 0.0f; _gsColorBuffer[(n * 9) + 8] = 1.0f;

            _baseColorBuffer[(n * 9) + 0] = 1.0f; _baseColorBuffer[(n * 9) + 1] = 0.0f; _baseColorBuffer[(n * 9) + 2] = 0.0f;
            _baseColorBuffer[((n - 1) * 9) + 0] = 0.5f; _baseColorBuffer[((n - 1) * 9) + 1] = 0.5f; _baseColorBuffer[((n - 1) * 9) + 2] = 0.5f;

            _vboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, _vboSize, (IntPtr)0, BufferUsageHint.StaticDraw);

            GL.BufferSubData(BufferTarget.ArrayBuffer,
                             _vertexTexcoordPtr,
                             _vertexTexcoordBuffer.Length * sizeof(float),
                             _vertexTexcoordBuffer);

            GL.BufferSubData(BufferTarget.ArrayBuffer,
                             _gsColorPtr,
                             _gsColorBuffer.Length * sizeof(float),
                             _gsColorBuffer);

            GL.BufferSubData(BufferTarget.ArrayBuffer,
                             _baseColorPtr,
                             _baseColorBuffer.Length * sizeof(float),
                             _baseColorBuffer);

            GL.BufferSubData(BufferTarget.ArrayBuffer,
                             _flagsPtr,
                             _flagsBuffer.Length * sizeof(uint),
                             _flagsBuffer);
        }

        public void SetShader(Shader shader) {
            Shader = shader;

            if (Shader == null) {
                return;
            }

            // XXX: Fix this
            Mesh mesh = Model.Meshes[0];

            int positionLocation = Shader.GetAttribLocation(_PositionAttribName);
            DebugUtility.CheckGLError("SetShader: PositionAttribName Shader.GetAttribLocation");

            GL.EnableVertexAttribArray(positionLocation);
            GL.VertexAttribPointer(positionLocation,
                                   3,
                                   VertexAttribPointerType.Float,
                                   normalized: false,
                                   stride: 5 * sizeof(float),
                                   offset: 0);
            DebugUtility.CheckGLError("SetShader: PositionAttribName GL.VertexAttribPointer");

            int texcoordLocation = Shader.GetAttribLocation(_TexcoordAttribName);

            GL.EnableVertexAttribArray(texcoordLocation);
            GL.VertexAttribPointer(texcoordLocation,
                                   2,
                                   VertexAttribPointerType.Float,
                                   normalized: false,
                                   stride: 5 * sizeof(float),
                                   offset: 3 * sizeof(float));
            DebugUtility.CheckGLError("SetShader: TexcoordAttribName GL.VertexAttribPointer");

            int gsColorLocation = Shader.GetAttribLocation(_GSColorAttribName);
            DebugUtility.CheckGLError("SetShader: GSColorAttribName Shader.GetAttribLocation");

            GL.EnableVertexAttribArray(gsColorLocation);
            GL.VertexAttribPointer(gsColorLocation,
                                   size: 3,
                                   VertexAttribPointerType.Float,
                                   normalized: false,
                                   stride: 3 * sizeof(float),
                                   _gsColorPtr);
            DebugUtility.CheckGLError("SetShader: GSColorAttribName GL.VertexAttribPointer");

            int baseColorLocation = Shader.GetAttribLocation(_BaseColorAttribName);
            DebugUtility.CheckGLError("SetShader: BaseColorAttribName Shader.GetAttribLocation");

            GL.EnableVertexAttribArray(baseColorLocation);
            GL.VertexAttribPointer(baseColorLocation,
                                   size: 3,
                                   VertexAttribPointerType.Float,
                                   normalized: false,
                                   stride: 3 * sizeof(float),
                                   _baseColorPtr);
            DebugUtility.CheckGLError("SetShader: BaseColorAttribName GL.VertexAttribPointer");

            int flagsLocation = Shader.GetAttribLocation(_FlagsAttribName);
            DebugUtility.CheckGLError("SetShader: FlagsAttribName Shader.GetAttribLocation");

            GL.EnableVertexAttribArray(flagsLocation);
            GL.VertexAttribIPointer(flagsLocation,
                                    size: 1,
                                    VertexAttribIntegerType.UnsignedInt,
                                    stride: 1 * sizeof(uint),
                                    _flagsPtr);
            DebugUtility.CheckGLError("SetShader: FlagsAttribName GL.VertexAttribIPointer");
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

                Shader.SetFloat(_ElapsedTimeUniformName, Time.ElapsedTime);
                Shader.SetMatrix4(_ModelViewMatrixUniformName, transpose: false, modelViewMatrix);
                Shader.SetMatrix4(_ProjectionMatrixUniformName, transpose: false, projectionMatrix);
            }

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.RasterizerDiscard);

            // Specifies the vertex to be used as the source of data for flat
            // shaded varyings. This is useful for the flags and colors that
            // we're changing
            GL.ProvokingVertex(ProvokingVertexMode.FirstVertexConvention);

            GL.BindVertexArray(_vaoHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.DrawArrays(PrimitiveType.Triangles, first: 0, (_vertexTexcoordBuffer.Length / 5));
            DebugUtility.CheckGLError("Render");
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

        private static void PopulateVertexTexcoordBuffer(Mesh mesh, float[] buffer) {
            var vertices = mesh.Vertices;
            var texcoords = mesh.Texcoords;

            for (int i = 0, j = 0, v = 0; i < mesh.Indices.Length; i++) {
                for (int primitive = 0; primitive < 3; primitive++) {
                    buffer[j + 0] = vertices[v].X; buffer[j + 1] = vertices[v].Y; buffer[j + 2] = vertices[v].Z; buffer[j + 3] = texcoords[v].X; buffer[j + 4] = texcoords[v].Y;
                    v++;
                    j += 5;
                }
            }
        }

        private static void PopulateGSColorBuffer(Mesh mesh, float[] buffer) {
            var gsColors = mesh.GSColors;

            if (mesh.GSColors == null) {
                return;
            }

            for (int i = 0, j = 0, v = 0; i < mesh.Indices.Length; i++) {
                for (int primitive = 0; primitive < 3; primitive++) {
                    buffer[j + 0] = gsColors[v].R; buffer[j + 1] = gsColors[v].G; buffer[j + 2] = gsColors[v].B;
                    v++;
                    j += 3;
                }
            }
        }

        private static void PopulateBaseColorBuffer(Mesh mesh, float[] buffer) {
            var gsColors = mesh.BaseColors;

            if (mesh.BaseColors == null) {
                return;
            }

            for (int i = 0, j = 0, v = 0; i < mesh.Indices.Length; i++) {
                for (int primitive = 0; primitive < 3; primitive++) {
                    buffer[j + 0] = gsColors[v].R; buffer[j + 1] = gsColors[v].G; buffer[j + 2] = gsColors[v].B;
                    v++;
                    j += 3;
                }
            }
        }

        private static void PopulateFlagsBuffer(Mesh mesh, uint[] buffer) {
            // XXX: Ideally, we would get an array of an enum that has a bunch
            //      of flags for us to set, or maybe this gets passed already to
            //      us
            for (int i = 0; i < mesh.Indices.Length; i++) {
                buffer[i * 3] = 1 << 0;
            }
        }
    }
}
