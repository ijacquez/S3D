using Newtonsoft.Json;
using S3D.ProjectManagement.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace S3D.ProjectManagement {
    public static class ProjectManager {
        private sealed class OpenedProjectSettings {
            public ProjectSettings ProjectSettings { get; }

            public string SaveFilePath { get; set; }

            public string SaveFileName {
                get {
                    string saveFilePath = SaveFilePath?.Trim();

                    return (string.IsNullOrEmpty(saveFilePath))
                        ? saveFilePath
                        : Path.GetFullPath(SaveFilePath);
                }
            }

            private OpenedProjectSettings() {
            }

            public OpenedProjectSettings(ProjectSettings projectSettings, string saveFilePath) {
                ProjectSettings = projectSettings;
                SaveFilePath = Path.GetFullPath(saveFilePath);
            }
        }

        private static readonly List<OpenedProjectSettings> _OpenedProjectSettings =
            new List<OpenedProjectSettings>();

        /// <summary>
        ///   Create a <see cref="ProjectSettings"/> instance mapped to
        ///   <paramref name="inputFilePath"/>.
        /// </summary>
        public static ProjectSettings Create(string inputFilePath) {
            var projectSettings = new ProjectSettings(inputFilePath);

            // Since we're creating a new project, use the filename of the input
            // file path as the save path, just with the extension added
            string saveFileName = $"{Path.GetFileNameWithoutExtension(inputFilePath)}_settings.json";
            string saveFilePath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(inputFilePath)),
                                               saveFileName);

            _OpenedProjectSettings.Add(new OpenedProjectSettings(projectSettings, saveFilePath));

            return projectSettings;
        }

        /// <summary>
        ///   Open project settings from <paramref name="loadPath"/>.
        /// </summary>
        public static ProjectSettings Open(string loadPath) {
            try {
                string jsonString = File.ReadAllText(loadPath);

                var projectSettings = JsonConvert.DeserializeObject<ProjectSettings>(jsonString);

                if (GetOpenedProjectSettings(projectSettings) != null) {
                    throw new ProjectAlreadyOpenedException();
                }

                _OpenedProjectSettings.Add(new OpenedProjectSettings(projectSettings, loadPath));

                return projectSettings;
            } catch (JsonException) {
                throw new ProjectSyntaxErrorException();
            }
        }

        /// <summary>
        /// </summary>
        public static void Close(ProjectSettings projectSettings) {
            OpenedProjectSettings openedProjectSettings =
                GetOpenedProjectSettings(projectSettings);

            if (openedProjectSettings == null) {
                throw new ProjectNotOpenedException();
            }

            _OpenedProjectSettings.Remove(openedProjectSettings);
        }

        /// <summary>
        ///   Get save path associated to <paramref name="projectSettings"/>.
        /// </summary>
        public static string GetSavePath(ProjectSettings projectSettings) {
            OpenedProjectSettings openedProjectSettings =
                GetOpenedProjectSettings(projectSettings);

            if (openedProjectSettings == null) {
                throw new ProjectNotOpenedException();
            }

            return openedProjectSettings.SaveFilePath;
        }

        /// <summary>
        ///   Set <paramref name="savePath"/> associated with <paramref name="projectSettings"/>.
        /// </summary>
        public static void SetSavePath(ProjectSettings projectSettings, string savePath) {
            OpenedProjectSettings openedProjectSettings =
                GetOpenedProjectSettings(projectSettings);

            if (openedProjectSettings == null) {
                throw new ProjectNotOpenedException();
            }

            openedProjectSettings.SaveFilePath = Path.GetFullPath(savePath);
        }

        /// <summary>
        /// </summary>
        public static void Save(ProjectSettings projectSettings) {
            OpenedProjectSettings openedProjectSettings =
                GetOpenedProjectSettings(projectSettings);

            if (openedProjectSettings == null) {
                throw new ProjectNotOpenedException();
            }

            string serializedString =
                JsonConvert.SerializeObject(projectSettings, Formatting.Indented);

            File.WriteAllText(openedProjectSettings.SaveFilePath, serializedString);
        }

        private static OpenedProjectSettings GetOpenedProjectSettings(ProjectSettings projectSettings) {
            return _OpenedProjectSettings.Find(PredicateFindProjectSettings);

            bool PredicateFindProjectSettings(OpenedProjectSettings x) =>
                (x.ProjectSettings == projectSettings);
        }
    }
}
