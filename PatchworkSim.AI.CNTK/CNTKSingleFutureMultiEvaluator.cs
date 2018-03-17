using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchworkSim.AI.CNTK
{
	class CNTKSingleFutureMultiEvaluator
	{
		private readonly BulkBoardEvaluator _boardEvaluator;
		private readonly int _gamesPlayedPerLoop;

		private readonly Random _rand = new Random(0);

		private readonly ListPool<BoardWithParent> _pool = new ListPool<BoardWithParent>();

		public CNTKSingleFutureMultiEvaluator(BulkBoardEvaluator boardEvaluator, int gamesPlayedPerLoop)
		{
			_boardEvaluator = boardEvaluator;
			_gamesPlayedPerLoop = gamesPlayedPerLoop;
		}

		public List<TrainingSample> GenerateTrainingData(List<PieceDefinition> pieces, out int totalAreaCovered)
		{
			var placementTrees = GetAllPossibleInitialPlacements(pieces[0]);
			placementTrees = PerformInitialCull(placementTrees);

			totalAreaCovered = 0;
			var placedArea = new int[_gamesPlayedPerLoop];
			for (var i = 0; i < _gamesPlayedPerLoop; i++)
				placedArea[i] = pieces[0].TotalUsedLocations;

			var hasStoppedPlacing = new bool[_gamesPlayedPerLoop]; //default false
			int amountStillPlacing = _gamesPlayedPerLoop;

			var pieceIndex = 1;
			while (amountStillPlacing > 0)
			{
				var nextPlacements = _pool.Get();

				var piece = pieces[pieceIndex];

				for (var i = 0; i < placementTrees.Count ; i++)
				{
					if (hasStoppedPlacing[i])
						continue;

					totalAreaCovered += piece.TotalUsedLocations;
					placedArea[i] += piece.TotalUsedLocations;

					//Calculate all children and add to nextPlacements
					var pIndex = nextPlacements.Count;
					GetAllPossiblePlacements(placementTrees[i], piece, nextPlacements);
					for (; pIndex < nextPlacements.Count; pIndex++)
						nextPlacements[pIndex].ParentIndex = i;
				}

				_boardEvaluator.Evaluate(nextPlacements);

				//Randomly pick a child for each and keep that one
				var firstChildIndex = 0;
				for (var i = 0; i < placementTrees.Count; i++)
				{
					if (hasStoppedPlacing[i])
						continue;

					//Weighted randomly pick a child to succeed them
					var lastChildIndex = firstChildIndex;
					for (; lastChildIndex < nextPlacements.Count; lastChildIndex++)
					{
						if (nextPlacements[lastChildIndex].ParentIndex != i)
							break;
					}

					var successorIndex = WeightedRandomPick(nextPlacements, firstChildIndex, lastChildIndex - firstChildIndex);
					placementTrees[i] = nextPlacements[successorIndex];


					firstChildIndex = lastChildIndex;
				}

				_pool.Return(nextPlacements);
				nextPlacements = null;

				pieceIndex++;

				//Update hasStoppedPlacing based on canPlace for next piece
				for (var i = 0; i < placementTrees.Count; i++)
				{
					if (hasStoppedPlacing[i])
						continue;

					if (!Helpers.CanPlace(placementTrees[i].Board, pieces[pieceIndex]))
					{
						hasStoppedPlacing[i] = true;
						amountStillPlacing--;
					}
				}
			}

			List<TrainingSample> result = new List<TrainingSample>(); //TODO: Can be a child variable for GC
			for (var i = 0 ; i < placementTrees.Count; i++)
			{
				var p = placementTrees[i];
				while (p != null)
				{
					result.Add(new TrainingSample(p.Board, placedArea[i]));
					p = p.Parent;
				}
			}

			return result;
			//return placementTrees.Select((p, i) => new TrainingSample(p.Board, placedArea[i])).ToList();
		}

		private int WeightedRandomPick(List<BoardWithParent> possible, int start, int count)
		{
			float totalScore = 0;
			for (var i = start; i < start + count; i++)
				totalScore += possible[i].Score;

			var targetScore = (float)_rand.NextDouble() * totalScore;

			for (var i = start; i < start + count; i++)
			{
				targetScore -= possible[i].Score;
				if (targetScore <= 0)
				{
					return i;
				}
			}

			throw new Exception("Failed to pick");
		}

		private List<BoardWithParent> PerformInitialCull(List<BoardWithParent> placementTrees)
		{
			List<BoardWithParent> result = _pool.Get();

			_boardEvaluator.Evaluate(placementTrees);
			Shuffle(placementTrees);


			//Randomly pick _gamesPlayedPerLoop items from placementTrees, likelyhood to pick any weighted by the individual score
			for (var a = 0; a < _gamesPlayedPerLoop; a++)
			{
				var index = WeightedRandomPick(placementTrees, 0, placementTrees.Count);
				result.Add(placementTrees[index]);
				placementTrees.RemoveAt(index);
			}

			_pool.Return(placementTrees);
			return result;
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

		private List<BoardWithParent> GetAllPossibleInitialPlacements(PieceDefinition piece)
		{
			//TODO: Maybe optimise empty board placement?

			var result = _pool.Get();

			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (int x = 0; x < BoardState.Width - bitmap.Width + 1; x++)
				{
					for (int y = 0; y < BoardState.Height - bitmap.Height + 1; y++)
					{
						//if (board.CanPlace(bitmap, x, y)) Can place anywhere on an empty board
						{
							var clone = new BoardState();
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
	}
}
