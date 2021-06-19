using System.Collections.Generic;
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
			const int scaleFactor = 30;

			var window = new RenderWindow(new VideoMode(Chip8Emulator.ScreenWidth * scaleFactor
					, Chip8Emulator.ScreenHeight * scaleFactor, 1
				)
				, "Chip-8 Emulator"
			);
			window.SetFramerateLimit(60);

			var emulator = new Chip8Emulator();
			emulator.LoadRom("TestFiles/IBM_Logo.ch8");
			emulator.LoadFont(Font.StandardFont);

			while (window.IsOpen)
			{
				window.Clear();

				emulator.Cycle();

				var displayData = emulator.GetDisplayData();
				var vertices = CreateVertices(displayData);

				foreach (var vertex in vertices)
				{
					window.Draw(vertex);
				}

				window.Display();
			}

			IEnumerable<RectangleShape> CreateVertices(byte[,] displayData)
			{
				var vertices = new List<RectangleShape>();

				for (var y = 0; y < Chip8Emulator.ScreenHeight; y++)
				{
					for (var x = 0; x < Chip8Emulator.ScreenWidth; x++)
					{
						var vertex = new RectangleShape(new Vector2f(scaleFactor, scaleFactor))
						{
							Position = new Vector2f(x * scaleFactor, y * scaleFactor),
							FillColor = displayData[x, y] == 1
								? Color.White
								: Color.Black,
						};
						vertices.Add(vertex);
					}
				}

				return vertices;
			}
		}
	}
}
