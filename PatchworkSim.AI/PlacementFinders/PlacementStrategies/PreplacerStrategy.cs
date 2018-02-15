using System;
using System.Collections.Generic;

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
		public string Name => $"Preplacer({_childStrategy.Name})";
		public bool ImplementsLookahead => _childStrategy.ImplementsLookahead;

		private readonly IPlacementStrategy _childStrategy;

		private bool _hasNext;
		private PieceBitmap _nextBitmap;
		private int _nextX;
		private int _nextY;

		public PreplacerStrategy(IPlacementStrategy childStrategy)
		{
			_childStrategy = childStrategy;
		}

		public void PreparePlacePiece(BoardState board, PieceDefinition piece, List<int> plannedFuturePieces)
		{
			if (_childStrategy.TryPlacePiece(board, piece, plannedFuturePieces, 0, out var bitmap, out var x, out var y))
			{
				_hasNext = true;
				_nextBitmap = bitmap;
				_nextX = x;
				_nextY = y;
			}
			else
			{
				throw new Exception("Child strategy cannot place piece");
			}
		}

		public bool TryPlacePiece(BoardState board, PieceDefinition piece, List<int> possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap bitmap, out int x, out int y)
		{
			if (!_hasNext)
				throw new Exception("Have no next move preplaced");
			_hasNext = false;

			bitmap = _nextBitmap;
			x = _nextX;
			y = _nextY;

			return true;
		}
	}
}