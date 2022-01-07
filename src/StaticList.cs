using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Texel.Collections
{
	/// <summary>
	///     A list of statically indexed elements
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StaticList<T> : IList<T>, IReadOnlyDictionary<int, T>
	{
		private readonly List<T> elements;
		private readonly Stack<int> freeIndices;

		public StaticList()
		{
			this.elements = new List<T>();
			this.freeIndices = new Stack<int>();
		}

		public StaticList(int capacity)
		{
			this.elements = new List<T>( capacity );
			this.freeIndices = new Stack<int>( capacity );
		}

		public StaticList(IEnumerable<T> items)
		{
			this.elements = new List<T>( items );
			this.freeIndices = new Stack<int>();
		}

		#region IEnumerable T

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < this.elements.Count; i++)
			{
				if (this.freeIndices.Contains( i ))
					continue;

				yield return this.elements[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region ICollection T

		void ICollection<T>.Add(T item)
		{
			this.Add( item );
		}

		public void AddRange(IEnumerable<T> items)
		{
			this.elements.AddRange( items );
		}

		public int Add(T item)
		{
			if (this.freeIndices.Count > 0)
			{
				int index = this.freeIndices.Pop();
				this.elements[index] = item;
				return index;
			}

			this.elements.Add( item );
			return this.elements.Count - 1;
		}

		public void Clear()
		{
			this.elements.Clear();
			this.freeIndices.Clear();
		}

		public bool Contains(T item)
		{
			return this.elements.Contains( item );
		}

		public T[] ToArray()
		{
			var array = new T[this.Count];
			this.CopyTo(array, 0);
			return array;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
				throw new ArgumentNullException( nameof(array) );

			int count = this.Count;
			if (arrayIndex + count > array.Length)
				throw new Exception( "Array was too small to hold all elements" );

			int offset = 0;
			for (int index = 0; index < this.elements.Count; index++)
			{
				if(this.freeIndices.Contains(index))
					continue;

				array[arrayIndex + offset] = this.elements[index];
				offset++;
			}
		}

		public bool Remove(T item)
		{
			int index = this.IndexOf( item );

			if (index == -1)
				return false;

			this.RemoveAt( index );
			return true;
		}

		public int Count => this.elements.Count - this.freeIndices.Count;

		public bool IsReadOnly => false;

		#endregion

		#region IList T

		public int IndexOf(T item)
		{
			for (int i = 0; i < this.elements.Count; i++)
			{
				if (this.freeIndices.Contains( i ))
					continue;

				if (Equals( this.elements[i], item ))
					return i;
			}

			return -1;
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			this.elements[index] = default;
			this.freeIndices.Push( index );
		}

		public T this[int index]
		{
			get => this.elements[index];
			set => this.elements[index] = value;
		}

		#endregion

		#region IEnumerable Kvp

		IEnumerator<KeyValuePair<int, T>> IEnumerable<KeyValuePair<int, T>>.GetEnumerator()
		{
			return this.KeyValuePairEnumerable()
			           .GetEnumerator();
		}

		private IEnumerable<KeyValuePair<int, T>> KeyValuePairEnumerable()
		{
			return this.elements
			           .Where( (_, i) => this.freeIndices.Contains( i ) == false )
			           .Select( (x, i) => new KeyValuePair<int, T>( i, x ) );
		}

		#endregion

		#region IReadonlyDictionary int T

		bool IReadOnlyDictionary<int, T>.ContainsKey(int key)
			=> this.ContainsKey( key );

		private bool ContainsKey(int key)
		{
			if (key < 0 || key >= this.elements.Count)
				return false;

			return this.freeIndices.Contains( key ) == false;
		}

		public bool TryGetValue(int key, out T value)
		{
			if (this.ContainsKey( key ))
			{
				value = this.elements[key];
				return true;
			}

			value = default;
			return false;
		}

		IEnumerable<int> IReadOnlyDictionary<int, T>.Keys => Enumerable.Range( 0, this.elements.Count )
		                                                               .Where( i => this.freeIndices.Contains( i ) == false );
		IEnumerable<T> IReadOnlyDictionary<int, T>.Values => this.KeyValuePairEnumerable()
		                                                         .Select( kvp => kvp.Value );

		#endregion
	}
}