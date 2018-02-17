namespace PatchworkSim.AI.MoveMakers
{
	/// <summary>
	/// Performs a MonteCarloTreeSearch of possible moves to figure out the best one to take.
	/// Only considers if the piece can be actually placed for the initial move, any future moves use no placement simulation.
	/// </summary>
	public class MoveOnlyMonteCarloTreeSearchMoveMaker : BaseMoveOnlyMonteCarloTreeSearchMoveMaker
	{
		public override string Name => $"MO-MCTS({Iterations}+{RolloutMoveMaker.Name})";

		public MoveOnlyMonteCarloTreeSearchMoveMaker(int iterations, IMoveDecisionMaker rolloutMoveMaker = null) : base(iterations, rolloutMoveMaker)
		{
		}

		public override void MakeMove(SimulationState state)
		{
			var root = PerformMCTS(state);

			var bestChild = Mcts.FindBestChild(root);
			//DumpChildren(root);

			if (bestChild.PieceToPurchase.HasValue)
				state.PerformPurchasePiece(bestChild.PieceToPurchase.Value);
			else
				state.PerformAdvanceMove();

			NodePool.Value.ReturnAll();
		}
	}
}