public static class ArrayExtensions
{
    public static void FlipAndRotateBits(this byte[] bytes)
    {
        for (var i = 0; i < bytes.Length; i++)
        {
            var flip = (bytes[i] ^ 0b0101_0101);
            var firstBit = flip & 0b1000_0000;
            var rotate = flip >> 1;
            var final = rotate | (firstBit << 7);
            bytes[i] = (byte)final;
        }
    }
}