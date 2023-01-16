public class VM
{
	public static byte[] TestProgram = new byte[] {
		0x50, 0x48,  // MVI b <- 72
		0xC2,        // ADD a <- b
		0x02,        // OUT a
		0xA8, 0x4D, 0x00, 0x00, 0x00, // MVI32 ptr <- 0x0000004d
		0x4F,        // MV a <- (ptr+c)
		0x02,        // OUT a
		0x50, 0x09,  // MVI b <- 9
		0xC4,        // XOR a <- b
		0x02,        // OUT a
		0x02,        // OUT a
		0xE1, 0x01,  // APTR 0x00000001
		0x4F,        // MV a <- (ptr+c)
		0x02,        // OUT a
		0xC1,        // CMP
		0x22, 0x1D, 0x00, 0x00, 0x00, // JNZ 0x0000001d
		0x48, 0x30,  // MVI a <- 48
		0x02,        // OUT a
		0x58, 0x03,  // MVI c <- 3
		0x4F,        // MV a <- (ptr+c)
		0x02,        // OUT a
		0xB0, 0x29, 0x00, 0x00, 0x00, // MVI32 pc <- 0x00000029
		0x48, 0x31,  // MVI a <- 49
		0x02,        // OUT a
		0x50, 0x0C,  // MVI b <- 12
		0xC3,        // SUB a <- b
		0x02,        // OUT a
		0xAA,        // MV32 ptr <- lb
		0x57,        // MV b <- (ptr+c)
		0x48, 0x02,  // MVI a <- 2
		0xC1,        // CMP
		0x21, 0x3A, 0x00, 0x00, 0x00, // JEZ 0x0000003a
		0x48, 0x32,  // MVI a <- 50
		0x02,        // OUT a
		0x48, 0x77,  // MVI a <- 119
		0x02,        // OUT a
		0x48, 0x6F,  // MVI a <- 111
		0x02,        // OUT a
		0x48, 0x72,  // MVI a <- 114
		0x02,        // OUT a
		0x48, 0x6C,  // MVI a <- 108
		0x02,        // OUT a
		0x48, 0x64,  // MVI a <- 100
		0x02,        // OUT a
		0x48, 0x21,  // MVI a <- 33
		0x02,        // OUT a
		0x01,        // HALT
		0x65, 0x6F, 0x33, 0x34, 0x2C  // non-instruction data
	};

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

	private byte[] _memory;

	public VM(byte[] program)
	{
		_memory = program;
	}

	public void Run()
	{

	}

	private void WriteReg8(byte dest, byte value)
	{
		if (dest == 7)
		{
			var memLoc = _reg32[REG32_PTR] + _reg8[REG8_C];
		}
	}

	private void WriteMem(int loc, byte value)
	{
		_memory[loc] = value;
	}
}