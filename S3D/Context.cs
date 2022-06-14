using S3D.IO;
using S3D.PaletteManagement;
using S3D.TextureManagement;

namespace S3D {
    public class Context {
        public TextureManager TextureManager { get; private set; }

        public PaletteManager PaletteManager { get; private set; }

        public WriteReferenceManager<Texture> TextureWriteReferenceManager { get; private set; }

        private Context() {
        }

        public Context(TextureManager textureManager,
                       PaletteManager paletteManager,
                       WriteReferenceManager<Texture> pictureManager) {
            TextureManager = textureManager;
            PaletteManager = paletteManager;
            TextureWriteReferenceManager = pictureManager;
        }
    }
}
