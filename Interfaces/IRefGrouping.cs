using System;

namespace pcysl5edgo.Collections.LINQ
{
    public interface IRefGrouping<TKey, TEnumerator, TElement>
        : IRefEnumerable<TEnumerator, TElement>
        where TElement : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TElement>
#endif
        where TEnumerator : struct, IRefEnumerator<TElement>
    {
        ref TKey Key { get; }
    }
}