using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class WhereIndexEnumerable
    {
        public static
            WhereIndexEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, TPredicate>
            WhereIndex<TSource, TPredicate>(this NativeArray<TSource> enumerable, TPredicate predicate)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource, TPredicate>(enumerable.AsRefEnumerable(), predicate);

#if UNSAFE_ARRAY_ENUMERABLE
        public static
            WhereIndexEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, TPredicate>
            WhereIndex<TSource, TPredicate>(this TSource[] enumerable, TPredicate predicate)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPredicate : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource, TPredicate>(enumerable.AsRefEnumerable(), predicate);
#endif
        
        public static Single Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            where TPredicate : unmanaged, IWhereIndex<Single>
            => @this.AverageSingle<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, float, TPredicate>.Enumerator>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            where TPredicate : unmanaged, IWhereIndex<Double>
            => @this.AverageDouble<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, double, TPredicate>.Enumerator>();

        public static Decimal Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            where TPredicate : unmanaged, IWhereIndex<Decimal>
            => @this.AverageDecimal<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, decimal, TPredicate>.Enumerator>();

        public static Int32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            where TPredicate : unmanaged, IWhereIndex<Int32>
            => @this.AverageInt32<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, int, TPredicate>.Enumerator>();

        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            where TPredicate : unmanaged, IWhereIndex<UInt32>
            => @this.AverageUInt32<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, uint, TPredicate>.Enumerator>();

        public static Int64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            where TPredicate : unmanaged, IWhereIndex<Int64>
            => @this.AverageInt64<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, long, TPredicate>.Enumerator>();

        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            where TPredicate : unmanaged, IWhereIndex<UInt64>
            => @this.AverageUInt64<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, ulong, TPredicate>.Enumerator>();
        
        public static Single Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            where TPredicate : unmanaged, IWhereIndex<Single>
            => @this.SumSingle<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, float, TPredicate>.Enumerator>();

        public static Double Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            where TPredicate : unmanaged, IWhereIndex<Double>
            => @this.SumDouble<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, double, TPredicate>.Enumerator>();

        public static Decimal Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            where TPredicate : unmanaged, IWhereIndex<Decimal>
            => @this.SumDecimal<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, decimal, TPredicate>.Enumerator>();

        public static Int32 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            where TPredicate : unmanaged, IWhereIndex<Int32>
            => @this.SumInt32<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, int, TPredicate>.Enumerator>();

        public static UInt32 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            where TPredicate : unmanaged, IWhereIndex<UInt32>
            => @this.SumUInt32<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, uint, TPredicate>.Enumerator>();

        public static Int64 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            where TPredicate : unmanaged, IWhereIndex<Int64>
            => @this.SumInt64<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, long, TPredicate>.Enumerator>();

        public static UInt64 Sum<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            where TPredicate : unmanaged, IWhereIndex<UInt64>
            => @this.SumUInt64<WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate>, WhereIndexEnumerable<TPrevEnumerable, TPrevEnumerator, ulong, TPredicate>.Enumerator>();
    }
}