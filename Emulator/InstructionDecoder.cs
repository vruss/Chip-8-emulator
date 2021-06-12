namespace Chip_8.Emulator
{
	public static class InstructionDecoder
	{
		public static Instruction DecodeInstruction(ushort encodedInstruction)
		{
			return (encodedInstruction & 0xF000) switch
			{
				0x0000 => (encodedInstruction & 0x00FF) switch
				{
					0x00E0 => Instruction.CLS,
					0x00EE => Instruction.RET,
					_ => Instruction.SYS_addr
				},
				0x1000 => Instruction.JP_addr,
				0x2000 => Instruction.CALL_addr,
				0x3000 => Instruction.SE_Vx_byte,
				0x4000 => Instruction.SNE_Vx_byte,
				0x5000 => Instruction.SE_Vx_Vy,
				0x6000 => Instruction.LD_Vx_byte,
				0x7000 => Instruction.ADD_Vx_byte,
				0x8000 => (encodedInstruction & 0x000F) switch
				{
					0x0000 => Instruction.LD_Vx_Vy,
					0x0001 => Instruction.OR_Vx_Vy,
					0x0002 => Instruction.AND_Vx_Vy,
					0x0003 => Instruction.XOR_Vx_Vy,
					0x0004 => Instruction.ADD_Vx_Vy,
					0x0005 => Instruction.SUB_Vx_Vy,
					0x0006 => Instruction.SHR_Vx_Vy,
					0x0007 => Instruction.SUBN_Vx_Vy,
					0x000E => Instruction.SHL_Vx_Vy,
					_ => Instruction.SYS_addr
				},
				0x9000 => Instruction.SNE_Vx_Vy,
				0xA000 => Instruction.LD_I_addr,
				0xB000 => Instruction.JP_V0_addr,
				0xC000 => Instruction.RND_Vx_byte,
				0xD000 => Instruction.DRW_Vx_Vy_nibble,
				0xE000 => (encodedInstruction & 0x00FF) switch
				{
					0x009E => Instruction.SKP_Vx,
					0x00A1 => Instruction.SKNP_Vx,
					_ => Instruction.SYS_addr
				},
				0xF000 => (encodedInstruction & 0x00FF) switch
				{
					0x0007 => Instruction.LD_Vx_DT,
					0x000A => Instruction.LD_Vx_K,
					0x0015 => Instruction.LD_DT_Vx,
					0x0018 => Instruction.LD_ST_Vx,
					0x001E => Instruction.ADD_I_Vx,
					0x0029 => Instruction.LD_F_Vx,
					0x0033 => Instruction.LD_B_Vx,
					0x0055 => Instruction.LD_I_Vx,
					0x0065 => Instruction.LD_Vx_I,
					_ => Instruction.SYS_addr
				},
				_ => Instruction.SYS_addr
			};
		}
	}
}
