using System;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	public class TightPlacementStrategy : IPlacementStrategy
	{
		private readonly bool _doubler;
		public string Name => "Tight" + (_doubler ? " x2" : " +1");

		public static TightPlacementStrategy InstanceDoubler = new TightPlacementStrategy(true);
		public static TightPlacementStrategy InstanceIncrement = new TightPlacementStrategy(false);

		/// <param name="doubler">If set, each toggle is 2x, otherwise each toggle is +1</param>
		private TightPlacementStrategy(bool doubler)
		{
			_doubler = doubler;
		}

		public bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
		{
			resultBitmap = null;
			resultX = -1;
			resultY = -1;

			//TODO: We would do better with a good tiebreaker
			int bestScore = int.MaxValue;

			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (board.CanPlace(bitmap,x , y))
						{
							CalculateScore(board, bitmap, x, y, out var score);
							if (score < bestScore)
							{
								bestScore = score;

								resultBitmap = bitmap;
								resultX = x;
								resultY = y;
							}
						}
					}
				}
			}

			return resultBitmap != null;
		}

		private void CalculateScore(BoardState board, PieceBitmap bitmap, int placeX, int placeY, out int score)
		{
			score = 0;

			board.Place(bitmap, placeX, placeY);

			//Scan down
			for (var x = 0; x < BoardState.Width; x++)
			{
				int thisScore = 1;
				var last = board[x, 0];

				for (var y = 1; y < BoardState.Height; y++)
				{
					var thisOne = board[x, y];

					if (thisOne != last)
					{
						if (_doubler)
							thisScore *= 2;
						else
							thisScore++;
					}

					last = thisOne;
				}

				score += thisScore;
			}


			//Scan across
			for (var y = 0; y < BoardState.Height; y++)
			{
				int thisScore = 1;
				var last = board[0, y];

				for (var x = 1; x < BoardState.Width; x++)
				{
					var thisOne = board[x, y];

					if (thisOne != last)
					{
						if (_doubler)
							thisScore *= 2;
						else
							thisScore++;
					}

					last = thisOne;
				}

				score += thisScore;
			}
		}
	}
}
