using System;

namespace PatchworkSim.AI.CNTK
{
	internal class BoardWithParent : IComparable<BoardWithParent>
	{
		public readonly BoardWithParent Parent;
		public readonly BoardState Board;

		/// <summary>
		/// % Change this is a good move
		/// </summary>
		public float Score;

		/// <summary>
		/// For SNTKSingleFutureMultiEvaluator
		/// </summary>
		public int ParentIndex;

		public BoardWithParent(BoardWithParent parent, BoardState board)
		{
			Parent = parent;
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