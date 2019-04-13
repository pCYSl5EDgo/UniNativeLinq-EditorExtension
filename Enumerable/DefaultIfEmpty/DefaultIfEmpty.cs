using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class DefaultIfEmptyEnumerable
    {
        public static
            DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>
            DefaultIfEmpty<TEnumerable, TEnumerator, TSource>
            (ref this TEnumerable enumerable, TSource defaultValue, Allocator allocator)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>(enumerable, defaultValue, allocator);

        public static
            DefaultIfEmptyEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>
            DefaultIfEmpty<TSource>(this NativeArray<TSource> array, TSource defaultValue, Allocator allocator = Allocator.Temp)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            => new DefaultIfEmptyEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(array.AsRefEnumerable(), defaultValue, allocator);
    }
}