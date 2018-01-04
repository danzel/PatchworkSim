namespace PatchworkSim
{
	public class PlayerDecisionMaker
	{
		public IMoveDecisionMaker MoveDecisionMaker { get; }
		public IPlacementDecisionMaker PlacementDecisionMaker { get; }

		public PlayerDecisionMaker(IMoveDecisionMaker moveDecisionMaker, IPlacementDecisionMaker placementDecisionMaker)
		{
			MoveDecisionMaker = moveDecisionMaker;
			PlacementDecisionMaker = placementDecisionMaker;
		}
	}

	/// <summary>
	/// Responsible for deciding what move to make.
	/// </summary>
	public interface IMoveDecisionMaker
	{
		/// <summary>
		/// Make a move for the active player
		/// </summary>
		void MakeMove(SimulationState state);
	}

	public interface IPlacementDecisionMaker
	{
		void PlacePiece(SimulationState state);
	}
}