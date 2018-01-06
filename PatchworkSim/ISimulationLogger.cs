namespace PatchworkSim
{
	public interface ISimulationLogger
	{
		void PlayerAdvanced(int player);

		void PlayerPurchasedPiece(int player, PieceDefinition piece);

		void PlayerPlacedPiece(int player, PieceDefinition piece, int x, int y, bool[,] bitmap);
	}

	public class NullSimulationLogger : ISimulationLogger
	{
		public static NullSimulationLogger Instance = new NullSimulationLogger();

		private NullSimulationLogger()
		{
		}

		public void PlayerAdvanced(int player)
		{
		}

		public void PlayerPurchasedPiece(int player, PieceDefinition piece)
		{
		}

		public void PlayerPlacedPiece(int player, PieceDefinition piece, int x, int y, bool[,] bitmap)
		{
		}
	}
}