using System.Collections.Generic;

namespace Texel.Collections
{
	public interface IMultiKeyDictionary<TKey1, TKey2, TValue> : IDictionary<CombinedKey<TKey1,TKey2>, TValue>
	{
		TValue this[TKey1 key1, TKey2 key2] { get; set; }

		bool ContainsKey(TKey1 key1, TKey2 key2);

		void Add(TKey1 key1, TKey2 key2, TValue value);

		bool Remove(TKey1 key1, TKey2 key2);

		bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value);
	}
}