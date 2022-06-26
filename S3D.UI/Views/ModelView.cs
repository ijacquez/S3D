using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using S3D.UI.MathUtilities.Raycasting;
using S3D.UI.OpenTKFramework.Types;
using S3D.UI.Views.Events;
using System;

namespace S3D.UI.Views {
    public class ModelView : View {
        private ModelRender _modelRender;
        private Shader _shader;

        private readonly ClickMeshPrimitiveEventArgs _clickMeshPrimitiveEventArgs =
            new ClickMeshPrimitiveEventArgs();

        private Model _model { get; set; }

        public event EventHandler<ClickMeshPrimitiveEventArgs> ClickMeshPrimitive;

        public ModelView() {
            _model = new Model();
        }

        public void LoadModel(Model model) {
            if (model == null) {
                return;
            }

            _model = model;
            _modelRender = new ModelRender(_model);
            _modelRender.SetShader(_shader);
        }

        protected override void OnLoad() {
            var vertexShaderSource =
                Resources.LoadShaderSource(ShaderType.VertexShader, "Shaders/model_view.vert");
            var fragmentShaderSource =
                Resources.LoadShaderSource(ShaderType.FragmentShader, "Shaders/model_view.frag");

            _shader = new Shader("model_view", vertexShaderSource, fragmentShaderSource);
        }

        protected override void OnUnload() {
        }

        protected override void OnUpdateFrame() {
            if (_modelRender == null) {
                return;
            }

            if (!Window.Input.IsMouseFocused) {
                return;
            }

            var mouseState = Window.Input.MouseState;

            if (mouseState.IsButtonPressed(MouseButton.Button1)) {
                Vector2i mousePoint = new Vector2i((int)mouseState.X, (int)mouseState.Y);

                Mesh mesh = _model.Meshes[0];

                if (Window.Camera.CastRay(mousePoint, mesh, out RaycastHitInfo hitInfo)) {
                    int index = (int)hitInfo.PrimitiveIndex;

                    _clickMeshPrimitiveEventArgs.MultiSelect =
                        (Window.Input.KeyboardState.IsKeyDown(Keys.LeftShift) ||
                         Window.Input.KeyboardState.IsKeyDown(Keys.RightShift));

                    _clickMeshPrimitiveEventArgs.Mesh = mesh;
                    _clickMeshPrimitiveEventArgs.MeshPrimitive = mesh.Primitives[index];
                    _clickMeshPrimitiveEventArgs.Index = index;
                    _clickMeshPrimitiveEventArgs.Point = hitInfo.Point;

                    ClickMeshPrimitive?.Invoke(this, _clickMeshPrimitiveEventArgs);
                }
            }
        }

        protected override void OnRenderFrame() {
            _modelRender?.Render();
        }
    }
}
