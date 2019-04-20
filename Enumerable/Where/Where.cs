using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class WhereEnumerable
    {
        public static WhereEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TPredicate> Where<T, TPredicate>(this NativeArray<T> array, TPredicate predicate)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => new WhereEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TPredicate>(array.AsRefEnumerable(), predicate);
        
#if UNSAFE_ARRAY_ENUMERABLE
        public static
            WhereEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, TPredicate>
            Where<TSource, TPredicate>(this TSource[] enumerable, TPredicate predicate)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPredicate : struct, IRefFunc<TSource, bool>
            => new WhereEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, TPredicate>(enumerable.AsRefEnumerable(), predicate);
#endif

        public static Single Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            where TPredicate : unmanaged, IRefFunc<Single, bool>
            => @this.AverageSingle<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, float, TPredicate>.Enumerator>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            where TPredicate : unmanaged, IRefFunc<Double, bool>
            => @this.AverageDouble<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, double, TPredicate>.Enumerator>();

        public static Decimal Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            where TPredicate : unmanaged, IRefFunc<Decimal, bool>
            => @this.AverageDecimal<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, decimal, TPredicate>.Enumerator>();

        public static Int32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            where TPredicate : unmanaged, IRefFunc<Int32, bool>
            => @this.AverageInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, int, TPredicate>.Enumerator>();

        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            where TPredicate : unmanaged, IRefFunc<UInt32, bool>
            => @this.AverageUInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, uint, TPredicate>.Enumerator>();

        public static Int64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            where TPredicate : unmanaged, IRefFunc<Int64, bool>
            => @this.AverageInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, long, TPredicate>.Enumerator>();

        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            where TPredicate : unmanaged, IRefFunc<UInt64, bool>
            => @this.AverageUInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, ulong, TPredicate>.Enumerator>();
        
        public static Single Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            where TPredicate : unmanaged, IRefFunc<Single, bool>
            => @this.SumSingle<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, float, TPredicate>.Enumerator>();

        public static Double Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            where TPredicate : unmanaged, IRefFunc<Double, bool>
            => @this.SumDouble<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, double, TPredicate>.Enumerator>();

        public static Decimal Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            where TPredicate : unmanaged, IRefFunc<Decimal, bool>
            => @this.SumDecimal<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, decimal, TPredicate>.Enumerator>();

        public static Int32 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            where TPredicate : unmanaged, IRefFunc<Int32, bool>
            => @this.SumInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, int, TPredicate>.Enumerator>();

        public static UInt32 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            where TPredicate : unmanaged, IRefFunc<UInt32, bool>
            => @this.SumUInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, uint, TPredicate>.Enumerator>();

        public static Int64 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            where TPredicate : unmanaged, IRefFunc<Int64, bool>
            => @this.SumInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, long, TPredicate>.Enumerator>();

        public static UInt64 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            where TPredicate : unmanaged, IRefFunc<UInt64, bool>
            => @this.SumUInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate>, WhereEnumerable<TPrevEnumerable, TPrevEnumerator, ulong, TPredicate>.Enumerator>();
    }
}