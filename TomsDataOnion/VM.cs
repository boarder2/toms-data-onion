public ref struct VM
{
	private const int REG8_A = 0;
	private const int REG8_B = 1;
	private const int REG8_C = 2;
	private const int REG8_D = 3;
	private const int REG8_E = 4;
	private const int REG8_F = 5;
	private const int REG32_LA = 0;
	private const int REG32_LB = 1;
	private const int REG32_LC = 2;
	private const int REG32_LD = 3;
	private const int REG32_PTR = 4;
	private const int REG32_PC = 5;

	byte[] _reg8 = new byte[5];
	UInt32[] _reg32 = new UInt32[5];

	private Span<byte> _memory;

	public VM(Span<byte> program)
	{
		_memory = program;
	}

	public void Run()
	{

	}

	private void WriteReg8(byte dest, byte value)
	{
		if(dest == 7)
		{
			var memLoc = _reg32[REG32_PTR] + _reg8[REG8_C];
		}
	}

	private void WriteMem(int loc, byte value)
	{
		_memory[loc] = value;
	}
}