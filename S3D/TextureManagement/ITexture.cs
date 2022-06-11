using System.Numerics;
using S3D.TextureConverters;

namespace S3D.TextureManagement {
    public interface ITexture {
        string FilePath { get; }

        Vector2[] Vertices { get; }

        VDP1Data VDP1Data { get; }

        int SlotNumber { get; }
    }
}
