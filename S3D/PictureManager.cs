using S3D.IO;
using S3D.TextureManagement;
using System.Collections.Generic;

namespace S3D {
    public class PictureManager {
        private sealed class Context {
            public List<IWriteReference> TextureDataReferences { get; } = new List<IWriteReference>();

            public List<IWriteReference> PaletteDataReferences { get; } = new List<IWriteReference>();
        }

        private readonly IDictionary<ITexture, Context> _textureContextDict =
            new Dictionary<ITexture, Context>();

        public void AddTextureDeferredReference(ITexture texture, IWriteReference writeReference) {
            Context context = GetContext(texture);

            context.TextureDataReferences.Add(writeReference);
        }

        public void AddPaletteDeferredReference(ITexture texture, IWriteReference writeReference) {
            Context context = GetContext(texture);

            context.PaletteDataReferences.Add(writeReference);
        }

        public IReadOnlyList<IWriteReference> GetTextureDeferredReferences(ITexture texture) {
            Context context = _textureContextDict[texture];

            return context.TextureDataReferences.AsReadOnly();
        }

        public IReadOnlyList<IWriteReference> GetPaletteDeferredReferences(ITexture texture) {
            Context context = _textureContextDict[texture];

            return context.PaletteDataReferences.AsReadOnly();
        }

        private Context GetContext(ITexture texture) {
            if (!_textureContextDict.TryGetValue(texture, out Context context)) {
                context = new Context();

                _textureContextDict[texture] = context;
            }

            return context;
        }
    }
}
