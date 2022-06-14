using System;

namespace S3D.Types {
    public readonly struct Fix16 {
        private readonly float _floatValue;

        public Fix16(float floatValue) {
            _floatValue = floatValue;
        }

        public static implicit operator float(Fix16 fix) =>
            fix._floatValue;

        public static explicit operator Fix16(float value) =>
            new Fix16(value);

        public static implicit operator Int32(Fix16 fix) {
            float shiftValue = (65536.0f + ((fix._floatValue >= 0) ? 0.5f : -0.5f));

            return (Int32)((fix._floatValue * shiftValue));
        }

        public override string ToString() {
            return _floatValue.ToString();
        }
    }
}
