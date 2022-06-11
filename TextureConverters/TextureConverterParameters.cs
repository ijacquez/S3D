using System.Numerics;

namespace S3D.TextureConverters {
    public struct TextureConverterParameters {
        /// <summary>
        ///   Texture coordinates to sample input image.
        /// </summary>
        public Vector2[] TextureVertices { get; set; }

        /// <summary>
        ///   Targeted texture width.
        /// </summary>
        public int TargetWidth { get; set; }

        /// <summary>
        ///   Targeted texture height.
        /// </summary>
        public int TargetHeight { get; set; }

        /// <summary>
        ///   Allow duplicates to be generated.
        /// </summary>
        public bool AllowDuplicates { get; set; }

        /// <summary>
        ///   Dump processed textures.
        /// </summary>
        public bool DumpFile { get; set; }

        // public TextureConverterParameters() {
        // }

        // public TextureConverterParameters(TextureConverterParameters parameters) {
        //     Array.Copy(parameters.TextureVertices, TextureVertices, TextureVertices.Length);
        //
        //     TargetWidth = parameters.TargetWidth;
        //     TargetHeight = parameters.TargetHeight;
        //     AllowDuplicates = parameters.AllowDuplicates;
        //     DumpFile = parameters.DumpFile;
        // }
    }
}
