using System;

namespace Chip_8.Emulator
{
	public static class InstructionDecoder
	{
		public static Action<EmulatorState> DecodeInstruction(ushort encodedInstruction)
		{
			return (encodedInstruction & 0xF000) switch
			{
				0x0000 => (encodedInstruction & 0x00FF) switch
				{
					0x00E0 => state => Instruction.CLS(state),
					0x00EE => state => Instruction.RET(state),
					_ => state => Instruction.SYS_addr(state)
				},
				0x1000 => state => Instruction.JP_addr(state),
				0x2000 => state => Instruction.CALL_addr(state),
				0x3000 => state => Instruction.SE_Vx_byte(state),
				0x4000 => state => Instruction.SNE_Vx_byte(state),
				0x5000 => state => Instruction.SE_Vx_Vy(state),
				0x6000 => state => Instruction.LD_Vx_byte(state),
				0x7000 => state => Instruction.ADD_Vx_byte(state),
				0x8000 => (encodedInstruction & 0x000F) switch
				{
					0x0000 => state => Instruction.LD_Vx_Vy(state),
					0x0001 => state => Instruction.OR_Vx_Vy(state),
					0x0002 => state => Instruction.AND_Vx_Vy(state),
					0x0003 => state => Instruction.XOR_Vx_Vy(state),
					0x0004 => state => Instruction.ADD_Vx_Vy(state),
					0x0005 => state => Instruction.SUB_Vx_Vy(state),
					0x0006 => state => Instruction.SHR_Vx_Vy(state),
					0x0007 => state => Instruction.SUBN_Vx_Vy(state),
					0x000E => state => Instruction.SHL_Vx_Vy(state),
					_ => state => Instruction.SYS_addr(state)
				},
				0x9000 => state => Instruction.SNE_Vx_Vy(state),
				0xA000 => state => Instruction.LD_I_addr(state),
				0xB000 => state => Instruction.JP_V0_addr(state),
				0xC000 => state => Instruction.RND_Vx_byte(state),
				0xD000 => state => Instruction.DRW_Vx_Vy_nibble(state),
				0xE000 => (encodedInstruction & 0x00FF) switch
				{
					0x009E => state => Instruction.SKP_Vx(state),
					0x00A1 => state => Instruction.SKNP_Vx(state),
					_ => state => Instruction.SYS_addr(state)
				},
				0xF000 => (encodedInstruction & 0x00FF) switch
				{
					0x0007 => state => Instruction.LD_Vx_DT(state),
					0x000A => state => Instruction.LD_Vx_K(state),
					0x0015 => state => Instruction.LD_DT_Vx(state),
					0x0018 => state => Instruction.LD_ST_Vx(state),
					0x001E => state => Instruction.ADD_I_Vx(state),
					0x0029 => state => Instruction.LD_F_Vx(state),
					0x0033 => state => Instruction.LD_B_Vx(state),
					0x0055 => state => Instruction.LD_I_Vx(state),
					0x0065 => state => Instruction.LD_Vx_I(state),
					_ => state => Instruction.SYS_addr(state)
				},
				_ => state => Instruction.SYS_addr(state)
			};
		}
	}
}
