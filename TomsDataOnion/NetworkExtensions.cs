using System.Net;

public static class NetworkExtensions
{

	public static byte[] ReadUDP(this byte[] bytes)
	{
		var result = new List<byte>();
		ReadOnlySpan<byte> data = bytes;
		for (int offset = 0; offset < data.Length; offset++)
		{
			try
			{
				var ipHeader = new IPPacket(data, offset);
				if (ipHeader.SourceAddress.ToString() != "10.1.1.10" || ipHeader.DestinationAddress.ToString() != "10.1.1.200" || ipHeader.Data == ReadOnlySpan<byte>.Empty)
				{
					offset += ipHeader.TotalLength;
					continue;
				}
				var udpPacket = new UDPPacket(data.Slice(offset + ipHeader.HeaderLength, ipHeader.TotalLength - ipHeader.HeaderLength), ipHeader.SourceAddress, ipHeader.DestinationAddress);
				if (udpPacket.Data != ReadOnlySpan<byte>.Empty)
				{
					result.AddRange(udpPacket.Data.ToArray());
				}
				offset += ipHeader.TotalLength;
			}
			catch (Exception)
			{
				offset += 20;
			}
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
			if (((sum & 0b1_0000_0000_0000_0000u) >> 16) == 1)
			{
				sum = sum & 0b0_1111_1111_1111_1111u;
				sum += 1;
			}
			index += 2;
			dataLength -= 2;
		}

		// If the header has an odd number of bytes, add the final byte
		if (dataLength > 0)
		{
			sum += data[index];
		}

		return (ushort)(~sum);
	}

	private ref struct UDPPacket
	{
		public int SourcePort { get; private set; }
		public int DestinationPort { get; private set; }
		public int Length { get; private set; }
		public int Checksum { get; private set; }
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

			var udpHeader = new byte[8];
			Buffer.BlockCopy(packetData.Slice(0, 8).ToArray(), 0, udpHeader, 0, 8);
			var udpChecksum = BitConverter.ToUInt16(udpHeader, 6);

			// Calculate the pseudo-header checksum
			var pseudoHeader = new byte[12 + Length];
			Buffer.BlockCopy(srcIp.GetAddressBytes(), 0, pseudoHeader, 0, 4);
			Buffer.BlockCopy(dstIp.GetAddressBytes(), 0, pseudoHeader, 4, 4);
			pseudoHeader[8] = 0;
			pseudoHeader[9] = 17; // UDP protocol number
			Buffer.BlockCopy(BitConverter.GetBytes((ushort)Length), 0, pseudoHeader, 10, 2);
			Buffer.BlockCopy(packetData.ToArray(), 0, pseudoHeader, 12, packetData.Length);
			var pseudoHeaderChecksum = CalculateChecksum((ReadOnlySpan<Byte>)pseudoHeader, pseudoHeader.Length);

			// Validate the UDP checksum
			if (Checksum == 0 || Checksum == pseudoHeaderChecksum)
			{
				Data = packetData.Slice(8);
			}
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
			if (HeaderLength < 20) throw new Exception("Header length is not valid");

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
	}

}