using System.Net;

public static class NetworkExtensions
{
    private static IPAddress RequiredSrcAddress = IPAddress.Parse("10.1.1.10");
    private static IPAddress RequiredDstAddress = IPAddress.Parse("10.1.1.200");
    public static byte[] ReadValidUDP(this byte[] bytes)
    {
        using var result = new MemoryStream();
        ReadOnlySpan<byte> data = bytes;
        var offset = 0;
        while (offset < data.Length)
        {
            var ipPacket = new IPPacket(data, offset);
            if (ipPacket.ValidChecksum
                && ipPacket.SourceAddress.Equals(RequiredSrcAddress)
                && ipPacket.DestinationAddress.Equals(RequiredDstAddress))
            {
                var udpPacket = new UDPPacket(data.Slice(offset, ipPacket.HeaderLength), ipPacket.Data);
                if (udpPacket.ValidChecksum && udpPacket.DestinationPort == 42069)
                {
                    result.Write(udpPacket.Data);
                }
            }
            offset += ipPacket.TotalLength;
        }

        return result.ToArray();
    }

    public static bool ValidateChecksum(ReadOnlySpan<byte> data)
    {
        long sum = 0;

        for (var index = 0; index < data.Length - 1; index += 2)
        {
            sum += ((data[index] << 8) | data[index + 1]);
        }

        if (data.Length % 2 != 0)
        {
            sum += (data[data.Length - 1] << 8);
        }

        while (sum > UInt16.MaxValue)
        {
            sum -= UInt16.MaxValue;
        }
        return sum == UInt16.MaxValue;
    }

    private ref struct UDPPacket
    {
        public ReadOnlySpan<byte> Data { get; } = ReadOnlySpan<byte>.Empty;
        public int SourcePort { get; }
        public int Length { get; }
        public int DestinationPort { get; }
        public int Checksum { get; }
        public bool ValidChecksum { get; }

        public UDPPacket(ReadOnlySpan<byte> ipHeader, ReadOnlySpan<byte> udpPacket)
        {
            SourcePort = (udpPacket[0] << 8) | udpPacket[1];
            DestinationPort = (udpPacket[2] << 8) | udpPacket[3];
            Length = (udpPacket[4] << 8) | udpPacket[5];
            Checksum = (udpPacket[6] << 8) | udpPacket[7];
            ValidChecksum = Checksum == 0;

            if (!ValidChecksum)
            {
                using var pseudoHeader = new MemoryStream(12 + Length);
                pseudoHeader.Write(ipHeader.Slice(12, 8)); // Source and Dest IP addresses
                // Reserved 0 byte, UDP Protocol number, UDP Length
                pseudoHeader.Write(new byte[] { 0, 17, udpPacket[4], udpPacket[5] });
                pseudoHeader.Write(udpPacket);
                ValidChecksum = ValidateChecksum((ReadOnlySpan<Byte>)pseudoHeader.ToArray());
            }

            if (ValidChecksum)
            {
                Data = udpPacket.Slice(8);
            }
        }
    }

    private ref struct IPPacket
    {
        public ReadOnlySpan<byte> Data { get; } = ReadOnlySpan<byte>.Empty;
        public IPAddress SourceAddress { get; }
        public IPAddress DestinationAddress { get; }
        public int Version { get; }
        public int TotalLength { get; }
        public int TimeToLive { get; }
        public int ServiceType { get; }
        public int Protocol { get; }
        public int Identification { get; }
        public int HeaderLength { get; }
        public int HeaderChecksum { get; }
        public int FragmentOffset { get; }
        public int Flags { get; }
        public bool ValidChecksum { get; }

        public IPPacket(ReadOnlySpan<byte> dataStream, int offset)
        {
            Version = dataStream[0 + offset] >> 4;
            HeaderLength = (dataStream[0 + offset] & 0x0F) * 4;
            ServiceType = (dataStream[1 + offset] >> 2) & 0x3F;
            TotalLength = (dataStream[2 + offset] << 8) | dataStream[3 + offset];
            Identification = (dataStream[4 + offset] << 8) | dataStream[5 + offset];
            Flags = (dataStream[6 + offset] >> 5) & 0x07;
            FragmentOffset = ((dataStream[6 + offset] & 0x1F) << 8) | dataStream[7 + offset];
            TimeToLive = dataStream[8 + offset];
            Protocol = dataStream[9 + offset];
            HeaderChecksum = (dataStream[10 + offset] << 8) | dataStream[11 + offset];
            SourceAddress = new IPAddress(new byte[] { dataStream[12 + offset], dataStream[13 + offset], dataStream[14 + offset], dataStream[15 + offset] });
            DestinationAddress = new IPAddress(new byte[] { dataStream[16 + offset], dataStream[17 + offset], dataStream[18 + offset], dataStream[19 + offset] });
            ValidChecksum = ValidateChecksum(dataStream.Slice(offset, HeaderLength));

            if (ValidChecksum)
            {
                Data = dataStream.Slice(offset + HeaderLength, TotalLength - HeaderLength);
            }
        }
    }

}