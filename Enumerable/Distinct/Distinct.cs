using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class DistinctEnumerable
    {
        public static DistinctEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct<TSource>(this NativeArray<TSource> array, Allocator allocator = Allocator.Temp)
            where TSource : unmanaged
            => new DistinctEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(array.AsRefEnumerable(), default, default, allocator);

        public static DistinctEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct<TSource>(this TSource[] array, Allocator allocator = Allocator.Temp)
            where TSource : unmanaged
            => new DistinctEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(array.AsRefEnumerable(), default, default, allocator);
    }
}