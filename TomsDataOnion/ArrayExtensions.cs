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

    // Modifies itself but also returns itself for fluent use.
    public static byte[] DecryptLayer4(this byte[] bytes)
    {
        // Obfuscated into byte representation to avoid spoilers.
        // I got the key by reverse engineering what I knew about the expected results based on previous layers.
        // I could store the computed key here in code instead, but leaving this is at least a bit of proof of work.
        var desiredResult = new byte[] {61,61,91,32,76,97,121,101,114,32,52,47,54,58,32,78,101,116,119,111,114,107,32,84,114,97,102,102,105,99,32,93};
        var decryptKey = new byte[32];
        for (int iByte = 0; iByte < desiredResult.Length; iByte++)
        {
            decryptKey[iByte] = (byte)(bytes[iByte] ^ desiredResult[iByte]);
        }

        for (int iByte = 0; iByte < bytes.Length; iByte++)
        {
            bytes[iByte] = (byte)(bytes[iByte] ^ (decryptKey[iByte % 32]));
        }
        return bytes;
    }
}