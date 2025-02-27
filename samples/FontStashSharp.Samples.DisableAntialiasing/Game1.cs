﻿using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.Mime.MediaTypeNames;

namespace FontStashSharp
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		const float FontSize = 64.0f;
		private const string SampleText = "The quick brown fox jumps over the lazy dog.";
		private static readonly Color DefaultColor = Color.White;

		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;
		private FontSystem _fontSystem, _fontSystemOldRasterizer;
		private Texture2D _white;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1800,
				PreferredBackBufferHeight = 450
			};

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		private FontSystem LoadFont(bool disableAntialiasing)
		{
			var settings = new FontSystemSettings();

			if (disableAntialiasing)
			{
				settings.GlyphRenderer = (input, output, options) =>
				{
					var size = options.Size.X * options.Size.Y;

					for (var i = 0; i < size; i++)
					{
						var c = input[i];
						var ci = i * 4;

						if (c == 0)
						{
							output[ci] = output[ci + 1] = output[ci + 2] = output[ci + 3] = 0;
						} else
						{
							output[ci] = output[ci + 1] = output[ci + 2] = output[ci + 3] = 255;
						}
					}
				};
			}

			var result = new FontSystem(settings);
			result.AddFont(File.ReadAllBytes(@"Fonts/pixel_perfect.ttf"));

			return result;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_fontSystem = LoadFont(false);
			_fontSystemOldRasterizer = LoadFont(true);

			_white = new Texture2D(GraphicsDevice, 1, 1);
			_white.SetData(new[] { Color.White });

			GC.Collect();
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			
			_spriteBatch.Begin();

			var font = _fontSystem.GetFont(FontSize);
			_spriteBatch.DrawString(font, SampleText, new Vector2(0, 10), DefaultColor);

			font = _fontSystemOldRasterizer.GetFont(FontSize);
			_spriteBatch.DrawString(font, SampleText, new Vector2(0, 15 + (int)FontSize), DefaultColor);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}