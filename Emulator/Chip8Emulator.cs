using System;
using System.IO;

namespace Chip_8.Emulator
{
	public class Chip8Emulator
	{
		public const byte ScreenWidth = 64;
		public const byte ScreenHeight = 32;

		internal const ushort ProgramOffset = 0x200;
		internal const ushort FontOffset = 0x50;

		private readonly EmulatorState emulatorState;

		public Chip8Emulator()
		{
			this.emulatorState = new EmulatorState();
		}

		public byte[,] GetNextFrame()
		{
			while (true)
			{
				// Get the next encoded instruction
				this.emulatorState.EncodedInstruction = this.Fetch();

				// Decode the instruction into a method
				var instruction = this.Decode();

				// Execute the instruction
				instruction.Invoke(this.emulatorState);

				// Keep executing until we have a new frame
				if (this.emulatorState.HasNewFrame)
				{
					this.emulatorState.HasNewFrame = false;
					return this.emulatorState.DisplayBytes;
				}
			}
		}

		public void LoadRom(string fileName)
		{
			var fileBytes = File.ReadAllBytes(fileName);

			fileBytes.CopyTo(this.emulatorState.Memory, ProgramOffset);
			// TODO: Load emulator state from dump or something
		}

		// Fetch the next instruction from memory at the current program counter
		private ushort Fetch()
		{
			var b1 = this.emulatorState.Memory[this.emulatorState.ProgramCounter++];
			var b2 = this.emulatorState.Memory[this.emulatorState.ProgramCounter++];

			// Always construct 16-bit instructions as big-endian
			return BitConverter.ToUInt16(BitConverter.IsLittleEndian
				? new[] { b2, b1 }
				: new[] { b1, b2 }
			);
		}

		// Decode the instruction to find out what the emulator should do
		private Action<EmulatorState> Decode()
		{
			return InstructionDecoder.DecodeInstruction(this.emulatorState.EncodedInstruction);
		}

		public void PrintMemory(byte start = 0, byte end = byte.MaxValue)
		{
			Console.WriteLine("Start of Chip-8 RAM");

			for (var address = 0; address < this.emulatorState.Memory.Length; address++)
			{
				var instruction = this.emulatorState.Memory[address];

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
