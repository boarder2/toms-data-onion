public static class payload
{
	public static string GetPayload(this string s)
	{
		var start = s.IndexOf("<~") + 2;
		return s.Substring(start, s.LastIndexOf("~>") - start);
	}
}