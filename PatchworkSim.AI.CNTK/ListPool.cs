using System.Collections.Generic;

namespace PatchworkSim.AI.CNTK
{
	class ListPool<T>
	{
		private readonly Stack<List<T>> _pool = new Stack<List<T>>();

		public List<T> Get()
		{
			if (_pool.Count == 0)
				return new List<T>();
			return _pool.Pop();
		}

		public void Return (List<T> item)
		{
			item.Clear();
			_pool.Push(item);
		}
	}
}
