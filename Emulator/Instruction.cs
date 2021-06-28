using System;
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
		public static void SYS_addr(EmulatorState emulator)
		{
		}

		/// <summary>
		/// 00E0 - CLS
		/// Clear the display.
		/// </summary>
		public static void CLS(EmulatorState emulator)
		{
			for (var y = 0; y < Chip8Emulator.ScreenHeight; y++)
			{
				for (var x = 0; x < Chip8Emulator.ScreenWidth; x++)
				{
					emulator.DisplayBytes[x, y] = 0;
				}
			}

			emulator.HasNewFrame = true;
		}

		/// <summary>
		/// 00EE - RET
		/// Return from a subroutine.
		///
		///	The interpreter sets the program counter to the address at the top of the stack,
		/// then subtracts 1 from the stack pointer.
		/// </summary>
		public static void RET(EmulatorState emulator)
		{
			emulator.ProgramCounter = emulator.Stack.Pop();
		}

		/// <summary>
		/// 1nnn - JP addr
		/// Jump to location nnn.
		///
		/// The interpreter sets the program counter to nnn.
		/// </summary>
		public static void JP_addr(EmulatorState emulator)
		{
			emulator.ProgramCounter = BitHelper.Get_nnn(emulator.EncodedInstruction);
		}

		/// <summary>
		/// 2nnn - CALL addr
		/// Call subroutine at nnn.
		///
		///	The interpreter increments the stack pointer,
		/// then puts the current PC on the top of the stack.
		/// The PC is then set to nnn.
		/// </summary>
		public static void CALL_addr(EmulatorState emulator)
		{
			var nnn = BitHelper.Get_nnn(emulator.EncodedInstruction);
			emulator.Stack.Push(emulator.ProgramCounter);
			emulator.ProgramCounter = nnn;
		}

		/// <summary>
		/// 3xkk - SE Vx, byte
		/// Skip next instruction if Vx = kk.
		///
		/// The interpreter compares register Vx to kk, and if they are equal,
		/// increments the program counter by 2.
		/// </summary>
		public static void SE_Vx_byte(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var kk = BitHelper.Get_kk(emulator.EncodedInstruction);
			if (Vx == kk)
			{
				emulator.ProgramCounter += 2;
			}
		}

		/// <summary>
		/// 4xkk - SNE Vx, byte
		/// Skip next instruction if Vx != kk.
		///
		///	The interpreter compares register Vx to kk, and if they are not equal,
		/// increments the program counter by 2.
		/// </summary>
		public static void SNE_Vx_byte(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var kk = BitHelper.Get_kk(emulator.EncodedInstruction);
			if (Vx != kk)
			{
				emulator.ProgramCounter += 2;
			}
		}

		/// <summary>
		/// 5xy0 - SE Vx, Vy
		/// Skip next instruction if Vx = Vy.
		///
		///	The interpreter compares register Vx to register Vy, and if they are equal,
		/// increments the program counter by 2.
		/// </summary>
		public static void SE_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			if (Vx == Vy)
			{
				emulator.ProgramCounter += 2;
			}

		}

		/// <summary>
		/// 6xkk - LD Vx, byte
		/// Set Vx = kk.
		///
		/// The interpreter puts the value kk into register Vx.
		/// </summary>
		public static void LD_Vx_byte(EmulatorState emulator)
		{
			var x = BitHelper.Get_x(emulator.EncodedInstruction);
			var kk = BitHelper.Get_kk(emulator.EncodedInstruction);
			emulator.Registers[x] = kk;
		}

		/// <summary>
		/// 7xkk - ADD Vx, byte
		/// Set Vx = Vx + kk.
		///
		///	Adds the value kk to the value of register Vx, then stores the result in Vx.
		/// </summary>
		public static void ADD_Vx_byte(EmulatorState emulator)
		{
			var x = BitHelper.Get_x(emulator.EncodedInstruction);
			var kk = BitHelper.Get_kk(emulator.EncodedInstruction);
			emulator.Registers[x] = (byte)(emulator.Registers[x] + kk);
		}

		/// <summary>
		/// 8xy0 - LD Vx, Vy
		/// Set Vx = Vy.
		///
		/// Stores the value of register Vy in register Vx.
		/// </summary>
		public static void LD_Vx_Vy(EmulatorState emulator)
		{
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] = Vy;
		}

		/// <summary>
		/// 8xy1 - OR Vx, Vy
		/// Set Vx = Vx OR Vy.
		///
		/// Performs a bitwise OR on the values of Vx and Vy, then stores the result in Vx.
		/// A bitwise OR compares the corresponding bits from two values,
		/// and if either bit is 1, then the same bit in the result is also 1. Otherwise, it is 0.
		/// </summary>
		public static void OR_Vx_Vy(EmulatorState emulator)
		{
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] |= Vy;
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
		public static void AND_Vx_Vy(EmulatorState emulator)
		{
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] &= Vy;
		}

		/// <summary>
		/// 8xy3 - XOR Vx, Vy
		/// Set Vx = Vx XOR Vy.
		///
		///	Performs a bitwise exclusive OR on the values of Vx and Vy, then stores the result in Vx.
		/// An exclusive OR compares the corresponding bits from two values, and if the bits are not both the same,
		/// then the corresponding bit in the result is set to 1. Otherwise, it is 0.
		/// </summary>
		public static void XOR_Vx_Vy(EmulatorState emulator)
		{
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] ^= Vy;
		}

		/// <summary>
		/// 8xy4 - ADD Vx, Vy
		/// Set Vx = Vx + Vy, set VF = carry.
		///
		///	The values of Vx and Vy are added together.
		/// If the result is greater than 8 bits (i.e., > 255,) VF is set to 1, otherwise 0.
		/// Only the lowest 8 bits of the result are kept, and stored in Vx.
		/// </summary>
		public static void ADD_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];

			var sum = Vx + Vy;
			emulator.Registers[0xF] = (byte)(sum > 255 ? 1 : 0);

			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] = (byte)(sum & 0xFF);
		}

		/// <summary>
		/// 8xy5 - SUB Vx, Vy
		/// Set Vx = Vx - Vy, set VF = NOT borrow.
		///
		///	If Vx > Vy, then VF is set to 1, otherwise 0.
		/// Then Vy is subtracted from Vx, and the results stored in Vx.
		/// </summary>
		public static void SUB_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];

			emulator.Registers[0xF] = (byte)(Vx > Vy ? 1 : 0);
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] -= Vy;
		}

		/// <summary>
		/// 8xy6 - SHR Vx {, Vy}
		/// Set Vx = Vx SHR 1.
		///
		/// If the least-significant bit of Vx is 1, then VF is set to 1, otherwise 0.
		/// Then Vx is divided by 2.
		/// </summary>
		public static void SHR_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];

			emulator.Registers[0xF] = (byte)((Vx & 0x1) == 1 ? 1 : 0);
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] /= 2;
		}

		/// <summary>
		/// 8xy7 - SUBN Vx, Vy
		/// Set Vx = Vy - Vx, set VF = NOT borrow.
		///
		///	If Vy > Vx, then VF is set to 1, otherwise 0.
		/// Then Vx is subtracted from Vy, and the results stored in Vx.
		/// </summary>
		public static void SUBN_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];

			emulator.Registers[0xF] = (byte)(Vy > Vx ? 1 : 0);
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] = (byte)(Vy - Vx);
		}

		/// <summary>
		/// 8xyE - SHL Vx {, Vy}
		/// Set Vx = Vx SHL 1.
		///
		/// If the most-significant bit of Vx is 1, then VF is set to 1, otherwise to 0.
		/// Then Vx is multiplied by 2.
		/// </summary>
		public static void SHL_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];

			emulator.Registers[0xF] = (byte)(Vx >> 7 & 0x1);
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] *= 2;
		}

		/// <summary>
		/// 9xy0 - SNE Vx, Vy
		/// Skip next instruction if Vx != Vy.
		///
		///	The values of Vx and Vy are compared, and if they are not equal, the program counter is increased by 2.
		/// </summary>
		public static void SNE_Vx_Vy(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var Vy = emulator.Registers[BitHelper.Get_y(emulator.EncodedInstruction)];

			if (Vx != Vy)
			{
				emulator.ProgramCounter += 2;
			}

		}

		/// <summary>
		/// Annn - LD I, addr
		/// Set I = nnn.
		///
		///	The value of register I is set to nnn.
		/// </summary>
		public static void LD_I_addr(EmulatorState emulator)
		{
			emulator.IndexRegister = BitHelper.Get_nnn(emulator.EncodedInstruction);
		}

		/// <summary>
		/// Bnnn - JP V0, addr
		/// Jump to location nnn + V0.
		///
		///The program counter is set to nnn plus the value of V0.
		/// </summary>
		public static void JP_V0_addr(EmulatorState emulator)
		{
			var nnn = BitHelper.Get_nnn(emulator.EncodedInstruction);
			emulator.ProgramCounter = (ushort)(nnn + emulator.Registers[0x0]);
		}

		/// <summary>
		/// Cxkk - RND Vx, byte
		/// Set Vx = random byte AND kk.
		///
		/// The interpreter generates a random number from 0 to 255, which is then ANDed with the value kk.
		/// The results are stored in Vx.
		/// </summary>
		public static void RND_Vx_byte(EmulatorState emulator)
		{
			var r = new Random();
			var randomNumber = (byte)r.Next(0, 255);

			var x = BitHelper.Get_x(emulator.EncodedInstruction);
			var kk = BitHelper.Get_kk(emulator.EncodedInstruction);
			emulator.Registers[x] = (byte)(randomNumber & kk);
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
		public static void DRW_Vx_Vy_nibble(EmulatorState emulator)
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

			emulator.HasNewFrame = true;
		}

		/// <summary>
		/// Ex9E - SKP Vx
		/// Skip next instruction if key with the value of Vx is pressed.
		///
		///	Checks the keyboard, and if the key corresponding to the value of Vx is currently in the down position,
		/// PC is increased by 2.
		/// </summary>
		public static void SKP_Vx(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			if (Vx == (int)emulator.PressedKey)
			{
				emulator.ProgramCounter += 2;
			}
		}

		/// <summary>
		/// ExA1 - SKNP Vx
		/// Skip next instruction if key with the value of Vx is not pressed.
		///
		///	Checks the keyboard, and if the key corresponding to the value of Vx is currently in the up position,
		/// PC is increased by 2.
		/// </summary>
		public static void SKNP_Vx(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			if (Vx != (int)emulator.PressedKey)
			{
				emulator.ProgramCounter += 2;
			}
		}

		/// <summary>
		/// Fx07 - LD Vx, DT
		/// Set Vx = delay timer value.
		///	
		///	The value of DT is placed into Vx.
		/// </summary>
		/// <param name="emulator"></param>
		public static void LD_Vx_DT(EmulatorState emulator)
		{
			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] = emulator.DelayTimer;
		}

		/// <summary>
		/// Fx0A - LD Vx, K
		/// Wait for a key press, store the value of the key in Vx.
		///
		///	All execution stops until a key is pressed, then the value of that key is stored in Vx.
		/// </summary>
		/// <param name="emulator"></param>
		public static void LD_Vx_K(EmulatorState emulator)
		{
			if (emulator.PressedKey == Key.None)
			{
				emulator.ProgramCounter -= 2;
				return;
			}

			emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)] = (byte)emulator.PressedKey;
		}

		/// <summary>
		/// Fx15 - LD DT, Vx
		/// Set delay timer = Vx.
		///
		///	DT is set equal to the value of Vx.
		/// </summary>
		/// <param name="emulator"></param>
		public static void LD_DT_Vx(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			emulator.DelayTimer = Vx;
		}

		/// <summary>
		/// Fx18 - LD ST, Vx
		/// Set sound timer = Vx.
		///
		///	ST is set equal to the value of Vx.
		/// </summary>
		/// <param name="emulator"></param>
		public static void LD_ST_Vx(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			emulator.SoundTimer = Vx;
		}

		/// <summary>
		/// Fx1E - ADD I, Vx
		///	Set I = I + Vx.
		///
		///	The values of I and Vx are added, and the results are stored in I.
		/// </summary>
		/// <param name="emulator"></param>
		public static void ADD_I_Vx(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			emulator.IndexRegister += Vx;
		}

		/// <summary>
		/// Fx29 - LD F, Vx
		/// Set I = location of sprite for digit Vx.
		///
		///	The value of I is set to the location for the hexadecimal sprite corresponding to the value of Vx.
		/// </summary>
		public static void LD_F_Vx(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];
			var spriteDigit = Chip8Emulator.FontOffset + Vx * Chip8Emulator.FontSize;
			
			emulator.IndexRegister = (ushort)spriteDigit;
		}

		/// <summary>
		/// Fx33 - LD B, Vx
		/// Store BCD representation of Vx in memory locations I, I+1, and I+2.
		///
		/// The interpreter takes the decimal value of Vx,
		/// and places the hundreds digit in memory at location in I,
		/// the tens digit at location I+1,
		/// and the ones digit at location I+2.
		/// </summary>
		public static void LD_B_Vx(EmulatorState emulator)
		{
			var Vx = emulator.Registers[BitHelper.Get_x(emulator.EncodedInstruction)];

			var hundredths = Vx % 1000 / 100;
			var tenths = Vx % 100 / 10;
			var ones = Vx % 10 / 1;

			emulator.Memory[emulator.IndexRegister + 0] = (byte)hundredths;
			emulator.Memory[emulator.IndexRegister + 1] = (byte)tenths;
			emulator.Memory[emulator.IndexRegister + 2] = (byte)ones;
		}

		/// <summary>
		/// Fx55 - LD [I], Vx
		/// Store registers V0 through Vx in memory starting at location I.
		///
		///	The interpreter copies the values of registers V0 through Vx into memory,
		/// starting at the address in I.
		/// </summary>
		public static void LD_I_Vx(EmulatorState emulator)
		{
			var x = BitHelper.Get_x(emulator.EncodedInstruction);

			for (var i = 0; i <= x; i++)
			{
				emulator.Memory[emulator.IndexRegister + i] = emulator.Registers[i];
			}
		}

		/// <summary>
		/// Fx65 - LD Vx, [I]
		/// Read registers V0 through Vx from memory starting at location I.
		///
		///	The interpreter reads values from memory starting at location I into registers V0 through Vx.
		/// </summary>
		public static void LD_Vx_I(EmulatorState emulator)
		{
			var x = BitHelper.Get_x(emulator.EncodedInstruction);

			for (var i = 0; i <= x; i++)
			{
				emulator.Registers[i] = emulator.Memory[emulator.IndexRegister + i];
			}
		}
	}
}
