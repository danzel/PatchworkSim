using System;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators
{
	public class Pattern2x2BoardEvaluator : IBoardEvaluator
	{
		public string Name => "Pattern2x2BoardEvaluator";

		/// <summary>
		/// If a 2x2 area is fully covered
		/// </summary>
		private const int Full = 10;

		/// <summary>
		/// If a 2x2 area is covered just on the left
		/// </summary>
		private const int LeftHalf = 5;

		/// <summary>
		/// If a 2x2 area is covered just on the top
		/// </summary>
		private const int TopHalf = 5;

		/// <summary>
		/// If only diagonally opposite corners are covered
		/// </summary>
		private const int DiagonalOppositeCorners = -5;

		public void BeginEvaluation(BoardState currentBoard)
		{
		}

		public int Evaluate(in BoardState board, int minX, int maxX, int minY, int maxY)
		{
			int points = 0;

			for (var x = minX - 1; x < maxX; x++)
			{
				for (var y = minY - 1; y < maxY; y++)
				{
					var topLeft = Read(in board, x, y);
					var topRight = Read(in board, x + 1, y);
					var bottomLeft = Read(in board, x, y + 1);
					var bottomRight = Read(in board, x + 1, y + 1);



					if (topLeft && topRight && bottomLeft && bottomRight)
						points += Full;
					else if (topLeft && bottomLeft && !topRight && !bottomRight)
						points += LeftHalf;
					else if (topLeft && topRight && !bottomLeft && !bottomRight)
						points += TopHalf;
					else if (topLeft && bottomRight && !topRight && !bottomLeft)
						points += DiagonalOppositeCorners;
					else if (topRight && bottomLeft && !topLeft && !bottomRight)
						points += DiagonalOppositeCorners;

					//TODO: right, bottom, singles, triples (inverse singles)
				}
			}

			return points;
		}

		private bool Read(in BoardState board, int x, int y)
		{
			if (x < 0 || y < 0 || x >= BoardState.Width || y >= BoardState.Height)
				return true;
			return board[x, y];
		}
	}
}