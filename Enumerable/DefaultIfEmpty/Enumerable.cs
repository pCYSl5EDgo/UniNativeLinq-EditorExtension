using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public struct DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, T> : IRefEnumerable<DefaultIfEmptyEnumerator<TEnumerator, T>, T>
        where T : unmanaged
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        where TEnumerator : struct, IRefEnumerator<T>
    {
        private TEnumerable source;
        private readonly T value;
        private readonly Allocator allocator;

        internal DefaultIfEmptyEnumerable(in TEnumerable source, in T value, Allocator allocator)
        {
            this.source = source;
            this.value = value;
            this.allocator = allocator;
        }

        public DefaultIfEmptyEnumerator<TEnumerator, T> GetEnumerator() =>
            new DefaultIfEmptyEnumerator<TEnumerator, T>(source.GetEnumerator(), value, allocator);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public unsafe struct DefaultIfEmptyPointerEnumerable<TEnumerable, TEnumerator, T> : IRefEnumerable<DefaultIfEmptyEnumerator<TEnumerator, T>, T>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, T>
        where TEnumerator : struct, IRefEnumerator<T>
        where T : unmanaged
    {
        private TEnumerable source;
        private readonly T* value;

        internal DefaultIfEmptyPointerEnumerable(in TEnumerable source, T* value)
        {
            this.source = source;
            this.value = value;
        }

        public DefaultIfEmptyEnumerator<TEnumerator, T> GetEnumerator() =>
            new DefaultIfEmptyEnumerator<TEnumerator, T>(source.GetEnumerator(), value);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}