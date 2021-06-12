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

		// Registers and memory
		private readonly ushort[] memory = new ushort[4096];
		private readonly Stack<ushort> stack = new(16); // used to call subroutines/functions and return from them
		private readonly byte[] registers = new byte[16]; // general-purpose variable registers
		private ushort indexRegister; // point at locations in memory

		// Timers
		private readonly byte delayTimer; // decremented at a rate of 60 Hz (60 times per second) until it reaches 0
		private readonly byte soundTimer; // functions like the delay timer, but which also gives off a beeping sound as long as it’s not 0

		// Pseudo-registers
		private ushort programCounter; // points at the current instruction in memory

		// High-level working environment
		private bool shouldQuit;
		private ushort encodedInstruction;

		public Emulator()
		{
			this.programCounter = ProgramOffset;
		}

		// Fetch -> Decode -> Execute loop
		public void Start()
		{
			while (!this.shouldQuit)
			{
				this.encodedInstruction = this.Fetch();

				var instruction = this.Decode();

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
		public Instruction Decode()
		{
			return InstructionDecoder.DecodeInstruction(this.encodedInstruction);
		}

		// Execute the instruction and do what it tells you
		public void Execute(Instruction instruction)
		{
			switch (instruction)
			{
				case Instruction.SYS_addr:
					break;
				case Instruction.CLS:
					this.CLS();
					break;
				case Instruction.RET:
					break;
				case Instruction.JP_addr:
					this.JP_addr();
					break;
				case Instruction.CALL_addr:
					break;
				case Instruction.SE_Vx_byte:
					break;
				case Instruction.SNE_Vx_byte:
					break;
				case Instruction.SE_Vx_Vy:
					break;
				case Instruction.LD_Vx_byte:
					this.LD_Vx_byte();
					break;
				case Instruction.ADD_Vx_byte:
					this.ADD_Vx_byte();
					break;
				case Instruction.LD_Vx_Vy:
					break;
				case Instruction.OR_Vx_Vy:
					break;
				case Instruction.AND_Vx_Vy:
					break;
				case Instruction.XOR_Vx_Vy:
					break;
				case Instruction.ADD_Vx_Vy:
					break;
				case Instruction.SUB_Vx_Vy:
					break;
				case Instruction.SHR_Vx_Vy:
					break;
				case Instruction.SUBN_Vx_Vy:
					break;
				case Instruction.SHL_Vx_Vy:
					break;
				case Instruction.SNE_Vx_Vy:
					break;
				case Instruction.LD_I_addr:
					this.LD_I_addr();
					break;
				case Instruction.JP_V0_addr:
					break;
				case Instruction.RND_Vx_byte:
					break;
				case Instruction.DRW_Vx_Vy_nibble:
					this.DRW_Vx_Vy_nibble();
					break;
				case Instruction.SKP_Vx:
					break;
				case Instruction.SKNP_Vx:
					break;
				case Instruction.LD_Vx_DT:
					break;
				case Instruction.LD_Vx_K:
					break;
				case Instruction.LD_DT_Vx:
					break;
				case Instruction.LD_ST_Vx:
					break;
				case Instruction.ADD_I_Vx:
					break;
				case Instruction.LD_F_Vx:
					break;
				case Instruction.LD_B_Vx:
					break;
				case Instruction.LD_I_Vx:
					break;
				case Instruction.LD_Vx_I:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(instruction), instruction, null);
			}
		}

		/// <summary>
		/// 00E0 - CLS
		/// Clear the display.
		/// </summary>
		private void CLS()
		{
			Console.WriteLine("Clear screen");
		}

		/// <summary>
		/// 1nnn - JP addr
		/// Jump to location nnn.
		///
		/// The interpreter sets the program counter to nnn.
		/// </summary>
		private void JP_addr()
		{
			this.programCounter = BitHelper.Get_nnn(this.encodedInstruction);
		}

		/// <summary>
		/// 6xkk - LD Vx, byte
		/// Set Vx = kk.
		///
		/// The interpreter puts the value kk into register Vx.
		/// </summary>
		private void LD_Vx_byte()
		{
			var Vx = BitHelper.Get_y(this.encodedInstruction);
			var kk = BitHelper.Get_kk(this.encodedInstruction);
			this.registers[Vx] = kk;
		}

		/// <summary>
		/// 7xkk - ADD Vx, byte
		/// Set Vx = Vx + kk.
		///
		///	Adds the value kk to the value of register Vx, then stores the result in Vx.
		/// </summary>
		private void ADD_Vx_byte()
		{
			var Vx = BitHelper.Get_y(this.encodedInstruction);
			var kk = BitHelper.Get_kk(this.encodedInstruction);
			this.registers[Vx] = (byte)(this.registers[Vx] + kk);
		}

		/// <summary>
		/// Annn - LD I, addr
		/// Set I = nnn.
		///
		///	The value of register I is set to nnn.
		/// </summary>
		private void LD_I_addr()
		{
			this.indexRegister = BitHelper.Get_nnn(this.encodedInstruction);
		}

		/// <summary>
		/// Dxyn - DRW Vx, Vy, nibble
		/// Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.
		///
		///	The interpreter reads n bytes from memory, starting at the address stored in I.
		/// These bytes are then displayed as sprites on screen at coordinates (Vx, Vy).
		/// Sprites are XORed onto the existing screen.
		/// If this causes any pixels to be erased, VF is set to 1, otherwise it is set to 0.
		/// If the sprite is positioned so part of it is outside the coordinates of the display,
		/// it wraps around to the opposite side of the screen.
		/// </summary>
		private void DRW_Vx_Vy_nibble()
		{
			Console.WriteLine("Draw to screen");
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
