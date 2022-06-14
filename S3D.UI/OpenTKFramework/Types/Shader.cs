using OpenTK.Graphics.OpenGL4;
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

    public class Shader {
        public string Name { get; }

        public int Program { get; private set; }

        private readonly Dictionary<string, int> _uniformToLocation =
            new Dictionary<string, int>();

        private bool Initialized { get; set; } = false;

        private (ShaderType Type, string Path)[] Files { get; }

        public Shader(string name, string vertexShader, string fragmentShader) {
            Name = name;
            Files = new[] {
                (ShaderType.VertexShader, vertexShader),
                (ShaderType.FragmentShader, fragmentShader),
            };
            Program = CreateProgram(name, Files);
        }

        public void UseShader() {
            GL.UseProgram(Program);
        }

        public void Dispose() {
            if (Initialized) {
                GL.DeleteProgram(Program);
                Initialized = false;
            }
        }

        public UniformFieldInfo[] GetUniforms() {
            GL.GetProgram(Program, GetProgramParameterName.ActiveUniforms, out int unifromCount);

            UniformFieldInfo[] uniforms = new UniformFieldInfo[unifromCount];

            for (int i = 0; i < unifromCount; i++) {
                string name = GL.GetActiveUniform(Program, i, out int Size, out ActiveUniformType Type);

                UniformFieldInfo fieldInfo = new UniformFieldInfo();
                fieldInfo.Location = GetUniformLocation(name);
                fieldInfo.Name = name;
                fieldInfo.Size = Size;
                fieldInfo.Type = Type;

                uniforms[i] = fieldInfo;
            }

            return uniforms;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetUniformLocation(string uniform) {
            if (_uniformToLocation.TryGetValue(uniform, out int location) == false) {
                location = GL.GetUniformLocation(Program, uniform);
                _uniformToLocation.Add(uniform, location);

                if (location == -1) {
                    Debug.Print($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
                }
            }

            return location;
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
                Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{infoLogString}");
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

                Debug.WriteLine($"GL.CompileShader for shader '{Name}' [{type}] had info log:\n{infoLogString}");
            }

            return shader;
        }
    }
}
