namespace PatchworkSim
{
	public class PieceBitmap
	{
		/// <summary>
		/// The piece aligned to the bottom right (0,0).
		/// Pieces are stored on a 9x9 grid like BoardState
		/// </summary>
		internal readonly UInt128 _bitmap;

		public readonly int Width;
		public readonly int Height;

		public PieceBitmap(bool[,] boolmap)
		{
			for (var x = 0; x < boolmap.GetLength(0); x++)
			{
				for (var y = 0; y < boolmap.GetLength(1); y++)
				{
					if (boolmap[x, y])
					{
						_bitmap |= UInt128.One << x << (y * BoardState.Width);
					}
				}
			}

			Width = boolmap.GetLength(0);
			Height = boolmap.GetLength(1);
		}
	}
}
