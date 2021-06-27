using System;
using System.Collections.Generic;
using Chip_8.Emulator;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Chip_8
{
	class Program
	{
		static void Main(string[] args)
		{
			// Scale-factor applied on screen-size and vertices 
			const int scaleFactor = 30;

			const int screenWidth = Chip8Emulator.ScreenWidth * scaleFactor;
			const int screenHeight = Chip8Emulator.ScreenHeight * scaleFactor;

			// Create the main window
			var window = new RenderWindow(new VideoMode(screenWidth, screenHeight, 1)
				, "Chip-8 Emulator"
			);
			window.SetFramerateLimit(60);

			// Setup event handlers
			window.Closed += OnClosed;
			window.KeyPressed += OnKeyPressed;

			var emulator = new Chip8Emulator();
			emulator.LoadRom("TestFiles/test_opcode.ch8");

			while (window.IsOpen)
			{
				// Process events
				window.DispatchEvents();
				window.SetActive();

				// Clear the previous frame
				window.Clear();

				var frameData = emulator.GetNextFrame();

				foreach (var vertex in CreateVertices(frameData, scaleFactor))
				{
					window.Draw(vertex);
				}

				// Display the rendered frame on screen
				window.Display();
			}
		}

		private static IEnumerable<RectangleShape> CreateVertices(byte[,] displayData, float scaleFactor)
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

		private static void OnClosed(object sender, EventArgs e)
		{
			if (sender is RenderWindow window)
			{
				window.Close();
			}
		}

		private static void OnKeyPressed(object sender, KeyEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}