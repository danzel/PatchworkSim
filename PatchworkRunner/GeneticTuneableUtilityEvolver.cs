using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;

namespace PatchworkRunner
{
	class GeneticTuneableUtilityEvolver
	{
		private const int PopulationSize = 50;
		private readonly Random _random = new Random();
		private readonly IMoveDecisionMaker _boss = new GreedyCardValueUtilityMoveMaker(2);
		List<PopulationMember> _population;
		const int MaxGeneration = 100_000;

		private void GenerateInitialPopulation()
		{
			Console.WriteLine("Generating initial population");
			_population = new List<PopulationMember>();
			for (var i = 0; i < PopulationSize; i++)
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

				if (generation % 10000 == 0)
					Console.WriteLine($"Generation {generation}. Fitness Range: {_population[0].Fitness} -- {_population[PopulationSize - 1].Fitness}");

				if (_population[0].Fitness > lastBestFitness)
				{
					lastBestFitness = _population[0].Fitness;
					Console.WriteLine(_population[0].MoveMaker.Name);
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
						gene[j] = (_random.NextDouble() < 0.5 ? parent0 : parent1).Gene[j]; //Crossover

						gene[j] += (_random.NextDouble() * 2 - 1) * 0.2; //Mutation
					}

					target.CreateMoveMaker();
					EvaluateFitness(_population[PopulationSize - 1 - i]);
				}

				generation++;

				if (generation == MaxGeneration)
				{
					File.AppendAllLines("best.txt", new [] { $"{lastBestFitness} {_population[0].MoveMaker.Name}" });
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
			populationMember.Fitness = 1 + CalculateChallengerWinsFrom100(_boss, populationMember.MoveMaker);
		}

		private double[] CreateRandomGene()
		{
			return new[]
			{
				_random.NextDouble() * 4 - 2,
				_random.NextDouble() * 4 - 2,
				_random.NextDouble() * 4 - 2,
				_random.NextDouble() * 4 - 2,
				_random.NextDouble() * 4 - 2,
				_random.NextDouble() * 4 - 2,
				_random.NextDouble() * 4 - 2,
				_random.NextDouble() * 4 - 2
			};
		}

		/// <summary>
		/// Returns the amount of wins the challenger got (from 100)
		/// </summary>
		private static int CalculateChallengerWinsFrom100(IMoveDecisionMaker best, IMoveDecisionMaker challenger)
		{
			int challengerWins = 0;

			Parallel.For(0, 100, (i) =>
			{
				var state = new SimulationState(SimulationHelpers.GetRandomPieces(i / 2), i % 2);
				state.Fidelity = SimulationFidelity.NoPiecePlacing;
				//TODO: May need a cheaper placement engine
				var runner = new SimulationRunner(state,
					new PlayerDecisionMaker(best, null),
					new PlayerDecisionMaker(challenger, null));

				while (!state.GameHasEnded)
					runner.PerformNextStep();

				if (state.WinningPlayer == 1)
					Interlocked.Increment(ref challengerWins);
			});

			return challengerWins;
		}

		class PopulationMember : IComparable<PopulationMember>
		{
			public const int GeneSize = 8;

			public double[] Gene;
			public TuneableUtilityMoveMaker MoveMaker;

			public int Fitness;

			public PopulationMember(double[] gene)
			{
				Gene = gene;
				CreateMoveMaker();
			}

			public void CreateMoveMaker()
			{
				MoveMaker = new TuneableUtilityMoveMaker(
					Gene[0],
					Gene[1],
					Gene[2],
					Gene[3],
					Gene[4],
					Gene[5],
					Gene[6],
					Gene[7]
				);
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
 