using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class SelectEnumerable
    {
        public static SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction> Select<T, TResult, TAction>(this NativeArray<T> array, TAction action, Allocator allocator)
            where T : unmanaged
            where TResult : unmanaged
            where TAction : struct, IRefAction<T, TResult>
            => new SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>(array.AsRefEnumerable(), action, allocator);

        public static float Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, float, TAction> values)
            where TPrevSource : unmanaged
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, float>
            => values.AverageSingle<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, float, TAction>, SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, float, TAction>.Enumerator>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction> values)
            where TPrevSource : unmanaged
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, Double>
            => values.AverageDouble<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction>, SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, double, TAction>.Enumerator>();

        public static Decimal Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction> values)
            where TPrevSource : unmanaged
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, Decimal>
            => values.AverageDecimal<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction>, SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, decimal, TAction>.Enumerator>();

        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction> values)
            where TPrevSource : unmanaged
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, UInt32>
            => values.AverageUInt32<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction>, SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, uint, TAction>.Enumerator>();

        public static Int32 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction> values)
            where TPrevSource : unmanaged
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, Int32>
            => values.AverageInt32<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction>, SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, int, TAction>.Enumerator>();

        public static Int64 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction> values)
            where TPrevSource : unmanaged
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, Int64>
            => values.AverageInt64<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction>, SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, long, TAction>.Enumerator>();

        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction> values)
            where TPrevSource : unmanaged
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, UInt64>
            => values.AverageUInt64<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction>, SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, ulong, TAction>.Enumerator>();
    }
}