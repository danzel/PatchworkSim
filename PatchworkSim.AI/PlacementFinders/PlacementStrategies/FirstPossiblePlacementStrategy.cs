namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	public class FirstPossiblePlacementStrategy : IPlacementStrategy
	{
		public static FirstPossiblePlacementStrategy Instance = new FirstPossiblePlacementStrategy();

		public string Name => "FirstPossible";

		private FirstPossiblePlacementStrategy()
		{
		}

		public bool TryPlacePiece(bool[,] board, PieceDefinition piece, out bool[,] resultBitmap, out int resultX, out int resultY)
		{
			foreach (var bitmap in piece.PossibleOrientations)
			{
				for (var y = 0; y <= SimulationState.PlayerBoardSize - bitmap.GetLength(1); y++)
				{
					for (var x = 0; x <= SimulationState.PlayerBoardSize - bitmap.GetLength(0); x++)
					{
						if (BitmapOps.CanPlace(board, bitmap, x, y))
						{
							resultBitmap = bitmap;
							resultX = x;
							resultY = y;
							return true;
						}
					}
				}
			}

			resultBitmap = null;
			resultX = -1;
			resultY = -1;
			return false;
		}
	}
}