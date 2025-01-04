namespace PatchworkSim.AI.MoveMakers;

/// <summary>
/// Performs a MonteCarloTreeSearch of possible moves to figure out the best one to take.
/// Only considers if the piece can be actually placed for the initial move, any future moves use no placement simulation.
/// </summary>
public class MoveOnlyMonteCarloTreeSearchMoveMaker : BaseMoveOnlyMonteCarloTreeSearchMoveMaker
{
	private readonly bool _useMinusOne;
	private readonly double _progressiveBiasWeight;

	public override string Name => $"MO-MCTS({Mcts.Iterations}+{Mcts.RolloutMoveMaker.Name}+{_useMinusOne}+{_progressiveBiasWeight})";

	public MoveOnlyMonteCarloTreeSearchMoveMaker(int iterations, IMoveDecisionMaker rolloutMoveMaker = null, bool useMinusOne = false, double progressiveBiasWeight = 0) : base(iterations, rolloutMoveMaker)
	{
		_useMinusOne = useMinusOne;
		_progressiveBiasWeight = progressiveBiasWeight;
	}

	public override void MakeMove(SimulationState state)
	{
		var root = Mcts.PerformMCTS(state, _useMinusOne, _progressiveBiasWeight);

		var bestChild = Mcts.FindBestChild(root);
		//DumpChildren(root);

		if (bestChild.PieceToPurchase.HasValue)
			state.PerformPurchasePiece(bestChild.PieceToPurchase.Value);
		else
			state.PerformAdvanceMove();

		MonteCarloTreeSearch<SearchNode>.NodePool.Value.ReturnAll();
	}
}