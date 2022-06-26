using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using S3D.UI.OpenTKFramework.Types;
using S3D.UI.OpenTKFramework.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System;

namespace S3D.UI.ImGuiGlue {
    public class ImGuiController : IDisposable {
        private static readonly ShaderSource _VertexSource =
            new ShaderSource() {
                Type   = ShaderType.VertexShader,
                Source =
@"#version 330 core

uniform mat4 projection_matrix;

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoord;
layout(location = 2) in vec4 in_color;

out vec4 color;
out vec2 texCoord;

void main()
{
    gl_Position = projection_matrix * vec4(in_position, 0, 1);
    color = in_color;
    texCoord = in_texCoord;
}"
            };

        private static readonly ShaderSource _FragmentSource =
            new ShaderSource() {
            Type   = ShaderType.FragmentShader,
            Source =
@"#version 330 core

uniform sampler2D in_fontTexture;

in vec4 color;
in vec2 texCoord;

out vec4 outputColor;

void main()
{
    outputColor = color * texture(in_fontTexture, texCoord);
}"
            };

        private bool _frameBegun;

        private int _vertexArray;
        private int _vertexBuffer;
        private int _vertexBufferSize;
        private int _indexBuffer;
        private int _indexBufferSize;

        private Texture _fontTexture;
        private Shader _shader;

        private int _windowWidth;
        private int _windowHeight;

        private readonly List<char> _pressedChars = new List<char>();

        private System.Numerics.Vector2 _scaleFactor = System.Numerics.Vector2.One;

        private readonly ImGuiFocusEventArgs _imGuiFocusEventArgs = new ImGuiFocusEventArgs();

        public event EventHandler<ImGuiFocusEventArgs> Focus;

        /// <summary>
        /// Constructs a new ImGuiController.
        /// </summary>
        public ImGuiController(int width, int height) {
            _windowWidth = width;
            _windowHeight = height;

            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            var io = ImGui.GetIO();
            io.Fonts.AddFontDefault();

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            CreateDeviceResources();
            SetKeyMappings();

            SetPerFrameImGuiData(1.0f / 60.0f);

            ImGui.NewFrame();

            _frameBegun = true;
        }

        public void Resize(int width, int height) {
            _windowWidth = width;
            _windowHeight = height;
        }

        public void DestroyDeviceObjects() {
            Dispose();
        }

        public void CreateDeviceResources() {
            ObjectUtility.CreateVertexArray("ImGui", out _vertexArray);

            _vertexBufferSize = 10000;
            _indexBufferSize = 2000;

            ObjectUtility.CreateVertexBuffer("ImGui", out _vertexBuffer);
            ObjectUtility.CreateElementBuffer("ImGui", out _indexBuffer);
            GL.NamedBufferData(_vertexBuffer, _vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.NamedBufferData(_indexBuffer, _indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            RecreateFontDeviceTexture();

            _shader = new Shader("ImGui", _VertexSource, _FragmentSource);

            GL.VertexArrayVertexBuffer(_vertexArray, 0, _vertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
            GL.VertexArrayElementBuffer(_vertexArray, _indexBuffer);

            GL.EnableVertexArrayAttrib(_vertexArray, 0);
            GL.VertexArrayAttribBinding(_vertexArray, 0, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 0, 2, VertexAttribType.Float, false, 0);

            GL.EnableVertexArrayAttrib(_vertexArray, 1);
            GL.VertexArrayAttribBinding(_vertexArray, 1, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 1, 2, VertexAttribType.Float, false, 8);

            GL.EnableVertexArrayAttrib(_vertexArray, 2);
            GL.VertexArrayAttribBinding(_vertexArray, 2, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 2, 4, VertexAttribType.UnsignedByte, true, 16);

            DebugUtility.CheckGLError("End of ImGui setup");
        }

        /// <summary>
        /// Recreates the device texture used to render text.
        /// </summary>
        public void RecreateFontDeviceTexture() {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            _fontTexture = new Texture("ImGui Text Atlas", width, height, pixels);
            _fontTexture.SetMagFilter(TextureMagFilter.Linear);
            _fontTexture.SetMinFilter(TextureMinFilter.Linear);

            io.Fonts.SetTexID((IntPtr)_fontTexture.Handle);

            io.Fonts.ClearTexData();
        }

        /// <summary>
        /// Renders the ImGui draw list data.
        /// </summary>
        public void Render() {
            if (_frameBegun) {
                _frameBegun = false;
                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData());
            }
        }

        /// <summary>
        /// Updates ImGui input and IO configuration state.
        /// </summary>
        public void Update(GameWindow gameWindow, float deltaSeconds) {
            if (_frameBegun) {
                ImGui.Render();
            }

            SetPerFrameImGuiData(deltaSeconds);
            UpdateImGuiInput(gameWindow);

            _frameBegun = true;

            ImGui.NewFrame();
        }

        /// <summary>
        /// Sets per-frame data based on the associated window.
        /// This is called by Update(float).
        /// </summary>
        private void SetPerFrameImGuiData(float deltaSeconds) {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(_windowWidth / _scaleFactor.X,
                                                         _windowHeight / _scaleFactor.Y);
            io.DisplayFramebufferScale = _scaleFactor;
            io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
        }

        private void UpdateImGuiInput(GameWindow gameWindow) {
            ImGuiIOPtr io = ImGui.GetIO();

            MouseState MouseState = gameWindow.MouseState;
            KeyboardState KeyboardState = gameWindow.KeyboardState;

            io.MouseDown[0] = MouseState[MouseButton.Left];
            io.MouseDown[1] = MouseState[MouseButton.Right];
            io.MouseDown[2] = MouseState[MouseButton.Middle];

            var screenPoint = new Vector2i((int)MouseState.X, (int)MouseState.Y);
            var point = screenPoint;
            io.MousePos = new System.Numerics.Vector2(point.X, point.Y);

            foreach (Keys key in Enum.GetValues(typeof(Keys))) {
                if (key == Keys.Unknown) {
                    continue;
                }
                io.KeysDown[(int)key] = KeyboardState.IsKeyDown(key);
            }

            foreach (var c in _pressedChars) {
                io.AddInputCharacter(c);
            }
            _pressedChars.Clear();

            io.KeyCtrl = KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl);
            io.KeyAlt = KeyboardState.IsKeyDown(Keys.LeftAlt) || KeyboardState.IsKeyDown(Keys.RightAlt);
            io.KeyShift = KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift);
            io.KeySuper = KeyboardState.IsKeyDown(Keys.LeftSuper) || KeyboardState.IsKeyDown(Keys.RightSuper);

            _imGuiFocusEventArgs.IsFocused = io.WantCaptureMouse || io.WantCaptureKeyboard;

            Focus?.Invoke(this, _imGuiFocusEventArgs);
        }

        internal void PressChar(char keyChar) {
            _pressedChars.Add(keyChar);
        }

        internal void MouseScroll(Vector2 offset) {
            ImGuiIOPtr io = ImGui.GetIO();

            io.MouseWheel = offset.Y;
            io.MouseWheelH = offset.X;
        }

        private static void SetKeyMappings() {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab]        = (int)Keys.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow]  = (int)Keys.Left;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
            io.KeyMap[(int)ImGuiKey.UpArrow]    = (int)Keys.Up;
            io.KeyMap[(int)ImGuiKey.DownArrow]  = (int)Keys.Down;
            io.KeyMap[(int)ImGuiKey.PageUp]     = (int)Keys.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown]   = (int)Keys.PageDown;
            io.KeyMap[(int)ImGuiKey.Home]       = (int)Keys.Home;
            io.KeyMap[(int)ImGuiKey.End]        = (int)Keys.End;
            io.KeyMap[(int)ImGuiKey.Delete]     = (int)Keys.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace]  = (int)Keys.Backspace;
            io.KeyMap[(int)ImGuiKey.Enter]      = (int)Keys.Enter;
            io.KeyMap[(int)ImGuiKey.Escape]     = (int)Keys.Escape;
            io.KeyMap[(int)ImGuiKey.A]          = (int)Keys.A;
            io.KeyMap[(int)ImGuiKey.C]          = (int)Keys.C;
            io.KeyMap[(int)ImGuiKey.V]          = (int)Keys.V;
            io.KeyMap[(int)ImGuiKey.X]          = (int)Keys.X;
            io.KeyMap[(int)ImGuiKey.Y]          = (int)Keys.Y;
            io.KeyMap[(int)ImGuiKey.Z]          = (int)Keys.Z;
        }

        private void RenderImDrawData(ImDrawDataPtr drawData) {
            if (drawData.CmdListsCount == 0) {
                return;
            }

            for (int i = 0; i < drawData.CmdListsCount; i++) {
                ImDrawListPtr cmdList = drawData.CmdListsRange[i];

                int vertexSize = cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if (vertexSize > _vertexBufferSize) {
                    int newSize = (int)Math.Max(_vertexBufferSize * 1.5f, vertexSize);
                    GL.NamedBufferData(_vertexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    _vertexBufferSize = newSize;

                    Debug.Print($"Resized dear imgui vertex buffer to new size {_vertexBufferSize}");
                }

                int indexSize = cmdList.IdxBuffer.Size * sizeof(ushort);
                if (indexSize > _indexBufferSize) {
                    int newSize = (int)Math.Max(_indexBufferSize * 1.5f, indexSize);
                    GL.NamedBufferData(_indexBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    _indexBufferSize = newSize;

                    Debug.Print($"Resized dear imgui index buffer to new size {_indexBufferSize}");
                }
            }

            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();

            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(
                left:       0.0f,
                right:     io.DisplaySize.X,
                bottom:    io.DisplaySize.Y,
                top:        0.0f,
                depthNear: -1.0f,
                depthFar:   1.0f);

            _shader.Bind();
            _shader.SetMatrix4("projection_matrix", false, mvp);
            _shader.SetInt("in_fontTexture", 0);

            GL.BindVertexArray(_vertexArray);
            DebugUtility.CheckGLError("BindVertexArray");

            drawData.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            // Render command lists
            for (int n = 0; n < drawData.CmdListsCount; n++) {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                GL.NamedBufferSubData(_vertexBuffer, IntPtr.Zero, cmdList.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmdList.VtxBuffer.Data);
                DebugUtility.CheckGLError($"Data Vert {n}");

                GL.NamedBufferSubData(_indexBuffer, IntPtr.Zero, cmdList.IdxBuffer.Size * sizeof(ushort), cmdList.IdxBuffer.Data);
                DebugUtility.CheckGLError($"Data Idx {n}");

                for (int cmdIndex = 0; cmdIndex < cmdList.CmdBuffer.Size; cmdIndex++) {
                    ImDrawCmdPtr pcmd = cmdList.CmdBuffer[cmdIndex];
                    if (pcmd.UserCallback != IntPtr.Zero) {
                        throw new NotImplementedException();
                    } else {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);
                        DebugUtility.CheckGLError("Texture");

                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, _windowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
                        DebugUtility.CheckGLError("Scissor");

                        if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0) {
                            GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(pcmd.IdxOffset * sizeof(ushort)), (int)pcmd.VtxOffset);
                        } else {
                            GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                        }
                        DebugUtility.CheckGLError("Draw");
                    }
                }
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
        }

        /// <summary>
        ///   Frees all graphics resources used by the renderer.
        /// </summary>
        public void Dispose() {
            GL.DeleteVertexArray(_vertexArray);
            GL.DeleteBuffer(_vertexBuffer);
            GL.DeleteBuffer(_indexBuffer);

            _fontTexture.Dispose();
            _shader.Dispose();
        }
    }
}
