using ImageMagick;
using System;
using System.Linq;

namespace TheForestWaiter.Content
{
	public class SpacedImageBuilder
	{
		private int TileWidth { get; set; }
		private int TileHeight { get; set; }

		public SpacedImageBuilder(int tileWidth, int tileHeight)
		{
			TileWidth = tileWidth;
			TileHeight = tileHeight;
		}

		public MagickImage BuildSpacedImage(MagickImage source)
		{
			int tilesWide = source.Width / TileWidth;
			int tilesHigh = source.Height / TileHeight;

			int newImageWidth = source.Width + 1 * tilesWide * 2;
			int newImageHeight = source.Height + 1 * tilesHigh * 2;


			MagickImage result = new(MagickColors.Transparent, newImageWidth, newImageHeight);

			IMagickImage<ushort>[] tiles = source.CropToTiles(TileWidth, TileHeight).ToArray();

			for (int y = 0; y < tilesHigh; y++)
			{
				for (int x = 0; x < tilesWide; x++)
				{
					using var subImage = tiles[x + y * tilesWide];
					subImage.BorderColor = MagickColors.Transparent;
					subImage.Border(1, 1);
					using var subImagePixels = subImage.GetPixels();

					DrawEdge(subImagePixels, subImage.Width, subImage.Height);
					result.Composite(subImage, x * subImage.Width, y * subImage.Height, CompositeOperator.SrcOver);
				}
			}

			Console.WriteLine($"Resized from {source.Width}x{source.Height} to {newImageWidth}x{newImageHeight}");
			return result;
		}

		public static void DrawEdge(IPixelCollection<ushort> pixels, int width, int height)
		{
			ushort[] brushXLeft = pixels.GetArea(1, 0, 1, height);
			ushort[] brushXRight = pixels.GetArea(width - 2, 0, 1, height);
			pixels.SetArea(0, 0, 1, height, brushXLeft);
			pixels.SetArea(width - 1, 0, 1, height, brushXRight);

			ushort[] brushYLeft = pixels.GetArea(0, 1, width, 1);
			ushort[] brushYRight = pixels.GetArea(0, height - 2, width, 1);
			pixels.SetArea(0, 0, width, 1, brushYLeft);
			pixels.SetArea(0, height - 1, width, 1, brushYRight);
		}
	}
}
