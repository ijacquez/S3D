using System.Numerics;
using S3D.TextureConverters;

namespace S3D.TextureManagement {
    public interface ITexture {
        string FilePath { get; }

        Vector2[] Vertices { get; }

        VDP1Data VDP1Data { get; }

        bool IsHorizontallyFlipped { get; }

        bool IsVerticallyFlipped { get; }

        int SlotNumber { get; }

        IPalette Palette { get; }
    }
}
