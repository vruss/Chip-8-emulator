using System;
using System.IO;
using System.Linq;
using Chip_8.Utilities;

namespace Chip_8.Emulator
{
	public class Emulator
	{
		private const ushort ProgramOffset = 0x200;
		private const ushort FontOffset = 0x50;

		// Registers and memory
		private readonly ushort[] memory = new ushort[4096];
		private readonly ushort[] stack = new ushort[16];
		private readonly byte[] registers = new byte[16];
		private readonly ushort indexRegister;

		// Timers
		private readonly byte soundTimer;
		private readonly byte delayTimer;

		// Pseudo-registers
		private readonly ushort programCounter;
		private readonly byte stackPointer;

		public void LoadRom(string fileName)
		{
			var fileBytes = File.ReadAllBytes("TestFiles/IBM_Logo.ch8");

			var instructions = fileBytes
				.SelectTwo((b1, b2) => BitConverter.ToUInt16(new[] { b2, b1 }))
				.ToArray();

			instructions.CopyTo(this.memory, ProgramOffset);
		}

		public void LoadFont(byte[] font)
		{
			font.CopyTo(this.memory, FontOffset);
		}

		public void PrintMemory(ushort start = 0, ushort end = ushort.MaxValue)
		{
			Console.WriteLine("Start of Chip-8 RAM");

			for (var address = 0; address < this.memory.Length; address++)
			{
				var instruction = this.memory[address];

				if (address == ProgramOffset)
				{
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine("Start of Chip-8 program");
				}

				if (address % 20 == 0)
				{
					Console.WriteLine();
				}

				Console.Write($"{instruction:X4} ");
			}
		}
	}
}
