using System;
using System.Runtime.CompilerServices;

namespace PatchworkSim
{
	/// <summary>
	/// The State of an individual players board
	/// </summary>
	public struct BoardState
	{
		public const int Width = 9;
		public const int Height = 9;

		/// <summary>
		/// We use the bottom 81 bits (9x9) to show whether an individual position is covered (true) or not (false)
		/// </summary>
		private UInt128 _state;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private UInt128 XYToPositionMask(int x, int y)
		{
			return UInt128.One << (x + y * Width);
		}

		[Obsolete("Shouldnt need bitwise operations like this hopefully")]
		public bool this[int x, int y]
		{
			get { return (_state & XYToPositionMask(x, y)) != UInt128.Zero; }
			set
			{
				if (!value) throw new Exception("Can only set positions, not unset them");
				_state |= XYToPositionMask(x, y);
			}
		}

		[Obsolete("Pieces should be stored in UInt128 too, then this can be a simple bitmask op")]
		public bool CanPlace(bool[,] bitmap, int x, int y)
		{
			//TODO: This should be done using awesome bitmap ops instead

			if (x + bitmap.GetLength(0) > Width)
				return false;
			if (y + bitmap.GetLength(1) > Height)
				return false;


			for (var bitmapY = 0; bitmapY < bitmap.GetLength(1); bitmapY++)
			{
				for (var bitmapX = 0; bitmapX < bitmap.GetLength(0); bitmapX++)
				{
					if (this[x + bitmapX, y + bitmapY] && bitmap[bitmapX, bitmapY])
						return false;
				}
			}

			return true;
		}

		[Obsolete("Pieces should be stored in UInt128 too, then this can be a simple bitmask op")]
		public void Place(bool[,] bitmap, int x, int y)
		{
			for (var bitmapY = 0; bitmapY < bitmap.GetLength(1); bitmapY++)
			{
				for (var bitmapX = 0; bitmapX < bitmap.GetLength(0); bitmapX++)
				{
					if (bitmap[bitmapX, bitmapY])
					{
						if (this[x + bitmapX, y + bitmapY])
							throw new Exception("Cannot place piece here, it overlaps");
						this[x + bitmapX, y + bitmapY] = true;
					}
				}
			}
		}

		public int UsedPositionCount
		{
			get
			{
				int sum = 0;
				for (var x = 0; x < Width; x++)
				{
					for (var y = 0; y < Height; y++)
					{
						if ((XYToPositionMask(x, y) & _state) != UInt128.Zero)
							sum++;
					}
				}

				return sum;
			}
		}
	}
}