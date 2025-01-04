using System;

namespace PatchworkSim
{
	public static class SevenXSevenTileChecker
	{
		private static readonly UInt128[] MatchingState;

		/// <summary>
		/// Calculate all 7x7 coverages
		/// </summary>
		static SevenXSevenTileChecker()
		{
			var line = (UInt128)0b1111111;

			var block = line
						| (line << (BoardState.Width * 1))
						| (line << (BoardState.Width * 2))
						| (line << (BoardState.Width * 3))
						| (line << (BoardState.Width * 4))
						| (line << (BoardState.Width * 5))
						| (line << (BoardState.Width * 6));

			MatchingState = new UInt128[(BoardState.Width - 7 + 1) * (BoardState.Height - 7 + 1)];
			int index = 0;
			for (var x = 0; x <= BoardState.Width - 7; x++)
			{
				for (var y = 0; y <= BoardState.Height - 7; y++)
				{
					MatchingState[index] = block << (x + y * BoardState.Width);
					index++;
				}
			}
		}

		public static bool Has7x7(UInt128 state)
		{
			for (var i = 0; i < MatchingState.Length; i++)
			{
				if ((MatchingState[i] & state) == MatchingState[i])
					return true;
			}

			return false;
		}
	}
}
