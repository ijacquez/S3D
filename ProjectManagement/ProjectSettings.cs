using Newtonsoft.Json;
using S3D.FileFormats;
using System.Collections.Generic;
using System.IO;
using System;

namespace S3D.ProjectManagement {
    public sealed class ProjectSettings {
        [JsonProperty]
        public string InputFilePath { get; private set; }

        [JsonIgnore]
        public string InputFileName =>
            Path.GetFileName(InputFilePath);

        [JsonIgnore]
        public string InputDirPath =>
            Path.GetDirectoryName(Path.GetFullPath(InputFilePath));

        [JsonProperty]
        public string OutputFilePath { get; set; }

        [JsonIgnore]
        public string OutputFileName {
            get {
                string outputFilePath = OutputFilePath?.Trim();

                return (string.IsNullOrEmpty(outputFilePath))
                    ? outputFilePath
                    : Path.GetFileName(OutputFilePath);
            }
        }

        [JsonIgnore]
        public string OutputDirPath {
            get {
                string outputFilePath = OutputFilePath?.Trim();

                return (string.IsNullOrEmpty(outputFilePath))
                    ? outputFilePath
                    : Path.GetDirectoryName(Path.GetFullPath(outputFilePath));
            }
        }

        [JsonIgnore]
        public IReadOnlyList<S3DObject> Objects => _objects.AsReadOnly();

        [JsonProperty("Objects")]
        private List<S3DObject> _objects { get; set; } = new List<S3DObject>();

        [JsonConstructor]
        private ProjectSettings() {
        }

        public ProjectSettings(string inputFilePath) {
            InputFilePath = Path.GetFullPath(inputFilePath);

            OutputFilePath = Path.Combine(InputDirPath, Path.ChangeExtension(InputFileName, "s3d"));
        }

        public void AddObject(S3DObject s3dObject) {
            if (s3dObject == null) {
                throw new NullReferenceException();
            }

            if (!_objects.Contains(s3dObject)) {
                _objects.Add(s3dObject);
            }
        }

        public void RemoveObject(S3DObject s3dObject) {
            if (s3dObject == null) {
                return;
            }

            _objects.Remove(s3dObject);
        }

        public void AddObjects(IEnumerable<S3DObject> s3dObjects) {
            foreach (S3DObject s3dObject in s3dObjects) {
                AddObject(s3dObject);
            }
        }
    }
}
