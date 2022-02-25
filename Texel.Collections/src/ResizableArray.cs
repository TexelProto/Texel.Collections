using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static System.Array;

namespace Texel.Collections
{
	/// <summary>
	/// A small wrapper for an array to make it seem as if the internal array is resizable.
	/// This class implements <see cref="IList{T}"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ResizableArray<T> : IList<T>
	{
		private class Enumerator : IEnumerator<T>
		{
			private readonly ResizableArray<T> array;
			private int index = -1;

			public T Current => this.array[this.index];
			object IEnumerator.Current => this.Current;

			public Enumerator(ResizableArray<T> array)
			{
				this.array = array;
			}

			public bool MoveNext()
			{
				this.index++;
				return this.index < this.array.Count;
			}

			public void Reset()
			{
				throw new NotSupportedException();
			}

			public void Dispose() { }
		}

		private const int DEFAULT_CAPACITY = 4;

		private int capacity;
		private T[] array;

		public T[] Array => this.array;

		public int TrueLength => this.array.Length;

		public int Count
		{
			get => this.capacity;
			set => this.SetCapacity( value );
		}

		public ResizableArray() : this( DEFAULT_CAPACITY )
		{
			this.capacity = 0;
		}

		public ResizableArray(int capacity)
		{
			this.capacity = capacity;
			this.array = new T[capacity];
		}

		public void SetCapacity(int value) => this.SetCapacity( value, true );

		public void SetCapacity(int value, bool preserveContent)
		{
			//no capacity change
			if (value == this.capacity)
				return;

			//the array shrunk clear any entries exceeding range
			if (value < this.capacity)
			{
				int range = this.capacity - value;
				Clear( this.array, value, range );
			}

			//array is too small for capacity => resize it
			if (value > this.array.Length)
			{
				if (preserveContent)
					Resize( ref this.array, value );
				else
					this.array = new T[value];
			}

			this.capacity = value;
		}

		#region IEnumerable

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator( this );
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		#endregion

		#region ICollection

		void ICollection<T>.Add(T item)
		{
			throw new NotSupportedException();
		}

		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(T item)
		{
			return this.array.Contains( item );
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.array.CopyTo( array, arrayIndex );
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new NotSupportedException();
		}

		bool ICollection<T>.IsReadOnly => this.array.IsReadOnly;

		#endregion

		#region IList

		public int IndexOf(T item)
		{
			return System.Array.IndexOf( this.array, item );
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		public T this[int index]
		{
			get => this.array[index];
			set => this.array[index] = value;
		}

		#endregion
	}
}