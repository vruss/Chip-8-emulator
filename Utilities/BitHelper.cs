namespace Chip_8.Utilities
{
	public static class BitHelper
	{
		// nnn or addr - A 12-bit value, the lowest 12 bits of the instruction
		public static ushort Get_nnn(ushort instruction)
		{
			return (ushort)(instruction & 0xFFF);
		}

		// n or nibble - A 4-bit value, the lowest 4 bits of the instruction
		public static byte Get_n(ushort instruction)
		{
			return (byte)(instruction & 0xF);
		}

		// x - A 4-bit value, the lower 4 bits of the high byte of the instruction
		public static byte Get_x(ushort instruction)
		{
			return (byte)(instruction >> 8 & 0xF);
		}

		// y - A 4-bit value, the upper 4 bits of the low byte of the instruction
		public static byte Get_y(ushort instruction)
		{
			return (byte)(instruction >> 4 & 0xF);
		}

		// kk or byte - An 8-bit value, the lowest 8 bits of the instruction
		public static byte Get_kk(ushort instruction)
		{
			return (byte)(instruction & 0xFF);
		}
	}
}
