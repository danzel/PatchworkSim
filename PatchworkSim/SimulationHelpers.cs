using System;
using System.Collections.Generic;

namespace PatchworkSim
{
	public static class SimulationHelpers
	{
		public static List<int> GetRandomPieces(int? randomSeed = null)
		{
			var rand = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();

			//Populate the array with 0 as the last item
			var array = new List<int>(PieceDefinition.AllPieceDefinitions.Length);
			for (var i = 1; i < PieceDefinition.AllPieceDefinitions.Length; i++)
				array.Add(i);
			array.Add(0);


			//Fisher-Yates shuffle, SKIPPING THE LAST ITEM (We always put the starting piece at the last index)
			for (int n = array.Count - 2; n > 0; n--)
			{
				int k = rand.Next(n + 1);
				int temp = array[n];
				array[n] = array[k];
				array[k] = temp;
			}

			return array;
		}
	}
}
