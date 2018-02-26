using System;
using System.Text;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators
{
	public class TuneablePattern3x3BoardEvaluator : IBoardEvaluator
	{
		private readonly int[] _weights;

		public string Name
		{
			get
			{
				var sb = new StringBuilder();
				sb.Append("TuneablePattern3x3BoardEvaluator(");

				for (var i = 0; i < _weights.Length; i++)
				{
					if (i > 0)
						sb.Append('|');
					sb.Append(_weights[i]);
				}

				sb.Append(')');
				return sb.ToString();
			}
		}

		public TuneablePattern3x3BoardEvaluator(int[] weights)
		{
			if (weights.Length != 512)
				throw new Exception("Expected 512 weights");
			_weights = weights;
		}

		public void BeginEvaluation(BoardState currentBoard)
		{
		}

		public int Evaluate(in BoardState board, int minX, int maxX, int minY, int maxY)
		{
			int points = 0;

			for (var x = minX - 2; x < maxX; x++)
			{
				var bottomLeft =  Read(in board, x    , minY - 2);
				var bottomMid =   Read(in board, x + 1, minY - 2);
				var bottomRight = Read(in board, x + 2, minY - 2);

				var midLeft =  Read(in board, x,     minY - 1);
				var midMid =   Read(in board, x + 1, minY - 1);
				var midRight = Read(in board, x + 2, minY - 1);

				for (var y = minY - 2; y < maxY; y++)
				{
					var topLeft =  midLeft;
					var topMid =   midMid;
					var topRight = midRight;

					midLeft =  bottomLeft;
					midMid =   bottomMid;
					midRight = bottomRight;

					bottomLeft =  Read(in board, x    , y + 1);
					bottomMid =   Read(in board, x + 1, y + 1);
					bottomRight = Read(in board, x + 2, y + 1);

					var index =
						(topLeft ?  0b100_000_000 : 0) |
						(topMid ?   0b010_000_000 : 0) |
						(topRight ? 0b001_000_000 : 0) |

						(midLeft ?  0b100_000 : 0) |
						(midMid ?   0b010_000 : 0) |
						(midRight ? 0b001_000 : 0) |

						(bottomLeft ?  0b100 : 0) | 
						(bottomMid ?   0b010 : 0) | 
						(bottomRight ? 0b001 : 0);

					points += _weights[index];
				}
			}

			return points;
		}

		private bool Read(in BoardState board, int x, int y)
		{
			if (x < 0 || y < 0 || x >= BoardState.Width || y >= BoardState.Height)
				return true;
			return board[x, y];
		}
	}
}
