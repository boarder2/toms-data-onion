namespace TomsDataOnion.Tests;

public class NetworkExtensionsTests
{
	[TestCase(
		new byte[]
		{
			0x45, 0x00,
			0x00, 0x3c,
			0x1c, 0x46,
			0x40, 0x00,
			0x40, 0x06,
			0x00, 0x00,
			//0xb1, 0xe6,
			0xac, 0x10,
			0x0a, 0x63,
			0xac, 0x10,
			0x0a, 0x0c
		},
		(ushort)0b1011000111100110 
	)]
	public void GetsExpectedChecksum(byte[] input, ushort expected)
	{
		Assert.AreEqual(expected, NetworkExtensions.CalculateChecksum((ReadOnlySpan<byte>) input, input.Length));
	}
}