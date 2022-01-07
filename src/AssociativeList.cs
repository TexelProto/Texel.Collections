using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Texel.Collections
{
	public class AssociativeList<TKey, TValue> : List<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
	{
		private const int DefaultCapacity = 4;

		private readonly IEqualityComparer<TKey> comparer;

		public AssociativeList()
			: this(DefaultCapacity, EqualityComparer<TKey>.Default) { }

		public AssociativeList(int capacity)
			: this(capacity, EqualityComparer<TKey>.Default) { }

		public AssociativeList(IEqualityComparer<TKey> comparer)
			: this(DefaultCapacity, comparer) { }

		public AssociativeList(IDictionary<TKey, TValue> dictionary)
			: this(dictionary, EqualityComparer<TKey>.Default) { }

		public AssociativeList(int capacity, IEqualityComparer<TKey> comparer)
			: base(capacity)
		{
			this.Keys = new KeyCollection(this);
			this.Values = new ValueCollection(this);
			this.comparer = comparer;
		}

		public AssociativeList(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			: this(dictionary.Count, comparer)
		{
			this.AddRange(dictionary);
		}

		public TValue this[TKey key]
		{
			get => this[this.FindKeyIndex(key)].Value;
			set => this[this.FindKeyIndex(key)] = new KeyValuePair<TKey, TValue>(key, value);
		}

		public ICollection<TKey> Keys { get; }

		public ICollection<TValue> Values { get; }

		public void Add(TKey key, TValue value)
		{
			this.Add(new KeyValuePair<TKey, TValue>(key, value));
		}

		public bool ContainsKey(TKey key)
		{
			return this.FindKeyIndex(key, false) != -1;
		}

		public bool Remove(TKey key)
		{
			int index = this.FindKeyIndex(key, false);
			if (index == -1)
				return false;
			this.RemoveAt(index);
			return true;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			int index = this.FindKeyIndex(key, false);
			if (index == -1)
			{
				value = default;
				return false;
			}

			value = this[index].Value;
			return true;
		}

		[Pure]
		private int FindKeyIndex(TKey key, bool throwIfNotFound = true)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].Key.Equals(key))
					return i;
			}

			if (!throwIfNotFound)
				return -1;
			throw new KeyNotFoundException();
		}

		private sealed class KeyCollection : ICollection<TKey>, ICollection, IReadOnlyCollection<TKey>
		{
			private readonly AssociativeList<TKey, TValue> list;

			public KeyCollection(AssociativeList<TKey, TValue> list)
			{
				this.list = list;
			}

			bool ICollection.IsSynchronized => false;
			object ICollection.SyncRoot => ( (ICollection) this.list ).SyncRoot;

			void ICollection.CopyTo(Array array, int index)
			{
				throw new NotImplementedException();
			}

			public int Count => this.list.Count;
			bool ICollection<TKey>.IsReadOnly => true;

			public void CopyTo(TKey[] array, int index)
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					var key = this.list[i].Key;
					array[i + index] = key;
				}
			}

			public IEnumerator<TKey> GetEnumerator()
			{
				return this.list.Select(kvp => kvp.Key).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			bool ICollection<TKey>.Contains(TKey item)
			{
				if (item == null)
					throw new ArgumentNullException(nameof(item));
				return this.list.ContainsKey(item);
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TKey>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw new NotSupportedException();
			}
		}

		private sealed class ValueCollection : ICollection<TValue>, ICollection, IReadOnlyCollection<TValue>
		{
			private readonly AssociativeList<TKey, TValue> list;

			public ValueCollection(AssociativeList<TKey, TValue> list)
			{
				this.list = list;
			}

			bool ICollection.IsSynchronized => false;
			object ICollection.SyncRoot => ( (ICollection) this.list ).SyncRoot;

			void ICollection.CopyTo(Array array, int index)
			{
				throw new NotImplementedException();
			}

			public int Count => this.list.Count;
			bool ICollection<TValue>.IsReadOnly => true;

			public void CopyTo(TValue[] array, int index)
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					var value = this.list[i].Value;
					array[i + index] = value;
				}
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				return this.list.Select(kvp => kvp.Value).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			bool ICollection<TValue>.Contains(TValue item)
			{
				if (item == null)
					throw new ArgumentNullException(nameof(item));
				return this.list.FindIndex(kvp => kvp.Value.Equals(item)) != -1;
			}

			void ICollection<TValue>.Add(TValue item)
			{
				throw new NotSupportedException();
			}

			void ICollection<TValue>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<TValue>.Remove(TValue item)
			{
				throw new NotSupportedException();
			}
		}
	}
}