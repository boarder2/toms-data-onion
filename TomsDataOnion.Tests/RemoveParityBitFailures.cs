namespace TomsDataOnion.Tests;

public class RemoveParityFailures
{
    [TestCase(
        new byte[]
        {
			0b0011_0100, //bad
			0b1011_0100, //good
			0b1011_0100, //good
			0b1011_0101, //bad
			0b1111_1111, //good
			0b0000_0000, //good
			0b1000_0001, //good
			0b0000_0011, //good
			0b0000_0011, //good
			0b0000_0011, //good
        },
        new byte[]
        {
			0b101_10101,
			0b011_01011,
			0b111_11000,
			0b000_01000,
			0b000_00000,
			0b010_00000,
			0b100_00001
		}
	)]
    public void GetsExpectedOutput(byte[] input, byte[] expected)
    {
        Assert.AreEqual(expected, input.RemoveParityFailures());
    }
}