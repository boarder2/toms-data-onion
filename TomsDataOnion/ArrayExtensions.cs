public static class ArrayExtensions
{
    public static void FlipAndRotateBits(this byte[] bytes)
    {
        for (var i = 0; i < bytes.Length; i++)
        {
            var flip = (bytes[i] ^ 0b0101_0101);
            var final = (flip >> 1) | ((flip & 1) << 7);
            bytes[i] = (byte)final;
        }
    }

    public static byte[] RemoveParityFailures(this byte[] bytes)
    {
        var valid = new List<byte>();
        UInt64 section = 0;
        int sectionByteCount = 0;
        for (var iByte = 0; iByte < bytes.Length; iByte++)
        {
            var count1 = 0;
            for (int iBit = 1; iBit < 8; iBit++)
            {
                count1 += (bytes[iByte] >> iBit) & 1;
            }
            if ((count1 % 2) == (bytes[iByte] & 1)) // parity check
            {
                sectionByteCount++;
                section |= (byte)(bytes[iByte] >> 1);
                section <<= 7;
                if (sectionByteCount % 8 == 0)
                {
                    section >>= 7;
                    var sectionBytes = BitConverter.GetBytes(section);
                    if (BitConverter.IsLittleEndian) { Array.Reverse(sectionBytes); }
                    valid.AddRange(sectionBytes.Skip(1));
                    section = 0;
                    sectionByteCount = 0;
                }
            }
        }
        return valid.ToArray();
    }
}