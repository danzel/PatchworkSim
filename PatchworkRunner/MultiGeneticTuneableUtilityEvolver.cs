using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PatchworkSim;
using PatchworkSim.AI.MoveMakers;

namespace PatchworkRunner
{
	class MultiGeneticTuneableUtilityEvolver
	{
		private const int PopulationSize = 50;
		private readonly Random _random = new Random();
		private readonly IMoveDecisionMaker _boss = TuneableUtilityMoveMaker.Tuning1;
		List<PopulationMember> _population;
		const int MaxGeneration = 120_000;

		private const double MinValue = -1;
		private const double MaxValue = 1;

		private void GenerateInitialPopulation()
		{
			_population = new List<PopulationMember>();

			//259/300, best found
			var pop = new PopulationMember(new[] { -0.137817920411251, 0.348339462002654, -0.939692934258616, -1, 0.392946436940513, 0.013266931251092, 0.9772746516448, 0.0327358908758132, -0.22855272760502, 0.714828049102879, -0.835131391605933, -1, 0.9143385827683, 0.39412640298958, -0.732167609600687, 0.286710773085549, 0.471946845497181, 0.864818019663445, -1, -0.645937013195383, 0.268954919158809, 0.157722040671476, 0.0261397513768403, 0.603945769167509, 0.382381300234945, 0.516047207167027, -0.903378820881996, -1, 0.778852461609, 0.33624564592856, 0.114161065432366, -0.119081037913768, 0.409629372946493, 0.588256376939306, -0.584313931035003, -1, 0.631820934957675, 0.105551921888773, 0.485059839775916, 0.175447079244464, 0.0217098708386692, 0.474072431877471, -0.947118854257723, -1, 0.718028288329669, -0.145274906309353, 0.484791417216903, 0.1941424631512, 0.476394381427975, 1, -0.97518891025172, -0.948060596319604, 0.692455759491859, -0.149145498820027, -0.251388924279415, 0.318568385011994, 0.0532339753343959, 0.645539831949718, -0.878931686002491, -1, 0.510821939109356, 0.222661765683986, 0.650455628576523, 0.286105323025822, -0.243382261833878, 0.881495317768023, -1, -0.828711459791814, 0.584037884882151, 0.972812693750812, 0.902031109941401, -0.458191507986741, 0.198774525977411, 0.813185222393995, -0.84087906535047, -1, 0.882744242611162, 0.11019463637709, -0.0421177152037021, -0.0313901289955418, -0.203343180209008, 0.990798914059953, -0.637496983695981, -0.731835189203312, 0.279036605104025, -0.0565015032292163, 1, -0.205774005854999, 0.443929210611482, 0.966045989147136, -0.633316641069006, -1, 0.669245740920092, -0.154124367929948, 0.322322177938829, 0.13390719907764, 0.209377112574094, 0.828499205308851, -1, -0.826378719803577, 0.559745584558305, -0.0908342236456067, 0.792238791971541, -0.630426233519134, -0.0318268467711907, 0.596968311614293, -0.910690158996904, -1, 0.93354049611665, -0.0687108514160368, 0.277535686413393, -0.0146272018105138, 0.214084878922041, 0.644978543591578, -0.520093920341718, -0.852078610534693, 1, -0.00314039392419943, 0.869128396631158, -0.00520376959822522, 0.0733905051534918, 1, -0.489315414677562, -0.978066439244424, 0.630762660309886, -0.214537786060343, 0.847750604820378, -0.00907394556810293, 0.30071889184497, 0.919962862641434, -0.749113876983935, -1, 0.462063216144937, -0.00678188210366169, 0.930016645272017, 0.396636643415295, -0.123264816582842, 0.778379910178117, -0.953304944219475, -0.823738576021005, 1, -0.292126666434668, 0.119401303259095, 0.129113946806973, -0.656009032145633, 1, -0.481231815390818, -0.709931711623139, 0.478662071158979, -0.0616698397805257, 0.806959779377188, 0.599496190681781, -0.175243242472751, 0.923666154424148, -1, -0.805576750940059, 0.804115593808764, -0.151256215023377, 0.67860872050468, 0.381543007127206, -0.374229396387762, 1, -0.963131164691694, -0.930783493788918, 0.338681538159244, -0.0584082020637183, 0.932155232332886, -0.263873436345557, -0.260291141239392, 1, -0.836173574854971, -0.932536100233076, 0.50294862997363, 0.00874311196978663, 0.999955760488673, -0.0663928984966414, 0.193868262354297, 1, -0.562851045203207, -0.738953521622497, 0.904293419301055, -0.0799083528244316, 0.97755090262145, -0.150541435333468, -0.293678717527102, 0.81617633679863, -0.432243861594198, -0.901725454673964, 0.963404034901265, -0.504801031271107, 1, -0.424352545311316, -0.371200636425745, 0.849684230035647, -0.221668933213376, -0.989270196523552, 0.390771395419372, -0.248857151262095, 1, -0.428030401186889, -0.909448146354481, 1, -0.698480777367349, -0.987151191503439, 0.549593426452923, -0.163334326521863, 0.375462047561564, -0.315165987312531, 0.012604830290279, 0.953841447319483, -0.609900214479754, -0.960590808384641, 0.855542106480821, -0.0661998073551207, 1, 0.406229565121033, 0.134201312206754, 0.752329769265286, -0.52075400222535, -0.65883109554401, 0.663811376193903, -0.189054350414012, 1, 0.511750966185949, -0.0864448445632803, 1, -0.54905886933079, -0.964003197311252, 0.710315717178646, -0.0522196051065104, 0.956189211286371, -0.140173382797672, -0.0277993838300481, 1, -0.586241201678001, -0.843092619720529, 0.36569891932879, -0.0482976123106802, -0.109779724476885, -0.244442059296289, -0.780959795504072, 0.909764904516089, -0.318253521709846, -1, 0.968632551239981, -0.908604687595058, 0.56166238493448, 0.217482437048033, -0.49087120626975, 0.76304643834172, -0.82960838495659, -0.905555083487242, 1, -0.245734195245569, 0.620446095921358, -0.160432976374573, -0.430322875191828, 0.968088946188369, -0.694120635405088, -1, 0.516935358223429, 0.0587125405272703, 0.643324835225544, -0.295962479988169, -0.39745721370724, 1, -0.278026287820188, -0.916681704486157, 0.800071584460815, -0.237557127832164, 0.128084772002784, -0.4227557609451, -0.371638123650331, 0.988470511479719, -0.0590171044290914, -0.887815945194431, 0.593135135614459, -0.231093844010952, 1, -0.638324658995731, -0.363572912034346, 1, -0.867858970811165, -0.964594508972438, 0.818374450586069, -0.167885371046531, 0.715437053673537, 0.384703319383763, -0.292603738623428, 1, -0.376870911835439, -0.967403488488819, 0.912936357782998, -0.593831932926807, 0.88883824784242, 0.382340348831027, -0.634386548110694, 0.909657929591977, -0.245700301243777, -1, 0.273443307067055, -0.056659881827767, 0.477363005543441, 0.842935026861545, -0.332027969383779, 0.978033849546659, -0.68514221954899, -1, 0.873429152338144, -0.333619513901896, 0.959833327458636, -0.104968672928, -0.233252895360624, 1, -0.173259296544248, -0.699998786321411, 0.664578261583711, -0.120250589628928, 0.900356157627674, 0.0874392263145573, -0.271481793808361, 0.983789640878001, -0.629468249207035, -1, 0.806864345581149, -0.254110092505144, 0.611610351435021, -0.246638131222046, -0.41916356296059, 1, 0.112223811076088, -0.643895909299535, 0.231210076884669, -0.283821147994125, 0.726877548881033, -0.333869430691705, 0.194675251492644, 1, -0.677587449154977, -0.776022412468337, -0.164937928347311, -0.426565407139431, 0.382982600825797, 0.246736291610919, -0.0677556722993119, 0.856350613243355, -0.556229615465683, -0.638895947784756, 1, -0.366932112033037, 0.55131692913781, 0.690468215806002, -0.037785141199325, 0.866613007472053, -0.696933765415353, -0.914396782239104, 1, -0.53154480764022, 0.857610779335651, -0.197458189482521, -0.837966671873203, 1, 0.274028116727997, -0.553723733926615, 0.516946129545573, -0.563067463923689, 0.840050146496187, 0.218961127253361, -0.0102363229396804, 1, -0.729817828849437, -0.847887506654613, 0.627085630472745, -0.0689514879023928, 0.817048011296443, -0.471155084389032, -1, 0.870294715275508, -0.459080926325629, -0.865247557589426, 0.428711652389482, 0.184708511084962, 0.881867544995118, -0.233632485518528, 0.368436013406466, 0.789992985619614, -0.666884319344627, -0.876750614446496, -0.522288482766681, -0.145575324143286, 1, 0.0368261718405474, -0.304164237008167, 0.222305832164043, -0.848568267475738, -1, 0.988015222012298, 0.201900357905859, -0.0559888372145778, -0.0116179518475058, 0.0919659203847996, 0.857015921108279, -0.491712171584863, -0.652718629720298, -0.0859660470115806, -0.0778316167685703, 0.670815271831417, -1, -0.319222076458962, 1, -0.590249807866369, 0.279109514202479, 0.790323082697038, -0.22476673690442, 0.91817685381318, 0.208718490619993, 0.422337040634374, 0.96977672596845, -1, 0.506848447736712, 0.683062197292398, 0.0410306504290415, 0.038479186931154, -0.0248677471870259 });
			EvaluateFitness(pop);
			_population.Add(pop);

			//Tuning1
			var tuning1 = CreateRandomGene();
			for (var i = 0; i < SimulationState.EndLocation; i++)
			{
				var offset = i * 8;
				tuning1[offset + 0] = -0.03892573;
				tuning1[offset + 1] = 1;
				tuning1[offset + 2] = -0.593965087;
				tuning1[offset + 3] = -0.984706754;
				tuning1[offset + 4] = 0.554044008;
				tuning1[offset + 5] = -0.052553921;
				tuning1[offset + 6] = 0.609957017;
				tuning1[offset + 7] = -0.015045333;
			}

			pop = new PopulationMember(tuning1);
			EvaluateFitness(pop);
			_population.Add(pop);


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

				if (generation % 1000 == 0)
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

					var mutationSize = _random.NextDouble() * 0.2;
					for (var j = 0; j < PopulationMember.GeneSize; j++)
					{
						//TODO: Could use random more efficiently
						if (_random.NextDouble() < 0.5)
							gene[j] = (_random.NextDouble() < 0.5 ? parent0 : parent1).Gene[j]; //Crossover
						else
							gene[j] = Clamp(gene[j] + (MinValue + _random.NextDouble() * (MaxValue - MinValue)) * mutationSize); //Mutation
					}

					target.CreateMoveMaker();
					EvaluateFitness(_population[PopulationSize - 1 - i]);
				}

				generation++;

				if (generation == MaxGeneration)
				{
					File.AppendAllLines($"best-{lastBestFitness}-{DateTimeOffset.UtcNow.Ticks}.txt", new[] { $"{lastBestFitness} {_population[0].MoveMaker.Name}" });
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
			var res = new double[PopulationMember.GeneSize];
			for (var i = 0; i < PopulationMember.GeneSize; i++)
				res[i] = MinValue + _random.NextDouble() * (MaxValue - MinValue);
			return res;
		}

		private const int GameCount = 300;

		private static readonly List<int>[] RandomPiecesCache = Enumerable.Range(0, GameCount / 2).Select(i => SimulationHelpers.GetRandomPieces(i)).ToArray();

		/// <summary>
		/// Returns the amount of wins the challenger got (from 100)
		/// </summary>
		private static int CalculateChallengerWinsFrom100(IMoveDecisionMaker best, IMoveDecisionMaker challenger)
		{
			int challengerWins = 0;

			Parallel.For(0, 300, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (i) =>
			{
				var state = new SimulationState(RandomPiecesCache[i / 2], 0);//SimulationHelpers.GetRandomPieces(i / 2), 0);
				state.ActivePlayer = i % 2;
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

		/// <summary>
		/// Clamp between MinValue and MaxValue inclusive
		/// </summary>
		private double Clamp(double value)
		{
			return value < MinValue ? MinValue : value > MaxValue ? MaxValue : value;
		}


		class PopulationMember : IComparable<PopulationMember>
		{
			public const int GeneSize = 8 * SimulationState.EndLocation;

			public double[] Gene;
			public RangeSplitByBoardPositionDelegationMoveMaker MoveMaker;

			public int Fitness;

			public PopulationMember(double[] gene)
			{
				Gene = gene;
				CreateMoveMaker();
			}

			public void CreateMoveMaker()
			{
				//TODO: Garbage generation here
				var moveMakers = new IMoveDecisionMaker[SimulationState.EndLocation];
				for (var i = 0; i < SimulationState.EndLocation; i++)
				{
					var offset = i * 8;
					moveMakers[i] = new TuneableUtilityMoveMaker(
						Gene[offset + 0],
						Gene[offset + 1],
						Gene[offset + 2],
						Gene[offset + 3],
						Gene[offset + 4],
						Gene[offset + 5],
						Gene[offset + 6],
						Gene[offset + 7]
					);
				}
				MoveMaker = new RangeSplitByBoardPositionDelegationMoveMaker(moveMakers);
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