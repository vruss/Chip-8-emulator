using System;
using System.Collections.Generic;
using System.Threading;
using Chip_8.Emulator;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Chip_8
{
	class Program
	{
		private static Chip8Emulator s_emulator;
		
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
			window.KeyReleased += OnKeyReleased;

			s_emulator = new Chip8Emulator();
			s_emulator.LoadRom("TestFiles/test_opcode.ch8");

			while (window.IsOpen)
			{
				// Process events
				window.DispatchEvents();
				window.SetActive();

				// Clear the previous frame
				window.Clear();

				var frameData = s_emulator.GetNextFrame();

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
			s_emulator.SendKey(GetEmulatorKey(e.Code));
		}
		
		private static void OnKeyReleased(object sender, KeyEventArgs e)
		{
			s_emulator.SendKey(Key.None);
		}

		/// <summary>
		/// Returns a mapping of QWERTY into CHIP-8 ->
		/// 1 	2 	3 	4
		/// Q 	W 	E 	R
		/// A 	S 	D 	F
		/// Z 	X 	C 	V
		/// </summary>
		private static Key GetEmulatorKey(Keyboard.Key key)
		{
			return key switch
			{
				Keyboard.Key.Num1 => Key.Num1,
				Keyboard.Key.Num2 => Key.Num2,
				Keyboard.Key.Num3 => Key.Num3,
				Keyboard.Key.Num4 => Key.C,
				Keyboard.Key.Q => Key.Num4,
				Keyboard.Key.W => Key.Num5,
				Keyboard.Key.E => Key.Num6,
				Keyboard.Key.R => Key.D,
				Keyboard.Key.A => Key.Num7,
				Keyboard.Key.S => Key.Num8,
				Keyboard.Key.D => Key.Num9,
				Keyboard.Key.F => Key.E,
				Keyboard.Key.Z => Key.A,
				Keyboard.Key.X => Key.Num0,
				Keyboard.Key.C => Key.B,
				Keyboard.Key.V => Key.F,
				_ => Key.None,
			};
		}
	}
}