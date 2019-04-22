using System.Collections.Generic;
using pcysl5edgo.Collections.LINQ;
using Unity.Collections;

namespace pcysl5edgo.Collections
{
    public interface IRefEnumerable<TEnumerator, T> : IEnumerable<T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
    {
        new TEnumerator GetEnumerator();

        bool CanFastCount();
        bool Any();
        int Count();
        long LongCount();

        bool TryGetFirst(out T first);
        bool TryGetLast(out T last);
        bool TryGetElementAt(long index, out T element);

        NativeEnumerable<T> ToNativeEnumerable(Allocator allocator);
        NativeArray<T> ToNativeArray(Allocator allocator);
        T[] ToArray();
        HashSet<T> ToHashSet();
        HashSet<T> ToHashSet(IEqualityComparer<T> comparer);
    }
}