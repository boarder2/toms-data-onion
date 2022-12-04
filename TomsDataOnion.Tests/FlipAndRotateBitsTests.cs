namespace TomsDataOnion.Tests;

public class FlipAndRotateBitsTests
{
    [Test]
    public void GetsExpectedOutput()
    {
        /*
        0110_0011
        flip
        0011_0110
        rot
        0001_1011
        */
        var b = new [] {(byte)0b0110_0011};
        b.FlipAndRotateBits();
        Assert.AreEqual(new [] {(byte)0b0001_1011}, b);
    }

    [Test]
    public void GetsExpectedOutput2()
    {
        /*
        1110_0010
        flip
        1011_0111
        rot
        1101_1011
        */
        var b = new [] {(byte)0b1110_0010};
        b.FlipAndRotateBits();
        Assert.AreEqual(new [] {(byte)0b1101_1011}, b);
    }

    [Test]
    public void GetsExpectedOutputForMultiple()
    {
        var b = new [] {(byte)0b1011_0100, (byte)0b0000_1111};
        b.FlipAndRotateBits();
        Assert.AreEqual(new [] {(byte)0b1111_0000, (byte)0b0010_1101}, b);
    }
}