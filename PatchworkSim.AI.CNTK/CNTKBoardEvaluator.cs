using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;

namespace PatchworkSim.AI.CNTK
{
	class CNTKNoLookaheadPlacementStrategy : NoLookaheadStrategy
	{
		public override string Name => "CNTK";

		private readonly BulkBoardEvaluator _boardEvaluator;
		private readonly List<BoardWithParent> _boards = new List<BoardWithParent>(BoardState.Width * BoardState.Height);
		private readonly List<Preplacement> _placements = new List<Preplacement>(BoardState.Width * BoardState.Height);

		public CNTKNoLookaheadPlacementStrategy(BulkBoardEvaluator boardEvaluator)
		{
			_boardEvaluator = boardEvaluator;
		}

		protected override bool TryPlacePiece(BoardState board, PieceDefinition piece, out PieceBitmap resultBitmap, out int resultX, out int resultY)
		{
			_boards.Clear();
			_placements.Clear();

			//Exhaustively place it and make new child nodes
			for (var index = 0; index < piece.PossibleOrientations.Length; index++)
			{
				var bitmap = piece.PossibleOrientations[index];
				var searchWidth = BoardState.Width - bitmap.Width + 1;
				var searchHeight = BoardState.Height - bitmap.Height + 1;

				//TODO? If this is the first piece, remove mirrors/rotations from the children
				//if (isFirstPiece)
				//{
				//	//TODO: This doesn't stop diagonal mirrors
				//	searchWidth = (BoardState.Width - bitmap.Width) / 2 + 1;
				//	searchHeight = (BoardState.Height - bitmap.Height) / 2 + 1;
				//}

				for (int x = 0; x < searchWidth; x++)
				{
					for (int y = 0; y < searchHeight; y++)
					{
						if (board.CanPlace(bitmap, x, y))
						{
							var clone = board;
							clone.Place(bitmap, x, y);
							_boards.Add(new BoardWithParent(null, clone));
							_placements.Add(new Preplacement(bitmap, x, y));
						}
					}
				}
			}

			_boardEvaluator.Evaluate(_boards);

			resultBitmap = null;
			resultX = -1;
			resultY = -1;

			float bestScore = -1;
			for (var i = 0; i < _boards.Count; i++)
			{
				if (_boards[i].Score > bestScore)
				{
					bestScore = _boards[i].Score;
					var p = _placements[i];
					resultBitmap = p.Bitmap;
					resultX = p.X;
					resultY = p.Y;
				}
			}

			return resultBitmap != null;
		}
	}
}
