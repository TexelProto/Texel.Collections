using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Texel.Collections
{
	/// <summary>
	/// Simplified wrapper for nesting dictionaries
	/// </summary>
	/// <typeparam name="TKey1"></typeparam>
	/// <typeparam name="TKey2"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class NestedDictionary<TKey1, TKey2, TValue> : IMultiKeyDictionary<TKey1, TKey2, TValue>, IDictionary<TKey1, Dictionary<TKey2, TValue>>
	{
		public int Count => this.dictionary.Sum( kvp => kvp.Value.Count );
		public bool IsReadOnly => false;

		public TValue this[TKey1 key1, TKey2 key2]
		{
			get
			{
				if (this.GetSubDictionary( key1 ).TryGetValue( key2, out var value ) == false)
					throw new KeyNotFoundException( $"Failed to find entry for key [{key1},{key2}]" );
				return value;
			}
			set => this.GetSubDictionary( key1 )[key2] = value;
		}

		private readonly IEqualityComparer<TKey1> key1Comparer;
		private readonly IEqualityComparer<TKey2> key2Comparer;
		private readonly Dictionary<TKey1, Dictionary<TKey2, TValue>> dictionary;

		#region ctor

		public NestedDictionary()
			: this( EqualityComparer<TKey1>.Default, EqualityComparer<TKey2>.Default ) { }

		public NestedDictionary(IEqualityComparer<TKey1> key1Comparer, IEqualityComparer<TKey2> key2Comparer)
		{
			this.key1Comparer = key1Comparer;
			this.key2Comparer = key2Comparer;
			this.dictionary = new Dictionary<TKey1, Dictionary<TKey2, TValue>>( key1Comparer );
		}

		public NestedDictionary(IEnumerable<TValue> collection, Func<TValue, TKey1> key1Getter, Func<TValue, TKey2> key2Getter)
			: this()
		{
			foreach (var element in collection)
			{
				var key1 = key1Getter( element );
				var key2 = key2Getter( element );
				this.Add( key1, key2, element );
			}
		}
		
		private NestedDictionary(Dictionary<TKey1, Dictionary<TKey2, TValue>> dictionary)
		{
			this.key1Comparer = dictionary.Comparer;
			var firstSub = dictionary.Values.FirstOrDefault();
			if (firstSub == null)
				this.key2Comparer = EqualityComparer<TKey2>.Default;
			else
				this.key2Comparer = firstSub.Comparer;
			this.dictionary = dictionary;
		}

		#endregion

		#region Collection Control

		public void Add(TKey1 key1, TKey2 key2, TValue element)
		{
			this.GetSubDictionary( key1 ).Add( key2, element );
		}

		public bool Remove(TKey1 key1, TKey2 key2)
		{
			return this.GetSubDictionary( key1 ).Remove( key2 );
		}

		public bool TryGetValue(TKey1 key, TKey2 key2, out TValue value)
		{
			if (this.dictionary.TryGetValue( key, out var subDic ) && subDic.TryGetValue( key2, out value ))
				return true;
			value = default;
			return false;
		}

		public bool ContainsKey(TKey1 key1, TKey2 key2)
		{
			return this.GetSubDictionary( key1 ).ContainsKey( key2 );
		}

		public void Clear()
		{
			this.dictionary.Clear();
		}

		private Dictionary<TKey2, TValue> GetSubDictionary(TKey1 key)
		{
			if (this.dictionary.TryGetValue( key, out var subDictionary ))
				return subDictionary;

			subDictionary = new Dictionary<TKey2, TValue>( this.key2Comparer );
			this.dictionary.Add( key, subDictionary );
			return subDictionary;
		}

		#endregion

		#region ICollection (Combined)

		bool ICollection<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.Contains(KeyValuePair<CombinedKey<TKey1, TKey2>, TValue> item)
		{
			throw new System.NotImplementedException();
		}

		void ICollection<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.CopyTo(KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		bool ICollection<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.Remove(KeyValuePair<CombinedKey<TKey1, TKey2>, TValue> item)
		{
			throw new System.NotImplementedException();
		}

		void ICollection<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>>.Add(KeyValuePair<CombinedKey<TKey1, TKey2>, TValue> item)
		{
			this.Add( item.Key.Key1, item.Key.Key2, item.Value );
		}

		#endregion

		#region ICollection (Nested)

		bool ICollection<KeyValuePair<TKey1, Dictionary<TKey2, TValue>>>.Remove(KeyValuePair<TKey1, Dictionary<TKey2, TValue>> item)
		{
			throw new System.NotImplementedException();
		}

		void ICollection<KeyValuePair<TKey1, Dictionary<TKey2, TValue>>>.Add(KeyValuePair<TKey1, Dictionary<TKey2, TValue>> item)
		{
			throw new System.NotImplementedException();
		}

		bool ICollection<KeyValuePair<TKey1, Dictionary<TKey2, TValue>>>.Contains(KeyValuePair<TKey1, Dictionary<TKey2, TValue>> item)
		{
			throw new System.NotImplementedException();
		}

		void ICollection<KeyValuePair<TKey1, Dictionary<TKey2, TValue>>>.CopyTo(KeyValuePair<TKey1, Dictionary<TKey2, TValue>>[] array, int arrayIndex)
		{
			throw new System.NotImplementedException();
		}

		#endregion

		#region IDictionary (Nested)

		public ICollection<TKey1> Keys => this.dictionary.Keys;
		public ICollection<Dictionary<TKey2, TValue>> Values => this.dictionary.Values;

		void IDictionary<TKey1, Dictionary<TKey2, TValue>>.Add(TKey1 key, Dictionary<TKey2, TValue> value)
		{
			this.dictionary.Add( key, value );
		}

		bool IDictionary<TKey1, Dictionary<TKey2, TValue>>.ContainsKey(TKey1 key)
		{
			return this.dictionary.ContainsKey( key );
		}

		public bool Remove(TKey1 key)
		{
			return this.dictionary.Remove( key );
		}

		public bool TryGetValue(TKey1 key, out Dictionary<TKey2, TValue> value)
		{
			return this.dictionary.TryGetValue( key, out value );
		}

		public Dictionary<TKey2, TValue> this[TKey1 key]
		{
			get => this.dictionary[key];
			set => this.dictionary[key] = value;
		}

		#endregion

		#region IDictionary (Combined)

		ICollection<CombinedKey<TKey1, TKey2>> IDictionary<CombinedKey<TKey1, TKey2>, TValue>.Keys => throw new System.NotImplementedException();

		ICollection<TValue> IDictionary<CombinedKey<TKey1, TKey2>, TValue>.Values => this.dictionary.SelectMany( d => d.Value.Values ).ToArray();

		void IDictionary<CombinedKey<TKey1, TKey2>, TValue>.Add(CombinedKey<TKey1, TKey2> key, TValue value)
		{
			this.Add( key.Key1, key.Key2, value );
		}

		bool IDictionary<CombinedKey<TKey1, TKey2>, TValue>.ContainsKey(CombinedKey<TKey1, TKey2> key)
		{
			return this.ContainsKey( key.Key1, key.Key2 );
		}

		bool IDictionary<CombinedKey<TKey1, TKey2>, TValue>.Remove(CombinedKey<TKey1, TKey2> key)
		{
			return this.Remove( key.Key1, key.Key2 );
		}

		bool IDictionary<CombinedKey<TKey1, TKey2>, TValue>.TryGetValue(CombinedKey<TKey1, TKey2> key, out TValue value)
		{
			var subDictionary = this.GetSubDictionary( key.Key1 );
			return subDictionary.TryGetValue( key.Key2, out value );
		}

		TValue IDictionary<CombinedKey<TKey1, TKey2>, TValue>.this[CombinedKey<TKey1, TKey2> key]
		{
			get => this.GetSubDictionary( key.Key1 )[key.Key2];
			set => this.GetSubDictionary( key.Key1 )[key.Key2] = value;
		}

		#endregion

		#region IEnumerable

		IEnumerator<KeyValuePair<TKey1, Dictionary<TKey2, TValue>>> IEnumerable<KeyValuePair<TKey1, Dictionary<TKey2, TValue>>>.GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		public IEnumerator<KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>> GetEnumerator()
		{
			foreach (var topKvp in this.dictionary)
			{
				var key1 = topKvp.Key;
				foreach (var subKvp in topKvp.Value)
				{
					var key2 = subKvp.Key;
					var key = new CombinedKey<TKey1, TKey2>( key1, key2 );
					var value = subKvp.Value;
					yield return new KeyValuePair<CombinedKey<TKey1, TKey2>, TValue>( key, value );
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}