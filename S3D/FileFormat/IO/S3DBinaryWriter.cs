using MiscUtil.Conversion;
using MiscUtil.IO;
using S3D.IO;
using S3D.TextureConverters;
using S3D.Types;
using System.Drawing;
using System.IO;
using System.Numerics;
using System;

namespace S3D.FileFormats.IO {
    public class S3DBinaryWriter : IDisposable {
        private class WriteReference : IWriteReference {
            public long Offset { get; private set; }

            public WriteReference(Stream baseStream) {
                Offset = baseStream.Length;
            }
        }

        private static readonly char[] _Signature = new char[] { 'S', '3', 'D', '\0' };
        private static readonly UInt32 _Version   = 0xDEADBEEF;

        private EndianBinaryWriter _binaryWriter;

        private bool _disposed;

        public Stream BaseStream => _binaryWriter.BaseStream;

        private S3DBinaryWriter() {
        }

        public S3DBinaryWriter(Stream stream) {
            _binaryWriter = new EndianBinaryWriter(EndianBitConverter.Big, stream);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _binaryWriter.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }

        // // TODO: Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~S3DBinaryWriter()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        public void WriteFloat(float value) {
            _binaryWriter.Write((Fix16)value);
        }

        #region 8-bit writes

        public void WriteInt8(char value) {
            _binaryWriter.Write(value);
        }

        public void WriteUInt8(char value) {
            WriteUInt8((byte)value);
        }

        public void WriteUInt8(byte value) {
            _binaryWriter.Write(value);
        }

        #endregion

        #region 16-bit writes

        public void WriteUInt16(Int16 value) {
            WriteUInt16((UInt16)value);
        }

        public void WriteUInt16(UInt16 value) {
            _binaryWriter.Write(value);
        }

        #endregion

        #region 32-bit writes

        public void WriteInt32(Int32 value) {
            _binaryWriter.Write(value);
        }

        public void WriteUInt32(Int32 value) {
            WriteUInt32((UInt32)value);
        }

        public void WriteUInt32(UInt32 value) {
            _binaryWriter.Write(value);
        }

        public void WritePaddedZeros(int amount) {
            for (int i = 0; i < amount; i++) {
                _binaryWriter.Write((byte)0);
            }
        }

        #endregion

        public void WriteSignature() {
            _binaryWriter.Write(_Signature);
        }

        public void WriteVersion() {
            _binaryWriter.Write(_Version);
        }

        public void WriteFlags(S3DFlags flags) {
            WriteUInt32(0);
        }

        public void WriteFaceAttributes(S3DFaceAttribStruct faceAttribStruct) {
            WriteUInt8(faceAttribStruct.Flag);
            WriteUInt8((byte)(faceAttribStruct.Sort | faceAttribStruct.FeatureFlags));
            WriteUInt16(faceAttribStruct.TextureNumber);
            WriteUInt16((UInt16)(faceAttribStruct.RenderFlags | faceAttribStruct.ColorCalculationMode | faceAttribStruct.TextureType));
            WriteUInt16(faceAttribStruct.PaletteNumberOrRGB1555);
            WriteUInt16(faceAttribStruct.GouraudShadingNumber);
            WriteUInt16((UInt16)(faceAttribStruct.Dir | faceAttribStruct.PrimitiveType));
        }

        public void WriteVector(Vector3 normal) {
            WriteFloat(normal.X);
            WriteFloat(normal.Y);
            WriteFloat(normal.Z);
        }

        public void WriteColors(RGB1555[] colors) {
            if (colors == null) {
                return;
            }

            foreach (RGB1555 color in colors) {
                WriteUInt16(color);
            }
        }

        public void WriteColors(Color[] colors) {
            if (colors == null) {
                return;
            }

            foreach (Color color in colors) {
                WriteUInt16((RGB1555)color);
            }
        }

        public void WriteVDP1Data(VDP1Data vdp1Data) {
            _binaryWriter.Write(vdp1Data.Data);
        }

        #region Reference writes

        public IWriteReference WriteDeferredReference() {
            IWriteReference reference = new WriteReference(BaseStream);

            WriteUInt32(0);

            return reference;
        }

        public void WriteReferenceOffset(IWriteReference reference) {
            var internalReference = (WriteReference)reference;

            long currentOffset = BaseStream.Length;

            BaseStream.Seek(internalReference.Offset, SeekOrigin.Begin);

            WriteUInt32((UInt32)currentOffset);

            BaseStream.Seek(currentOffset, SeekOrigin.Begin);
        }

        #endregion
    }
}
