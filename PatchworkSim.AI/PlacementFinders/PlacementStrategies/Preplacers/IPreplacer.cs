using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers
{
	public interface IPreplacer
	{
		string Name { get; }

		Preplacement Preplace(BoardState board, List<PieceDefinition> plannedFuturePieces);
	}
}
