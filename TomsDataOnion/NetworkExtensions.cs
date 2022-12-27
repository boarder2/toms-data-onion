public static class NetworkExtensions
{

    public static byte[] ReadUDP(this byte[] bytes)
    {
        var result = new List<byte>();
        Span<byte> data = bytes;
        for (int iByte = 0; iByte < data.Length; iByte++)
        {
            var ipHeader = new IPHeader(data.Slice(iByte, 20));
            iByte += 20;
            var udpHeader = new UDPHeader(data.Slice(iByte, 8));
            iByte += 8;

            var packetData = data.Slice(iByte, udpHeader.Length);
        }

        return result.ToArray();
    }

    private class UDPHeader
    {
        public int SourcePort { get; private set;}
        public int DestinationPort { get; private set;}
        public int Length { get; private set;}
        public int Checksum { get; private set;}

        public UDPHeader(Span<byte> headerData)
        {
            // Parse the source and destination ports
            SourcePort = (int)((headerData[0] << 8) | headerData[1]);
            DestinationPort = (int)((headerData[2] << 8) | headerData[3]);

            // Parse the length
            Length = (int)((headerData[4] << 8) | headerData[5]);

            // Parse the checksum
            Checksum = (int)((headerData[6] << 8) | headerData[7]);
        }
    }

    private class IPHeader
    {
        // Constructor that takes in a byte array and parses the header
        public IPHeader(Span<byte> headerData)
        {
            // Parse the version and header length
            Version = (int)(headerData[0] >> 4);
            HeaderLength = (int)(headerData[0] & 0x0F) * 4;

            // Parse the service type
            ServiceType = (int)((headerData[1] >> 2) & 0x3F);

            // Parse the total length
            TotalLength = (int)((headerData[2] << 8) | headerData[3]);

            // Parse the identification
            Identification = (int)((headerData[4] << 8) | headerData[5]);

            // Parse the flags and fragment offset
            Flags = (int)((headerData[6] >> 5) & 0x07);
            FragmentOffset = (int)(((headerData[6] & 0x1F) << 8) | headerData[7]);

            // Parse the time-to-live
            TimeToLive = (int)headerData[8];

            // Parse the protocol
            Protocol = (int)headerData[9];

            // Parse the header checksum
            HeaderChecksum = (int)((headerData[10] << 8) | headerData[11]);

            // Parse the source and destination addresses
            SourceAddress = headerData[12].ToString() + "." + headerData[13].ToString() + "." + headerData[14].ToString() + "." + headerData[15].ToString();
            DestinationAddress = headerData[16].ToString() + "." + headerData[17].ToString() + "." + headerData[18].ToString() + "." + headerData[19].ToString();
        }

        // Properties to allow access to the header information
        public int Version { get; private set; }
        public int HeaderLength { get; private set;}
        public int ServiceType { get; private set; }
        public int TotalLength { get; private set; }
        public int Identification { get; private set; }
        public int Flags { get; private set; }
        public int FragmentOffset { get; private set; }
        public int TimeToLive { get; private set; }
        public int Protocol { get; private set; }
        public int HeaderChecksum { get; private set; }
        public string SourceAddress { get; private set; }
        public string DestinationAddress { get; private set; }
    }

}