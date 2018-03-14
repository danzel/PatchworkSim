using System;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;

namespace PatchworkSim.AI.CNTK
{
	internal class BoardWithParent : IComparable<BoardWithParent>
	{
		public readonly BoardWithParent Parent;
		public readonly BoardState Board;
		public int Score;

		public BoardWithParent(BoardWithParent parent, BoardState board)
		{
			Parent = parent;
			Board = board;
			Score = 0;
		}

		public int CompareTo(BoardWithParent other)
		{
			return other.Score - Score;
		}

		public void SetScore(int score)
		{
			Score = score;
		}
	}
}