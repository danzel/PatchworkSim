using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead
{
	/// <summary>
	/// Strategy that tries to minimise amount of toggles between filled and not filled when looking across and up/down the board.
	/// This minimises the amount of holes, and helps to get holes filled in (even if it won't fill them in completely).
	/// Holes here are not strictly defined by flood filling, but can also include cave-like areas that are fully connected to the unused area but function like holes.
	/// </summary>
	public class TightPlacementStrategy : NoLookaheadStrategy
	{
		public override string Name => "Tight" + (_doubler ? " x2" : " +1");

		public static readonly TightPlacementStrategy InstanceDoubler = new TightPlacementStrategy(true);
		public static readonly TightPlacementStrategy InstanceIncrement = new TightPlacementStrategy(false);

		private readonly bool _doubler;

		/// <param name="doubler">If set, each toggle is 2x, otherwise each toggle is +1 score. Not sure which should be better</param>
		private TightPlacementStrategy(bool doubler)
		{
			_doubler = doubler;
		}

		protected override bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
		{
			resultBitmap = null;
			resultX = -1;
			resultY = -1;

			//TODO: We could do better with a good tiebreaker
			int bestScore = int.MaxValue;
			int tieBreakerScore = int.MaxValue;
			int tiedForBest = 0;

			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (board.CanPlace(bitmap,x , y))
						{
							CalculateScore(board, bitmap, x, y, _doubler, out var score);
							if (score < bestScore)
							{
								bestScore = score;
								tieBreakerScore = x + y;

								resultBitmap = bitmap;
								resultX = x;
								resultY = y;
								tiedForBest = 0;
							}
							else if (score == bestScore)
							{
								tiedForBest++;

								int ourTieBreaker = x + y;

								if (ourTieBreaker < tieBreakerScore)
								{
									//Console.WriteLine("Beat the tie");
									tieBreakerScore = ourTieBreaker;

									resultBitmap = bitmap;
									resultX = x;
									resultY = y;
								}
							}
						}
					}
				}
			}

			//if (tiedForBest > 0)
			//	Console.WriteLine($"{_doubler} {tiedForBest}");

			return resultBitmap != null;
		}

		private static void CalculateScore(BoardState board, PieceBitmap bitmap, int placeX, int placeY, bool doubler, out int score)
		{
			board.Place(bitmap, placeX, placeY);

			score = CalculateScore(board, doubler, 0, BoardState.Width, 0, BoardState.Height);
		}

		public static int CalculateScore(BoardState board, bool doubler, int minX, int maxX, int minY, int maxY)
		{
			int score = 0;

			//Scan down
			for (var x = minX; x < maxX; x++)
			{
				int thisScore = 1;
				var last = board[x, 0];

				for (var y = 1; y < BoardState.Height; y++)
				{
					var thisOne = board[x, y];

					if (thisOne != last)
					{
						if (doubler)
							thisScore *= 2;
						else
							thisScore++;
					}

					last = thisOne;
				}

				score += thisScore;
			}


			//Scan across
			for (var y = minY; y < maxY; y++)
			{
				int thisScore = 1;
				var last = board[0, y];

				for (var x = 1; x < BoardState.Width; x++)
				{
					var thisOne = board[x, y];

					if (thisOne != last)
					{
						if (doubler)
							thisScore *= 2;
						else
							thisScore++;
					}

					last = thisOne;
				}

				score += thisScore;
			}

			return score;
		}
	}
}
