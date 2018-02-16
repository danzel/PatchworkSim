using System.Collections.Generic;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;

namespace PatchworkSim.AI.MoveMakers
{
	public class MoveOnlyMonteCarloTreeSearchWithPreplacerMoveMaker : BaseMonteCarloTreeSearchMoveMaker
	{
		public override string Name => $"MO-MCTS-P({_iterations}+{_rolloutMoveMaker.Name})";

		private readonly PreplacerStrategy _preplacer;
		private readonly List<PieceDefinition> _lookahead = new List<PieceDefinition>(PieceDefinition.AllPieceDefinitions.Length + SimulationState.LeatherPatches.Length);

		public MoveOnlyMonteCarloTreeSearchWithPreplacerMoveMaker(int iterations, IMoveDecisionMaker rolloutMoveMaker, PreplacerStrategy preplacer) : base(iterations, rolloutMoveMaker)
		{
			_preplacer = preplacer;
		}


		public override void MakeMove(SimulationState state)
		{
			var root = PerformMCTS(state);
			//DumpChildren(root);

			_lookahead.Clear();

			SearchNode bestRootChild = null;
			int preplaceAmount = 0;

			//Go through what we currently think are our best moves and record the pieces we'll get
			while (root.Children.Count > 0)
			{
				var bestChild = FindBestChild(root);

				if (bestRootChild == null)
					bestRootChild = bestChild;

				//If this move was made by us
				if (root.State.ActivePlayer == state.ActivePlayer)
				{
					if (bestChild.PieceToPurchase.HasValue)
					{
						_lookahead.Add(PieceDefinition.AllPieceDefinitions[root.State.Pieces[bestChild.PieceToPurchase.Value % root.State.Pieces.Count]]);
					}

					//If we moved past a leather patch, we must have got it
					if (bestChild.State.LeatherPatchesIndex > root.State.LeatherPatchesIndex)
					{
						_lookahead.Add(PieceDefinition.LeatherTile);
					}
				}

				//Only need to preplace if we get a piece in our first move
				if (bestRootChild == bestChild && _lookahead.Count > 0)
				{
					//If we have made one move and got 2 pieces, that means we purchased and got a leather tile, so we need to plan for that
					preplaceAmount = _lookahead.Count;
				}

				root = bestChild;
			}


			//Tell the Preplacer what we are planning on getting
			if (preplaceAmount > 0)
			{
				_preplacer.PreparePlacePiece(state.PlayerBoardState[state.ActivePlayer], _lookahead, preplaceAmount);
			}


			if (bestRootChild.PieceToPurchase.HasValue)
				state.PerformPurchasePiece(bestRootChild.PieceToPurchase.Value);
			else
				state.PerformAdvanceMove();

			NodePool.Value.ReturnAll();
		}
	}
}
