using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class SelectIndexEnumerable
    {
        public static SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>
            SelectIndex<T, TResult, TAction>(this NativeArray<T> array, TAction action, Allocator allocator = Allocator.Temp)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<T, TResult>
            => new SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>(array.AsRefEnumerable(), action, allocator);

        public static SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>
            SelectIndex<T, TResult, TAction>(this NativeEnumerable<T> enumerable, TAction action, Allocator allocator = Allocator.Temp)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<T, TResult>
            => new SelectIndexEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>(enumerable, action, allocator);

        public static Single Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Single, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, ISelectIndex<TPrevSource, Single>
            => @this.AverageSingle<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Single, TAction>, SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Single, TAction>.Enumerator>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, ISelectIndex<TPrevSource, Double>
            => @this.AverageDouble<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction>, SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction>.Enumerator>();

        public static Decimal Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, ISelectIndex<TPrevSource, Decimal>
            => @this.AverageDecimal<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction>, SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction>.Enumerator>();

        public static Int32 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, ISelectIndex<TPrevSource, Int32>
            => @this.AverageInt32<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction>, SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction>.Enumerator>();

        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, ISelectIndex<TPrevSource, UInt32>
            => @this.AverageUInt32<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction>, SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction>.Enumerator>();

        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, ISelectIndex<TPrevSource, UInt64>
            => @this.AverageUInt64<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction>, SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction>.Enumerator>();

        public static Int64 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction> @this)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TPrevEnumerator : unmanaged, IRefEnumerator<TPrevSource>
            where TAction : unmanaged, ISelectIndex<TPrevSource, Int64>
            => @this.AverageInt64<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction>, SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction>.Enumerator>();
    }
}