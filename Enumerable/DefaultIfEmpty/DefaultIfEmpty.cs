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
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>(enumerable, defaultValue, allocator);

        public static
            DefaultIfEmptyEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>
            DefaultIfEmpty<TSource>(this NativeArray<TSource> array, TSource defaultValue, Allocator allocator = Allocator.Temp)
            where TSource : unmanaged
            => new DefaultIfEmptyEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(array.AsRefEnumerable(), defaultValue, allocator);

        #region Average
        public static Single
            Average<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Single> values)
            where TEnumerator : struct, IRefEnumerator<Single>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Single>
            => values.AverageSingle<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Single>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, float>.Enumerator>
                ();

        public static Double
            Average<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Double> values)
            where TEnumerator : struct, IRefEnumerator<Double>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Double>
            => values.AverageDouble<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Double>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, double>.Enumerator>
                ();

        public static Decimal
            Average<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Decimal> values)
            where TEnumerator : struct, IRefEnumerator<Decimal>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Decimal>
            => values.AverageDecimal<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Decimal>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, decimal>.Enumerator>
                ();

        public static Int32
            Average<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Int32> values)
            where TEnumerator : struct, IRefEnumerator<Int32>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int32>
            => values.AverageInt32<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Int32>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, int>.Enumerator>
                ();

        public static Int64
            Average<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Int64> values)
            where TEnumerator : struct, IRefEnumerator<Int64>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int64>
            => values.AverageInt64<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Int64>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, long>.Enumerator>
                ();

        public static UInt32
            Average<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, UInt32> values)
            where TEnumerator : struct, IRefEnumerator<UInt32>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt32>
            => values.AverageUInt32<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, UInt32>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, uint>.Enumerator>
                ();

        public static UInt64
            Average<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, UInt64> values)
            where TEnumerator : struct, IRefEnumerator<UInt64>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt64>
            => values.AverageUInt64<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, UInt64>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, ulong>.Enumerator>
                ();
        #endregion Average

        #region Sum
        public static Single
            Sum<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Single> values)
            where TEnumerator : struct, IRefEnumerator<Single>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Single>
            => values.SumSingle<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Single>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, float>.Enumerator
                >
                ();

        public static Double
            Sum<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Double> values)
            where TEnumerator : struct, IRefEnumerator<Double>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Double>
            => values.SumDouble<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Double>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, double>.Enumerator
                >
                ();

        public static Decimal
            Sum<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Decimal> values)
            where TEnumerator : struct, IRefEnumerator<Decimal>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Decimal>
            => values.SumDecimal<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Decimal>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, decimal>.Enumerator
                >
                ();

        public static Int32
            Sum<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Int32> values)
            where TEnumerator : struct, IRefEnumerator<Int32>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int32>
            => values.SumInt32<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Int32>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, int>.Enumerator
                >
                ();

        public static Int64
            Sum<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Int64> values)
            where TEnumerator : struct, IRefEnumerator<Int64>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int64>
            => values.SumInt64<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, Int64>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, long>.Enumerator
                >
                ();

        public static UInt32
            Sum<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, UInt32> values)
            where TEnumerator : struct, IRefEnumerator<UInt32>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt32>
            => values.SumUInt32<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, UInt32>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, uint>.Enumerator
                >
                ();

        public static UInt64
            Sum<TEnumerable, TEnumerator>
            (ref this DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, UInt64> values)
            where TEnumerator : struct, IRefEnumerator<UInt64>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt64>
            => values.SumUInt64<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, UInt64>,
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, ulong>.Enumerator
                >
                ();
        #endregion
    }
}