using System;
using System.Collections.Generic;

namespace PatchworkSim.AI
{
	/// <summary>
	/// An item pool that requires you always first return the last item it gave to you
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SingleThreadedStackPool<T> where T : new()
	{
		private readonly List<T> _pool = new List<T>();
		private int _getIndex;

		public T Get()
		{
			if (_getIndex < _pool.Count)
			{
				var index = _getIndex;
				_getIndex++;
				//FetchedFromPool++;
				return _pool[index];
			}

			var res = new T();
			_pool.Add(res);
			_getIndex++;
			return res;
		}

		public void Return(T item)
		{
#if DEBUG
			if (!ReferenceEquals(_pool[_getIndex - 1], item))
				throw new Exception("You are not returning the latest item");
#endif
			_getIndex--;
		}
	}
}