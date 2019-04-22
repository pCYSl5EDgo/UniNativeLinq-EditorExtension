using System.Linq;

namespace pcysl5edgo.Collections.LINQ
{
    public interface IRefOrderedEnumerable<TEnumerator, TSource>
        : IRefEnumerable<TEnumerator, TSource>, IOrderedEnumerable<TSource>
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TSource>
    {
        TEnumerable0 CreateRefOrderedEnumerable<TEnumerable0, TEnumerator0, TKey, TKeySelector, TComparer>(TKeySelector keySelector, TComparer comparer)
            where TKey : unmanaged
            where TKeySelector : struct, IRefAction<TSource, TKey>
            where TComparer : struct, IRefFunc<TKey, TKey, int>
            where TEnumerator0 : struct, IRefEnumerator<TKey>
            where TEnumerable0 : struct, IRefOrderedEnumerable<TEnumerator0, TKey>;
    }
}