using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chip_8.Utilities;

namespace Chip_8.Emulator
{
	public class Emulator
	{
		private const ushort ProgramOffset = 0x200;
		private const ushort FontOffset = 0x50;

		private bool shouldQuit;

		// Registers and memory
		private readonly ushort[] memory = new ushort[4096];
		private readonly Stack<ushort> stack = new(16); // used to call subroutines/functions and return from them
		private readonly byte[] registers = new byte[16]; // general-purpose variable registers
		private readonly ushort indexRegister; // point at locations in memory

		// Timers
		private readonly byte delayTimer; // decremented at a rate of 60 Hz (60 times per second) until it reaches 0
		private readonly byte soundTimer; // functions like the delay timer, but which also gives off a beeping sound as long as it’s not 0

		// Pseudo-registers
		private ushort programCounter; // points at the current instruction in memory

		public Emulator()
		{
			this.programCounter = ProgramOffset;
		}

		// Fetch -> Decode -> Execute loop
		public void Start()
		{
			while (!this.shouldQuit)
			{
				var encodedInstruction = this.Fetch();

				var instruction = this.Decode(encodedInstruction);

				this.Execute(instruction);
			}
		}

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

		// Fetch the instruction from memory at the current program counter
		public ushort Fetch()
		{
			return this.memory[this.programCounter++];
		}

		// Decode the instruction to find out what the emulator should do
		public Instruction Decode(ushort encodedInstruction)
		{
			return InstructionDecoder.DecodeInstruction(encodedInstruction);
		}

		// Execute the instruction and do what it tells you
		public void Execute(Instruction instruction)
		{
			Console.WriteLine(instruction);
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
