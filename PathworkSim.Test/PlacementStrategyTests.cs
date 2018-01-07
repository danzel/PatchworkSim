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

			var board = new bool[9, 9];

			foreach (var piece in pieces)
			{
				if (FirstPossiblePlacementStrategy.Instance.TryPlacePiece(board, PieceDefinition.AllPieceDefinitions[piece], out var bitmap, out var x, out var y))
				{
					placed++;
					BitmapOps.Place(board, bitmap, x, y);
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
	}
}
