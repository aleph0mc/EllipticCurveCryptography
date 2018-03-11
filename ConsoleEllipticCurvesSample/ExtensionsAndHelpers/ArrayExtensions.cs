using System;
using System.Collections.Generic;
using System.Text;

namespace EllipticCurves.ExtensionsAndHelpers
{
    public static class ArrayExtensions
    {
        private const ushort BASE256 = 256;

        public static ushort[] ToUShortArray(this byte[] Source)
        {
            var len = Source.Length;
            var lstShorts = new List<ushort>();
            for (int i = 0; i < len; i += 2)
            {
                var val = (ushort)(Source[i] + Source[i + 1] * BASE256);
                lstShorts.Add(val);
            }

            return lstShorts.ToArray();
        }

        public static byte[] ToByteArray(this ushort[] Source)
        {
            var len = Source.Length;
            var lstBytes = new List<byte>();
            for (int i = 0; i < len; i++)
            {
                var byteLs = (byte)(Source[i] % BASE256);
                lstBytes.Add(byteLs);
                var byteMs = (byte)(Source[i] / BASE256);
                lstBytes.Add(byteMs);
            }

            return lstBytes.ToArray();
        }
    }
}
