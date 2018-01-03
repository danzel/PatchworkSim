namespace PatchworkSim
{
	/// <summary>
	/// An IMoveDecisionMaker that always chooses to advance in front of the opponent (never purchases a piece)
	/// </summary>
	public class AlwaysAdvanceDecisionMaker : IMoveDecisionMaker
	{
		public static readonly AlwaysAdvanceDecisionMaker Instance = new AlwaysAdvanceDecisionMaker();

		private AlwaysAdvanceDecisionMaker()
		{
		}

		public void MakeMove(SimulationState state)
		{
			state.PerformAdvanceMove();
		}
	}
}
