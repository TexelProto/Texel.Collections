using System.Collections;
using System.Collections.Generic;

namespace Texel.Collections
{
	public class Bag<T> : IList<T>
	{
		private readonly List<T> list;

		public Bag()
		{
			this.list = new List<T>();
		}

		public Bag(int capacity)
		{
			this.list = new List<T>(capacity);
		}

		public Bag(IEnumerable<T> collection)
		{
			this.list = new List<T>(collection);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable) this.list ).GetEnumerator();
		}

		public void Add(T item)
		{
			this.list.Add(item);
		}

		public void Clear()
		{
			this.list.Clear();
		}

		public bool Contains(T item)
		{
			return this.list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.list.CopyTo(array, arrayIndex);
		}

		public int Count => this.list.Count;

		public bool IsReadOnly => ( (IList<T>) this.list ).IsReadOnly;

		public int IndexOf(T item)
		{
			return this.list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.list.Insert(index, item);
		}

		public bool Remove(T item)
		{
			int index = this.IndexOf(item);

			if (index == -1)
				return false;

			this.RemoveAt(index);
			return true;
		}

		public void RemoveAt(int index)
		{
			int lastIndex = this.Count - 1;

			if (index == lastIndex)
			{
				this.list.RemoveAt(index);
				return;
			}

			var last = this.list[lastIndex];
			this.list.RemoveAt(lastIndex);
			this.list[index] = last;
		}

		public T this[int index]
		{
			get => this.list[index];
			set => this.list[index] = value;
		}

		public void AddRange(IEnumerable<T> collection)
		{
			this.list.AddRange(collection);
		}
	}
}