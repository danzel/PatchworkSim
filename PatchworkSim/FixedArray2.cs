using System;

namespace PatchworkSim
{
	public unsafe struct FixedArray2Int
	{
		private fixed int _value[2];

		public int this[int key]
		{
			get
			{
#if DEBUG
				if (key >= 2 || key < 0)
					throw new Exception();
#endif
				fixed(int* p = _value)
				{
					return p[key];
				}
			}

			set
			{
#if DEBUG
				if (key >= 2 || key < 0)
					throw new Exception();
#endif
				fixed (int* p = _value)
				{
					p[key] = value;
				}
			}
		}

	}
}
