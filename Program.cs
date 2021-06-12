using Chip_8.Emulator;

namespace Chip_8
{
	class Program
	{
		static void Main(string[] args)
		{
			var emulator = new Emulator.Emulator();

			emulator.LoadRom("TestFiles/IBM_Logo.ch8");
			emulator.LoadFont(Font.StandardFont);

			emulator.PrintMemory();

			emulator.Start();
		}
	}
}
