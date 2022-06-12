using System.IO;
using Newtonsoft.Json;

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

        [JsonConstructor]
        private ProjectSettings() {
        }

        public ProjectSettings(string inputFilePath) {
            InputFilePath = Path.GetFullPath(inputFilePath);

            OutputFilePath = Path.Combine(InputDirPath, Path.ChangeExtension(InputFileName, "s3d"));
        }
    }
}
