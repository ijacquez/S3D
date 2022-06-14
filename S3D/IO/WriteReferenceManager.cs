using System;
using System.Collections.Generic;

namespace S3D.IO {
    public class WriteReferenceManager<TObject> {
        private sealed class Context {
            public List<IWriteReference> WriteReferences { get; } = new List<IWriteReference>();
        }

        private readonly IDictionary<TObject, Context> _contextDict =
            new Dictionary<TObject, Context>();

        public void AddDeferredReference(TObject @object, IWriteReference writeReference) {
            if (@object == null) {
                throw new NullReferenceException();
            }

            if (!_contextDict.TryGetValue(@object, out Context context)) {
                context = new Context();

                _contextDict[@object] = context;
            }

            context.WriteReferences.Add(writeReference);
        }

        public IReadOnlyList<IWriteReference> GetDeferredReferences(TObject @object) {
            if (@object == null) {
                throw new NullReferenceException();
            }

            if (_contextDict.TryGetValue(@object, out Context context)) {
                return context.WriteReferences.AsReadOnly();
            }

            return new List<IWriteReference>();
        }
    }
}
