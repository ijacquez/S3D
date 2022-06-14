// using Raylib_cs;
// using S3D.FileFormats;
// using System.Collections.Generic;
// using System.Numerics;
// using System;

// namespace S3D.UI.Views {
//     public class ModelView : View {
//         private Camera3D _camera = new Camera3D();
//         private Vector3 _cubePosition = new Vector3(0.0f, 0.0f, 0.0f);

//         private readonly List<DisplayModel> _displayModels = new List<DisplayModel>();

//         public void Display(S3DObject s3dObject) {
//             if (s3dObject == null) {
//                 throw new NullReferenceException();
//             }

//             if (!IsDisplayModel(s3dObject)) {
//                 _displayModels.Add(new DisplayModel(s3dObject));
//             }
//         }

//         public void ToggleDisplay(S3DObject s3dObject, bool active) {
//             DisplayModel displayModel = GetDisplayModel(s3dObject);

//             if (displayModel == null) {
//                 return;
//             }

//             displayModel.Toggle(active);
//         }

//         public void ClearDisplay() {
//             _displayModels.Clear();
//         }

//         protected override void ViewInit() {
//             _camera.position = new Vector3(5.0f, 5.0f, 5.0f);
//             _camera.target = new Vector3(0.0f, 0.0f, 0.0f);
//             _camera.up = new Vector3(0.0f, 1.0f, 0.0f);
//             _camera.fovy = 90.0f;
//             _camera.projection = CameraProjection.CAMERA_ORTHOGRAPHIC;

//             Raylib.SetCameraMode(_camera, CameraMode.CAMERA_FREE);
//         }

//         protected override void ViewUpdate(float dt) {
//             Raylib.UpdateCamera(ref _camera);

//             foreach (DisplayModel displayModel in _displayModels) {
//                 if (displayModel.IsActive) {
//                 }
//             }
//         }

//         protected override void ViewDraw() {
//             Raylib.BeginMode3D(_camera);

//             Raylib.DrawCube(_cubePosition, 2.0f, 2.0f, 2.0f, Color.RED);
//             Raylib.DrawCubeWires(_cubePosition, 2.0f, 2.0f, 2.0f, Color.MAROON);

//             Raylib.DrawGrid(1000, 5.0f);

//             Raylib.EndMode3D();
//         }
// 
//         #region Display model

//         private class DisplayModel {
//             public S3DObject S3DObject { get; }

//             public Model Model { get; }

//             public bool IsActive { get; private set; }

//             private DisplayModel() {
//             }

//             public DisplayModel(S3DObject s3dObject) {
//                 S3DObject = s3dObject;
//             }

//             public void Toggle(bool active) {
//                 IsActive = active;
//             }
//         }

//         private bool IsDisplayModel(S3DObject s3dObject) {
//             return (GetDisplayModel(s3dObject) != null);
//         }

//         private DisplayModel GetDisplayModel(S3DObject s3dObject) {
//             return _displayModels.Find(PredicateFindS3DObject);

//             bool PredicateFindS3DObject(DisplayModel x) {
//                 return (x.S3DObject == s3dObject);
//             }
//         }

//         #endregion
//     }
// }
