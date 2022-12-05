public static class Payload
{
	public static string GetPayload(this string s)
	{
		var start = s.IndexOf("<~") + 2;
		return s.Substring(start, s.LastIndexOf("~>") - start);
	}
}