using System.Collections.Generic;

namespace PatchworkSim
{
	public class PieceDefinition
	{
		public string Name { get; }
		public int ButtonCost { get; }
		public int TimeCost { get; }
		public int ButtonsIncome { get; }

		public bool[,] Bitmap { get; }
		public int TotalUsedLocations { get; }

		public bool[][,] PossibleOrientations { get; }

		public PieceDefinition(string name, int buttonCost, int timeCost, int buttonsIncome, bool[,] bitmap)
		{
			Name = name;
			ButtonCost = buttonCost;
			TimeCost = timeCost;
			ButtonsIncome = buttonsIncome;
			Bitmap = bitmap;
			TotalUsedLocations = BitmapOps.SumUsed(bitmap);

			PossibleOrientations = BitmapOps.CalculatePossibleOrientations(bitmap);
		}

		public static readonly PieceDefinition[] AllPieceDefinitions =
		{
			new PieceDefinition("2x1 line (starting piece)", 2, 1, 0,
				new[,] { { true, true } }),

			new PieceDefinition("3x1 line", 2, 2, 0,
				new[,] { { true, true, true } }),

			new PieceDefinition("4x1 line", 3, 3, 1,
				new[,] { { true, true, true, true } }),

			new PieceDefinition("5x1 line", 7, 1, 1,
				new[,] { { true, true, true, true, true } }),


			new PieceDefinition("2x2 square", 6, 5, 2,
				new[,]
				{
					{ true, true },
					{ true, true }
				}),

			new PieceDefinition("2x2 square (one stick out variant)", 2, 2, 0,
				new[,]
				{
					{ true, true, false },
					{ true, true, true }
				}),

			new PieceDefinition("2x2 square (two stick out variant)", 10, 5, 3,
				new[,]
				{
					{ true, true, false, false },
					{ true, true, true, true }
				}),

			new PieceDefinition("2x2 square (one stick out each side variant)", 7, 4, 2,
				new[,]
				{
					{ false, true, true, false },
					{ true, true, true, true }
				}),

			new PieceDefinition("2x2 square (one stick out each side alternating variant)", 4, 2, 0,
				new[,]
				{
					{ true, true, true, false },
					{ false, true, true, true }
				}),

			new PieceDefinition("2x2 square (two under sideways variant)", 8, 6, 3,
				new[,]
				{
					{ false, true, true },
					{ false, true, true },
					{ true, true, false }
				}),


			new PieceDefinition("u", 1, 2, 0,
				new[,]
				{
					{ true, false, true },
					{ true, true, true }
				}),

			new PieceDefinition("u (wide variant)", 1, 5, 1,
				new[,]
				{
					{ true, false, false, true },
					{ true, true, true, true }
				}),

			new PieceDefinition("Y", 3, 6, 2,
				new[,]
				{
					{ true, false, true },
					{ true, true, true },
					{ false, true, false }
				}),


			new PieceDefinition("small T (1)", 2, 2, 0,
				new[,]
				{
					{ false, true, false },
					{ true, true, true }
				}),

			new PieceDefinition("medium T (2)", 5, 5, 2,
				new[,]
				{
					{ false, true, false },
					{ false, true, false },
					{ true, true, true }
				}),

			new PieceDefinition("long T (3)", 7, 2, 2,
				new[,]
				{
					{ false, true, false },
					{ false, true, false },
					{ false, true, false },
					{ true, true, true }
				}),

			new PieceDefinition("free cross (medium t (2) with a double top)", 0, 3, 1,
				new[,]
				{
					{ false, true, false },
					{ false, true, false },
					{ true, true, true },
					{ false, true, false }
				}),


			new PieceDefinition("L (low cost variant)", 4, 2, 1,
				new[,]
				{
					{ false, false, true },
					{ true, true, true }
				}),

			new PieceDefinition("L (high cost variant)", 4, 6, 2,
				new[,]
				{
					{ false, false, true },
					{ true, true, true }
				}),

			new PieceDefinition("L (long variant)", 10, 3, 2,
				new[,]
				{
					{ false, false, false, true },
					{ true, true, true, true }
				}),

			new PieceDefinition("L (one extra same line variant)", 3, 4, 1,
				new[,]
				{
					{ false, false, true, false },
					{ true, true, true, true }
				}),


			new PieceDefinition("+", 5, 4, 2,
				new[,]
				{
					{ false, true, false },
					{ true, true, true },
					{ false, true, false }
				}),

			new PieceDefinition("+ (long variant)", 1, 4, 1,
				new[,]
				{
					{ false, false, true, false, false },
					{ true, true, true, true, true },
					{ false, false, true, false, false }
				}),

			new PieceDefinition("+ (double width variant)", 5, 3, 1,
				new[,]
				{
					{ false, true, true, true, false },
					{ true, true, true, true, true },
					{ false, true, true, true, false }
				}),


			new PieceDefinition("H", 2, 3, 0,
				new[,]
				{
					{ true, false, true },
					{ true, true, true },
					{ true, false, true }
				}),


			new PieceDefinition("corner (button variant)", 3, 1, 0,
				new[,]
				{
					{ false, true },
					{ true, true }
				}),

			new PieceDefinition("corner (time variant)", 1, 3, 0,
				new[,]
				{
					{ false, true },
					{ true, true }
				}),


			new PieceDefinition("s (cheap variant)", 3, 2, 1,
				new[,]
				{
					{ false, true },
					{ true, true },
					{ true, false }
				}),

			new PieceDefinition("s (expensive variant)", 7, 6, 3,
				new[,]
				{
					{ false, true },
					{ true, true },
					{ true, false }
				}),

			new PieceDefinition("s (long variant)", 2, 3, 1,
				new[,]
				{
					{ false, true },
					{ false, true },
					{ true, true },
					{ true, false }
				}),

			new PieceDefinition("s (wide variant)", 1, 2, 0,
				new[,]
				{
					{ false, false, false, true },
					{ true, true, true, true },
					{ true, false, false, false }
				}),

			new PieceDefinition("s (wings variant)", 2, 1, 0,
				new[,]
				{
					{ false, false, true, false },
					{ true, true, true, true },
					{ false, true, false, false }
				}),


			new PieceDefinition("w", 10, 4, 3,
				new[,]
				{
					{ false, false, true },
					{ false, true, true },
					{ true, true, false }
				})
		};
	}
}