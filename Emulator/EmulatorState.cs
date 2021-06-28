using System.Collections.Generic;

namespace Chip_8.Emulator
{
	public class EmulatorState
	{
		// Registers and memory
		public byte[] Memory { get; }				// memory map of ROM and Font data
		public Stack<ushort> Stack { get; }			// used to call subroutines/functions and return from them
		public byte[] Registers { get; }			// general-purpose variable registers
		public ushort IndexRegister { get; set; }	// point at locations in memory

		// Timers
		public byte DelayTimer { get; set; } // decremented at a rate of 60 Hz (60 times per second) until it reaches 0
		public byte SoundTimer { get; set; } // functions like the delay timer, but which also gives off a beeping sound as long as it’s not 0

		// Pseudo-registers
		public ushort ProgramCounter { get; set; } // points at the current instruction in memory

		// High-level working environment
		public ushort EncodedInstruction { get; set; } // the currently executing instruction
		public Key PressedKey { get; set; }			   // the currently pressed key

		// Display variables
		public byte[,] DisplayBytes { get; set; } // internal frame buffer for frame manipulation
		public bool HasNewFrame { get; set; }	  // indicates if the frame has been updated

		public EmulatorState()
		{
			this.Memory = new byte[4096];
			this.Stack = new Stack<ushort>(16);
			this.Registers = new byte[16];

			this.ProgramCounter = Chip8Emulator.ProgramOffset;

			this.DisplayBytes = new byte[Chip8Emulator.ScreenWidth, Chip8Emulator.ScreenHeight];

			Font.StandardFont.CopyTo(this.Memory, Chip8Emulator.FontOffset);
		}
	}
}
