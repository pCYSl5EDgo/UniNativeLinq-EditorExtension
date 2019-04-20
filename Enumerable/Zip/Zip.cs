using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class ZipEnumerable
    {
        public static
            ZipEnumerable<
                TFirstEnumerable,
                TFirstEnumerator,
                TFirstSource,
                TSecondEnumerable,
                TSecondEnumerator,
                TSecondSource,
                TSource,
                TAction
            >
            Zip<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>
            (ref this TFirstEnumerable firstEnumerable, in TSecondEnumerable secondEnumerable, TAction action, TFirstSource firstDefaultValue = default, TSecondSource secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, TSource>
            => new ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>(firstEnumerable, secondEnumerable, action, firstDefaultValue, secondDefaultValue, allocator);

        public static
            ZipEnumerable<
                NativeEnumerable<TFirstSource>,
                NativeEnumerable<TFirstSource>.Enumerator,
                TFirstSource,
                TSecondEnumerable,
                TSecondEnumerator,
                TSecondSource,
                TSource,
                TAction
            >
            Zip<TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>
            (this NativeArray<TFirstSource> first, in TSecondEnumerable second, TAction action, TFirstSource firstDefaultValue = default, TSecondSource secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, TSource>
            => new ZipEnumerable<NativeEnumerable<TFirstSource>, NativeEnumerable<TFirstSource>.Enumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>(first.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue, allocator);

#if UNSAFE_ARRAY_ENUMERABLE
        public static
            ZipEnumerable<
                ArrayEnumerable<TFirstSource>,
                ArrayEnumerable<TFirstSource>.Enumerator,
                TFirstSource,
                TSecondEnumerable,
                TSecondEnumerator,
                TSecondSource,
                TSource,
                TAction
            >
            Zip<TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>
            (this TFirstSource[] first, in TSecondEnumerable second, TAction action, TFirstSource firstDefaultValue = default, TSecondSource secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, TSource>
#endif
            => new ZipEnumerable<ArrayEnumerable<TFirstSource>, ArrayEnumerable<TFirstSource>.Enumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>(first.AsRefEnumerable(), second, action, firstDefaultValue, secondDefaultValue, allocator);

        public static Single
            Sum<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Single, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Single>
            => enumerable.SumSingle<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Single, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, float, TAction>.Enumerator>();

        public static Double
            Sum<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Double, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Double>
            => enumerable.SumDouble<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Double, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, double, TAction>.Enumerator>();

        public static Decimal
            Sum<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Decimal, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Decimal>
            => enumerable.SumDecimal<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Decimal, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, decimal, TAction>.Enumerator>();

        public static Int32
            Sum<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Int32, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Int32>
            => enumerable.SumInt32<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Int32, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, int, TAction>.Enumerator>();

        public static Int64
            Sum<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Int64, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Int64>
            => enumerable.SumInt64<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Int64, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, long, TAction>.Enumerator>();

        public static UInt64
            Sum<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, UInt64, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, UInt64>
            => enumerable.SumUInt64<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, UInt64, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, ulong, TAction>.Enumerator>();

        public static UInt32
            Sum<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, UInt32, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, UInt32>
            => enumerable.SumUInt32<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, UInt32, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, uint, TAction>.Enumerator>();
        
        public static Single
            Average<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Single, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Single>
            => enumerable.AverageSingle<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Single, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, float, TAction>.Enumerator>();
        
        public static Double
            Average<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Double, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Double>
            => enumerable.AverageDouble<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Double, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, double, TAction>.Enumerator>();
        
        public static Decimal
            Average<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Decimal, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Decimal>
            => enumerable.AverageDecimal<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Decimal, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, decimal, TAction>.Enumerator>();
        
        public static Int32
            Average<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Int32, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Int32>
            => enumerable.AverageInt32<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Int32, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, int, TAction>.Enumerator>();
        
        public static Int64
            Average<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Int64, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, Int64>
            => enumerable.AverageInt64<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, Int64, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, long, TAction>.Enumerator>();
        
        public static UInt32
            Average<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, UInt32, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, UInt32>
            => enumerable.AverageUInt32<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, UInt32, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, uint, TAction>.Enumerator>();
        
        public static UInt64
            Average<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TAction>
            (ref this ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, UInt64, TAction> enumerable)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
            where TFirstSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TFirstSource>
#endif
            where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
            where TSecondSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSecondSource>
#endif
            where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
            where TAction : struct, IRefAction<TFirstSource, TSecondSource, UInt64>
            => enumerable.AverageUInt64<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, UInt64, TAction>, ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, ulong, TAction>.Enumerator>();
    }
}