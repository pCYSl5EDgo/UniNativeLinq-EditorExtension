namespace pcysl5edgo.Collections.LINQ
{
    public interface IRefGrouping<TKey, TEnumerator, TElement>
        : IRefEnumerable<TEnumerator, TElement>
        where TElement : unmanaged
        where TEnumerator : struct, IRefEnumerator<TElement>
    {
        ref TKey Key { get; }
    }
}