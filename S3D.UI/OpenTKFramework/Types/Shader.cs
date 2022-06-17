using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using S3D.UI.OpenTKFramework.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace S3D.UI.OpenTKFramework.Types {
    public struct UniformFieldInfo {
        public int Location { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public ActiveUniformType Type { get; set; }
    }

    public struct AttribFieldInfo {
        public int Location { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public ActiveAttribType Type { get; set; }
    }

    public class Shader {
        public string Name { get; }

        public int Handler { get; private set; }

        private readonly Dictionary<string, int> _uniformToLocation =
            new Dictionary<string, int>();
        private readonly Dictionary<string, int> _attribToLocation =
            new Dictionary<string, int>();

        private bool Initialized { get; set; } = false;

        private (ShaderType Type, string Path)[] Files { get; }

        public Shader(string name, string vertexShader, string fragmentShader) {
            Name = name;

            Files = new[] {
                (ShaderType.VertexShader, vertexShader),
                (ShaderType.FragmentShader, fragmentShader),
            };

            Handler = CreateProgram(name, Files);

            foreach (UniformFieldInfo fieldInfo in GetUniforms()) {
                _uniformToLocation.Add(fieldInfo.Name, fieldInfo.Location);
            }

            foreach (AttribFieldInfo fieldInfo in GetAttribs()) {
                _attribToLocation.Add(fieldInfo.Name, fieldInfo.Location);
            }
        }

        public void Bind() {
            GL.UseProgram(Handler);
        }

        public void Dispose() {
            if (Initialized) {
                GL.DeleteProgram(Handler);

                Initialized = false;
            }
        }

        public UniformFieldInfo[] GetUniforms() {
            GL.GetProgram(Handler, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            UniformFieldInfo[] uniforms = new UniformFieldInfo[uniformCount];

            for (int i = 0; i < uniformCount; i++) {
                string name = GL.GetActiveUniform(Handler, i, out int Size, out ActiveUniformType Type);

                UniformFieldInfo fieldInfo = new UniformFieldInfo();

                fieldInfo.Location = GL.GetUniformLocation(Handler, name);
                fieldInfo.Name = name;
                fieldInfo.Size = Size;
                fieldInfo.Type = Type;

                uniforms[i] = fieldInfo;
            }

            return uniforms;
        }


        public AttribFieldInfo[] GetAttribs() {
            GL.GetProgram(Handler, GetProgramParameterName.ActiveAttributes, out int attribCount);

            AttribFieldInfo[] attribs = new AttribFieldInfo[attribCount];

            for (int i = 0; i < attribCount; i++) {
                string name = GL.GetActiveAttrib(Handler, i, out int Size, out ActiveAttribType Type);

                AttribFieldInfo fieldInfo = new AttribFieldInfo();

                fieldInfo.Location = GetAttribLocation(name);
                fieldInfo.Name = name;
                fieldInfo.Size = Size;
                fieldInfo.Type = Type;

                attribs[i] = fieldInfo;
            }

            return attribs;
        }

        public void SetMatrix4(string uniform, bool transpose, Matrix4 transform) {
            if (!_uniformToLocation.TryGetValue(uniform, out int location)) {
                Debug.Print($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
            } else {
                Bind();

                GL.UniformMatrix4(location, transpose: transpose, ref transform);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetUniformLocation(string uniform) {
            if (!_uniformToLocation.TryGetValue(uniform, out int location)) {
                Debug.Print($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
                return -1;
            }

            return GL.GetUniformLocation(Handler, uniform);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAttribLocation(string attrib) {
            if (!_attribToLocation.TryGetValue(attrib, out int location)) {
                Debug.Print($"The attrib '{attrib}' does not exist in the shader '{Name}'!");
            }

            return GL.GetAttribLocation(Handler, attrib);
        }

        private int CreateProgram(string name, params (ShaderType Type, string source)[] shaderPaths) {
            CreationUtility.CreateProgram(name, out int program);

            int[] shaders = new int[shaderPaths.Length];
            for (int i = 0; i < shaderPaths.Length; i++) {
                shaders[i] = CompileShader(name, shaderPaths[i].Type, shaderPaths[i].source);
            }

            foreach (var shader in shaders) {
                GL.AttachShader(program, shader);
            }

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0) {
                string infoLogString = GL.GetProgramInfoLog(program);
                Debug.Print($"GL.LinkProgram had info log [{name}]:\n{infoLogString}");
            }

            foreach (var shader in shaders) {
                GL.DetachShader(program, shader);
                GL.DeleteShader(shader);
            }

            Initialized = true;

            return program;
        }

        private int CompileShader(string name, ShaderType type, string source) {
            CreationUtility.CreateShader(type, name, out int shader);

            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0) {
                string infoLogString = GL.GetShaderInfoLog(shader);

                Debug.Print($"GL.CompileShader for shader '{Name}' [{type}] had info log:\n{infoLogString}");
            }

            return shader;
        }
    }
}
