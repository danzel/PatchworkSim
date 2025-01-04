namespace PatchworkSim.AI.MoveMakers;

/// <summary>
/// An IMoveDecisionMaker that always chooses to advance in front of the opponent (never purchases a piece)
/// </summary>
public class AlwaysAdvanceMoveMaker : IMoveDecisionMaker
{
	public static readonly AlwaysAdvanceMoveMaker Instance = new AlwaysAdvanceMoveMaker();

	private AlwaysAdvanceMoveMaker()
	{
	}

	public void MakeMove(SimulationState state)
	{
		state.PerformAdvanceMove();
	}

	public string Name => "AlwaysAdvance";
}
