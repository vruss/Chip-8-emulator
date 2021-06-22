using System;
using System.Collections.Generic;
using System.IO;
using Chip_8.Utilities;

namespace Chip_8.Emulator
{
	public class Chip8Emulator
	{
		public const byte ScreenWidth = 64;
		public const byte ScreenHeight = 32;

		private const ushort ProgramOffset = 0x200;
		private const ushort FontOffset = 0x50;

		// Registers and memory
		private readonly byte[] memory = new byte[4096];
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
		private readonly byte[,] displayBytes = new byte[ScreenWidth, ScreenHeight];

		public Chip8Emulator()
		{
			this.programCounter = ProgramOffset;

			Font.StandardFont.CopyTo(this.memory, FontOffset);
		}

		public byte[,] GetNextFrame()
		{
			this.encodedInstruction = this.Fetch();

			var instruction = this.Decode();

			this.Execute(instruction);

			// TODO: Cycle until we have new displayBytes to display to speed up emulation
			return this.displayBytes;
		}

		public void LoadRom(string fileName)
		{
			var fileBytes = File.ReadAllBytes(fileName);

			fileBytes.CopyTo(this.memory, ProgramOffset);
		}

		// Fetch the next instruction from memory at the current program counter
		private ushort Fetch()
		{
			var b1 = this.memory[this.programCounter++];
			var b2 = this.memory[this.programCounter++];

			// Always construct 16-bit instructions as big-endian
			return BitConverter.ToUInt16(BitConverter.IsLittleEndian
				? new[] { b2, b1 }
				: new[] { b1, b2 }
			);
		}

		// Decode the instruction to find out what the emulator should do
		private Instruction Decode()
		{
			return InstructionDecoder.DecodeInstruction(this.encodedInstruction);
		}

		// Execute the instruction and do what it tells you
		private void Execute(Instruction instruction)
		{
			switch (instruction)
			{
				case Instruction.SYS_addr:
					break;
				case Instruction.CLS:
					this.CLS();
					break;
				case Instruction.RET:
					this.RET();
					break;
				case Instruction.JP_addr:
					this.JP_addr();
					break;
				case Instruction.CALL_addr:
					this.Call_addr();
					break;
				case Instruction.SE_Vx_byte:
					this.SE_Vx_byte();
					break;
				case Instruction.SNE_Vx_byte:
					this.SNE_Vx_byte();
					break;
				case Instruction.SE_Vx_Vy:
					this.SE_Vx_Vy();
					break;
				case Instruction.LD_Vx_byte:
					this.LD_Vx_byte();
					break;
				case Instruction.ADD_Vx_byte:
					this.ADD_Vx_byte();
					break;
				case Instruction.LD_Vx_Vy:
					this.LD_Vx_Vy();
					break;
				case Instruction.OR_Vx_Vy:
					this.OR_Vx_Vy();
					break;
				case Instruction.AND_Vx_Vy:
					this.AND_Vx_Vy();
					break;
				case Instruction.XOR_Vx_Vy:
					this.XOR_Vx_Vy();
					break;
				case Instruction.ADD_Vx_Vy:
					this.ADD_Vx_Vy();
					break;
				case Instruction.SUB_Vx_Vy:
					this.SUB_Vx_Vy();
					break;
				case Instruction.SHR_Vx_Vy:
					this.SHR_Vx_Vy();
					break;
				case Instruction.SUBN_Vx_Vy:
					this.SUBN_Vx_Vy();
					break;
				case Instruction.SHL_Vx_Vy:
					this.SHL_Vx_Vy();
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
			for (var y = 0; y < ScreenHeight; y++)
			{
				for (var x = 0; x < ScreenWidth; x++)
				{
					this.displayBytes[x, y] = 0;
				}
			}
		}

		/// <summary>
		/// 00EE - RET
		/// Return from a subroutine.
		///
		///	The interpreter sets the program counter to the address at the top of the stack,
		/// then subtracts 1 from the stack pointer.
		/// </summary>
		private void RET()
		{
			this.programCounter = this.stack.Pop();
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
		/// 2nnn - CALL addr
		/// Call subroutine at nnn.
		///
		///	The interpreter increments the stack pointer,
		/// then puts the current PC on the top of the stack.
		/// The PC is then set to nnn.
		/// </summary>
		private void Call_addr()
		{
			var nnn = BitHelper.Get_nnn(this.encodedInstruction);
			this.stack.Push(this.programCounter);
			this.programCounter = nnn;
		}

		/// <summary>
		/// 3xkk - SE Vx, byte
		/// Skip next instruction if Vx = kk.
		///
		/// The interpreter compares register Vx to kk, and if they are equal,
		/// increments the program counter by 2.
		/// </summary>
		private void SE_Vx_byte()
		{
			var Vx = this.registers[BitHelper.Get_x(this.encodedInstruction)];
			var kk = BitHelper.Get_kk(this.encodedInstruction);
			if (Vx == kk)
			{
				this.programCounter += 2;
			}
		}

		/// <summary>
		/// 4xkk - SNE Vx, byte
		/// Skip next instruction if Vx != kk.
		///
		///	The interpreter compares register Vx to kk, and if they are not equal,
		/// increments the program counter by 2.
		/// </summary>
		private void SNE_Vx_byte()
		{
			var Vx = this.registers[BitHelper.Get_x(this.encodedInstruction)];
			var kk = BitHelper.Get_kk(this.encodedInstruction);
			if (Vx != kk)
			{
				this.programCounter += 2;
			}
		}

		/// <summary>
		/// 5xy0 - SE Vx, Vy
		/// Skip next instruction if Vx = Vy.
		///
		///	The interpreter compares register Vx to register Vy, and if they are equal,
		/// increments the program counter by 2.
		/// </summary>
		private void SE_Vx_Vy()
		{
			var Vx = this.registers[BitHelper.Get_x(this.encodedInstruction)];
			var Vy = this.registers[BitHelper.Get_y(this.encodedInstruction)];
			if (Vx == Vy)
			{
				this.programCounter += 2;
			}
		}

		/// <summary>
		/// 6xkk - LD Vx, byte
		/// Set Vx = kk.
		///
		/// The interpreter puts the value kk into register Vx.
		/// </summary>
		private void LD_Vx_byte()
		{
			var x = BitHelper.Get_x(this.encodedInstruction);
			var kk = BitHelper.Get_kk(this.encodedInstruction);
			this.registers[x] = kk;
		}

		/// <summary>
		/// 8xy0 - LD Vx, Vy
		/// Set Vx = Vy.
		///
		/// Stores the value of register Vy in register Vx.
		/// </summary>
		private void LD_Vx_Vy()
		{
			var Vy = this.registers[BitHelper.Get_y(this.encodedInstruction)];
			this.registers[BitHelper.Get_x(this.encodedInstruction)] = Vy;
		}

		/// <summary>
		/// 8xy1 - OR Vx, Vy
		/// Set Vx = Vx OR Vy.
		///
		/// Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx.
		/// A bitwise OR compares the corresponding bits from two values,
		/// and if either bit is 1, then the same bit in the result is also 1. Otherwise, it is 0.
		/// </summary>
		private void OR_Vx_Vy()
		{
			var Vy = this.registers[BitHelper.Get_y(this.encodedInstruction)];
			this.registers[BitHelper.Get_x(this.encodedInstruction)] |= Vy;
		}

		/// <summary>
		/// 8xy2 - AND Vx, Vy
		/// Set Vx = Vx AND Vy.
		///
		/// Performs a bitwise AND on the values of Vx and Vy, then stores the result in Vx.
		/// A bitwise AND compares the corresponding bits from two values, and if both bits are 1,
		/// then the same bit in the result is also 1. Otherwise, it is 0.
		///
		/// </summary>
		private void AND_Vx_Vy()
		{
			var Vy = this.registers[BitHelper.Get_y(this.encodedInstruction)];
			this.registers[BitHelper.Get_x(this.encodedInstruction)] &= Vy;
		}

		/// <summary>
		/// 8xy3 - XOR Vx, Vy
		/// Set Vx = Vx XOR Vy.
		///
		///	Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx.
		/// An exclusive OR compares the corresponding bits from two values, and if the bits are not both the same,
		/// then the corresponding bit in the result is set to 1. Otherwise, it is 0.
		/// </summary>
		private void XOR_Vx_Vy()
		{
			var Vy = this.registers[BitHelper.Get_y(this.encodedInstruction)];
			this.registers[BitHelper.Get_x(this.encodedInstruction)] ^= Vy;
		}

		/// <summary>
		/// 8xy4 - ADD Vx, Vy
		/// Set Vx = Vx + Vy, set VF = carry.
		///
		///	The values of Vx and Vy are added together.
		/// If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, otherwise 0.
		/// Only the lowest 8 bits of the result are kept, and stored in Vx.
		/// </summary>
		private void ADD_Vx_Vy()
		{
			var Vx = this.registers[BitHelper.Get_x(this.encodedInstruction)];
			var Vy = this.registers[BitHelper.Get_y(this.encodedInstruction)];

			var sum = Vx + Vy;
			this.registers[0xF] = (byte)(sum > 255 ? 1 : 0);

			this.registers[BitHelper.Get_x(this.encodedInstruction)] = (byte)(sum & 0xFF);
		}

		/// <summary>
		/// 8xy5 - SUB Vx, Vy
		/// Set Vx = Vx - Vy, set VF = NOT borrow.
		///
		///	If Vx > Vy, then VF is set to 1, otherwise 0.
		/// Then Vy is subtracted from Vx, and the results stored in Vx.
		/// </summary>
		private void SUB_Vx_Vy()
		{
			var Vx = this.registers[BitHelper.Get_x(this.encodedInstruction)];
			var Vy = this.registers[BitHelper.Get_y(this.encodedInstruction)];

			this.registers[0xF] = (byte)(Vx > Vy ? 1 : 0);
			this.registers[BitHelper.Get_x(this.encodedInstruction)] -= Vy;
		}

		/// <summary>
		/// 8xy6 - SHR Vx {, Vy}
		/// Set Vx = Vx SHR 1.
		///
		/// If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0.
		/// Then Vx is divided by 2.
		/// </summary>
		private void SHR_Vx_Vy()
		{
			var Vx = this.registers[BitHelper.Get_x(this.encodedInstruction)];

			this.registers[0xF] = (byte)((Vx & 0x1) == 1 ? 1 : 0);
			this.registers[BitHelper.Get_x(this.encodedInstruction)] /= 2;
		}

		/// <summary>
		/// 8xy7 - SUBN Vx, Vy
		/// Set Vx = Vy - Vx, set VF = NOT borrow.
		///
		///	If Vy > Vx, then VF is set to 1, otherwise 0.
		/// Then Vx is subtracted from Vy, and the results stored in Vx.
		/// </summary>
		private void SUBN_Vx_Vy()
		{
			var Vx = this.registers[BitHelper.Get_x(this.encodedInstruction)];
			var Vy = this.registers[BitHelper.Get_y(this.encodedInstruction)];

			this.registers[0xF] = (byte)(Vy > Vx ? 1 : 0);
			this.registers[BitHelper.Get_x(this.encodedInstruction)] = (byte)(Vy - Vx);
		}

		/// <summary>
		/// 8xyE - SHL Vx {, Vy}
		/// Set Vx = Vx SHL 1.
		///
		/// If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0.
		/// Then Vx is multiplied by 2.
		/// </summary>
		private void SHL_Vx_Vy()
		{
			var Vx = this.registers[BitHelper.Get_x(this.encodedInstruction)];

			this.registers[0xF] = (byte)(Vx >> 7 & 0x1);
			this.registers[BitHelper.Get_x(this.encodedInstruction)] *= 2;
		}

		/// <summary>
		/// 7xkk - ADD Vx, byte
		/// Set Vx = Vx + kk.
		///
		///	Adds the value kk to the value of register Vx, then stores the result in Vx.
		/// </summary>
		private void ADD_Vx_byte()
		{
			var Vx = BitHelper.Get_x(this.encodedInstruction);
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
			var x = BitHelper.Get_x(this.encodedInstruction);
			var y = BitHelper.Get_y(this.encodedInstruction);
			var n = BitHelper.Get_n(this.encodedInstruction);

			// Coordinates
			var X = this.registers[x] % ScreenWidth;
			var Y = this.registers[y] % ScreenHeight;

			// Collision state to 0
			this.registers[0xF] = 0;

			// Get n sprite rows
			for (var i = 0; i < n; i++)
			{
				// Reset X back to start coordinate of sprite row
				X = this.registers[x] % ScreenWidth;

				var spriteRow = this.memory[this.indexRegister + i];
				for (var bit = 7; bit >= 0; bit--)
				{
					// Get current pixel from display and sprite bit
					var currentPixel = this.displayBytes[X, Y];
					var spriteBit = spriteRow >> bit & 0b1;

					// If current pixel and sprite bit is on,
					// turn of the pixel and set collision state to 1
					if (currentPixel == 1 && spriteBit == 1)
					{
						this.displayBytes[X, Y] = 0;
						this.registers[0xF] = 1;
					}
					else if (currentPixel == 0 && spriteBit == 1)
					{
						this.displayBytes[X, Y] = 1;
					}

					X++;

					// Stop drawing the sprite row if we're at the end of the display
					if (X == ScreenWidth)
					{
						break;
					}
				}

				Y++;

				// Stop drawing the sprite height if we're at the end of the display.
				if (Y == ScreenHeight)
				{
					break;
				}
			}
		}

		public void PrintMemory(byte start = 0, byte end = byte.MaxValue)
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
