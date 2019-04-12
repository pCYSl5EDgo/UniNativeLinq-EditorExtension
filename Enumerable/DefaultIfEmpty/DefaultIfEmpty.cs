using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class DefaultIfEmptyEnumerable
    {
        public static DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource> DefaultIfEmpty<TEnumerable, TEnumerator, TSource>(ref this TEnumerable source, in TEnumerator defaultEnumerator,in TSource defaultValue, Allocator allocator = Allocator.Temp)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        => new DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>(source, defaultValue, allocator);

        public unsafe static DefaultIfEmptyPointerEnumerable<TEnumerable, TEnumerator, TSource> DefaultIfEmpty<TEnumerable, TEnumerator, TSource>(ref this TEnumerable source, in TEnumerator defaultEnumerator,TSource* defaultValue)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        => new DefaultIfEmptyPointerEnumerable<TEnumerable, TEnumerator, TSource>(source, defaultValue);
    }
}