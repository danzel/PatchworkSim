using System;
using System.Linq;
using System.Threading;

namespace PatchworkSim.AI.MoveMakers
{
	public abstract class BaseMoveOnlyMonteCarloTreeSearchMoveMaker : IMoveDecisionMaker
	{
		public abstract string Name { get; }

		protected readonly int Iterations;
		protected readonly IMoveDecisionMaker RolloutMoveMaker;

		protected readonly MonteCarloTreeSearch<SearchNode> Mcts;


		/// <summary>
		/// Used by SimulateRollout
		/// </summary>
		private readonly SimulationState _rolloutState = new SimulationState();

		protected static readonly ThreadLocal<SingleThreadedPool<SearchNode>> NodePool = new ThreadLocal<SingleThreadedPool<SearchNode>>(() => new SingleThreadedPool<SearchNode>(), false);

		protected BaseMoveOnlyMonteCarloTreeSearchMoveMaker(int iterations, IMoveDecisionMaker rolloutMoveMaker = null)
		{
			Iterations = iterations;
			RolloutMoveMaker = rolloutMoveMaker ?? new RandomMoveMaker(0);

			Mcts = new MonteCarloTreeSearch<SearchNode>();
		}

		public abstract void MakeMove(SimulationState state);

		// http://mcts.ai/about/index.html
		// https://en.wikipedia.org/wiki/Monte_Carlo_tree_search
		/// <summary>
		/// Performs a MCTS search starting at the given state.
		/// Returns the root of the search tree, you must call NodePool.Value.ReturnAll() afterwards.
		/// </summary>
		protected SearchNode PerformMCTS(SimulationState state)
		{
			var root = NodePool.Value.Get();
			state.CloneTo(root.State);

			for (var i = 0; i < Iterations; i++)
			{
				//Selection
				var leaf = Mcts.Select(root);

				int winningPlayer;
				if (leaf.IsGameEnd)
				{
					winningPlayer = leaf.State.WinningPlayer;
				}
				else
				{
					//Expansion
					leaf.Expand();

					//Randomly choose one of the newly expanded nodes
					leaf = Mcts.Select(leaf);

					//Simulation
					winningPlayer = SimulateRollout(leaf.State);
				}

				//Backpropagation
				do
				{
					leaf.ReceiveBackpropagation(winningPlayer);
					leaf = leaf.Parent;
				} while (leaf != null);
			}

			return root;
		}

		private int SimulateRollout(SimulationState baseState)
		{
			if (baseState.GameHasEnded)
				return baseState.WinningPlayer;

			_rolloutState.Pieces.Clear();
			baseState.CloneTo(_rolloutState);
			_rolloutState.Fidelity = SimulationFidelity.NoPiecePlacing;

			//Run the game
			while (!_rolloutState.GameHasEnded)
			{
				RolloutMoveMaker.MakeMove(_rolloutState);
			}

			return _rolloutState.WinningPlayer;
		}

		protected void DumpChildren(SearchNode root)
		{
			Console.WriteLine(string.Join(", ", root.Children.Select(s => s.GetDebugText(root.State))));
		}

		public class SearchNode : MCTSNode<SearchNode>, IPoolableItem
		{
			public readonly SimulationState State = new SimulationState();
			public SearchNode Parent;

			/// <summary>
			/// The piece that was purchased to arrive at this node, or null for advance
			/// </summary>
			public int? PieceToPurchase;

			public bool IsGameEnd => State.GameHasEnded;

			public SearchNode() : base(4)
			{
			}

			public override void Expand()
			{
#if DEBUG
				if (Children.Count != 0)
					throw new Exception("Cannot expand an already expanded node");
				if (IsGameEnd)
					throw new Exception("Cannot expand a GameEnd node");
#endif
				//Advance
				{
					var node = NodePool.Value.Get();

					State.CloneTo(node.State);
					node.State.Fidelity = SimulationFidelity.NoPiecePlacing; //TODO Could do real placing
					node.State.PerformAdvanceMove();

					node.Parent = this;
					Children.Add(node);
				}

				for (var i = 0; i < 3; i++)
				{
					//This cares if they can actually place the piece only when expanding the root node
					if (Helpers.ActivePlayerCanPurchasePiece(State, Helpers.GetNextPiece(State, i)))
					{
						var node = NodePool.Value.Get();

						State.CloneTo(node.State);
						node.State.Fidelity = SimulationFidelity.NoPiecePlacing; //TODO Could do real placing
						var pieceIndex = node.State.NextPieceIndex + i;
						node.State.PerformPurchasePiece(pieceIndex);

						node.Parent = this;
						node.PieceToPurchase = pieceIndex;
						Children.Add(node);
					}
				}
			}

			public void ReceiveBackpropagation(int winningPlayer)
			{
				VisitCount++;
				if (Parent != null && winningPlayer == Parent.State.ActivePlayer)
					Value++;
			}

			public string GetDebugText(SimulationState parentState)
			{
				if (PieceToPurchase.HasValue)
				{
					return $"Purchase ({PieceDefinition.AllPieceDefinitions[parentState.Pieces[PieceToPurchase.Value % parentState.Pieces.Count]].Name}) {Value}/{VisitCount}";
				}
				else
				{
					return $"Advance {Value}/{VisitCount}";
				}
			}

			public void Reset()
			{
				Children.Clear();
				State.Pieces.Clear();
				Value = 0;
				VisitCount = 0;
				Parent = null;
				PieceToPurchase = null;
			}
		}
	}
}
