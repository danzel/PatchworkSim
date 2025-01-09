using PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies;

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
	private readonly bool _calculatePredictions;

	private readonly Queue<Preplacement> _preplacements = new Queue<Preplacement>(2);

	private List<PieceDefinition[]>? _allPlannedFuturePieces;
	private List<PieceDefinition[]>? _allActualPieces;

	public PreplacerStrategy(IPreplacer preplacer, bool calculatePredictions = false)
	{
		_preplacer = preplacer;
		_calculatePredictions = calculatePredictions;
	}

	public void PreparePlacePiece(BoardState board, List<PieceDefinition> plannedFuturePieces, int preplaceAmount)
	{
		if (_preplacements.Count > 0)
			throw new Exception("Asked to prepare when we haven't used all of our previously prepared placements");

		//Console.WriteLine($"Considering placing {preplaceAmount} pieces, with {plannedFuturePieces.Count} to look at: {string.Join(", ", plannedFuturePieces.Select(s => s.Name))}");

		if (_calculatePredictions)
			RecordPredictions(plannedFuturePieces, preplaceAmount);

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

	public bool TryPlacePiece(BoardState board, PieceDefinition piece, in PieceCollection possibleFuturePieces, int possibleFuturePiecesOffset, out PieceBitmap bitmap, out int x, out int y)
	{
		if (_preplacements.Count == 0)
			throw new Exception("Have no next move preplaced");

		var placement = _preplacements.Dequeue();

		bitmap = placement.Bitmap;
		x = placement.X;
		y = placement.Y;

		return true;
	}

	private void RecordPredictions(List<PieceDefinition> plannedFuturePieces, int preplaceAmount)
	{
		if (_allPlannedFuturePieces == null)
		{
			_allPlannedFuturePieces = new List<PieceDefinition[]>();
			_allActualPieces = new List<PieceDefinition[]>();
		}

		_allPlannedFuturePieces.Add(plannedFuturePieces.Skip(preplaceAmount).ToArray());
		_allActualPieces!.Add(plannedFuturePieces.Take(preplaceAmount).ToArray());
	}

	public void PrintPredictionAccuracy()
	{
		var allActual = _allActualPieces!.SelectMany(x => x).ToArray();
		var allActualIndex = _allActualPieces![0].Length; //Skip these cause they were placed before the first prediction

		int totalCorrect = 0;
		int totalIncorrect = 0;

		for (var i = 0; i < _allPlannedFuturePieces!.Count; i++)
		{
			if (i < _allActualPieces.Count)
				Console.WriteLine($"Purchased {i.ToString().PadLeft(2)}: " + String.Join(", ", _allActualPieces[i].Select(p => p.Name)));

			//See how many guesses were correct
			var prediction = _allPlannedFuturePieces[i];
			Console.WriteLine("Predicted " + String.Join(", ", prediction.Select(p => p.Name)));

			int correct = 0;
			int incorrect = 0;

			//Foreach of our individual predictions
			for (var j = 0; j < prediction.Length; j++)
			{
				//If there is an actual purchase in range
				if (allActual.Length >= allActualIndex + j)
				{
					if (prediction[j] == allActual[allActualIndex + j])
						correct++;
					else
						incorrect++;
				}
				else
				{
					//We predicted we'd buy one more than we did
					incorrect++;
				}
			}

			Console.WriteLine($"{i.ToString().PadLeft(2)} : {correct} / {incorrect}");
			totalCorrect += correct;
			totalIncorrect += incorrect;

			if (i + 1 < _allActualPieces.Count)
				allActualIndex += _allActualPieces[i + 1].Length;
		}

		Console.WriteLine($"Tot: {totalCorrect} / {totalIncorrect}");
	}
}