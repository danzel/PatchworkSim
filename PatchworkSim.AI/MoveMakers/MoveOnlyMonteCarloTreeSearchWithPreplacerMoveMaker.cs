using System.Collections.Generic;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;

namespace PatchworkSim.AI.MoveMakers
{
	public class MoveOnlyMonteCarloTreeSearchWithPreplacerMoveMaker : BaseMonteCarloTreeSearchMoveMaker
	{
		public override string Name => $"MO-MCTS-P({_iterations}+{_rolloutMoveMaker.Name})";

		private readonly PreplacerStrategy _preplacer;
		private readonly List<int> _lookahead = new List<int>(PieceDefinition.AllPieceDefinitions.Length);

		public MoveOnlyMonteCarloTreeSearchWithPreplacerMoveMaker(int iterations, IMoveDecisionMaker rolloutMoveMaker, PreplacerStrategy preplacer) : base(iterations, rolloutMoveMaker)
		{
			_preplacer = preplacer;
		}


		public override void MakeMove(SimulationState state)
		{
			var root = PerformMCTS(state);

			_lookahead.Clear();

			SearchNode bestRootChild = null;

			//Go through what we currently think are our best moves and record the pieces we'll get
			//TODO: Need to handle patches in this loop
			while (root.Children.Count > 0)
			{
				var bestChild = FindBestChild(root);

				if (bestRootChild == null)
					bestRootChild = bestChild;

				if (bestChild.PieceToPurchase.HasValue)
				{
					_lookahead.Add(root.State.Pieces[bestChild.PieceToPurchase.Value % root.State.Pieces.Count]);//TODO
				}

				root = bestChild;
			}


			//Tell the Preplacer what we are planning on getting
			if (bestRootChild.PieceToPurchase.HasValue)
			{
				var piece = PieceDefinition.AllPieceDefinitions[_lookahead[0]];
				_lookahead.RemoveAt(0);
				//TODO: How do we put patches in to piece and the lookahead?
				_preplacer.PreparePlacePiece(state.PlayerBoardState[state.ActivePlayer], piece, _lookahead);
			}


			if (bestRootChild.PieceToPurchase.HasValue)
				state.PerformPurchasePiece(bestRootChild.PieceToPurchase.Value);
			else
				state.PerformAdvanceMove();

			NodePool.Value.ReturnAll();
		}
	}
}
