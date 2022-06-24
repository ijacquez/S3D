using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using S3D.FileFormats;
using S3D.UI.MathUtilities.Raycasting;
using S3D.UI.MeshUtilities;
using S3D.UI.OpenTKFramework.Types;
using System.Collections.Generic;
using System;

namespace S3D.UI.Views {
    public class ModelView : View {
        private ModelRender _modelRender;
        private Shader _shader;

        private readonly ClickedMeshPrimitiveEventArgs _clickedMeshPrimitiveEventArgs =
            new ClickedMeshPrimitiveEventArgs();

        public Model Model { get; }

        public event EventHandler<ClickedMeshPrimitiveEventArgs> ClickedMeshPrimitive;

        public ModelView() {
            Model = new Model();
        }

        public void DisplayObject(S3DObject s3dObject) {
            Mesh mesh = S3DMeshGenerator.Generate(s3dObject);

            Model.Meshes = new Mesh[] {
                mesh
            };

            // XXX: Remove
            foreach (var meshPrimitive in mesh.Primitives) {
                meshPrimitive.Flags |= MeshTriangleFlags.Textured;
            }

            _modelRender = new ModelRender(Model);
            _modelRender.SetShader(_shader);
        }

        protected override void OnLoad() {
            string vertexShader = System.IO.File.ReadAllText("Shaders/shader.vert");
            string fragmentShader = System.IO.File.ReadAllText("Shaders/shader.frag");

            _shader = new Shader("model_view", vertexShader, fragmentShader);
        }

        protected override void OnUnload() {
        }

        protected override void OnUpdateFrame() {
            if (Window.Input.MouseState.IsButtonPressed(MouseButton.Button1)) {
                Vector2 mousePoint = new Vector2(Window.Input.MouseState.X, Window.Input.MouseState.Y);

                Mesh mesh = Model.Meshes[0];

                if (Window.Camera.CastRay(mousePoint, mesh, out RaycastHitInfo hitInfo)) {
                    int index = (int)hitInfo.PrimitiveIndex;

                    _clickedMeshPrimitiveEventArgs.Mesh = mesh;
                    _clickedMeshPrimitiveEventArgs.MeshPrimitive = mesh.Primitives[index];
                    _clickedMeshPrimitiveEventArgs.Index = index;
                    _clickedMeshPrimitiveEventArgs.Point = hitInfo.Point;

                    ClickedMeshPrimitive?.Invoke(this, _clickedMeshPrimitiveEventArgs);
                }
            }
        }

        protected override void OnRenderFrame() {
            _modelRender.Render();
        }
    }
}
