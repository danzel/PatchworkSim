using System.Collections.Generic;

namespace PatchworkSim.AI;

public interface IPoolableItem
{
	void Reset();
}

/// <summary>
/// An item pool that is only usable by a single thread. Use with ThreadLocal for threadsafe use
/// </summary>
public class SingleThreadedPool<T> where T: IPoolableItem, new()
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

	public void ReturnAll()
	{
		for (var i = 0; i < _getIndex; i++)
		{
			_pool[i].Reset();
		}

		_getIndex = 0;
	}
}
