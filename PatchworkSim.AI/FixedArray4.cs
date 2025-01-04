using System;

namespace PatchworkSim.AI;

public unsafe struct FixedArray4Int
{
	private fixed int _value[4];

	public int this[int key]
	{
		get
		{
#if DEBUG
			if (key >= 4 || key < 0)
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
			if (key >= 4 || key < 0)
				throw new Exception();
#endif
			fixed (int* p = _value)
			{
				p[key] = value;
			}
		}
	}
}

public unsafe struct FixedArray4Double
{
	private fixed double _value[4];

	public double this[int key]
	{
		get
		{
#if DEBUG
			if (key >= 4 || key < 0)
				throw new Exception();
#endif
			fixed (double* p = _value)
			{
				return p[key];
			}
		}

		set
		{
#if DEBUG
			if (key >= 4 || key < 0)
				throw new Exception();
#endif
			fixed (double* p = _value)
			{
				p[key] = value;
			}
		}
	}
}
