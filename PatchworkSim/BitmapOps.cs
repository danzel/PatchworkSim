using System;
using System.Collections.Generic;
using System.Linq;

namespace PatchworkSim
{
	public static class BitmapOps
	{
		public static bool[][,] CalculatePossibleOrientations(bool[,] bitmap)
		{
			var temp = new List<bool[,]>();

			temp.Add(bitmap);
			temp.Add(MirrorHorizontally(bitmap));

			temp.Add(MirrorVertically(temp[0]));
			temp.Add(MirrorVertically(temp[1]));

			for (var i = 0; i < 4; i++)
				temp.Add(RotateAntiClockwise(temp[i]));

			return temp.Distinct(new BoolArrayComparer()).ToArray();
		}

		private static bool[,] MirrorHorizontally(bool[,] bitmap)
		{
			var copy = new bool[bitmap.GetLength(0), bitmap.GetLength(1)];
			for (var y = 0; y < bitmap.GetLength(1); y++)
			{
				for (var x = 0; x < bitmap.GetLength(0); x++)
				{
					copy[x, y] = bitmap[bitmap.GetLength(0) - 1 - x, y];
				}
			}
			return copy;
		}

		private static bool[,] MirrorVertically(bool[,] bitmap)
		{
			var copy = new bool[bitmap.GetLength(0), bitmap.GetLength(1)];
			for (var y = 0; y < bitmap.GetLength(1); y++)
			{
				for (var x = 0; x < bitmap.GetLength(0); x++)
				{
					copy[x, y] = bitmap[x, bitmap.GetLength(1) - 1 - y];
				}
			}
			return copy;
		}

		private static bool[,] RotateAntiClockwise(bool[,] bitmap)
		{
			var copy = new bool[bitmap.GetLength(1), bitmap.GetLength(0)];
			for (var y = 0; y < bitmap.GetLength(1); y++)
			{
				for (var x = 0; x < bitmap.GetLength(0); x++)
				{
					copy[bitmap.GetLength(1) - 1 - y, x] = bitmap[x, y];
				}
			}
			return copy;
		}

		internal class BoolArrayComparer : IEqualityComparer<bool[,]>
		{
			public bool Equals(bool[,] bitmap0, bool[,] bitmap1)
			{
				if (bitmap0.GetLength(0) != bitmap1.GetLength(0))
					return false;
				if (bitmap0.GetLength(1) != bitmap1.GetLength(1))
					return false;

				for (var y = 0; y < bitmap0.GetLength(1); y++)
				{
					for (var x = 0; x < bitmap0.GetLength(0); x++)
					{
						if (bitmap0[x, y] != bitmap1[x, y])
							return false;
					}
				}

				return true;
			}

			public int GetHashCode(bool[,] bitmap)
			{
				//This is a terrible hash lol
				int result = 0;
				for (var y = 0; y < bitmap.GetLength(1); y++)
				{
					for (var x = 0; x < bitmap.GetLength(0); x++)
					{
						result = (result | (bitmap[x, y] ? 1 : 0)) << 1;
					}
				}
				return result;
			}
		}

		public static int SumUsed(bool[,] bitmap)
		{
			int result = 0;
			for (var y = 0; y < bitmap.GetLength(1); y++)
			{
				for (var x = 0; x < bitmap.GetLength(0); x++)
				{
					if (bitmap[x, y])
						result++;
				}
			}
			return result;
		}

		/// <summary>
		/// Returns true if all of the positions the given bitmap requires for placement are empty
		/// </summary>
		public static bool CanPlace(bool[,] board, bool[,] bitmap, int x, int y)
		{
			if (x + bitmap.GetLength(0) > board.GetLength(0))
				return false;
			if (y + bitmap.GetLength(1) > board.GetLength(1))
				return false;


			for (var bitmapY = 0; bitmapY < bitmap.GetLength(1); bitmapY++)
			{
				for (var bitmapX = 0; bitmapX < bitmap.GetLength(0); bitmapX++)
				{
					if (board[x + bitmapX, y + bitmapY] && bitmap[bitmapX, bitmapY])
						return false;
				}
			}
			return true;
		}

		public static void Place(bool[,] board, bool[,] bitmap, int x, int y)
		{
			for (var bitmapY = 0; bitmapY < bitmap.GetLength(1); bitmapY++)
			{
				for (var bitmapX = 0; bitmapX < bitmap.GetLength(0); bitmapX++)
				{
					if (bitmap[bitmapX, bitmapY])
					{
						if (board[x + bitmapX, y + bitmapY])
							throw new Exception("Cannot place piece here, it overlaps");
						board[x + bitmapX, y + bitmapY] = true;
					}
				}
			}
		}
	}
}