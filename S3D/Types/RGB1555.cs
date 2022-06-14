using System;
using System.Drawing;

namespace S3D.Types {
    public readonly struct RGB1555 {
        private readonly Color _colorValue;

        public readonly byte R { get; }

        public readonly byte G { get; }

        public readonly byte B { get; }

        public RGB1555(Color colorValue) {
            _colorValue = colorValue;

            R = (byte)(colorValue.R >> 3);
            G = (byte)(colorValue.G >> 3);
            B = (byte)(colorValue.B >> 3);
        }

        public static implicit operator Color(RGB1555 value) =>
            value._colorValue;

        public static explicit operator RGB1555(Color value) =>
            new RGB1555(value);

        public static implicit operator UInt16(RGB1555 value) {
            return (UInt16)((1 << 15) | (value.B << 10) | (value.G << 5) | value.R);
        }

        public override string ToString() {
            return _colorValue.ToString();
        }
    }
}
