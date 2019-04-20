using System.Collections.Generic;

namespace pcysl5edgo.Collections
{
    public interface IRefEnumerator<T> : IEnumerator<T>
        where T : unmanaged
    {
        new ref T Current { get; }
    }
}