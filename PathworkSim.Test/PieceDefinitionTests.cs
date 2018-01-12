using PatchworkSim;
using Xunit;

namespace PathworkSim.Test
{
	public class PieceDefinitionTests
	{
		[Fact]
		public void SinglePossibleOrientation()
		{
			var piece = new PieceDefinition("test", 0, 0, 0,
				new[]
				{
					"#"
				});

			Assert.Single(piece.PossibleOrientations);
			Assert.Equal(1, piece.TotalUsedLocations);
		}

		[Fact]
		public void TwoPossibleOrientations()
		{
			var piece = new PieceDefinition("test", 0, 0, 0,
				new[]
				{
					"##"
				});

			Assert.Equal(2, piece.PossibleOrientations.Length);
			Assert.Equal(2, piece.TotalUsedLocations);
		}

		[Fact]
		public void FourPossibleOrientations()
		{
			var piece = new PieceDefinition("test", 0, 0, 0,
				new[]
				{
					"###",
					" # "
				});

			Assert.Equal(4, piece.PossibleOrientations.Length);
			Assert.Equal(4, piece.TotalUsedLocations);
		}

		[Fact]
		public void EightPossibleOrientations()
		{
			var piece = new PieceDefinition("test", 0, 0, 0,
				new[]
				{
					"###",
					"  #"
				});

			Assert.Equal(8, piece.PossibleOrientations.Length);
			Assert.Equal(4, piece.TotalUsedLocations);
		}
	}
}
