namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies.Preplacers
{
	public struct Preplacement
	{
		public readonly PieceBitmap Bitmap;
		public readonly int X;
		public readonly int Y;

		public Preplacement(PieceBitmap bitmap, int x, int y)
		{
			Bitmap = bitmap;
			X = x;
			Y = y;
		}
	}
}
