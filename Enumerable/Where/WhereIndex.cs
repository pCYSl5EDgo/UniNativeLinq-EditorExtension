using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class WhereIndexEnumerable
    {
        public static
            WhereIndexEnumerable<
                TEnumerable,
                TEnumerator,
                TSource,
                TPredicate
            >
            WhereIndex<TEnumerable, TEnumerator, TSource, TPredicate>
            (ref this TEnumerable enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<TEnumerable, TEnumerator, TSource, TPredicate>(enumerable, predicate);

        public static
            WhereIndexEnumerable<
                TEnumerable,
                TEnumerator,
                TSource,
                DefaultSkipIndex<TSource>
            >
            Skip<TEnumerable, TEnumerator, TSource>
            (ref this TEnumerable enumerable, long count)
            where TSource : unmanaged
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new WhereIndexEnumerable<TEnumerable, TEnumerator, TSource, DefaultSkipIndex<TSource>>(enumerable, new DefaultSkipIndex<TSource>(count));
        
        public static
            WhereIndexEnumerable<
                TEnumerable,
                TEnumerator,
                TSource,
                DefaultTakeIndex<TSource>
            >
            Take<TEnumerable, TEnumerator, TSource>
            (ref this TEnumerable enumerable, long count)
            where TSource : unmanaged
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new WhereIndexEnumerable<TEnumerable, TEnumerator, TSource, DefaultTakeIndex<TSource>>(enumerable, new DefaultTakeIndex<TSource>(count));
        
        public static
            WhereIndexEnumerable<
                TEnumerable,
                TEnumerator,
                TSource,
                DefaultSkipWhileIndex<TSource, TPredicate>
            >
            SkipWhileIndex<TEnumerable, TEnumerator, TSource, TPredicate>
            (ref this TEnumerable enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<TEnumerable, TEnumerator, TSource, DefaultSkipWhileIndex<TSource, TPredicate>>(enumerable, new DefaultSkipWhileIndex<TSource, TPredicate>(predicate));

        public static
            WhereIndexEnumerable<
                TEnumerable,
                TEnumerator,
                TSource,
                DefaultTakeWhileIndex<TSource, TPredicate>
            >
            TakeWhileIndex<TEnumerable, TEnumerator, TSource, TPredicate>
            (ref this TEnumerable enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<TEnumerable, TEnumerator, TSource, DefaultTakeWhileIndex<TSource, TPredicate>>(enumerable, new DefaultTakeWhileIndex<TSource, TPredicate>(predicate));

        public static
            WhereIndexEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, TPredicate>
            WhereIndex<TSource, TPredicate>(this NativeArray<TSource> enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, TPredicate>(enumerable.AsRefEnumerable(), predicate);

        public static
            WhereIndexEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, DefaultSkipWhileIndex<TSource, TPredicate>>
            SkipWhileIndex<TSource, TPredicate>(this NativeArray<TSource> enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, DefaultSkipWhileIndex<TSource, TPredicate>>(enumerable.AsRefEnumerable(), new DefaultSkipWhileIndex<TSource, TPredicate>(predicate));

        public static
            WhereIndexEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, DefaultTakeWhileIndex<TSource, TPredicate>>
            TakeWhileIndex<TSource, TPredicate>(this NativeArray<TSource> enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, DefaultTakeWhileIndex<TSource, TPredicate>>(enumerable.AsRefEnumerable(), new DefaultTakeWhileIndex<TSource, TPredicate>(predicate));


        public static
            WhereIndexEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, TPredicate>
            WhereIndex<TSource, TPredicate>(this TSource[] enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, TPredicate>(enumerable.AsRefEnumerable(), predicate);

        public static
            WhereIndexEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, DefaultSkipWhileIndex<TSource, TPredicate>>
            SkipWhileIndex<TSource, TPredicate>(this TSource[] enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, DefaultSkipWhileIndex<TSource, TPredicate>>(enumerable.AsRefEnumerable(), new DefaultSkipWhileIndex<TSource, TPredicate>(predicate));

        public static
            WhereIndexEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, DefaultTakeWhileIndex<TSource, TPredicate>>
            TakeWhileIndex<TSource, TPredicate>(this TSource[] enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, DefaultTakeWhileIndex<TSource, TPredicate>>(enumerable.AsRefEnumerable(), new DefaultTakeWhileIndex<TSource, TPredicate>(predicate));

        public static Single Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            where TPredicate : unmanaged, IWhereIndex<Single>
            => enumerable.AverageSingle<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, float, TPredicate>.Enumerator>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            where TPredicate : unmanaged, IWhereIndex<Double>
            => enumerable.AverageDouble<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, double, TPredicate>.Enumerator>();

        public static Decimal Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            where TPredicate : unmanaged, IWhereIndex<Decimal>
            => enumerable.AverageDecimal<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, decimal, TPredicate>.Enumerator>();

        public static Int32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            where TPredicate : unmanaged, IWhereIndex<Int32>
            => enumerable.AverageInt32<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, int, TPredicate>.Enumerator>();

        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            where TPredicate : unmanaged, IWhereIndex<UInt32>
            => enumerable.AverageUInt32<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, uint, TPredicate>.Enumerator>();

        public static Int64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            where TPredicate : unmanaged, IWhereIndex<Int64>
            => enumerable.AverageInt64<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, long, TPredicate>.Enumerator>();

        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            where TPredicate : unmanaged, IWhereIndex<UInt64>
            => enumerable.AverageUInt64<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, ulong, TPredicate>.Enumerator>();

        public static Single Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            where TPredicate : unmanaged, IWhereIndex<Single>
            => enumerable.SumSingle<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, float, TPredicate>.Enumerator>();

        public static Double Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            where TPredicate : unmanaged, IWhereIndex<Double>
            => enumerable.SumDouble<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, double, TPredicate>.Enumerator>();

        public static Decimal Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            where TPredicate : unmanaged, IWhereIndex<Decimal>
            => enumerable.SumDecimal<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, decimal, TPredicate>.Enumerator>();

        public static Int32 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            where TPredicate : unmanaged, IWhereIndex<Int32>
            => enumerable.SumInt32<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, int, TPredicate>.Enumerator>();

        public static UInt32 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            where TPredicate : unmanaged, IWhereIndex<UInt32>
            => enumerable.SumUInt32<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, uint, TPredicate>.Enumerator>();

        public static Int64 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            where TPredicate : unmanaged, IWhereIndex<Int64>
            => enumerable.SumInt64<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, long, TPredicate>.Enumerator>();

        public static UInt64 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            where TPredicate : unmanaged, IWhereIndex<UInt64>
            => enumerable.SumUInt64<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, ulong, TPredicate>.Enumerator>();
    }
}