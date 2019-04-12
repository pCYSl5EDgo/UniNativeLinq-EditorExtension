using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class SelectEnumerable
    {
        public static SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction> Select<T, TResult, TAction>(this NativeArray<T> array, TAction action, Allocator allocator)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T> 
#endif
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult> 
#endif
            where TAction : struct, IRefAction<T, TResult>
            => new SelectEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T, TResult, TAction>(array.AsRefEnumerable(), action, allocator);

        public static float Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, float, TAction> values)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource> 
#endif
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, float>
            => values.AverageSingle<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, float, TAction>, SelectEnumerator<TPrevEnumerator, TPrevSource, float, TAction>>();

        public static Double Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction> values)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource> 
#endif
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, Double>
            => values.AverageDouble<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Double, TAction>, SelectEnumerator<TPrevEnumerator, TPrevSource, Double, TAction>>();
        
        public static Decimal Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction> values)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource> 
#endif
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, Decimal>
            => values.AverageDecimal<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Decimal, TAction>, SelectEnumerator<TPrevEnumerator, TPrevSource, Decimal, TAction>>();
        
        public static UInt32 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction> values)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource> 
#endif
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, UInt32>
            => values.AverageUInt32<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt32, TAction>, SelectEnumerator<TPrevEnumerator, TPrevSource, UInt32, TAction>>();
        
        public static Int32 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction> values)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource> 
#endif
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, Int32>
            => values.AverageInt32<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int32, TAction>, SelectEnumerator<TPrevEnumerator, TPrevSource, Int32, TAction>>();
        
        public static Int64 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction> values)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource> 
#endif
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, Int64>
            => values.AverageInt64<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, Int64, TAction>, SelectEnumerator<TPrevEnumerator, TPrevSource, Int64, TAction>>();
        
        public static UInt64 Average<TPrevEnumerable, TPrevEnumerator, TPrevSource, TAction>(ref this SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction> values)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource> 
#endif
            where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
            where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
            where TAction : struct, IRefAction<TPrevSource, UInt64>
            => values.AverageUInt64<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, UInt64, TAction>, SelectEnumerator<TPrevEnumerator, TPrevSource, UInt64, TAction>>();
    }
}