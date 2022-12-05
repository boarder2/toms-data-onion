public static class EncodingExtensions
{
    public static string DecodeASCII85String(this string s)
    {
        return ASCIIEncoding.ASCII.GetString(s.DecodeASCII85Bytes());
    }

    public static byte[] DecodeASCII85Bytes(this string s)
    {
        s = s.Replace(" ", "")
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("z", "!!!!!");

        var fileBytes = ASCIIEncoding.ASCII.GetBytes(s);

        var fileLoc = 0;
        var result = new List<Byte>();
        while (true)
        {
            UInt32 resultInt = 0;
            var pad = 0;
            if (fileLoc + 5 > fileBytes.Length - 1)
            {
                pad = ((fileLoc + 5) - (fileBytes.Length - 1));
            }
            for (int iChunk = 0; iChunk < 5; iChunk++)
            {
                resultInt += (UInt32)(((fileLoc + iChunk > fileBytes.Length - 1) ? 75 : fileBytes[fileLoc + iChunk] - 33) * Math.Pow(85, 4 - iChunk));
            }
            var bytesToAdd = BitConverter.GetBytes(resultInt);
            if (BitConverter.IsLittleEndian) { Array.Reverse(bytesToAdd); }
            result.AddRange(bytesToAdd.Take(5 - pad));
            if (fileLoc >= fileBytes.Length - 1) break;
            fileLoc += 5;
        }
        return result.ToArray();
    }
}