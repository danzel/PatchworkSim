using System;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;

namespace PatchworkSim.AI.CNTK
{
	internal struct BoardWithPlacement : IComparable<BoardWithPlacement>
	{
		public readonly BoardState Board;

		public int Score;

		public readonly Preplacement FirstPiecePlacement;

		public BoardWithPlacement(BoardState board, Preplacement firstPiecePlacement)
		{
			Board = board;
			Score = 0;
			FirstPiecePlacement = firstPiecePlacement;
		}

		public int CompareTo(BoardWithPlacement other)
		{
			return other.Score - Score;
		}

		public void SetScore(int score)
		{
			Score = score;
		}
	}
}