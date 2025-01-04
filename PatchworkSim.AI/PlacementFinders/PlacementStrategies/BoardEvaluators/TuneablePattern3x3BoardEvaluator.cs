using System;
using System.Text;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators;

public class TuneablePattern3x3BoardEvaluator : IBoardEvaluator
{
	//2721 for 200 evaluations
	public static readonly TuneablePattern3x3BoardEvaluator Tuning1 = new TuneablePattern3x3BoardEvaluator(new[] { 100, -73, -24, -47, -100, -14, -100, -39, -46, 52, -18, 71, 93, 53, -93, 96, -100, -28, 94, -49, 21, -100, -100, 9, -49, -58, 9, 25, 55, -27, 43, 47, 11, 24, -5, -67, -90, 23, -49, 100, -14, -25, 2, -84, -46, 41, -60, 84, -46, -43, -20, -37, 27, -29, 76, -82, -22, -32, -59, 25, 12, -83, -20, 47, 99, 47, 100, 31, -40, -100, -64, 100, -10, 34, 44, 100, 2, -12, 67, -95, 6, 55, 71, -38, 82, 47, -9, -79, -10, 83, -20, 90, -19, 38, -59, -4, 62, 75, -39, 11, 60, -86, -30, -57, -63, -9, 72, -34, -64, -85, -2, -27, 89, 5, -14, 91, 97, 28, -40, 3, 66, -51, -59, 87, 89, -60, -64, 100, 27, 100, 19, -42, 36, 92, 46, 14, -54, -53, 43, -72, -81, 10, -82, 9, 2, -74, -13, 44, -45, 93, -11, 12, 27, 34, -77, 73, -40, 15, 55, 37, -100, -58, -29, 40, -30, 63, -73, -45, 96, -30, 90, 79, 81, -13, 38, 57, -80, 87, 19, -73, -26, -55, 24, -50, 0, 21, -41, 11, 0, -49, -83, -14, -11, -10, -36, 65, -63, 100, -86, -93, -77, 66, -54, 14, -100, 39, -34, -100, -50, -50, -36, -47, -54, 100, -61, -28, 1, 94, -72, 3, -100, -84, -55, 100, 90, -54, -14, -68, -59, 100, 96, 100, -82, -100, -54, -81, 11, -85, 49, -94, -62, -79, -10, -88, 25, -60, -23, 45, -85, -100, -2, 47, -39, -38, -60, 100, 29, -26, 61, -80, 91, -93, 23, 58, 45, 74, 74, -75, -82, 25, 14, 10, -92, 50, 32, -25, 0, -63, 67, 25, 87, -63, 58, -100, -87, 80, 12, -41, 100, -71, 41, -76, -16, -55, -21, -70, -56, -15, 79, 19, -94, -4, -30, 49, -100, -46, 1, -10, -29, -14, 1, 77, 23, -1, -6, -74, -52, -44, -29, 99, -32, -69, 27, -71, 79, -75, -65, -71, -68, -14, 48, 87, -82, -23, -92, -84, 9, 88, 37, -48, 25, -45, -100, -45, 26, 63, -100, 33, -73, -69, 81, 1, 5, -53, 88, 5, -69, -24, -24, 5, 9, 40, -100, -60, -35, 17, 100, -86, -100, -75, -52, -100, -19, -46, -71, 100, -12, -64, -92, -22, -55, -38, -24, 96, 60, -81, -96, 37, -54, -84, 67, -85, -42, -58, -25, -26, -9, 17, 19, -78, -100, 61, -45, -80, -57, -41, 100, -100, 27, 28, -11, -70, -71, 13, -51, 59, 17, -18, -30, 16, 33, 19, 73, -23, -25, -83, 100, 36, -69, -66, -59, -60, -75, -10, -57, 82, 65, -53, 82, 84, -85, -6, -62, 24, -95, 10, 69, 73, 29, -7, 64, -37, -30, 14, 25, -46, 31, -46, 24, 4, 45, -5, 31, -64, 64, 70, -93, -38, -60, 100, -93, 70, 100, 57, -86, 100, -74, -16, -66, 100, 84, -16, 21, 54, 74, 55, 92, -85, 19, 62, -28, -75, 54, -79, -18, -10, -47, 24, 87, -42, 40, 61, 5, 90, 8, -17, -70, 27, -30, 19, -5, 100 }, "Tuning1");

	private readonly int[] _weights;
	private readonly string _name;

	public string Name
	{
		get
		{
			if (_name != null)
				return $"TuneablePattern3x3BoardEvaluator({_name})";

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

	public TuneablePattern3x3BoardEvaluator(int[] weights, string name = null)
	{
		if (weights.Length != 512)
			throw new Exception("Expected 512 weights");
		_weights = weights;
		_name = name;
	}

	public void BeginEvaluation(BoardState currentBoard)
	{
	}	

	public int Evaluate(in BoardState board, int minX, int maxX, int minY, int maxY)
	{
		int points = 0;

		for (var x = minX - 2; x < maxX; x++)
		{
			var bottomLeft = Read(in board, x, minY - 2);
			var bottomMid = Read(in board, x + 1, minY - 2);
			var bottomRight = Read(in board, x + 2, minY - 2);

			var midLeft = Read(in board, x, minY - 1);
			var midMid = Read(in board, x + 1, minY - 1);
			var midRight = Read(in board, x + 2, minY - 1);

			for (var y = minY - 2; y < maxY; y++)
			{
				var topLeft = midLeft;
				var topMid = midMid;
				var topRight = midRight;

				midLeft = bottomLeft;
				midMid = bottomMid;
				midRight = bottomRight;

				bottomLeft = Read(in board, x, y + 1);
				bottomMid = Read(in board, x + 1, y + 1);
				bottomRight = Read(in board, x + 2, y + 1);

				var index =
					(topLeft ? 0b100_000_000 : 0) |
					(topMid ? 0b010_000_000 : 0) |
					(topRight ? 0b001_000_000 : 0) |
					(midLeft ? 0b100_000 : 0) |
					(midMid ? 0b010_000 : 0) |
					(midRight ? 0b001_000 : 0) |
					(bottomLeft ? 0b100 : 0) |
					(bottomMid ? 0b010 : 0) |
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