using System;
using System.Text;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators
{
	public class TuneablePattern2x2BoardEvaluator : IBoardEvaluator
	{
		public static readonly TuneablePattern2x2BoardEvaluator Tuning1 = new TuneablePattern2x2BoardEvaluator(new[]   { 82, -36, -33, -23, 11, -36, -71, 10, -42, -63, -81, 55, -39, -10, 60, 99 }, "Tuning1");
		public static readonly TuneablePattern2x2BoardEvaluator HandTuned = new TuneablePattern2x2BoardEvaluator(new[] {  0,   0,   0,   0,  0,   0,  -5,  0,   0,  -5,   5,  0,   5,   0,  0, 10 }, "HandTuned");

		private readonly int[] _weights;
		private readonly string _name;

		public string Name
		{
			get
			{
				if (_name != null)
					return $"TuneablePattern2x2BoardEvaluator({_name})";

				var sb = new StringBuilder();
				sb.Append("TuneablePattern2x2BoardEvaluator(");

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

		public TuneablePattern2x2BoardEvaluator(int[] weights, string name = null)
		{
			if (weights.Length != 16)
				throw new Exception("Expected 16 weights");
			_weights = weights;
			_name = name;
		}

		public void BeginEvaluation(BoardState currentBoard)
		{
		}

		public int Evaluate(in BoardState board, int minX, int maxX, int minY, int maxY)
		{
			int points = 0;

			for (var x = minX - 1; x < maxX; x++)
			{
				var bottomLeft = Read(in board, x, minY - 1);
				var bottomRight = Read(in board, x + 1, minY - 1);

				for (var y = minY - 1; y < maxY; y++)
				{
					var topLeft = bottomLeft;
					var topRight = bottomRight;

					bottomLeft = Read(in board, x, y + 1);
					bottomRight = Read(in board, x + 1, y + 1);

					var index = (topLeft ? 0b1000 : 0) | (topRight ? 0b0100 : 0) | (bottomLeft ? 0b0010 : 0) | (bottomRight ? 0b0001 : 0);

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
