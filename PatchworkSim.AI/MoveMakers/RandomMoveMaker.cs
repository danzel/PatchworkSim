using System;

namespace PatchworkSim.AI.MoveMakers;

/// <summary>
/// Enumerates all possible moves and randomly picks one of them
/// </summary>
public class RandomMoveMaker : IMoveDecisionMaker
{
	private readonly Random _random;

	public RandomMoveMaker(int? randomSeed = null)
	{
		_random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
	}

	public void MakeMove(SimulationState state)
	{
		bool canPurchase0 = Helpers.ActivePlayerCanPurchasePiece(state, Helpers.GetNextPiece(state, 0));
		bool canPurchase1 = Helpers.ActivePlayerCanPurchasePiece(state, Helpers.GetNextPiece(state, 1));
		bool canPurchase2 = Helpers.ActivePlayerCanPurchasePiece(state, Helpers.GetNextPiece(state, 2));

		int choices = 1 + (canPurchase0 ? 1 : 0) + (canPurchase1 ? 1 : 0) + (canPurchase2 ? 1 : 0);

		var choice = _random.Next(0, choices);

		if (choice == 0)
		{
			state.PerformAdvanceMove();
			return;
		}
		choice--;

		if (canPurchase0)
		{
			if (choice == 0)
			{
				state.PerformPurchasePiece(state.NextPieceIndex + 0);
				return;
			}
			choice--;
		}

		if (canPurchase1)
		{
			if (choice == 0)
			{
				state.PerformPurchasePiece(state.NextPieceIndex + 1);
				return;
			}
			choice--;
		}

		state.PerformPurchasePiece(state.NextPieceIndex + 2);
	}

	public string Name => "Random";
}
