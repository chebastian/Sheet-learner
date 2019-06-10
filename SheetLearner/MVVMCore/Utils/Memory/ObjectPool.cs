using System.Collections.Generic;
using System.Linq;

namespace MVVMCore.Utils.Memory
{

	public class PoolItem<T> where T : new()
	{
		public PoolItem(T t)
		{
			Item = t;
		}

		public int Identifier { get; set; }
		public T Item { get; set; }
		public bool Free { get; set; }
	}

	public class ObjectPool<T> where T : new()
	{
		private List<PoolItem<T>> _list;
		private Stack<PoolItem<T>> FreeStack;

		public ObjectPool()
		{
			_list = new List<PoolItem<T>>();
			_list.AddRange(Enumerable.Repeat<PoolItem<T>>(new PoolItem<T>(new T()), PoolSize));

			FreeStack = new Stack<PoolItem<T>>();
		}

		public int PoolSize { get; }

		public T Create()
		{
			return new T();
		}
	}

}
