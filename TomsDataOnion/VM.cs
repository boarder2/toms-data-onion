public class VM
{
	Dictionary<string, byte> reg8 = new Dictionary<string, byte>
	{
		{"a", 0},
		{"b", 0},
		{"c", 0},
		{"d", 0},
		{"e", 0},
		{"f", 0}
	};

	Dictionary<string, UInt32> reg32 = new Dictionary<string, UInt32>
	{
		{"la", 0},
		{"lb", 0},
		{"lc", 0},
		{"ld", 0},
		{"ptr", 0},
		{"pc", 0}
	};

	public VM(ReadOnlySpan<byte> program)
	{

	}

	public void Run()
	{

	}
}