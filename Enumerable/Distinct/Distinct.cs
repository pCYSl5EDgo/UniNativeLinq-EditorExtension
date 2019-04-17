using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class DistinctEnumerable
    {
        public static DistinctEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct<TSource>(this NativeArray<TSource> array, Allocator allocator = Allocator.Temp)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            => new DistinctEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(array.AsRefEnumerable(), default, default, allocator);

#if UNSAFE_ARRAY_ENUMERABLE
        public static DistinctEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct<TSource>(this TSource[] array, Allocator allocator = Allocator.Temp)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            => new DistinctEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(array.AsRefEnumerable(), default, default, allocator);
#endif
    }
}