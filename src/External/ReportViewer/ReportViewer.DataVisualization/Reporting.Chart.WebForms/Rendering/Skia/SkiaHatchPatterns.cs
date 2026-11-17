using System;
using System.Drawing.Drawing2D;
using SkiaSharp;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>
	/// Builds a small repeating tile bitmap per <see cref="HatchStyle"/> for <see cref="SkiaHatchBrush"/>.
	/// GDI+'s <c>HatchBrush</c> patterns are undocumented internal bitmaps with no SkiaSharp equivalent,
	/// so every style here is a deterministic, tileable approximation (line/dot/diamond predicates driven
	/// by pixel coordinates modulo a period) rather than a byte-for-byte port — visually representative of
	/// the named style's family (diagonal hatch, percent-fill density, checkerboard, grid, ...), not pixel-exact.
	/// </summary>
	internal static class SkiaHatchPatterns
	{
		private const int TileSize = 24;

		internal static SKBitmap BuildTile(HatchStyle style, SKColor foreColor, SKColor backColor)
		{
			SKBitmap bitmap = new SKBitmap(TileSize, TileSize);
			for (int y = 0; y < TileSize; y++)
			{
				for (int x = 0; x < TileSize; x++)
				{
					bitmap.SetPixel(x, y, IsForeground(style, x, y) ? foreColor : backColor);
				}
			}
			return bitmap;
		}

		private static bool IsForeground(HatchStyle style, int x, int y) => style switch
		{
			HatchStyle.Horizontal => Lines(y, 8, 2),
			HatchStyle.LightHorizontal => Lines(y, 8, 1),
			HatchStyle.DarkHorizontal => Lines(y, 4, 2),
			HatchStyle.NarrowHorizontal => Lines(y, 4, 1),
			HatchStyle.DashedHorizontal => Lines(y, 8, 2) && Mod(x, 8) < 4,

			HatchStyle.Vertical => Lines(x, 8, 2),
			HatchStyle.LightVertical => Lines(x, 8, 1),
			HatchStyle.DarkVertical => Lines(x, 4, 2),
			HatchStyle.NarrowVertical => Lines(x, 4, 1),
			HatchStyle.DashedVertical => Lines(x, 8, 2) && Mod(y, 8) < 4,

			HatchStyle.ForwardDiagonal => Diagonal(x, y, 8, 2, false),
			HatchStyle.LightUpwardDiagonal => Diagonal(x, y, 8, 1, false),
			HatchStyle.DarkUpwardDiagonal => Diagonal(x, y, 4, 2, false),
			HatchStyle.WideUpwardDiagonal => Diagonal(x, y, 8, 3, false),
			HatchStyle.DashedUpwardDiagonal => Diagonal(x, y, 8, 2, false) && Mod(x, 8) < 4,

			HatchStyle.BackwardDiagonal => Diagonal(x, y, 8, 2, true),
			HatchStyle.LightDownwardDiagonal => Diagonal(x, y, 8, 1, true),
			HatchStyle.DarkDownwardDiagonal => Diagonal(x, y, 4, 2, true),
			HatchStyle.WideDownwardDiagonal => Diagonal(x, y, 8, 3, true),
			HatchStyle.DashedDownwardDiagonal => Diagonal(x, y, 8, 2, true) && Mod(x, 8) < 4,

			HatchStyle.Cross => Lines(x, 8, 2) || Lines(y, 8, 2), // == LargeGrid
			HatchStyle.SmallGrid => Lines(x, 6, 1) || Lines(y, 6, 1),
			HatchStyle.DottedGrid => (Lines(x, 8, 1) || Lines(y, 8, 1)) && Mod(x + y, 2) == 0,

			HatchStyle.DiagonalCross => Diagonal(x, y, 8, 2, false) || Diagonal(x, y, 8, 2, true),
			HatchStyle.Weave => Diagonal(x, y, 6, 1, false) || Diagonal(x, y, 6, 1, true),
			HatchStyle.Plaid => Lines(x, 8, 1) || Lines(y, 8, 1) || Diagonal(x, y, 12, 1, false),
			HatchStyle.Trellis => Diagonal(x, y, 8, 1, false) || Diagonal(x, y, 8, 1, true) || Lines(y, 12, 1),

			HatchStyle.Percent05 => Percent(x, y, 5),
			HatchStyle.Percent10 => Percent(x, y, 10),
			HatchStyle.Percent20 => Percent(x, y, 20),
			HatchStyle.Percent25 => Percent(x, y, 25),
			HatchStyle.Percent30 => Percent(x, y, 30),
			HatchStyle.Percent40 => Percent(x, y, 40),
			HatchStyle.Percent50 => Percent(x, y, 50),
			HatchStyle.Percent60 => Percent(x, y, 60),
			HatchStyle.Percent70 => Percent(x, y, 70),
			HatchStyle.Percent75 => Percent(x, y, 75),
			HatchStyle.Percent80 => Percent(x, y, 80),
			HatchStyle.Percent90 => Percent(x, y, 90),

			HatchStyle.SmallConfetti => Confetti(x, y, 25),
			HatchStyle.LargeConfetti => Confetti(x, y, 15) || Confetti(x + 7, y + 3, 10),

			HatchStyle.SmallCheckerBoard => Checker(x, y, 4),
			HatchStyle.LargeCheckerBoard => Checker(x, y, 8),

			HatchStyle.HorizontalBrick => HorizontalBrick(x, y),
			HatchStyle.DiagonalBrick => Diagonal(x, y, 8, 2, false),

			HatchStyle.ZigZag => TriangleWave(x, y, 8, 2),
			HatchStyle.Wave => TriangleWave(x, y, 12, 2),
			HatchStyle.Shingle => HorizontalBrick(x, y) || Diagonal(x, y, 8, 1, false),
			HatchStyle.Divot => Circle(x, y, 8, 1),
			HatchStyle.Sphere => Circle(x, y, 6, 2),

			HatchStyle.SolidDiamond => DiamondFill(x, y, 12),
			HatchStyle.OutlinedDiamond => DiamondOutline(x, y, 12, 1),
			HatchStyle.DottedDiamond => DiamondOutline(x, y, 12, 1) && Mod(x + y, 2) == 0,

			_ => Diagonal(x, y, 8, 2, false) || Diagonal(x, y, 8, 2, true), // unmapped styles fall back to a generic cross-hatch
		};

		private static int Mod(int a, int m)
		{
			int r = a % m;
			return r < 0 ? r + m : r;
		}

		private static bool Lines(int coord, int period, int thickness) => Mod(coord, period) < thickness;

		private static bool Diagonal(int x, int y, int period, int thickness, bool backward) =>
			Mod(backward ? x + y : x - y, period) < thickness;

		/// <summary>Deterministic pseudo-dither: hashes each tile-local pixel into a stable 0-99 bucket so the fraction below <paramref name="percent"/> approximates GDI+'s density fill without banding.</summary>
		private static bool Percent(int x, int y, int percent) => Mod((Mod(x, 10) + Mod(y, 10) * 10) * 37, 100) < percent;

		private static bool Checker(int x, int y, int cellSize) => (Mod(x, cellSize * 2) < cellSize) ^ (Mod(y, cellSize * 2) < cellSize);

		private static bool Confetti(int x, int y, int density)
		{
			unchecked
			{
				int h = x * 374761393 + y * 668265263;
				h = (h ^ (h >> 13)) * 1274126177;
				h ^= h >> 16;
				return Mod(h, 100) < density;
			}
		}

		private static bool Circle(int x, int y, int period, int radius)
		{
			int cx = Mod(x, period) - period / 2;
			int cy = Mod(y, period) - period / 2;
			return cx * cx + cy * cy <= radius * radius;
		}

		private static bool DiamondFill(int x, int y, int period)
		{
			int cx = Mod(x, period) - period / 2;
			int cy = Mod(y, period) - period / 2;
			return Math.Abs(cx) + Math.Abs(cy) <= period / 2;
		}

		private static bool DiamondOutline(int x, int y, int period, int thickness)
		{
			int cx = Mod(x, period) - period / 2;
			int cy = Mod(y, period) - period / 2;
			return Math.Abs(Math.Abs(cx) + Math.Abs(cy) - period / 2) < thickness;
		}

		private static bool TriangleWave(int x, int y, int period, int thickness)
		{
			int wave = Math.Abs(Mod(y, period) - period / 2);
			return Mod(x + wave, period) < thickness;
		}

		private static bool HorizontalBrick(int x, int y)
		{
			bool mortarRow = Lines(y, 8, 1);
			int xOffset = Mod(y, 16) < 8 ? x : x + 4;
			return mortarRow || Lines(xOffset, 8, 1);
		}
	}
}
