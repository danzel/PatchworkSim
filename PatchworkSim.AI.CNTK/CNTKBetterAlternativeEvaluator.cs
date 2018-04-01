using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatchworkSim.Loggers;

namespace PatchworkSim.AI.CNTK
{
	class CNTKBetterAlternativeEvaluator
	{
		private readonly BulkBoardEvaluator _boardEvaluator;

		private readonly Random _rand = new Random(0);
		private readonly ListPool<BoardWithParent> _pool = new ListPool<BoardWithParent>();


		public CNTKBetterAlternativeEvaluator(BulkBoardEvaluator boardEvaluator)
		{
			_boardEvaluator = boardEvaluator;
		}

		public List<TrainingSample> GenerateTrainingData(List<PieceDefinition> pieces, out int areaCovered)
		{
			areaCovered = 0;

			var boards = new List<BoardState> { new BoardState() };

			var board = new BoardState();
			var pieceIndex = 0;

			//Perform initial placements
			while (Helpers.CanPlace(board, pieces[pieceIndex]))
			{
				areaCovered += pieces[pieceIndex].TotalUsedLocations;
				var placements = GetAllPossiblePlacements(board, pieces[pieceIndex]);

				_boardEvaluator.Evaluate(placements);

				//Find the best score and do that move
				var bestIndex = FindBest(placements, 0, placements.Count);

				//Add to boards
				board = placements[bestIndex].Board;
				boards.Add(board);

				_pool.Return(placements);
				pieceIndex++;
			}
			//We placed boards.Count - 1 pieces

			//Randomly pick one of the moves (from boards)
			var pickIndex = _rand.Next(boards.Count);

			//Starting from all of the alternate moves we could have done there, play out the rest of the game using the recommended moves
			//Get all of the possible child moves
			var currentPlacements = GetAllPossiblePlacements(boards[pickIndex], pieces[pickIndex]);

			//There were no alternative moves
			if (currentPlacements.Count == 1)
				return new List<TrainingSample>();

			int level = 0;
			pieceIndex = pickIndex;

			while (true)
			{
				pieceIndex++;

				//Playout the future moves from there (using the recommended placements for all future placements)
				var nextPlacements = _pool.Get();
				for (var i = 0; i < currentPlacements.Count; i++)
				{
					//Calculate all children and add to nextPlacements
					var pIndex = nextPlacements.Count;
					GetAllPossiblePlacements(currentPlacements[i], pieces[pieceIndex], nextPlacements);
					for (; pIndex < nextPlacements.Count; pIndex++)
						nextPlacements[pIndex].ParentIndex = i;
				}

				if (nextPlacements.Count == 0)
					break;
				level++;
				_boardEvaluator.Evaluate(nextPlacements);

				//Replace each with their 'best' child
				var nextCurrentPlacements = _pool.Get();
				var firstChildIndex = 0;
				for (var i = 0; i < currentPlacements.Count; i++)
				{
					//Find the range that our children are in
					var lastChildIndex = firstChildIndex;
					for (; lastChildIndex < nextPlacements.Count; lastChildIndex++)
					{
						if (nextPlacements[lastChildIndex].ParentIndex != i)
							break;
					}

					//Get the best child (If we had any child placements)
					if (lastChildIndex != firstChildIndex)
					{
						var bestChildIndex = FindBest(nextPlacements, firstChildIndex, lastChildIndex - firstChildIndex);
						nextCurrentPlacements.Add(nextPlacements[bestChildIndex]);
						firstChildIndex = lastChildIndex;
					}
				}

				_pool.Return(nextPlacements);
				_pool.Return(currentPlacements);
				currentPlacements = nextCurrentPlacements;

				//Stop if we are just down to one child
				if (currentPlacements.Count == 1)
					break;
			}
			
			//If any of them did better, remember that they did
			if (level > boards.Count - pickIndex)
			{
				//We did better
				//Return training example of that one being good and the one we did being bad
				//Console.WriteLine("better");
				var result = new List<TrainingSample>(currentPlacements.Count + 1);
				result.Add(new TrainingSample(boards[pickIndex], 0));

				for (var i = 0; i < currentPlacements.Count; i++)
				{
					var best = currentPlacements[i];
					while (best.Parent != null)
						best = best.Parent;
					result.Add(new TrainingSample(best.Board, 1));
				}

				return result;
			}

			//We didn't do better, so return an empty list
			return new List<TrainingSample>();
		}

		private int FindBest(List<BoardWithParent> placements, int startIndex, int count)
		{
			//TODO: What if there is a tie in weights

			var bestIndex = startIndex;
			var bestScore = placements[startIndex].Score;
			for (var i = startIndex + 1; i < startIndex + count; i++)
			{
				if (placements[i].Score > bestScore)
				{
					bestIndex = i;
					bestScore = placements[i].Score;
				}
			}

			return bestIndex;
		}

		private List<BoardWithParent> GetAllPossiblePlacements(BoardState board, PieceDefinition piece)
		{
			//TODO: Maybe optimise empty board placement?

			var result = _pool.Get();

			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (board.CanPlace(bitmap, x, y))
						{
							var clone = board;
							clone.Place(bitmap, x, y);
							result.Add(new BoardWithParent(null, clone));
						}
					}
				}
			}

			return result;
		}

		private void GetAllPossiblePlacements(BoardWithParent boardWithParent, PieceDefinition piece, List<BoardWithParent> result)
		{
			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						if (boardWithParent.Board.CanPlace(bitmap, x, y))
						{
							var clone = boardWithParent.Board;
							clone.Place(bitmap, x, y);
							result.Add(new BoardWithParent(boardWithParent, clone));
						}
					}
				}
			}
		}

		public void Shuffle(List<BoardWithParent> placements)
		{
			int n = placements.Count;
			while (n > 1)
			{
				n--;
				int k = _rand.Next(n + 1);
				var value = placements[k];
				placements[k] = placements[n];
				placements[n] = value;
			}
		}
	}
}