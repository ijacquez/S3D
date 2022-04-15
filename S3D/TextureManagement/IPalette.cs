using System.Drawing;
using S3D.Types;

namespace S3D.TextureManagement {
    public interface IPalette {
        RGB1555[] Colors { get; }

        int SlotNumber { get; }
    }
}
