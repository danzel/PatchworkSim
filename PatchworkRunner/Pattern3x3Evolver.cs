using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.BoardEvaluators;
using PatchworkSim.AI.PlacementFinders.PlacementStrategies.NoLookahead;
using Redzen.Random.Double;

namespace PatchworkRunner
{
	class Pattern3x3Evolver
	{
		private const int PopulationSize = 1000;
		private const int PopulationToKeep = 100;
		private readonly Random _random = new Random();
		private readonly ZigguratGaussianDistribution _zig = new ZigguratGaussianDistribution(0, 0, 50);

		List<PopulationMember> _population;
		const int MaxGeneration = 10_000;

		private const int MinValue = -100;
		private const int MaxValue = 100;

		private void GenerateInitialPopulation()
		{
			_population = new List<PopulationMember>();

			//2721
			_population.Add(new PopulationMember(new[] { 100, -73, -24, -47, -100, -14, -100, -39, -46, 52, -18, 71, 93, 53, -93, 96, -100, -28, 94, -49, 21, -100, -100, 9, -49, -58, 9, 25, 55, -27, 43, 47, 11, 24, -5, -67, -90, 23, -49, 100, -14, -25, 2, -84, -46, 41, -60, 84, -46, -43, -20, -37, 27, -29, 76, -82, -22, -32, -59, 25, 12, -83, -20, 47, 99, 47, 100, 31, -40, -100, -64, 100, -10, 34, 44, 100, 2, -12, 67, -95, 6, 55, 71, -38, 82, 47, -9, -79, -10, 83, -20, 90, -19, 38, -59, -4, 62, 75, -39, 11, 60, -86, -30, -57, -63, -9, 72, -34, -64, -85, -2, -27, 89, 5, -14, 91, 97, 28, -40, 3, 66, -51, -59, 87, 89, -60, -64, 100, 27, 100, 19, -42, 36, 92, 46, 14, -54, -53, 43, -72, -81, 10, -82, 9, 2, -74, -13, 44, -45, 93, -11, 12, 27, 34, -77, 73, -40, 15, 55, 37, -100, -58, -29, 40, -30, 63, -73, -45, 96, -30, 90, 79, 81, -13, 38, 57, -80, 87, 19, -73, -26, -55, 24, -50, 0, 21, -41, 11, 0, -49, -83, -14, -11, -10, -36, 65, -63, 100, -86, -93, -77, 66, -54, 14, -100, 39, -34, -100, -50, -50, -36, -47, -54, 100, -61, -28, 1, 94, -72, 3, -100, -84, -55, 100, 90, -54, -14, -68, -59, 100, 96, 100, -82, -100, -54, -81, 11, -85, 49, -94, -62, -79, -10, -88, 25, -60, -23, 45, -85, -100, -2, 47, -39, -38, -60, 100, 29, -26, 61, -80, 91, -93, 23, 58, 45, 74, 74, -75, -82, 25, 14, 10, -92, 50, 32, -25, 0, -63, 67, 25, 87, -63, 58, -100, -87, 80, 12, -41, 100, -71, 41, -76, -16, -55, -21, -70, -56, -15, 79, 19, -94, -4, -30, 49, -100, -46, 1, -10, -29, -14, 1, 77, 23, -1, -6, -74, -52, -44, -29, 99, -32, -69, 27, -71, 79, -75, -65, -71, -68, -14, 48, 87, -82, -23, -92, -84, 9, 88, 37, -48, 25, -45, -100, -45, 26, 63, -100, 33, -73, -69, 81, 1, 5, -53, 88, 5, -69, -24, -24, 5, 9, 40, -100, -60, -35, 17, 100, -86, -100, -75, -52, -100, -19, -46, -71, 100, -12, -64, -92, -22, -55, -38, -24, 96, 60, -81, -96, 37, -54, -84, 67, -85, -42, -58, -25, -26, -9, 17, 19, -78, -100, 61, -45, -80, -57, -41, 100, -100, 27, 28, -11, -70, -71, 13, -51, 59, 17, -18, -30, 16, 33, 19, 73, -23, -25, -83, 100, 36, -69, -66, -59, -60, -75, -10, -57, 82, 65, -53, 82, 84, -85, -6, -62, 24, -95, 10, 69, 73, 29, -7, 64, -37, -30, 14, 25, -46, 31, -46, 24, 4, 45, -5, 31, -64, 64, 70, -93, -38, -60, 100, -93, 70, 100, 57, -86, 100, -74, -16, -66, 100, 84, -16, 21, 54, 74, 55, 92, -85, 19, 62, -28, -75, 54, -79, -18, -10, -47, 24, 87, -42, 40, 61, 5, 90, 8, -17, -70, 27, -30, 19, -5, 100 }));

			for (var i = _population.Count; i < PopulationSize; i++)
			{
				var populationMember = new PopulationMember(CreateRandomGene());
				_population.Add(populationMember);
			}

			Parallel.For(0, PopulationSize, i => EvaluateFitness(_population[i]));
		}

		public void Run()
		{
			var generation = 0;
			var lastImprovedGeneration = 0;
			var lastBestFitness = 0;
			GenerateInitialPopulation();

			while (true)
			{
				_population.Sort();

				//if (generation % 100 == 0)
				Console.WriteLine($"Generation {generation}. Fitness Range: {_population[0].Fitness} -- {_population[PopulationSize - 1].Fitness}");

				if (_population[0].Fitness > lastBestFitness)
				{
					lastBestFitness = _population[0].Fitness;
					Console.WriteLine($"{lastBestFitness} {_population[0].Strategy.Name}");
					lastImprovedGeneration = generation - 1;
				}

				//Do the genetic thing.
				//Keep the top PopulationToKeep

				//TODO: This can be parallel too (If rand is threadsafe...)
				for (var i = PopulationToKeep; i < PopulationSize; i++)
				{
					var parent = _population[_random.Next(0, PopulationToKeep)];

					//Mutate from the parent
					var target = _population[i];
					var gene = target.Gene;

					for (var j = 0; j < PopulationMember.GeneSize; j++)
					{
						gene[j] = Clamp(parent.Gene[j] + GaussianRand());
					}

					target.CreateMoveMaker();
				}

				//Evaluate them all
				Parallel.For(PopulationToKeep, PopulationSize, i => EvaluateFitness(_population[i]));

				generation++;

				//Hit the max or went 500 without improvement
				if (generation == MaxGeneration || (generation >= lastImprovedGeneration + 500))
				{
					File.AppendAllLines($"best-3x3-{lastBestFitness}-{DateTimeOffset.UtcNow.Ticks}.txt", new[] { $"{lastBestFitness} {_population[0].Strategy.Name}" });
					generation = 0;
					lastImprovedGeneration = 0;
					lastBestFitness = 0;
					GenerateInitialPopulation();
					//return;
				}
			}
		}

		private int GaussianRand()
		{
			return (int)_zig.Sample();
		}

		private void EvaluateFitness(PopulationMember populationMember)
		{
			populationMember.Fitness = 1 + CalculateChallengerWinsFrom100(populationMember.Strategy);
		}

		private int[] CreateRandomGene()
		{
			var res = new int[PopulationMember.GeneSize];
			for (var i = 0; i < PopulationMember.GeneSize; i++)
				res[i] = _random.Next(MinValue, 1 + MaxValue);
			return res;
		}

		private const int GameCount = 200;

		private static readonly List<PieceDefinition>[] RandomPiecesCache = Enumerable.Range(0, GameCount).Select(i => SimulationHelpers.GetRandomPieces(i).Select(p => PieceDefinition.AllPieceDefinitions[p]).ToList()).ToArray();

		/// <summary>
		/// Returns the amount of wins the challenger got (from 100)
		/// </summary>
		private static int CalculateChallengerWinsFrom100(NoLookaheadStrategy strategy)
		{
			int totalPlacements = 0;

			PieceCollection empty = new PieceCollection();

			//Parallel.For(0, GameCount, (i) =>
			for (var i = 0; i < GameCount; i++)
			{
				var pieces = RandomPiecesCache[i];

				var board = new BoardState();
				var pieceIndex = 0;
				while (strategy.TryPlacePiece(board, pieces[pieceIndex], in empty, 0, out var bitmap, out var x, out var y))
				{
					board.Place(bitmap, x, y);

					pieceIndex++;
				}

				totalPlacements += pieceIndex; //Interlocked.Add(ref totalPlacements, pieceIndex);
			}

			return totalPlacements;
		}

		/// <summary>
		/// Clamp between MinValue and MaxValue inclusive
		/// </summary>
		private int Clamp(int value)
		{
			return value < MinValue ? MinValue : value > MaxValue ? MaxValue : value;
		}


		class PopulationMember : IComparable<PopulationMember>
		{
			public const int GeneSize = 512;

			public readonly int[] Gene;
			public BestEvaluatorStrategy Strategy;

			public int Fitness;

			public PopulationMember(int[] gene)
			{
				Gene = gene;
				CreateMoveMaker();
			}

			public void CreateMoveMaker()
			{
				//TODO: Garbage generation here
				Strategy = new BestEvaluatorStrategy(new TuneablePattern3x3BoardEvaluator(Gene));
			}

			public int CompareTo(PopulationMember other)
			{
				if (ReferenceEquals(this, other)) return 0;
				if (ReferenceEquals(null, other)) return 1;
				return other.Fitness - Fitness; //Fitness.CompareTo(other.Fitness);
			}
		}
	}
}