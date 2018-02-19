using System.Linq;

namespace PatchworkSim
{
	public class PieceDefinition
	{
		public const int TotalPieces = 33;

		public string Name { get; }
		public int ButtonCost { get; }
		public int TimeCost { get; }
		public int ButtonsIncome { get; }

		public PieceBitmap Bitmap { get; }
		public int TotalUsedLocations { get; }

		public PieceBitmap[] PossibleOrientations { get; }

		public PieceDefinition(string name, int buttonCost, int timeCost, int buttonsIncome, string[] bitmap)
		{
			Name = name;
			ButtonCost = buttonCost;
			TimeCost = timeCost;
			ButtonsIncome = buttonsIncome;
			var boolmap = Parse(bitmap);
			Bitmap = new PieceBitmap(boolmap);
			TotalUsedLocations = BoolmapOps.SumUsed(boolmap);

			PossibleOrientations = BoolmapOps.CalculatePossibleOrientations(boolmap).Select(b => new PieceBitmap(b)).ToArray();
		}

		private bool[,] Parse(string[] bitmap)
		{
			var result = new bool[bitmap[0].Length, bitmap.Length];

			for (var y = 0; y < bitmap.Length; y++)
			for (var x = 0; x < bitmap[0].Length; x++)
				result[x, y] = bitmap[y][x] == '#';
			return result;
		}

		public static readonly PieceDefinition LeatherTile = new PieceDefinition("leather tile", 0, 0, 0, new[] { "#" });

		public static readonly PieceDefinition[] AllPieceDefinitions =
		{
			//This must be the first item in the list so we know where it is
			new PieceDefinition("2x1 line (starting piece)", 2, 1, 0,
				new[] { "##" }),

			new PieceDefinition("3x1 line", 2, 2, 0,
				new[] { "###" }),

			new PieceDefinition("4x1 line", 3, 3, 1,
				new[] { "####" }),

			new PieceDefinition("5x1 line", 7, 1, 1,
				new[] { "#####" }),


			new PieceDefinition("2x2 square", 6, 5, 2,
				new[]
				{
					"##",
					"##"
				}),

			new PieceDefinition("2x2 square (one stick out variant)", 2, 2, 0,
				new[]
				{
					"## ",
					"###"
				}),

			new PieceDefinition("2x2 square (two stick out variant)", 10, 5, 3,
				new[]
				{
					"##  ",
					"####"
				}),

			new PieceDefinition("2x2 square (one stick out each side variant)", 7, 4, 2,
				new[]
				{
					" ## ",
					"####"
				}),

			new PieceDefinition("2x2 square (one stick out each side alternating variant)", 4, 2, 0,
				new[]
				{
					"### ",
					" ###"
				}),

			new PieceDefinition("2x2 square (two under sideways variant)", 8, 6, 3,
				new[]
				{
					" ##",
					" ##",
					"## "
				}),


			new PieceDefinition("u", 1, 2, 0,
				new[]
				{
					"# #",
					"###"
				}),

			new PieceDefinition("u (wide variant)", 1, 5, 1,
				new[]
				{
					"#  #",
					"####"
				}),

			new PieceDefinition("Y", 3, 6, 2,
				new[]
				{
					"# #",
					"###",
					" # "
				}),


			new PieceDefinition("small T (1)", 2, 2, 0,
				new[]
				{
					"###",
					" # "
				}),

			new PieceDefinition("medium T (2)", 5, 5, 2,
				new[]
				{
					"###",
					" # ",
					" # "
				}),

			new PieceDefinition("long T (3)", 7, 2, 2,
				new[]
				{
					"###",
					" # ",
					" # ",
					" # "
				}),

			new PieceDefinition("free cross (medium t (2) with a double top)", 0, 3, 1,
				new[]
				{
					" # ",
					"###",
					" # ",
					" # "
				}),


			new PieceDefinition("L (low cost variant)", 4, 2, 1,
				new[]
				{
					"# ",
					"# ",
					"##"
				}),

			new PieceDefinition("L (high cost variant)", 4, 6, 2,
				new[]
				{
					"# ",
					"# ",
					"##"
				}),

			new PieceDefinition("L (long variant)", 10, 3, 2,
				new[]
				{
					"# ",
					"# ",
					"# ",
					"##"
				}),

			new PieceDefinition("L (one extra same line variant)", 3, 4, 1,
				new[]
				{
					"# ",
					"# ",
					"##",
					"# "
				}),


			new PieceDefinition("+", 5, 4, 2,
				new[]
				{
					" # ",
					"###",
					" # "
				}),

			new PieceDefinition("+ (long variant)", 1, 4, 1,
				new[]
				{
					" # ",
					" # ",
					"###",
					" # ",
					" # "
				}),

			new PieceDefinition("+ (double width variant)", 5, 3, 1,
				new[]
				{
					" ## ",
					"####",
					" ## "
				}),


			new PieceDefinition("H", 2, 3, 0,
				new[]
				{
					"# #",
					"###",
					"# #"
				}),


			new PieceDefinition("corner (button variant)", 3, 1, 0,
				new[]
				{
					" #",
					"##"
				}),

			new PieceDefinition("corner (time variant)", 1, 3, 0,
				new[]
				{
					" #",
					"##"
				}),


			new PieceDefinition("s (cheap variant)", 3, 2, 1,
				new[]
				{
					" #",
					"##",
					"# "
				}),

			new PieceDefinition("s (expensive variant)", 7, 6, 3,
				new[]
				{
					" #",
					"##",
					"# "
				}),

			new PieceDefinition("s (long variant)", 2, 3, 1,
				new[]
				{
					" #",
					" #",
					"##",
					"# "
				}),

			new PieceDefinition("s (wide variant)", 1, 2, 0,
				new[]
				{
					"   #",
					"####",
					"#   "
				}),

			new PieceDefinition("s (wings variant)", 2, 1, 0,
				new[]
				{
					"  # ",
					"####",
					" #  "
				}),


			new PieceDefinition("w", 10, 4, 3,
				new[]
				{
					"  #",
					" ##",
					"## "
				})
		};
	}
}