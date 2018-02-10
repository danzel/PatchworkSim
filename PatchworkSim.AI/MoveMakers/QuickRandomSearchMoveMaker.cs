namespace PatchworkSim.AI.MoveMakers
{
	/// <summary>
	/// Performs random playouts of the game for a given amount of turns. Makes the decision that leads to the least losses.
	/// Simulation quality in the random playouts is reduced for no placements to make this quicker.
	/// This can be seen as related to MCTS (but simpler and probably not as good)
	/// </summary>
	public class QuickRandomSearchMoveMaker : IMoveDecisionMaker
	{
		public string Name => $"QuickRandomSearch({_depth}-{_playouts})";

		private readonly int _depth;
		private readonly int _playouts;
		private readonly RandomMoveMaker _randomMoveMaker;

		private readonly SimulationState _simulationState = new SimulationState();

		/// <param name="depth">How many turns are evaluated before stopping (Should be an even number)</param>
		/// <param name="playouts">How many playouts of the game are performed for each move evaluation</param>
		public QuickRandomSearchMoveMaker(int depth, int playouts, int randomSeed = 0)
		{
			_depth = depth;
			_playouts = playouts;
			_randomMoveMaker = new RandomMoveMaker(randomSeed);
		}

		public void MakeMove(SimulationState state)
		{
			//Figure out how many different moves we can make
			int possibleMoves = 1; //can always advance
			for (var i = 0; i < 3; i++)
			{
				if (Helpers.ActivePlayerCanPurchasePiece(state, Helpers.GetNextPiece(state, i)))
					possibleMoves++;
			}

			//Divide playout
			var playoutsPerMove = _playouts / possibleMoves;

			//Perform playouts
			var winsAdvance = PerformAdvancePlayouts(state, playoutsPerMove);
			var winsPurchase0 = PerformPurchasePlayouts(state, 0, playoutsPerMove);
			var winsPurchase1 = PerformPurchasePlayouts(state, 1, playoutsPerMove);
			var winsPurchase2 = PerformPurchasePlayouts(state, 2, playoutsPerMove);


			//Do the best (TODO Should we favor purchasing over advancing in a draw?)
			if (winsAdvance >= winsPurchase0 && winsAdvance >= winsPurchase1 && winsAdvance >= winsPurchase2)
				state.PerformAdvanceMove();
			else if (winsPurchase0 >= winsPurchase1 && winsPurchase0 >= winsPurchase2)
				state.PerformPurchasePiece(state.NextPieceIndex + 0);
			else if (winsPurchase1 >= winsPurchase2)
				state.PerformPurchasePiece(state.NextPieceIndex + 1);
			else
				state.PerformPurchasePiece(state.NextPieceIndex + 2);
		}

		private int PerformAdvancePlayouts(SimulationState baseState, int playoutsPerMove)
		{
			int wins = 0;

			for (var i = 0; i < playoutsPerMove; i++)
			{
				_simulationState.Pieces.Clear();
				baseState.CloneTo(_simulationState);

				_simulationState.Fidelity = SimulationFidelity.NoPiecePlacing;

				_simulationState.PerformAdvanceMove();
				
				//Run the game
				for (var move = 1; move < _depth && !_simulationState.GameHasEnded; move++)
				{
					_randomMoveMaker.MakeMove(_simulationState);
				}

				var winner = EvaluateWinningPlayer(_simulationState);
				if (winner == baseState.ActivePlayer)
					wins++;
			}

			return wins;
		}

		//TODO: Mostly copy/paste of above except CanPurchasePiece and PerformPurchasePiece
		private int PerformPurchasePlayouts(SimulationState baseState, int pieceIndex, int playoutsPerMove)
		{
			if (!Helpers.ActivePlayerCanPurchasePiece(baseState, Helpers.GetNextPiece(baseState, pieceIndex)))
				return 0;

			int wins = 0;

			for (var i = 0; i < playoutsPerMove; i++)
			{
				_simulationState.Pieces.Clear();
				baseState.CloneTo(_simulationState);
				_simulationState.Fidelity = SimulationFidelity.NoPiecePlacing;

				_simulationState.PerformPurchasePiece(_simulationState.NextPieceIndex + pieceIndex);
				
				//Run the game
				for (var move = 1; move < _depth && !_simulationState.GameHasEnded; move++)
				{
					_randomMoveMaker.MakeMove(_simulationState);
				}

				var winner = EvaluateWinningPlayer(_simulationState);
				if (winner == baseState.ActivePlayer)
					wins++;
			}

			return wins;
		}

		private int EvaluateWinningPlayer(SimulationState state)
		{
			if (state.GameHasEnded)
				return state.WinningPlayer;

			//space used * 2 + buttons + income * incomes remaining
			//TODO? This doesn't really value incomes at the start of the game, we need deep playouts for that
			var worth0 = Helpers.EstimateEndgameValue(state, 0);
			var worth1 = Helpers.EstimateEndgameValue(state, 1);

			if (worth0 == worth1)
				return -100; //No one wins (TODO: There is a rule for who wins in a tie)
			if (worth0 > worth1)
				return 0;
			return 1;
		}
	}
}
