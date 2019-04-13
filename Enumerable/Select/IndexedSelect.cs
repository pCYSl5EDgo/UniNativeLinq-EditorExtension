using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class IndexedSelectEnumerable
    {
        public static IndexedSelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>
            SelectIndex<T, TResult, TAction>(this NativeArray<T> array, TAction action, Allocator allocator = Allocator.Temp)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IIndexedSelect<T, TResult>
            => new IndexedSelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>(array.AsRefEnumerable(), action, allocator);

        public static IndexedSelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>
            SelectIndex<T, TResult, TAction>(this NativeEnumerable<T> enumerable, TAction action, Allocator allocator = Allocator.Temp)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IIndexedSelect<T, TResult>
            => new IndexedSelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>(enumerable, action, allocator);

        public static Single Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Single, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, IIndexedSelect<TPrevSource, Single>
            => @this.AverageSingle<IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Single, TAction>, IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Single, TAction>.Enumerator>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, IIndexedSelect<TPrevSource, Double>
            => @this.AverageDouble<IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction>, IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction>.Enumerator>();

        public static Decimal Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, IIndexedSelect<TPrevSource, Decimal>
            => @this.AverageDecimal<IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction>, IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction>.Enumerator>();

        public static Int32 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, IIndexedSelect<TPrevSource, Int32>
            => @this.AverageInt32<IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction>, IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction>.Enumerator>();

        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, IIndexedSelect<TPrevSource, UInt32>
            => @this.AverageUInt32<IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction>, IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction>.Enumerator>();

        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, IIndexedSelect<TPrevSource, UInt64>
            => @this.AverageUInt64<IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction>, IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction>.Enumerator>();

        public static Int64 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, IIndexedSelect<TPrevSource, Int64>
            => @this.AverageInt64<IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction>, IndexedSelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction>.Enumerator>();
    }
}