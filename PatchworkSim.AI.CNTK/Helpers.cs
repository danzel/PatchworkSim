namespace PatchworkSim.AI.CNTK
{
	static class CNTKHelpers
	{
		internal static void CopyBoardToArray(BoardState board, float[] dest, int offset)
		{
			for (var x = 0; x < BoardState.Width; x++)
			{
				for (var y = 0; y < BoardState.Height; y++)
				{
					dest[offset] = board[x, y] ? 1 : 0;
					offset++;
				}
			}
		}
	}
}
