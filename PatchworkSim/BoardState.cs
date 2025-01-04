#define SAFE_MODE
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace PatchworkSim
{
	/// <summary>
	/// The State of an individual players board
	/// </summary>
	public struct BoardState : IComparable<BoardState>
	{
		public const int Width = 9;
		public const int Height = 9;

		/// <summary>
		/// We use the bottom 81 bits (9x9) to show whether an individual position is covered (true) or not (false)
		/// </summary>
		private UInt128 _state;

		public bool this[int x, int y]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return (_state & XYToPositionMask[x + Width * y]) != 0; }
			set
			{
#if SAFE_MODE || DEBUG
				if (!value) throw new Exception("Can only set positions, not unset them");
#endif
				_state |= XYToPositionMask[x + Width * y];
			}
		}

		[Pure]
		public bool CanPlace(PieceBitmap bitmap, int x, int y)
		{
#if SAFE_MODE || DEBUG
			if (x < 0)
				throw new Exception("X is outside of range");
			if (y < 0)
				throw new Exception("Y is outside of range");

			if (x + bitmap.Width > Width)
				throw new Exception("X is outside of range");
			if (y + bitmap.Height > Height)
				throw new Exception("Y is outside of range");
#endif

			//var shifted = bitmap.Bitmap << (x + y * Width);
			var shifted = bitmap.GetShifted(x, y);

			return (shifted & _state) == 0;
		}

		public void Place(PieceBitmap bitmap, int x, int y)
		{
#if SAFE_MODE || DEBUG
			if (!CanPlace(bitmap, x, y))
				throw new Exception("Cannot place piece here, it overlaps");
#endif
			//var shifted = bitmap.Bitmap << (x + y * Width);
			var shifted = bitmap.GetShifted(x, y);

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
						if ((XYToPositionMask[x + Width * y] & _state) != 0)
							sum++;
					}
				}

				return sum;
			}
		}

		/// <summary>
		/// True if this board has a 7x7 area covered (for getting the special 7x7 reward)
		/// </summary>
		public bool Has7x7Coverage => SevenXSevenTileChecker.Has7x7(_state);

		public bool IsEmpty => _state == 0;

		private static readonly UInt128[] XYToPositionMask;

		static BoardState()
		{
			XYToPositionMask = new UInt128[Width * Height];
			for (var x = 0; x < Width; x++)
				for (var y = 0; y < Height; y++)
					XYToPositionMask[x + Width * y] = UInt128.One << (x + y * Width);
		}

		public int CompareTo(BoardState other)
		{
			return _state.CompareTo(other._state);
		}

		public override int GetHashCode()
		{
			return _state.GetHashCode();
		}
	}
}