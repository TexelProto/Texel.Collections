using System;
using System.Collections;
using System.Collections.Generic;

namespace Texel.Collections
{
	public class CompositeKeyDictionary<TKey1, TKey2, TValue> : IMultiKeyDictionary<TKey1, TKey2, TValue>
		where TKey1 : struct
		where TKey2 : struct
	{
		private readonly Dictionary<CombinedKey<TKey1, TKey2>, TValue> dictionary = new Dictionary<CombinedKey<TKey1, TKey2>, TValue>();

		#region Properties

		public TValue this[TKey1 key1, TKey2 key2]
		{
			get
			{
				var key = new CombinedKey<TKey1, TKey2>( key1, key2 );
				try
				{
					return this.dictionary[key];
				}
				catch (Exception e)
				{
					throw new KeyNotFoundException( $"Failed to find key {key}", e );
				}
			}
			set
			{
				var key = new CombinedKey<TKey1, TKey2>( key1, key2 );
				this.dictionary[key] = value;
			}
		}

		#endregion

		#region Factory Methods

		public static CompositeKeyDictionary<TKey1, TKey2, TValue> FromList(IEnumerable<TValue> collection, Func<TValue, TKey1> key1Selector, Func<TValue, TKey2> key2Selector)
		{
			var table = new CompositeKeyDictionary<TKey1, TKey2, TValue>();

			foreach (var element in collection)
			{
				var key1 = key1Selector( element );
				var key2 = key2Selector( element );
				table.Add( key1, key2, element );
			}

			return table;
		}

		#endregion

		#region Collection Control

		public void Add(TKey1 key1, TKey2 key2, TValue value)
		{
			this.dictionary.Add( new CombinedKey<TKey1, TKey2>( key1, key2 ), value );
		}

		public bool Remove(TKey1 key1, TKey2 key2)
		{
			var key = new CombinedKey<TKey1, TKey2>( key1, key2 );
			return this.dictionary.Remove( key );
		}

		public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
		{
			var key = new CombinedKey<TKey1, TKey2>( key1, key2 );
			return this.dictionary.TryGetValue( key, out value );
		}

		public bool ContainsKey(TKey1 key1, TKey2 key2)
		{
			var key = new CombinedKey<TKey1, TKey2>( key1, key2 );
			return this.dictionary.ContainsKey( key );
		}

		#endregion

		#region IDictionary Implementation

		public int Count => this.dictionary.Count;
		bool ICollection<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.IsReadOnly => false;
		ICollection<CombinedKey<TKey1, TKey2>> IDictionary<CombinedKey<TKey1, TKey2>, TValue>.Keys => this.dictionary.Keys;
		public ICollection<TValue> Values => this.dictionary.Values;

		void ICollection<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.Add(KeyValuePair<CombinedKey<TKey1, TKey2>, TValue> item)
		{
			this.dictionary.Add( item.Key, item.Value );
		}

		public void Clear()
		{
			this.dictionary.Clear();
		}

		bool ICollection<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.Contains(KeyValuePair<CombinedKey<TKey1, TKey2>, TValue> item)
		{
			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.CopyTo(KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		bool ICollection<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.Remove(KeyValuePair<CombinedKey<TKey1, TKey2>, TValue> item)
		{
			throw new NotImplementedException();
		}

		void IDictionary<CombinedKey<TKey1, TKey2>, TValue>.Add(CombinedKey<TKey1, TKey2> key, TValue value)
		{
			this.dictionary.Add( key, value );
		}

		bool IDictionary<CombinedKey<TKey1, TKey2>, TValue>.ContainsKey(CombinedKey<TKey1, TKey2> key)
		{
			return this.dictionary.ContainsKey( key );
		}

		bool IDictionary<CombinedKey<TKey1, TKey2>, TValue>.Remove(CombinedKey<TKey1, TKey2> key)
		{
			return this.dictionary.Remove( key );
		}

		bool IDictionary<CombinedKey<TKey1, TKey2>, TValue>.TryGetValue(CombinedKey<TKey1, TKey2> key, out TValue value)
		{
			return this.dictionary.TryGetValue( key, out value );
		}

		TValue IDictionary<CombinedKey<TKey1, TKey2>, TValue>.this[CombinedKey<TKey1, TKey2> key]
		{
			get => this.dictionary[key];
			set => this.dictionary[key] = value;
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return this.Values.GetEnumerator();
		}

		IEnumerator<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>> IEnumerable<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( (IEnumerable) this.dictionary ).GetEnumerator();
		}

		#endregion
	}
}