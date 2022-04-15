using System;

namespace S3D {
    public class S3DFaceAttribStruct {
        public byte Flag { get; set; }

        public byte Sort { get; set; }
        public byte FeatureFlags { get; set; }

        public UInt16 TextureNumber { get; set; }

        public UInt16 RenderFlags { get; set; }
        public UInt16 ColorCalculationMode { get; set; }
        public UInt16 TextureType { get; set; }

        public UInt16 PaletteNumberOrRGB1555 { get; set; }

        public UInt16 GouraudShadingNumber { get; set; }

        public UInt16 PrimitiveType { get; set; }
        public UInt16 Dir { get; set; }
    }
}
