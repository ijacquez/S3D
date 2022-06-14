using S3D.Converters;
using S3D.FileFormats;
using S3D.IO;
using S3D.PaletteManagement;
using S3D.ProjectManagement;
using S3D.TextureManagement;
using System.IO;
using System;

namespace S3D.Core {
    public class Program {
        public static void Main(string[] args) {
            UI.Window w = new UI.Window("Test", 800, 800);

            UI.Views.MainView mainView = new UI.Views.MainView();

            w.Load += mainView.Load;
            w.RenderFrame += mainView.RenderFrame;

            w.Run();

            if (args.Length != 1) {
                return;
            }

            var paletteManager = new PaletteManager();
            var textureManager = new TextureManager(paletteManager);
            var textureWriteReferenceManager = new WriteReferenceManager<Texture>();
            var context = new Context(textureManager, paletteManager, textureWriteReferenceManager);

            string testFileName = args[0];
            string testFileExtension = Path.GetExtension(testFileName);

            Console.WriteLine(testFileExtension);

            // XXX: Checking for an extension is just for testing
            if (string.Equals(testFileExtension, ".obj")) {
                ProjectSettings projectSettings = ProjectManager.Create(testFileName);

                S3DConverter converter =
                    new WavefrontOBJS3DConverter(textureManager,
                                                 paletteManager,
                                                 projectSettings.InputDirPath,
                                                 projectSettings.InputFileName);

                projectSettings.AddObjects(converter.ToS3DObjects());

                S3DWriter.WriteS3DFile(projectSettings.OutputFilePath, projectSettings.Objects, context);

                ProjectManager.Save(projectSettings);
            } else if (string.Equals(testFileExtension, ".json")) {
                ProjectSettings projectSettings = ProjectManager.Open(testFileName);

                foreach (S3DObject s3dObject in projectSettings.Objects) {
                    foreach (S3DFace s3dFace in s3dObject.Faces) {
                        context.TextureManager.AddPicture(s3dFace.Picture);
                    }
                }

                S3DWriter.WriteS3DFile(projectSettings.OutputFilePath, projectSettings.Objects, context);

                ProjectManager.Save(projectSettings);
            }
        }
    }
}
