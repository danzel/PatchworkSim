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

namespace PatchworkRunner
{
	class Pattern2x2Evolver
	{
		private const int PopulationSize = 60;
		private readonly Random _random = new Random();
		List<PopulationMember> _population;
		const int MaxGeneration = 10_000;

		private const int MinValue = -100;
		private const int MaxValue = 100;

		private void GenerateInitialPopulation()
		{
			_population = new List<PopulationMember>();

			//Best 4 I've found so far
			//2754
			_population.Add(new PopulationMember(new[] { 82, -36, -33, -23, 11, -36, -71, 10, -42, -63, -81, 55, -39, -10, 60, 99 }));
			//2753
			_population.Add(new PopulationMember(new[] { 74, -3, -64, -53, 11, -91, -76, -93, -100, -84, -100, 35, -82, -26, 50, 100 }));
			//2751
			_population.Add(new PopulationMember(new[] { 67, -74, -29, -56, -12, -57, -96, -10, -52, -89, -100, 29, -67, -3, 39, 100 }));
			//2749
			_population.Add(new PopulationMember(new[] { 50, -36, -45, -66, -79, -52, -92, -18, -49, -100, -100, 19, -42, -1, 19, 75 }));
			foreach (var p in _population)
				EvaluateFitness(p);

			for (var i = _population.Count; i < PopulationSize; i++)
			{
				var populationMember = new PopulationMember(CreateRandomGene());
				EvaluateFitness(populationMember);
				_population.Add(populationMember);
			}
		}

		public void Run()
		{
			var generation = 0;
			var lastBestFitness = 0;
			GenerateInitialPopulation();

			while (true)
			{
				_population.Sort();

				if (generation % 100 == 0)
					Console.WriteLine($"Generation {generation}. Fitness Range: {_population[0].Fitness} -- {_population[PopulationSize - 1].Fitness}");

				if (_population[0].Fitness > lastBestFitness)
				{
					lastBestFitness = _population[0].Fitness;
					Console.WriteLine(_population[0].Strategy.Name);
				}

				//Do the genetic thing.
				//Replace the quarter with new versions based on the best ones
				int fitnessSum = 0;
				for (var i = 0; i < (3 * PopulationSize / 4); i++)
				{
					var p = _population[i];
					fitnessSum += p.Fitness;
				}

				for (var i = 0; i < PopulationSize / 4; i++)
				{
					var index0 = randomPick(fitnessSum);
					var index1 = randomPick(fitnessSum);

					var parent0 = _population[index0];
					var parent1 = _population[index1];

					//Crossover and Mutation
					var target = _population[PopulationSize - 1 - i];
					var gene = target.Gene;

					for (var j = 0; j < PopulationMember.GeneSize; j++)
					{
						//TODO: Could use random more efficiently
						if (_random.NextDouble() < 0.5)
							gene[j] = (_random.NextDouble() < 0.5 ? parent0 : parent1).Gene[j]; //Crossover
						else
							gene[j] = Clamp(gene[j] + _random.Next(MinValue / 5, 1 + MaxValue / 5)); //Mutation
					}

					target.CreateMoveMaker();
					EvaluateFitness(_population[PopulationSize - 1 - i]);
				}

				generation++;

				if (generation == MaxGeneration)
				{
					File.AppendAllLines($"best-{lastBestFitness}-{DateTimeOffset.UtcNow.Ticks}.txt", new[] { $"{lastBestFitness} {_population[0].Strategy.Name}" });
					generation = 0;
					lastBestFitness = 0;
					GenerateInitialPopulation();
					//return;
				}
			}
		}

		private int randomPick(int fitnessSum)
		{
			var rand = _random.Next(0, fitnessSum);

			for (var i = 0; i < PopulationSize; i++)
			{
				rand -= _population[i].Fitness;
				if (rand <= 0)
					return i;
			}

			throw new Exception();
		}

		private void EvaluateFitness(PopulationMember populationMember)
		{
			populationMember.Fitness = 1 + CalculateChallengerWinsFrom100(populationMember.Strategy);
			Console.WriteLine(populationMember.Fitness);
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

			Parallel.For(0, GameCount, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (i) =>
			{
				var pieces = RandomPiecesCache[i];

				var board = new BoardState();
				var pieceIndex = 0;
				while (strategy.TryPlacePiece(board, pieces[pieceIndex], in empty, 0, out var bitmap, out var x, out var y))
				{
					board.Place(bitmap, x, y);

					pieceIndex++;
				}

				Interlocked.Add(ref totalPlacements, pieceIndex);
			});

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
			public const int GeneSize = 16;

			public int[] Gene;
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
				Strategy = new BestEvaluatorStrategy(new TuneablePattern2x2BoardEvaluator(Gene));
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