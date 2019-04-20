using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class AppendEnumerable
    {
        public static AppendEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T> Append<T>(this NativeArray<T> array, T value, Allocator allocator = Allocator.Temp)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            => new AppendEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>(array.AsRefEnumerable(), value, allocator);

        public static Single Average<TPrevEnumerable, TPrevEnumerator>(ref this AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Single> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Single>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Single>
            => @this.AverageSingle<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Single>, AppendEnumerable<TPrevEnumerable, TPrevEnumerator, float>.Enumerator>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator>(ref this AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Double> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Double>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Double>
            => @this.AverageDouble<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Double>, AppendEnumerable<TPrevEnumerable, TPrevEnumerator, double>.Enumerator>();

        public static Decimal Average<TPrevEnumerable, TPrevEnumerator>(ref this AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Decimal>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Decimal>
            => @this.AverageDecimal<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Decimal>, AppendEnumerable<TPrevEnumerable, TPrevEnumerator, decimal>.Enumerator>();

        public static Int32 Average<TPrevEnumerable, TPrevEnumerator>(ref this AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Int32> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int32>
            => @this.AverageInt32<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Int32>, AppendEnumerable<TPrevEnumerable, TPrevEnumerator, int>.Enumerator>();

        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator>(ref this AppendEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt32>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt32>
            => @this.AverageUInt32<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, UInt32>, AppendEnumerable<TPrevEnumerable, TPrevEnumerator, uint>.Enumerator>();

        public static Int64 Average<TPrevEnumerable, TPrevEnumerator>(ref this AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Int64> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, Int64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<Int64>
            => @this.AverageInt64<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, Int64>, AppendEnumerable<TPrevEnumerable, TPrevEnumerator, long>.Enumerator>();

        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator>(ref this AppendEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64> @this)
            where TPrevEnumerable : unmanaged, IRefEnumerable<TPrevEnumerator, UInt64>
            where TPrevEnumerator : unmanaged, IRefEnumerator<UInt64>
            => @this.AverageUInt64<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, UInt64>, AppendEnumerable<TPrevEnumerable, TPrevEnumerator, ulong>.Enumerator>();
    }
}