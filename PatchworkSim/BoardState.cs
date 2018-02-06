using System;

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

		public bool this[int x, int y]
		{
			get { return !(_state & XYToPositionMask[x, y]).IsZero; }
			set
			{
#if DEBUG
				if (!value) throw new Exception("Can only set positions, not unset them");
#endif
				_state |= XYToPositionMask[x, y];
			}
		}

		public bool CanPlace(PieceBitmap bitmap, int x, int y)
		{
#if DEBUG
			if (x < 0)
				throw new Exception("X is outside of range");
			if (y < 0)
				throw new Exception("Y is outside of range");

			if (x + bitmap.Width > Width)
				throw new Exception("X is outside of range");
			if (y + bitmap.Height > Height)
				throw new Exception("Y is outside of range");
#endif
			var shifted = bitmap._bitmap << (x + y * Width);
			return (shifted & _state).IsZero;
		}

		public void Place(PieceBitmap bitmap, int x, int y)
		{
#if DEBUG
			if (!CanPlace(bitmap, x, y))
				throw new Exception("Cannot place piece here, it overlaps");
#endif
			var shifted = bitmap._bitmap << (x + y * Width);
			_state |= shifted;
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
						if (!(XYToPositionMask[x, y] & _state).IsZero)
							sum++;
					}
				}

				return sum;
			}
		}

		private static readonly UInt128[,] XYToPositionMask;

		static BoardState()
		{
			XYToPositionMask = new UInt128[Width, Height];
			for (var x = 0; x < Width; x++)
				for (var y = 0; y < Height; y++)
					XYToPositionMask[x, y] = UInt128.One << (x + y * Width);
		}
	}
}