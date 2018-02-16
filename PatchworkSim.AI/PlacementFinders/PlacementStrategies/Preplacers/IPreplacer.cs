using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers
{
	/// <summary>
	/// A helper for PreplacerStrategy.
	/// Decides the best place to put the first piece of plannedFuturePieces, under the assumption that the remaining pieces in plannedFuturePieces will need to be placed next (they will be purchased next)
	/// </summary>
	public interface IPreplacer
	{
		string Name { get; }

		Preplacement Preplace(BoardState board, List<PieceDefinition> plannedFuturePieces);
	}
}
