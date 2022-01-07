namespace Texel.Collections
{
	public struct CombinedKey<TKey1, TKey2>
	{
		public TKey1 Key1 { get; }
		public TKey2 Key2 { get; }

		public CombinedKey(TKey1 key1, TKey2 key2)
		{
			this.Key1 = key1;
			this.Key2 = key2;
		}
	}
}