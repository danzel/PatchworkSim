using System;
using System.Linq;
using PatchworkSim;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;
using PatchworkSim.Loggers;
using Xunit;

namespace PathworkSim.Test;

public class SevenXSevenTest
{
	[Fact]
	void Test()
	{
		var pieceIndexes = new[]
		{
			"L (long variant)",
			"s (cheap variant)",
			"s (long variant)",
			"u",
			"2x2 square (one stick out variant)",
			"L (high cost variant)",
			"L (low cost variant)",
			"+ (double width variant)",
			"small T (1)",
			"L (one extra same line variant)",

			//We don't use this one
			"2x1 line (starting piece)"
		}.Select(IndexOfNamedPiece).ToArray();
		var pieces = pieceIndexes.Select(p => PieceDefinition.AllPieceDefinitions[p]).ToArray();

		var s = new SimulationState(pieceIndexes.ToList(), 0);

		s.PlayerButtonAmount[0] = 9999;

		var ps = new[]
		{
			new Preplacement(pieces[0].PossibleOrientations[4], 0, 0),
			new Preplacement(pieces[1].PossibleOrientations[3], 3, 0),
			new Preplacement(pieces[2].PossibleOrientations[4], 1, 1),
			new Preplacement(pieces[3].PossibleOrientations[2], 0, 2),
			new Preplacement(pieces[4].PossibleOrientations[3], 1, 3),
			new Preplacement(pieces[5].PossibleOrientations[5], 0, 5),
			new Preplacement(pieces[6].PossibleOrientations[7], 4, 5),
			new Preplacement(pieces[7].PossibleOrientations[1], 4, 2),
			new Preplacement(pieces[8].PossibleOrientations[2], 5, 0),
			new Preplacement(pieces[9].PossibleOrientations[6], 1, 5),

		};
		for (var i = 0; i < ps.Length; i++)
		{
			Assert.Null(s.SevenXSevenBonusPlayer);

			var p = ps[i];
			//Player 0 buys and places
			s.PerformPurchasePiece(0);
			s.PerformPlacePiece(p.Bitmap, p.X, p.Y);

			//We got a leather tile
			if (s.PieceToPlace != null)
				s.PerformPlacePiece(s.PieceToPlace.PossibleOrientations[0], 8, 8);

			if (i > 6)
				ConsoleLogger.PrintBoard(s.PlayerBoardState[0]);

			//Player 1 does nothing
			s.PerformAdvanceMove();
			//We got a leather tile
			if (s.PieceToPlace != null)
				s.PerformPlacePiece(s.PieceToPlace.PossibleOrientations[0], 8, 8);
		}

		Assert.Equal(0, s.SevenXSevenBonusPlayer);
	}

	private static int IndexOfNamedPiece(string name)
	{
		for (var i = 0; i < PieceDefinition.AllPieceDefinitions.Length; i++)
		{
			if (PieceDefinition.AllPieceDefinitions[i].Name == name)
				return i;
		}

		throw new Exception();
	}
}
