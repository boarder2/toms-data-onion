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
}