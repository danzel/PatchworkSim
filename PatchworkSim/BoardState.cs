﻿using System;
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

		public bool CanPlace(PieceBitmap bitmap, int x, int y)
		{
			if (x + bitmap.Width > Width)
				return false;
			if (y + bitmap.Height > Height)
				return false;

			var shifted = bitmap._bitmap << (x + y * Width);
			return (shifted & _state) == UInt128.Zero;
		}

		public void Place(PieceBitmap bitmap, int x, int y)
		{
			if (!CanPlace(bitmap, x, y))
				throw new Exception("Cannot place piece here, it overlaps");
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
						if ((XYToPositionMask(x, y) & _state) != UInt128.Zero)
							sum++;
					}
				}

				return sum;
			}
		}
	}
}