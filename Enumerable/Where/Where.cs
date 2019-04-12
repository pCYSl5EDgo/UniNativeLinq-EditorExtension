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

        public static Single Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            where TPredicate : unmanaged, IRefFunc<Single, bool>
            => @this.AverageSingle<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Single, TPredicate>, WhereEnumerator<TPrevEnumerator, Single, TPredicate>>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            where TPredicate : unmanaged, IRefFunc<Double, bool>
            => @this.AverageDouble<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Double, TPredicate>, WhereEnumerator<TPrevEnumerator, Double, TPredicate>>();

        public static Decimal Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            where TPredicate : unmanaged, IRefFunc<Decimal, bool>
            => @this.AverageDecimal<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal, TPredicate>, WhereEnumerator<TPrevEnumerator, Decimal, TPredicate>>();

        public static Int32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            where TPredicate : unmanaged, IRefFunc<Int32, bool>
            => @this.AverageInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int32, TPredicate>, WhereEnumerator<TPrevEnumerator, Int32, TPredicate>>();

        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            where TPredicate : unmanaged, IRefFunc<UInt32, bool>
            => @this.AverageUInt32<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32, TPredicate>, WhereEnumerator<TPrevEnumerator, UInt32, TPredicate>>();

        public static Int64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            where TPredicate : unmanaged, IRefFunc<Int64, bool>
            => @this.AverageInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, Int64, TPredicate>, WhereEnumerator<TPrevEnumerator, Int64, TPredicate>>();

        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator, TPredicate>(ref this WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            where TPredicate : unmanaged, IRefFunc<UInt64, bool>
            => @this.AverageUInt64<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64, TPredicate>, WhereEnumerator<TPrevEnumerator, UInt64, TPredicate>>();
    }
}