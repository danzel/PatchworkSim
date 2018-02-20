using System;
using System.Collections.Generic;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	/// <summary>
	/// A strategy that does no placement itself, the MoveMaker needs to tell it where to place pieces.
	/// </summary>
	public class StoredPlacementStrategy : IPlacementStrategy
	{
		public string Name => "Stored";

		public bool ImplementsLookahead => false;

		private readonly Queue<Preplacement> _preplacements = new Queue<Preplacement>(2);

		public void EnqueuePlacement(Preplacement placement)
		{
			_preplacements.Enqueue(placement);
		}

		public bool TryPlacePiece(BoardState board, PieceDefinition piece, in PieceCollection possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap bitmap, out int x, out int y)
		{
			if (_preplacements.Count == 0)
				throw new Exception("Have no next move preplaced");

			var placement = _preplacements.Dequeue();

			bitmap = placement.Bitmap;
			x = placement.X;
			y = placement.Y;

			return true;
		}
	}
}
