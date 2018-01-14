using PatchworkSim;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using Xunit;

namespace PathworkSim.Test
{
	public class PlacementStrategyTests
	{
		[Fact]
		public void FirstPossible()
		{
			var pieces = SimulationHelpers.GetRandomPieces(1);
			int placed = 0;

			var board = new BoardState();

			foreach (var piece in pieces)
			{
				if (FirstPossiblePlacementStrategy.Instance.TryPlacePiece(board, PieceDefinition.AllPieceDefinitions[piece], out var bitmap, out var x, out var y))
				{
					placed++;
					board.Place(bitmap, x, y);
				}
				else
				{
					break;
				}
			}

			//This number totally relies on the order of pieces in the array.
			//Update it if the ordering changes, the extact amount isn't important
			Assert.Equal(12, placed);
		}

		[Fact]
		public void SimpleClosestToWallAndCorner()
		{
			var pieces = SimulationHelpers.GetRandomPieces(1);
			int placed = 0;

			var board = new BoardState();

			foreach (var piece in pieces)
			{
				if (SimpleClosestToWallAndCornerStrategy.Instance.TryPlacePiece(board, PieceDefinition.AllPieceDefinitions[piece], out var bitmap, out var x, out var y))
				{
					placed++;
					board.Place(bitmap, x, y);
				}
				else
				{
					break;
				}
			}

			//This number totally relies on the order of pieces in the array.
			//Update it if the ordering changes, the extact amount isn't important
			//Hopefully this is at least as many places as FirstPossible
			Assert.Equal(13, placed);
		}

		[Fact]
		public void SmallestBoundingBox()
		{
			var pieces = SimulationHelpers.GetRandomPieces(1);
			int placed = 0;

			var board = new BoardState();

			foreach (var piece in pieces)
			{
				if (SmallestBoundingBoxPlacementStrategy.Instance.TryPlacePiece(board, PieceDefinition.AllPieceDefinitions[piece], out var bitmap, out var x, out var y))
				{
					placed++;
					board.Place(bitmap, x, y);
				}
				else
				{
					break;
				}
			}

			//This number totally relies on the order of pieces in the array.
			//Update it if the ordering changes, the extact amount isn't important
			//Hopefully this is at least as many places as FirstPossible
			Assert.Equal(12, placed);
		}
	}
}
