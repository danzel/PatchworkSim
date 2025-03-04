﻿using PatchworkSim;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;
using Xunit;

namespace PathworkSim.Test;

public class PlacementStrategyTests
{
	[Fact]
	public void FirstPossible()
	{
		TestStrategy(FirstPossiblePlacementStrategy.Instance, 12);
	}

	[Fact]
	public void SimpleClosestToWallAndCorner()
	{
		TestStrategy(SimpleClosestToWallAndCornerStrategy.Instance, 13);
	}

	[Fact]
	public void SmallestBoundingBox()
	{
		TestStrategy(SmallestBoundingBoxPlacementStrategy.Instance, 12);
	}

	[Fact]
	public void ClosestToCornerLeastHolesTieBreaker()
	{
		TestStrategy(ClosestToCornerLeastHolesTieBreakerPlacementStrategy.Instance, 12);
	}

	[Fact]
	public void NextToPieceEdgeLeastHolesTieBreaker()
	{
		TestStrategy(NextToPieceEdgeLeastHolesTieBreakerPlacementStrategy.Instance, 15);
	}

	[Fact]
	public void TightPlacement()
	{
		TestStrategy(TightPlacementStrategy.InstanceIncrement, 15);
	}

	[Fact]
	public void ExhaustiveMostFuturePlacements()
	{
		TestStrategy(ExhaustiveMostFuturePlacementsPlacementStrategy.Instance1_1, 13);
	}

	private void TestStrategy(IPlacementStrategy strategy, int expectPiecesPlaced)
	{
		var pieces = new PieceCollection();
		pieces.Populate(SimulationHelpers.GetRandomPieces(1));
		int placed = 0;

		var board = new BoardState();

		for(var i = 0; i < pieces.Count; i++)
		{
			if (strategy.TryPlacePiece(board, PieceDefinition.AllPieceDefinitions[pieces[i]], in pieces, i + 1, out var bitmap, out var x, out var y))
			{
				placed++;
				board.Place(bitmap, x, y);
			}
			else
			{
				break;
			}
		}

		//This number totally relies on the order of pieces in the array.
		//Update it if the ordering changes, the exact amount isn't important
		Assert.Equal(expectPiecesPlaced, placed);
	}
}
