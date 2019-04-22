using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class ConcatEnumerable
    {
        #region NativeArray
        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TEnumerable,
                TEnumerator,
                TSource
            >
            Concat<TEnumerable, TEnumerator, TSource>(
                this NativeArray<TSource> first,
                in TEnumerable second)
            where TSource : unmanaged
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TEnumerable, TEnumerator, TSource>(first.AsRefEnumerable(), second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TSource>(
                this NativeArray<TSource> first,
                NativeArray<TSource> second)
            where TSource : unmanaged
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first.AsRefEnumerable(), second.AsRefEnumerable());

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>(
                this NativeArray<TSource> first,
                in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(first.AsRefEnumerable(), second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource>(
                this NativeArray<TSource> first,
                in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TSource : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator, TSource>(first.AsRefEnumerable(), second);


        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TSource>(
                this NativeArray<TSource> first,
                in ArrayEnumerable<TSource> second)
            where TSource : unmanaged
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(first.AsRefEnumerable(), second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TSource>(
                this NativeArray<TSource> first,
                TSource[] second)
            where TSource : unmanaged
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(first.AsRefEnumerable(), second.AsRefEnumerable());


        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPrevSource, TAction>(
                this NativeArray<TSource> first,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction> second)
            where TPrevSource : unmanaged
            where TSource : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource>
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator, TSource>(first.AsRefEnumerable(), second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate>(
                this NativeArray<TSource> first,
                in WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate> second)
            where TSource : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator, TSource>(first.AsRefEnumerable(), second);
        #endregion

        #region Average
        public static Single
            Average<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Single> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Single>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Single>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Single>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Single>
            => @this.AverageSingle<
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Single>,
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, float>.Enumerator
            >();

        public static Double
            Average<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Double> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Double>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Double>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Double>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Double>
            => @this.AverageDouble<
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Double>,
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, double>.Enumerator
            >();

        public static Decimal
            Average<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Decimal> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Decimal>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Decimal>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Decimal>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Decimal>
            => @this.AverageDecimal<
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Decimal>,
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, decimal>.Enumerator
            >();

        public static Int32
            Average<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Int32> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Int32>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Int32>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Int32>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Int32>
            => @this.AverageInt32<
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Int32>,
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, int>.Enumerator
            >();

        public static Int64
            Average<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Int64> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Int64>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Int64>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Int64>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Int64>
            => @this.AverageInt64<
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Int64>,
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, long>.Enumerator
            >();

        public static UInt32
            Average<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, UInt32> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, UInt32>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<UInt32>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, UInt32>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<UInt32>
            => @this.AverageUInt32<
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, UInt32>,
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, uint>.Enumerator
            >();

        public static UInt64
            Average<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, UInt64> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, UInt64>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<UInt64>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, UInt64>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<UInt64>
            => @this.AverageUInt64<
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, UInt64>,
                ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, ulong>.Enumerator
            >();
        #endregion

        #region Sum
        public static Single
            Sum<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Single> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Single>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Single>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Single>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Single>
            => @this.FirstEnumerable.SumSingle<TPrevEnumerable0, TPrevEnumerator0>()
               +
               @this.SecondEnumerable.SumSingle<TPrevEnumerable1, TPrevEnumerator1>();

        public static Double
            Sum<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Double> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Double>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Double>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Double>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Double>
            => @this.FirstEnumerable.SumDouble<TPrevEnumerable0, TPrevEnumerator0>()
               +
               @this.SecondEnumerable.SumDouble<TPrevEnumerable1, TPrevEnumerator1>();

        public static Decimal
            Sum<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Decimal> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Decimal>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Decimal>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Decimal>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Decimal>
            => @this.FirstEnumerable.SumDecimal<TPrevEnumerable0, TPrevEnumerator0>()
               +
               @this.SecondEnumerable.SumDecimal<TPrevEnumerable1, TPrevEnumerator1>();

        public static Int32
            Sum<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Int32> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Int32>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Int32>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Int32>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Int32>
            => @this.FirstEnumerable.SumInt32<TPrevEnumerable0, TPrevEnumerator0>()
               +
               @this.SecondEnumerable.SumInt32<TPrevEnumerable1, TPrevEnumerator1>();

        public static Int64
            Sum<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, Int64> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, Int64>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<Int64>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, Int64>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<Int64>
            => @this.FirstEnumerable.SumInt64<TPrevEnumerable0, TPrevEnumerator0>()
               +
               @this.SecondEnumerable.SumInt64<TPrevEnumerable1, TPrevEnumerator1>();

        public static UInt64
            Sum<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, UInt64> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, UInt64>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<UInt64>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, UInt64>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<UInt64>
            => @this.FirstEnumerable.SumUInt64<TPrevEnumerable0, TPrevEnumerator0>()
               +
               @this.SecondEnumerable.SumUInt64<TPrevEnumerable1, TPrevEnumerator1>();

        public static UInt32
            Sum<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1>
            (ref this ConcatEnumerable<TPrevEnumerable0, TPrevEnumerator0, TPrevEnumerable1, TPrevEnumerator1, UInt32> @this)
            where TPrevEnumerable0 : unmanaged, IRefEnumerable<TPrevEnumerator0, UInt32>
            where TPrevEnumerator0 : unmanaged, IRefEnumerator<UInt32>
            where TPrevEnumerable1 : unmanaged, IRefEnumerable<TPrevEnumerator1, UInt32>
            where TPrevEnumerator1 : unmanaged, IRefEnumerator<UInt32>
            => @this.FirstEnumerable.SumUInt32<TPrevEnumerable0, TPrevEnumerator0>()
               +
               @this.SecondEnumerable.SumUInt32<TPrevEnumerable1, TPrevEnumerator1>();
        #endregion
    }
}