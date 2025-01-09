namespace PatchworkSim.AI.KerasAlphaZero;

public class MoveOnlyMonteCarloTreeSearchAlphaZeroMoveMaker : IMoveDecisionMaker
{
	public string Name => $"MonteCarloTreeSearchAlphaZero({_mcts.Iterations})";

	private MonteCarloTreeSearchAlphaZero _mcts;

	public MoveOnlyMonteCarloTreeSearchAlphaZeroMoveMaker(PatchworkServer.IPatchworkServerClient client, int iterations)
	{
		_mcts = new MonteCarloTreeSearchAlphaZero(client, iterations);
	}

	public void MakeMove(SimulationState state)
	{
		MakeMoveWithResult(state);
	}

	internal TrainSample MakeMoveWithResult(SimulationState state)
	{
		var res = new TrainSample();
		res.State = GameStateFactory.CreateGameState(state);

		var root = _mcts.PerformMCTS(state);

		var bestChild = _mcts.FindBestChild(root);
		//DumpChildren(root);

		if (bestChild.PieceToPurchase.HasValue)
			state.PerformPurchasePiece(bestChild.PieceToPurchase.Value);
		else
			state.PerformAdvanceMove();

		//Add all the moves, with 0 if they aren't possible
		res.MoveRating.Add(root.Children[0].VisitCount / (float)root.VisitCount);

		for (var i = 0; i < 3; i++)
		{
			bool foundIt = false;

			for (var j = 0; j < root.Children.Count; j++)
			{
				if (root.Children[j].PieceToPurchase == i)
				{
					foundIt = true;
					res.MoveRating.Add(root.Children[j].VisitCount / (float)root.VisitCount);
					break;
				}
			}

			if (!foundIt)
				res.MoveRating.Add(0);
		}

		MonteCarloTreeSearchAlphaZero.NodePool.Value!.ReturnAll();

		return res;
	}
}
