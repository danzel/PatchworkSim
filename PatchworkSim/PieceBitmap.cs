using System;

namespace PatchworkSim;

public class PieceBitmap
{
	/// <summary>
	/// The piece aligned to the bottom right (0,0).
	/// Pieces are stored on a 9x9 grid like BoardState
	/// </summary>
	internal readonly UInt128 Bitmap;

	public readonly int Width;
	public readonly int Height;

	private readonly UInt128[] _shifted = new UInt128[BoardState.Width * BoardState.Height];

	public PieceBitmap(bool[,] boolmap)
	{
		for (var x = 0; x < boolmap.GetLength(0); x++)
		{
			for (var y = 0; y < boolmap.GetLength(1); y++)
			{
				if (boolmap[x, y])
				{
					Bitmap |= UInt128.One << (x + y * BoardState.Width);
				}
			}
		}

		Width = boolmap.GetLength(0);
		Height = boolmap.GetLength(1);

		PopulateShifts();
	}

	public UInt128 GetShifted(int x, int y)
	{
		return _shifted[x + y * BoardState.Width];
	}

	private void PopulateShifts()
	{
		for (var x = 0; x <= BoardState.Width - Width; x++)
		{
			for (var y = 0; y <= BoardState.Height - Height; y++)
			{
				_shifted[x + y * BoardState.Width] = Shift(x, y);
			}
		}
	}

	/// <summary>
	/// Shift the piece moved the given (x,y) location.
	/// Pieces are stored on a 9x9 grid like BoardState
	/// </summary>
	private UInt128 Shift(int x, int y)
	{
		if (x < 0)
			throw new Exception("X is out of range");
		if (y < 0)
			throw new Exception("Y is out of range");
		if (Width + x > BoardState.Width)
			throw new Exception("X is out of range");
		if (Height + y > BoardState.Height)
			throw new Exception("Y is out of range");

		return Bitmap << (x + y * BoardState.Width);
	}

}
