using System.Numerics;
using System;

namespace S3D.TextureConverters {
    public class TextureConverterParameters {
        /// <summary>
        ///   Texture coordinates to sample input image.
        /// </summary>
        public Vector2[] TextureVertices { get; } = new Vector2[] {
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f)
        };

        /// <summary>
        ///   Targeted texture width.
        /// </summary>
        public int TargetWidth { get; set; } = -1;

        /// <summary>
        ///   Targeted texture height.
        /// </summary>
        public int TargetHeight { get; set; } = -1;

        public bool AllowDuplicates { get; set; } = false;

        public bool DumpFile { get; set; } = false;

        public TextureConverterParameters() {
        }

        public TextureConverterParameters(TextureConverterParameters parameters) {
            Array.Copy(parameters.TextureVertices, TextureVertices, TextureVertices.Length);

            TargetWidth = parameters.TargetWidth;
            TargetHeight = parameters.TargetHeight;
            DumpFile = parameters.DumpFile;
        }
    }
}
