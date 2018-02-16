using System;
using System.Collections.Generic;
using System.Linq;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	/// <summary>
	/// A strategy that plans where to place the piece before TryPlacePiece is called.
	/// You must call PreparePlacePiece with the piece to place and the piece we are planning to place after it,
	/// and we'll get our child strategy to calculate the best place to place it so they can be placed also.
	/// The child strategy should implement lookahead under the assumption that each next piece will need to be placed next
	/// </summary>
	public class PreplacerStrategy : IPlacementStrategy
	{
		public string Name => $"Preplacer({_preplacer.Name})";
		public bool ImplementsLookahead => true;

		private readonly IPreplacer _preplacer;

		private readonly Queue<Preplacement> _preplacements = new Queue<Preplacement>(2);

		public PreplacerStrategy(IPreplacer preplacer)
		{
			_preplacer = preplacer;
		}

		public void PreparePlacePiece(BoardState board, List<PieceDefinition> plannedFuturePieces, int preplaceAmount)
		{
			if (_preplacements.Count > 0)
				throw new Exception("Asked to prepare when we haven't used all of our previously prepared placements");

			//Console.WriteLine($"Considering placing {preplaceAmount} pieces, with {plannedFuturePieces.Count} to look at: {string.Join(", ", plannedFuturePieces.Select(s => s.Name))}");

			for (var i = 0; i < preplaceAmount; i++)
			{
				var preplacement = _preplacer.Preplace(board, plannedFuturePieces);
				_preplacements.Enqueue(preplacement);

				//Apply the preplacement (only if we are going to do another preplacement)
				if (i < preplaceAmount - 1)
				{
					plannedFuturePieces.RemoveAt(0);
					board.Place(preplacement.Bitmap, preplacement.X, preplacement.Y);
				}
			}
		}

		public bool TryPlacePiece(BoardState board, PieceDefinition piece, List<int> possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap bitmap, out int x, out int y)
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