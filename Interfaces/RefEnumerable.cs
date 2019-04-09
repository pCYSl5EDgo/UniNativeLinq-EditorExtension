using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections
{
    public interface IRefEnumerable<TEnumerator, T> : IEnumerable<T>
        where T : unmanaged
        where TEnumerator : struct, IRefEnumerator<T>
    {
        new TEnumerator GetEnumerator();
    }
}