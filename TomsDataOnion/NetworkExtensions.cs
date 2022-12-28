using System.Net;

public static class NetworkExtensions
{

    public static byte[] ReadUDP(this byte[] bytes)
    {
        using var result = new MemoryStream();
        ReadOnlySpan<byte> data = bytes;
        var offset = 0;
        while (offset < data.Length)
        {
            var ipHeader = new IPPacket(data, offset);
            if (!ipHeader.ValidChecksum
                || ipHeader.SourceAddress.ToString() != "10.1.1.10"
                || ipHeader.DestinationAddress.ToString() != "10.1.1.200")
            {
                offset += ipHeader.TotalLength;
                continue;
            }
            
            var udpPacket = new UDPPacket(data.Slice(offset + ipHeader.HeaderLength, ipHeader.TotalLength - ipHeader.HeaderLength), ipHeader.SourceAddress, ipHeader.DestinationAddress);
            if (udpPacket.ValidChecksum && udpPacket.DestinationPort == 42069)
            {
                result.Write(udpPacket.Data);
            }
            offset += ipHeader.TotalLength;
        }

        return result.ToArray();
    }

    public static ushort CalculateChecksum(ReadOnlySpan<byte> data, int dataLength)
    {
        int index = 0;
        long sum = 0;

        while (dataLength > 1)
        {
            var val = ((data[index] << 8) + data[index + 1]);
            sum += val;
            if (sum > UInt16.MaxValue) { sum -= UInt16.MaxValue; }
            index += 2;
            dataLength -= 2;
        }

        // If the header has an odd number of bytes, add the final byte
        if (dataLength > 0)
        {
            sum += data[index];
            if (sum > UInt16.MaxValue) { sum -= UInt16.MaxValue; }
        }

        return (ushort)(~sum);
    }

    private ref struct UDPPacket
    {
        public int SourcePort { get; private set; }
        public int DestinationPort { get; private set; }
        public int Length { get; private set; }
        public int Checksum { get; private set; }
        public bool ValidChecksum { get; private set; }
        public ReadOnlySpan<byte> Data { get; private set; } = ReadOnlySpan<byte>.Empty;

        public UDPPacket(ReadOnlySpan<byte> packetData, IPAddress srcIp, IPAddress dstIp)
        {
            // Parse the source and destination ports
            SourcePort = (int)((packetData[0] << 8) | packetData[1]);
            DestinationPort = (int)((packetData[2] << 8) | packetData[3]);

            // Parse the length
            Length = (int)((packetData[4] << 8) | packetData[5]);

            // Parse the checksum
            Checksum = (int)((packetData[6] << 8) | packetData[7]);

            ValidChecksum = Checksum == 0;

            if (!ValidChecksum)
            {
                // Calculate the pseudo-header checksum
                var pseudoHeader = new byte[12 + Length];
                Buffer.BlockCopy(srcIp.GetAddressBytes(), 0, pseudoHeader, 0, 4);
                Buffer.BlockCopy(dstIp.GetAddressBytes(), 0, pseudoHeader, 4, 4);
                pseudoHeader[8] = 0;
                pseudoHeader[9] = 17; // UDP protocol number
                pseudoHeader[10] = packetData[4];
                pseudoHeader[11] = packetData[5];
                var packetBytes = packetData.ToArray();
                packetBytes[6] = packetBytes[7] = 0;
                Buffer.BlockCopy(packetBytes, 0, pseudoHeader, 12, packetBytes.Length);
                var pseudoHeaderChecksum = CalculateChecksum((ReadOnlySpan<Byte>)pseudoHeader, pseudoHeader.Length);
                ValidChecksum = Checksum == pseudoHeaderChecksum;
            }

            // if (ValidChecksum)
            // {
            Data = packetData.Slice(8);
            //}
        }
    }

    private ref struct IPPacket
    {
        // Constructor that takes in a byte array and parses the header
        public IPPacket(ReadOnlySpan<byte> dataStream, int offset)
        {
            // Parse the version and header length
            Version = (int)(dataStream[0 + offset] >> 4);
            HeaderLength = (int)(dataStream[0 + offset] & 0x0F) * 4;

            // Parse the service type
            ServiceType = (int)((dataStream[1 + offset] >> 2) & 0x3F);

            // Parse the total length
            TotalLength = (int)((dataStream[2 + offset] << 8) | dataStream[3 + offset]);

            // Parse the identification
            Identification = (int)((dataStream[4 + offset] << 8) | dataStream[5 + offset]);

            // Parse the flags and fragment offset
            Flags = (int)((dataStream[6 + offset] >> 5) & 0x07);
            FragmentOffset = (int)(((dataStream[6 + offset] & 0x1F) << 8) | dataStream[7 + offset]);

            // Parse the time-to-live
            TimeToLive = (int)dataStream[8 + offset];

            // Parse the protocol
            Protocol = (int)dataStream[9 + offset];

            // Parse the header checksum
            HeaderChecksum = (int)((dataStream[10 + offset] << 8) | dataStream[11 + offset]);

            // Parse the source and destination addresses
            SourceAddress = new System.Net.IPAddress(new byte[] { dataStream[12 + offset], dataStream[13 + offset], dataStream[14 + offset], dataStream[15 + offset] });
            DestinationAddress = new System.Net.IPAddress(new byte[] { dataStream[16 + offset], dataStream[17 + offset], dataStream[18 + offset], dataStream[19 + offset] });

            var headerBytes = dataStream.Slice(offset, HeaderLength).ToArray();
            headerBytes[10] = headerBytes[11] = 0; // Checksum in the header should be zero when calculating it
            if (HeaderChecksum == CalculateChecksum(headerBytes, HeaderLength))
            {
                Data = dataStream.Slice(offset + HeaderLength, TotalLength - HeaderLength);
                ValidChecksum = true;
            }
        }

        // Properties to allow access to the header information
        public int Version { get; private set; }
        public int HeaderLength { get; private set; }
        public int ServiceType { get; private set; }
        public int TotalLength { get; private set; }
        public int Identification { get; private set; }
        public int Flags { get; private set; }
        public int FragmentOffset { get; private set; }
        public int TimeToLive { get; private set; }
        public int Protocol { get; private set; }
        public int HeaderChecksum { get; private set; }
        public System.Net.IPAddress SourceAddress { get; private set; }
        public System.Net.IPAddress DestinationAddress { get; private set; }
        public ReadOnlySpan<byte> Data { get; private set; } = ReadOnlySpan<byte>.Empty;
        public bool ValidChecksum { get; private set; }
    }

}