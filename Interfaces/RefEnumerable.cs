using System.Collections.Generic;

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
    }
}