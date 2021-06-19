using System;
using Chip_8.Emulator;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Font = Chip_8.Emulator.Font;

namespace Chip_8
{
	class Program
	{
		static void Main(string[] args)
		{
			var window = new RenderWindow(new VideoMode(64, 32, 1), "Chip-8 Emulator");
			window.SetFramerateLimit(60);

			var emulator = new Chip8Emulator();
			emulator.LoadRom("TestFiles/IBM_Logo.ch8");
			emulator.LoadFont(Font.StandardFont);

			while (window.IsOpen)
			{
				window.Clear();

				emulator.Cycle();

				var displayData = emulator.GetDisplayData();
				var vertices = new VertexArray();

				for (int y = 0; y < 32; y++)
				{
					for (int x = 0; x < 64; x++)
					{
						vertices.Append(new Vertex(new Vector2f(x, y), displayData[x, y] == 1
							? Color.White
							: Color.Black
							)
						);
					}
				}

				window.Draw(vertices);

				window.Display();
			}
		}
	}
}
