using System;

namespace PatchworkSim.AI.CNTK
{
	internal class BoardWithParent : IComparable<BoardWithParent>
	{
		public readonly BoardState Board;

		/// <summary>
		/// % Chance this is a good move
		/// </summary>
		public float Score;

		public BoardWithParent(BoardState board)
		{
			Board = board;
			Score = 0;
		}

		public int CompareTo(BoardWithParent other)
		{
			return other.Score.CompareTo(Score);
		}

		public void SetScore(float score)
		{
			Score = score;
		}
	}
}