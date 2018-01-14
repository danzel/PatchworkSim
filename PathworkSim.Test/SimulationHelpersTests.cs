using System.Linq;
using PatchworkSim;
using Xunit;

namespace PathworkSim.Test
{
	public class SimulationHelpersTests
	{
		[Fact]
		public void GetRandomPiecesPlaces0AtEnd()
		{
			for (var i = 0; i < 1000; i++)
			{
				var pieces = SimulationHelpers.GetRandomPieces(i);

				//0 at end
				Assert.Equal(0, pieces[pieces.Count - 1]);

				//Array contains all pieces
				pieces = pieces.OrderBy(p => p).ToList();
				for (var j = 0; j < pieces.Count; j++)
					Assert.Equal(j, pieces[j]);
			}
		}
	}
}