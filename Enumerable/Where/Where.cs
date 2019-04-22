using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class WhereEnumerable
    {
        public static
            WhereEnumerable<
                TEnumerable,
                TEnumerator,
                TSource,
                TPredicate
            >
            Where<TEnumerable, TEnumerator, TSource, TPredicate>
            (ref this TEnumerable enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new WhereEnumerable<TEnumerable, TEnumerator, TSource, TPredicate>(enumerable, predicate);

        public static
            WhereEnumerable<
                TEnumerable,
                TEnumerator,
                TSource,
                DefaultSkipWhile<TSource, TPredicate>
            >
            SkipWhile<TEnumerable, TEnumerator, TSource, TPredicate>
            (ref this TEnumerable enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new WhereEnumerable<TEnumerable, TEnumerator, TSource, DefaultSkipWhile<TSource, TPredicate>>(enumerable, new DefaultSkipWhile<TSource, TPredicate>(predicate));

        public static
            WhereEnumerable<
                TEnumerable,
                TEnumerator,
                TSource,
                DefaultTakeWhile<TSource, TPredicate>
            >
            TakeWhile<TEnumerable, TEnumerator, TSource, TPredicate>
            (ref this TEnumerable enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new WhereEnumerable<TEnumerable, TEnumerator, TSource, DefaultTakeWhile<TSource, TPredicate>>(enumerable, new DefaultTakeWhile<TSource, TPredicate>(predicate));

        public static WhereEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TPredicate> Where<T, TPredicate>(this NativeArray<T> array, TPredicate predicate)
            where T : unmanaged
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => new WhereEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TPredicate>(array.AsRefEnumerable(), predicate);

        public static
            NativeEnumerable<T>
            Skip<T>(this NativeArray<T> enumerable, long count)
            where T : unmanaged
            => new NativeEnumerable<T>(enumerable, count, enumerable.Length - count);

        public static
            NativeEnumerable<T>
            Take<T>(this NativeArray<T> enumerable, long count)
            where T : unmanaged
            => new NativeEnumerable<T>(enumerable, 0, count);

        public static
            NativeEnumerable<T>
            SkipLast<T>(this NativeArray<T> enumerable, long count)
            where T : unmanaged
            => new NativeEnumerable<T>(enumerable, 0, enumerable.Length - count);

        public static
            NativeEnumerable<T>
            TakeLast<T>(this NativeArray<T> enumerable, long count)
            where T : unmanaged
            => new NativeEnumerable<T>(enumerable, enumerable.Length - count, count);

        public static WhereEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultSkipWhile<T, TPredicate>
            >
            SkipWhile<T, TPredicate>(this NativeArray<T> enumerable, TPredicate predicate)
            where T : unmanaged
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => new WhereEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DefaultSkipWhile<T, TPredicate>>(enumerable.AsRefEnumerable(), new DefaultSkipWhile<T, TPredicate>(predicate));

        public static WhereEnumerable<
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T,
                DefaultTakeWhile<T, TPredicate>
            >
            TakeWhile<T, TPredicate>(this NativeArray<T> enumerable, TPredicate predicate)
            where T : unmanaged
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => new WhereEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, DefaultTakeWhile<T, TPredicate>>(enumerable.AsRefEnumerable(), new DefaultTakeWhile<T, TPredicate>(predicate));


        public static
            WhereEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, TPredicate>
            Where<TSource, TPredicate>(this TSource[] enumerable, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
            => new WhereEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, TPredicate>(enumerable.AsRefEnumerable(), predicate);

        public static
            ArrayEnumerable<T>
            Skip<T>(this T[] enumerable, long count)
            where T : unmanaged
            => new ArrayEnumerable<T>(enumerable, count, enumerable.Length - count);

        public static
            ArrayEnumerable<T>
            Take<T>(this T[] enumerable, long count)
            where T : unmanaged
            => new ArrayEnumerable<T>(enumerable, 0, count);

        public static
            ArrayEnumerable<T>
            SkipLast<T>(this T[] enumerable, long count)
            where T : unmanaged
            => new ArrayEnumerable<T>(enumerable, 0, enumerable.Length - count);

        public static
            ArrayEnumerable<T>
            TakeLast<T>(this T[] enumerable, long count)
            where T : unmanaged
            => new ArrayEnumerable<T>(enumerable, enumerable.Length - count, count);

        public static WhereEnumerable<
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                DefaultSkipWhile<T, TPredicate>
            >
            SkipWhile<T, TPredicate>(this T[] enumerable, TPredicate predicate)
            where T : unmanaged
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => new WhereEnumerable<ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, DefaultSkipWhile<T, TPredicate>>(enumerable.AsRefEnumerable(), new DefaultSkipWhile<T, TPredicate>(predicate));

        public static WhereEnumerable<
                ArrayEnumerable<T>,
                ArrayEnumerable<T>.Enumerator,
                T,
                DefaultTakeWhile<T, TPredicate>
            >
            TakeWhile<T, TPredicate>(this T[] enumerable, TPredicate predicate)
            where T : unmanaged
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => new WhereEnumerable<ArrayEnumerable<T>, ArrayEnumerable<T>.Enumerator, T, DefaultTakeWhile<T, TPredicate>>(enumerable.AsRefEnumerable(), new DefaultTakeWhile<T, TPredicate>(predicate));

        public static Single Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            where TPredicate : unmanaged, IRefFunc<Single, bool>
            => enumerable.AverageSingle<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, float, TPredicate>.Enumerator>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            where TPredicate : unmanaged, IRefFunc<Double, bool>
            => enumerable.AverageDouble<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, double, TPredicate>.Enumerator>();

        public static Decimal Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            where TPredicate : unmanaged, IRefFunc<Decimal, bool>
            => enumerable.AverageDecimal<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, decimal, TPredicate>.Enumerator>();

        public static Int32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            where TPredicate : unmanaged, IRefFunc<Int32, bool>
            => enumerable.AverageInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, int, TPredicate>.Enumerator>();

        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            where TPredicate : unmanaged, IRefFunc<UInt32, bool>
            => enumerable.AverageUInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, uint, TPredicate>.Enumerator>();

        public static Int64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            where TPredicate : unmanaged, IRefFunc<Int64, bool>
            => enumerable.AverageInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, long, TPredicate>.Enumerator>();

        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            where TPredicate : unmanaged, IRefFunc<UInt64, bool>
            => enumerable.AverageUInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, ulong, TPredicate>.Enumerator>();

        public static Single Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            where TPredicate : unmanaged, IRefFunc<Single, bool>
            => enumerable.SumSingle<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, float, TPredicate>.Enumerator>();

        public static Double Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            where TPredicate : unmanaged, IRefFunc<Double, bool>
            => enumerable.SumDouble<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, double, TPredicate>.Enumerator>();

        public static Decimal Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            where TPredicate : unmanaged, IRefFunc<Decimal, bool>
            => enumerable.SumDecimal<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, decimal, TPredicate>.Enumerator>();

        public static Int32 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            where TPredicate : unmanaged, IRefFunc<Int32, bool>
            => enumerable.SumInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, int, TPredicate>.Enumerator>();

        public static UInt32 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            where TPredicate : unmanaged, IRefFunc<UInt32, bool>
            => enumerable.SumUInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, uint, TPredicate>.Enumerator>();

        public static Int64 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            where TPredicate : unmanaged, IRefFunc<Int64, bool>
            => enumerable.SumInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, long, TPredicate>.Enumerator>();

        public static UInt64 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate> enumerable)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            where TPredicate : unmanaged, IRefFunc<UInt64, bool>
            => enumerable.SumUInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, ulong, TPredicate>.Enumerator>();
    }
}