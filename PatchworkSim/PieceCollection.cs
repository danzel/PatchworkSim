using System;
using System.Collections.Generic;

namespace PatchworkSim;

public unsafe struct PieceCollection
{
	private fixed int _value[PieceDefinition.TotalPieces];

	public int Count;

	public void RemoveAt(int index)
	{
#if DEBUG
		if (index >= Count || index < 0)
			throw new Exception();
#endif
		//Ref https://stackoverflow.com/questions/1389821/array-copy-vs-buffer-blockcopy
		fixed (int* p = _value)
		{
			for (var i = index; i < Count - 1; i++)
			{
				p[i] = p[i + 1];
			}
		}

		Count--;
	}

	public int this[int key]
	{
		get
		{
#if DEBUG
			if (key >= PieceDefinition.TotalPieces || key >= Count || key < 0)
				throw new Exception();
#endif
			fixed (int* p = _value)
			{
				return p[key];
			}
		}

		set
		{
#if DEBUG
			if (key >= PieceDefinition.TotalPieces || key >= Count || key < 0)
				throw new Exception();
#endif
			fixed (int* p = _value)
			{
				p[key] = value;
			}
		}
	}

	public void Populate(List<int> pieces)
	{
		fixed (int* p = _value)
		{
			for (var i = 0; i < pieces.Count; i++)
			{
				p[i] = pieces[i];
			}
		}
		Count = pieces.Count;
	}
}
