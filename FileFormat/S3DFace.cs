using System.Numerics;
using System.Drawing;
using S3D.TextureManagement;

namespace S3D.FileFormats {
    public class S3DFace {
        public int[] Indices { get; } = new int[4];

        public Vector3 Normal { get; set; }

        public Color[] GouraudShadingColors { get; } = new Color[4];

        public int GouraudShadingNumber { get; set; }

        public IPicture Picture { get; set; }

        public S3DFaceAttribs.FeatureFlags FeatureFlags { get; set; }

        public S3DFaceAttribs.PrimitiveType PrimitiveType { get; set; }

        public S3DFaceAttribs.PlaneType PlaneType { get; set; }

        public S3DFaceAttribs.SortType SortType { get; set; } = S3DFaceAttribs.SortType.Center;

        public S3DFaceAttribs.RenderFlags RenderFlags { get; set; }

        public S3DFaceAttribs.ColorCalculationMode ColorCalculationMode { get; set; }

        public S3DFaceAttribs.TextureType TextureType { get; set; }

        public S3DFaceAttribs.TextureFlipFlags TextureFlipFlags { get; set; }
    }
}
