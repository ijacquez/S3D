using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using S3D.UI.OpenTKFramework.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace S3D.UI.OpenTKFramework.Types {
    public class Shader {
        public string Name { get; }

        public int Handle { get; private set; }

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

            Handle = CreateProgram(name, Files);

            foreach (UniformFieldInfo fieldInfo in GetUniforms()) {
                _uniformToLocation.Add(fieldInfo.Name, fieldInfo.Location);
            }

            foreach (AttribFieldInfo fieldInfo in GetAttribs()) {
                _attribToLocation.Add(fieldInfo.Name, fieldInfo.Location);
            }
        }

        public void Bind() {
            GL.UseProgram(Handle);
            DebugUtility.CheckGLError($"GL.UseProgram({Handle})");
        }

        public void Dispose() {
            if (Initialized) {
                GL.DeleteProgram(Handle);

                Initialized = false;
            }
        }

        public UniformFieldInfo[] GetUniforms() {
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            UniformFieldInfo[] uniforms = new UniformFieldInfo[uniformCount];

            for (int i = 0; i < uniformCount; i++) {
                string name = GL.GetActiveUniform(Handle, i, out int Size, out ActiveUniformType Type);

                UniformFieldInfo fieldInfo = new UniformFieldInfo();

                fieldInfo.Location = GL.GetUniformLocation(Handle, name);
                fieldInfo.Name = name;
                fieldInfo.Size = Size;
                fieldInfo.Type = Type;

                uniforms[i] = fieldInfo;
            }

            return uniforms;
        }

        public AttribFieldInfo[] GetAttribs() {
            GL.GetProgram(Handle, GetProgramParameterName.ActiveAttributes, out int attribCount);

            AttribFieldInfo[] attribs = new AttribFieldInfo[attribCount];

            for (int i = 0; i < attribCount; i++) {
                string name = GL.GetActiveAttrib(Handle, i, out int Size, out ActiveAttribType Type);

                AttribFieldInfo fieldInfo = new AttribFieldInfo();

                fieldInfo.Location = GL.GetAttribLocation(Handle, name);
                fieldInfo.Name = name;
                fieldInfo.Size = Size;
                fieldInfo.Type = Type;

                attribs[i] = fieldInfo;
            }

            return attribs;
        }

        public void SetInt(string uniform, int value) {
            if (!_uniformToLocation.TryGetValue(uniform, out int location)) {
                System.Console.WriteLine($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
            } else {
                Bind();

                GL.Uniform1(location, value);
                DebugUtility.CheckGLError($"GL.Uniform1({Name}, {location})");
            }
        }

        public void SetFloat(string uniform, float value) {
            if (!_uniformToLocation.TryGetValue(uniform, out int location)) {
                System.Console.WriteLine($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
            } else {
                Bind();

                GL.Uniform1(location, value);
                DebugUtility.CheckGLError($"GL.Uniform1({Name}, {location})");
            }
        }

        public void SetMatrix4(string uniform, bool transpose, Matrix4 transform) {
            if (!_uniformToLocation.TryGetValue(uniform, out int location)) {
                System.Console.WriteLine($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
            } else {
                Bind();

                GL.UniformMatrix4(location, transpose: transpose, ref transform);
                DebugUtility.CheckGLError($"GL.UniformMatrix4({Name}, {location})");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetUniformLocation(string uniform) {
            if (!_uniformToLocation.TryGetValue(uniform, out int location)) {
                System.Console.WriteLine($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
                return -1;
            }

            return GL.GetUniformLocation(Handle, uniform);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetAttribLocation(string attrib) {
            if (!_attribToLocation.TryGetValue(attrib, out int location)) {
                System.Console.WriteLine($"The attrib '{attrib}' does not exist in the shader '{Name}'!");
            }

            return GL.GetAttribLocation(Handle, attrib);
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
                System.Console.WriteLine($"GL.LinkProgram had info log [{name}]:\n{infoLogString}");
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

                System.Console.WriteLine($"GL.CompileShader for shader '{Name}' [{type}] had info log:\n{infoLogString}");
            }

            return shader;
        }
    }
}
