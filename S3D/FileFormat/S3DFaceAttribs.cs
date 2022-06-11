using System;

namespace S3D.FileFormats {
    public static class S3DFaceAttribs {
        [Flags]
        public enum FeatureFlags {
            None,
            UseTexture        = 1 << 2,
            UseLighting       = 1 << 3,
            UseDepthCueing    = 1 << 4,
            UsePalette        = 1 << 5,
            UseNearClipping   = 1 << 6,
            UseGouraudShading = 1 << 7
        }

        public enum PrimitiveType {
            NormalSprite    = 0,
            ScaledSprite    = 1,
            DistortedSprite = 2,
            Polygon         = 4,
            Polyline        = 5,
            Line            = 6
        }

        public enum PlaneType {
            Single,
            Dual
        }

        public enum SortType {
            Before,
            Min,
            Max,
            Center
        }

        [Flags]
        public enum RenderFlags {
            MSB                      = 1 << 15,
            HSS                      = 1 << 12,
            WindowIn                 = 2 << 9,
            WindowOut                = 3 << 9,
            Mesh                     = 1 << 8,
            DisableEndCodes          = 1 << 7,
            DisableTransparencyIndex = 1 << 6
        }

        [Flags]
        public enum ColorCalculationMode {
            Replace                          = 0,
            Shadow                           = 1,
            HalfLuminance                    = 2,
            HalfTransparent                  = 3,
            GouraudShading                   = 4,
            GouraudShadingAndHalfLuminance   = 5,
            GouraudShadingAndHalfTransparent = 6
        }

        public enum TextureType {
            ColorBank16  = 0,
            CLUT16       = 1 << 3,
            ColorBank64  = 2 << 3,
            ColorBank128 = 3 << 3,
            ColorBank256 = 4 << 3,
            RGB1555      = 5 << 3
        }

        [Flags]
        public enum TextureFlipFlags {
            None = 0,
            H    = 1 << 4,
            V    = 1 << 5
        }
    }
}
