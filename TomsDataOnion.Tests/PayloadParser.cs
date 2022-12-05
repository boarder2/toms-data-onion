namespace TomsDataOnion.Tests;

public class PayloadParserTests
{
    const string payload = "<~test~>";

    const string expected = "test";

    private string _result = "";

    [SetUp]
    public void Setup()
    {
        _result = payload.GetPayload();
    }

    [Test]
    public void GetsExpectedOutput()
    {
        Assert.AreEqual(expected, _result);
    }
}