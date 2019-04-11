using System.Collections.Generic;

namespace pcysl5edgo.Collections
{
    public interface IRefEnumerable<TEnumerator, T> : IEnumerable<T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
    {
        new TEnumerator GetEnumerator();
    }
}