using Chip_8.Utilities;

namespace Chip_8.Emulator
{
	public class Instruction
	{
		/// <summary>
		///  0nnn - SYS addr
		/// Jump to a machine code routine at nnn.
		///
		///	This instruction is only used on the old computers on which Chip-8 was originally implemented.
		/// It is ignored by modern interpreters.
		/// </summary>
		public static bool SYS_addr(EmulatorState emulator)
		{
			return false;
		}

		/// <summary>
		/// 00E0 - CLS
		/// Clear the display.
		/// </summary>
		public static bool CLS(EmulatorState emulator)
		{
			for (var y = 0; y < Chip8Emulator.ScreenHeight; y++)
			{
				for (var x = 0; x < Chip8Emulator.ScreenWidth; x++)
				{
					emulator.DisplayBytes[x, y] = 0;
				}
			}

			return true;
		}

		/// <summary>
		/// 00EE - RET
		/// Return from a subroutine.
		///
		///	The interpreter sets the program counter to the address at the top of the stack,
		/// then subtracts 1 from the stack pointer.
		/// </summary>
		public static bool RET(EmulatorState emulator)
		{
			emulator.ProgramCounter = emulator.Stack.Pop();
			return false;
		}

		/// <summary>
		/// 1nnn - JP addr
		/// Jump to location nnn.
		///
		/// The interpreter sets the program counter to nnn.
		/// </summary>
		public static bool JP_addr(EmulatorState emulator)
		{
			emulator.ProgramCounter = BitHelper.Get_nnn(emulator.EncodedInstruction);
			return false;
		}

		/// <summary>
		/// 2nnn - CALL addr
		/// Call subroutine at nnn.
		///
		///	The interpreter increments the stack pointer,
		/// then puts the current PC on the top of the stack.
		/// The PC is then set to nnn.
		/// </summary>
		public static bool CALL_addr(EmulatorState emulator)
		{
			var nnn = BitHelper.Get_nnn(emulator.EncodedInstruction);
			emulator.Stack.Push(emulator.ProgramCounter);
			emulator.ProgramCounter = nnn;
			return false;
		}

		/// <summary>
		/// 3xkk - SE Vx, byte
		/// Skip next instruction if Vx = kk.
		///
		/// The interpreter compares register Vx to kk, and if they are equal,
		/// increments the program counter by 2.
		/// </summary>
		public static bool SE_Vx_byte(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var kk = BitHelper.Get_kk(emulator.EncodedInstruction);
			if (Vx == kk)
			{
				emulator.ProgramCounter += 2;
			}

			return false;
		}

		/// <summary>
		/// 4xkk - SNE Vx, byte
		/// Skip next instruction if Vx != kk.
		///
		///	The interpreter compares register Vx to kk, and if they are not equal,
		/// increments the program counter by 2.
		/// </summary>
		public static bool SNE_Vx_byte(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var kk = BitHelper.Get_kk(emulator.EncodedInstruction);
			if (Vx != kk)
			{
				emulator.ProgramCounter += 2;
			}

			return false;
		}

		/// <summary>
		/// 5xy0 - SE Vx, Vy
		/// Skip next instruction if Vx = Vy.
		///
		///	The interpreter compares register Vx to register Vy, and if they are equal,
		/// increments the program counter by 2.
		/// </summary>
		public static bool SE_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			if (Vx == Vy)
			{
				emulator.ProgramCounter += 2;
			}

			return false;
		}

		/// <summary>
		/// 6xkk - LD Vx, byte
		/// Set Vx = kk.
		///
		/// The interpreter puts the value kk into register Vx.
		/// </summary>
		public static bool LD_Vx_byte(EmulatorState emulator)
		{
			var x = BitHelper.Get_x(emulator.EncodedInstruction);
			var kk = BitHelper.Get_kk(emulator.EncodedInstruction);
			emulator.Registers[x] = kk;
			return false;
		}

		/// <summary>
		/// 7xkk - ADD Vx, byte
		/// Set Vx = Vx + kk.
		///
		///	Adds the value kk to the value of register Vx, then stores the result in Vx.
		/// </summary>
		public static bool ADD_Vx_byte(EmulatorState emulator)
		{
			var x = BitHelper.Get_x(emulator.EncodedInstruction);
			var kk = BitHelper.Get_kk(emulator.EncodedInstruction);
			emulator.Registers[x] = (byte)(emulator.Registers[x] + kk);
			return false;
		}

		/// <summary>
		/// 8xy0 - LD Vx, Vy
		/// Set Vx = Vy.
		///
		/// Stores the value of register Vy in register Vx.
		/// </summary>
		public static bool LD_Vx_Vy(EmulatorState emulator)
		{
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] = Vy;
			return false;
		}

		/// <summary>
		/// 8xy1 - OR Vx, Vy
		/// Set Vx = Vx OR Vy.
		///
		/// Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx.
		/// A bitwise OR compares the corresponding bits from two values,
		/// and if either bit is 1, then the same bit in the result is also 1. Otherwise, it is 0.
		/// </summary>
		public static bool OR_Vx_Vy(EmulatorState emulator)
		{
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] |= Vy;
			return false;
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
		public static bool AND_Vx_Vy(EmulatorState emulator)
		{
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] &= Vy;
			return false;
		}

		/// <summary>
		/// 8xy3 - XOR Vx, Vy
		/// Set Vx = Vx XOR Vy.
		///
		///	Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx.
		/// An exclusive OR compares the corresponding bits from two values, and if the bits are not both the same,
		/// then the corresponding bit in the result is set to 1. Otherwise, it is 0.
		/// </summary>
		public static bool XOR_Vx_Vy(EmulatorState emulator)
		{
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] ^= Vy;
			return false;
		}

		/// <summary>
		/// 8xy4 - ADD Vx, Vy
		/// Set Vx = Vx + Vy, set VF = carry.
		///
		///	The values of Vx and Vy are added together.
		/// If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, otherwise 0.
		/// Only the lowest 8 bits of the result are kept, and stored in Vx.
		/// </summary>
		public static bool ADD_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];

			var sum = Vx + Vy;
			emulator.Registers[0xF] = (byte)(sum > 255 ? 1 : 0);

			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] = (byte)(sum & 0xFF);
			return false;
		}

		/// <summary>
		/// 8xy5 - SUB Vx, Vy
		/// Set Vx = Vx - Vy, set VF = NOT borrow.
		///
		///	If Vx > Vy, then VF is set to 1, otherwise 0.
		/// Then Vy is subtracted from Vx, and the results stored in Vx.
		/// </summary>
		public static bool SUB_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];

			emulator.Registers[0xF] = (byte)(Vx > Vy ? 1 : 0);
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] -= Vy;
			return false;
		}

		/// <summary>
		/// 8xy6 - SHR Vx {, Vy}
		/// Set Vx = Vx SHR 1.
		///
		/// If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0.
		/// Then Vx is divided by 2.
		/// </summary>
		public static bool SHR_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];

			emulator.Registers[0xF] = (byte)((Vx & 0x1) == 1 ? 1 : 0);
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] /= 2;
			return false;
		}

		/// <summary>
		/// 8xy7 - SUBN Vx, Vy
		/// Set Vx = Vy - Vx, set VF = NOT borrow.
		///
		///	If Vy > Vx, then VF is set to 1, otherwise 0.
		/// Then Vx is subtracted from Vy, and the results stored in Vx.
		/// </summary>
		public static bool SUBN_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];

			emulator.Registers[0xF] = (byte)(Vy > Vx ? 1 : 0);
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] = (byte)(Vy - Vx);
			return false;
		}

		/// <summary>
		/// 8xyE - SHL Vx {, Vy}
		/// Set Vx = Vx SHL 1.
		///
		/// If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0.
		/// Then Vx is multiplied by 2.
		/// </summary>
		public static bool SHL_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];

			emulator.Registers[0xF] = (byte)(Vx >> 7 & 0x1);
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] *= 2;
			return false;
		}

		public static bool SNE_Vx_Vy(EmulatorState emulator)
		{
			return false;
		}

		/// <summary>
		/// Annn - LD I, addr
		/// Set I = nnn.
		///
		///	The value of register I is set to nnn.
		/// </summary>
		public static bool LD_I_addr(EmulatorState emulator)
		{
			emulator.IndexRegister = BitHelper.Get_nnn(emulator.EncodedInstruction);
			return false;
		}

		public static bool JP_V0_addr(EmulatorState emulator)
		{
			return false;
		}

		public static bool RND_Vx_byte(EmulatorState emulator)
		{
			return false;
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
		public static bool DRW_Vx_Vy_nibble(EmulatorState emulator)
		{
			var x = BitHelper.Get_x(emulator.EncodedInstruction);
			var y = BitHelper.Get_y(emulator.EncodedInstruction);
			var n = BitHelper.Get_n(emulator.EncodedInstruction);

			// Coordinates
			var X = emulator.Registers[x] % Chip8Emulator.ScreenWidth;
			var Y = emulator.Registers[y] % Chip8Emulator.ScreenHeight;

			// Collision state to 0
			emulator.Registers[0xF] = 0;

			// Get n sprite rows
			for (var i = 0; i < n; i++)
			{
				// Reset X back to start coordinate of sprite row
				X = emulator.Registers[x] % Chip8Emulator.ScreenWidth;

				var spriteRow = emulator.Memory[emulator.IndexRegister + i];
				for (var bit = 7; bit >= 0; bit--)
				{
					// Get current pixel from display and sprite bit
					var currentPixel = emulator.DisplayBytes[X, Y];
					var spriteBit = spriteRow >> bit & 0b1;

					// If current pixel and sprite bit is on,
					// turn of the pixel and set collision state to 1
					if (currentPixel == 1 && spriteBit == 1)
					{
						emulator.DisplayBytes[X, Y] = 0;
						emulator.Registers[0xF] = 1;
					}
					else if (currentPixel == 0 && spriteBit == 1)
					{
						emulator.DisplayBytes[X, Y] = 1;
					}

					X++;

					// Stop drawing the sprite row if we're at the end of the display
					if (X == Chip8Emulator.ScreenWidth)
					{
						break;
					}
				}

				Y++;

				// Stop drawing the sprite height if we're at the end of the display.
				if (Y == Chip8Emulator.ScreenHeight)
				{
					break;
				}
			}

			return true;
		}

		public static bool SKP_Vx(EmulatorState emulator)
		{
			return false;
		}

		public static bool SKNP_Vx(EmulatorState emulator)
		{
			return false;
		}

		public static bool LD_Vx_DT(EmulatorState emulator)
		{
			return false;
		}

		public static bool LD_Vx_K(EmulatorState emulator)
		{
			return false;
		}

		public static bool LD_DT_Vx(EmulatorState emulator)
		{
			return false;
		}

		public static bool LD_ST_Vx(EmulatorState emulator)
		{
			return false;
		}

		public static bool ADD_I_Vx(EmulatorState emulator)
		{
			return false;
		}

		public static bool LD_F_Vx(EmulatorState emulator)
		{
			return false;
		}

		public static bool LD_B_Vx(EmulatorState emulator)
		{
			return false;
		}

		public static bool LD_I_Vx(EmulatorState emulator)
		{
			return false;
		}

		public static bool LD_Vx_I(EmulatorState emulator)
		{
			return false;
		}
	}
}
