using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public struct
        ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>
        : IRefEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>.Enumerator, TSource>, ILinq<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TSource>
        where TFirstEnumerator : struct, IRefEnumerator<TSource>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSource>
        where TSecondEnumerator : struct, IRefEnumerator<TSource>
    {
        internal TFirstEnumerable FirstEnumerable;
        internal TSecondEnumerable SecondEnumerable;

        internal ConcatEnumerable(in TFirstEnumerable firstEnumerable, in TSecondEnumerable secondEnumerable)
        {
            this.FirstEnumerable = firstEnumerable;
            this.SecondEnumerable = secondEnumerable;
        }

        public Enumerator GetEnumerator() => new Enumerator(FirstEnumerable.GetEnumerator(), SecondEnumerable.GetEnumerator());

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TFirstEnumerator firstEnumerator;
            private TSecondEnumerator secondEnumerator;
            private bool isCurrentSecond;

            public ref TSource Current => ref (isCurrentSecond ? ref secondEnumerator.Current : ref firstEnumerator.Current);
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(in TFirstEnumerator firstEnumerator, in TSecondEnumerator secondEnumerator)
            {
                this.firstEnumerator = firstEnumerator;
                this.secondEnumerator = secondEnumerator;
                isCurrentSecond = false;
            }

            public void Dispose()
            {
                firstEnumerator.Dispose();
                secondEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                if (isCurrentSecond) return secondEnumerator.MoveNext();
                if (firstEnumerator.MoveNext()) return true;
                isCurrentSecond = true;
                return secondEnumerator.MoveNext();
            }

            public void Reset() => throw new InvalidOperationException();
        }

        #region Enumerable
        public AppendEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource> Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource>(this, value, allocator);

        public ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource> AsRefEnumerable() => this;

        public DefaultIfEmptyEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource>
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);

        public DistinctEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
            Distinct<TEqualityComparer, TGetHashCodeFunc>(TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new DistinctEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TResult, TAction> Select<TResult, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IRefAction<TSource, TResult>
            => new SelectEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectIndexEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TResult, TAction> SelectIndex<TResult, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectManyEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                TSource,
                TResult,
                TResultEnumerable,
                TResultEnumerator,
                TResultAction
            >
            SelectMany<TResult,
                TResultEnumerable,
                TResultEnumerator,
                TResultAction>
            (TResultAction action)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TResultEnumerator : struct, IRefEnumerator<TResult>
            where TResultEnumerable : struct, IRefEnumerable<TResultEnumerator, TResult>
            where TResultAction : struct, IRefAction<TSource, TResultEnumerable>
            => new SelectManyEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public
            WhereIndexEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                TSource,
                DefaultSkipIndex<TSource>
            >
            Skip(long count)
            => new WhereIndexEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, DefaultSkipIndex<TSource>>(this, new DefaultSkipIndex<TSource>(count));

        public
            WhereIndexEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                TSource,
                DefaultSkipWhileIndex<TSource, TPredicate0>
            >
            SkipWhileIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, DefaultSkipWhileIndex<TSource, TPredicate0>>(this, new DefaultSkipWhileIndex<TSource, TPredicate0>(predicate));

        public
            WhereIndexEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                TSource,
                DefaultTakeIndex<TSource>
            >
            Take(long count)
            => new WhereIndexEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, DefaultTakeIndex<TSource>>(this, new DefaultTakeIndex<TSource>(count));

        public
            WhereIndexEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                TSource,
                DefaultTakeWhileIndex<TSource, TPredicate0>
            >
            TakeWhileIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, DefaultTakeWhileIndex<TSource, TPredicate0>>(this, new DefaultTakeWhileIndex<TSource, TPredicate0>(predicate));

        public WhereEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TPredicate>(this, predicate);

        public WhereIndexEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                TSource,
                TPredicate0
            >
            WhereIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TPredicate0>(this, predicate);

        public ZipEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
            Zip<TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
            (in TEnumerable0 second, TAction0 action, TSource firstDefaultValue = default, TSource0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource0>
            where TEnumerator0 : struct, IRefEnumerator<TSource0>
            where TSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource0>
#endif
            where TAction0 : unmanaged, IRefAction<TSource, TSource0, TResult0>
            where TResult0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult0>
#endif
            => new ZipEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                TEnumerable,
                TEnumerator,
                TSource
            >
            Concat<TEnumerable, TEnumerator>
            (in TEnumerable second)
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TEnumerable, TEnumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource>,
                ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3>
            (in ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource> second)
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            where TEnumerator3 : struct, IRefEnumerator<TSource>
            where TEnumerable3 : struct, IRefEnumerable<TEnumerator3, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource>, ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                AppendEnumerable<TEnumerable2, TEnumerator2, TSource>,
                AppendEnumerable<TEnumerable2, TEnumerator2, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable2, TEnumerator2>
            (in AppendEnumerable<TEnumerable2, TEnumerator2, TSource> second)
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, AppendEnumerable<TEnumerable2, TEnumerator2, TSource>, AppendEnumerable<TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(TSource[] second)
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable2, TEnumerator2, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable2, TEnumerator2, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable2, TEnumerator2>
            (in DefaultIfEmptyEnumerable<TEnumerable2, TEnumerator2, TSource> second)
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, DefaultIfEmptyEnumerable<TEnumerable2, TEnumerator2, TSource>, DefaultIfEmptyEnumerable<TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                DistinctEnumerable<TEnumerable2, TEnumerator2, TSource, TEqualityComparer, TGetHashCodeFunc>,
                DistinctEnumerable<TEnumerable2, TEnumerator2, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator,
                TSource
            >
            Concat<TEnumerable2, TEnumerator2, TEqualityComparer, TGetHashCodeFunc>
            (in DistinctEnumerable<TEnumerable2, TEnumerator2, TSource, TEqualityComparer, TGetHashCodeFunc> second)
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, DistinctEnumerable<TEnumerable2, TEnumerator2, TSource, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable2, TEnumerator2, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                RangeRepeatEnumerable<TSource, TAction>,
                RangeRepeatEnumerable<TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TAction>
            (in RangeRepeatEnumerable<TSource, TAction> second)
            where TAction : struct, IRefAction<TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, RangeRepeatEnumerable<TSource, TAction>, RangeRepeatEnumerable<TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                SelectIndexEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>,
                SelectIndexEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable2, TEnumerator2, TPrevSource, TAction>
            (in SelectIndexEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction> second)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TEnumerator2 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TPrevSource>
            where TAction : unmanaged, ISelectIndex<TPrevSource, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, SelectIndexEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>, SelectIndexEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>,
                SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource0, TResultEnumerable0, TResultEnumerator0, TAction0>
            (in SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0> second)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TResultEnumerator0 : struct, IRefEnumerator<TSource>
            where TResultEnumerable0 : struct, IRefEnumerable<TResultEnumerator0, TSource>
            where TAction0 : struct, IRefAction<TPrevSource0, TResultEnumerable0>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                WhereEnumerable<TEnumerable2, TEnumerator2, TSource, TPredicate>,
                WhereEnumerable<TEnumerable2, TEnumerator2, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TEnumerable2, TEnumerator2, TPredicate>
            (in WhereEnumerable<TEnumerable2, TEnumerator2, TSource, TPredicate> second)
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, WhereEnumerable<TEnumerable2, TEnumerator2, TSource, TPredicate>, WhereEnumerable<TEnumerable2, TEnumerator2, TSource, TPredicate>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TPrevEnumerable0, TPrevEnumerator0, TPredicate>
            (in WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate> second)
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, TSource>
            where TPrevEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate : struct, IWhereIndex<TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                SelectEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>,
                SelectEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable2, TEnumerator2, TPrevSource, TAction>
            (in SelectEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction> second)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TEnumerator2 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TPrevSource>
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, SelectEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>, SelectEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TAction0>
            (in ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0> second)
            where TSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource0>
            where TSource1 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource1>
#endif
            where TEnumerator1 : struct, IRefEnumerator<TSource1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource1>
            where TAction0 : struct, IRefAction<TSource0, TSource1, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => FirstEnumerable.CanFastCount() && SecondEnumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => FirstEnumerable.Any() || SecondEnumerable.Any();

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => FirstEnumerable.Any<TFirstEnumerable, TFirstEnumerator, TSource, TPredicate>(predicate) || SecondEnumerable.Any<TSecondEnumerable, TSecondEnumerator, TSource, TPredicate>(predicate);

        public bool Any(Func<TSource, bool> predicate)
            => FirstEnumerable.Any<TFirstEnumerable, TFirstEnumerator, TSource>(predicate) || SecondEnumerable.Any<TSecondEnumerable, TSecondEnumerator, TSource>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => FirstEnumerable.All<TFirstEnumerable, TFirstEnumerator, TSource, TPredicate>(predicate) || SecondEnumerable.All<TSecondEnumerable, TSecondEnumerator, TSource, TPredicate>(predicate);

        public bool All(Func<TSource, bool> predicate)
            => FirstEnumerable.All<TFirstEnumerable, TFirstEnumerator, TSource>(predicate) || SecondEnumerable.All<TSecondEnumerable, TSecondEnumerator, TSource>(predicate);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
        {
            FirstEnumerable.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TAccumulate, TFunc>(ref seed, func);
            SecondEnumerable.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TAccumulate, TFunc>(ref seed, func);
        }

        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
        {
            FirstEnumerable.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TAccumulate, TFunc>(ref seed, func);
            return SecondEnumerable.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);
        }

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
            => SecondEnumerable.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TSource>(
                FirstEnumerable.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource>(func),
                func
            );

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => SecondEnumerable.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TAccumulate>(FirstEnumerable.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TAccumulate>(seed, func), func);

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => SecondEnumerable.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TAccumulate, TResult>(FirstEnumerable.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TAccumulate>(seed, func), func, resultFunc);

        public bool Contains(TSource value)
            => FirstEnumerable.Contains<TFirstEnumerable, TFirstEnumerator, TSource>(value) || SecondEnumerable.Contains<TSecondEnumerable, TSecondEnumerator, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => FirstEnumerable.Contains<TFirstEnumerable, TFirstEnumerator, TSource>(value, comparer) || SecondEnumerable.Contains<TSecondEnumerable, TSecondEnumerator, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => FirstEnumerable.Contains<TFirstEnumerable, TFirstEnumerator, TSource, TComparer>(value, comparer) || SecondEnumerable.Contains<TSecondEnumerable, TSecondEnumerator, TSource, TComparer>(value, comparer);

        public int Count() => FirstEnumerable.Count() + SecondEnumerable.Count();

        public int Count(Func<TSource, bool> predicate)
            => FirstEnumerable.Count<TFirstEnumerable, TFirstEnumerator, TSource>(predicate) + SecondEnumerable.Count<TSecondEnumerable, TSecondEnumerator, TSource>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => FirstEnumerable.Count<TFirstEnumerable, TFirstEnumerator, TSource, TPredicate>(predicate) + SecondEnumerable.Count<TSecondEnumerable, TSecondEnumerator, TSource, TPredicate>(predicate);

        public long LongCount() => FirstEnumerable.LongCount() + SecondEnumerable.LongCount();

        public long LongCount(Func<TSource, bool> predicate)
            => Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => Count(predicate);

        public bool TryGetElementAt(long index, out TSource element)
        {
            var firstEnumerator = FirstEnumerable.GetEnumerator();
            while (firstEnumerator.MoveNext())
            {
                if (--index >= 0) continue;
                element = firstEnumerator.Current;
                firstEnumerator.Dispose();
                return true;
            }
            firstEnumerator.Dispose();
            var secondEnumerator = SecondEnumerable.GetEnumerator();
            while (secondEnumerator.MoveNext())
            {
                if (--index >= 0) continue;
                element = secondEnumerator.Current;
                secondEnumerator.Dispose();
                return true;
            }
            secondEnumerator.Dispose();
            element = default;
            return false;
        }

        public NativeEnumerable<TSource> ToNativeEnumerable(Allocator allocator)
            => this.ToNativeEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator, TSource>(allocator);

        // ReSharper disable once ParameterHidesMember
        public bool TryGetFirst(out TSource first)
            => this.FirstEnumerable.TryGetFirst(out first) || SecondEnumerable.TryGetFirst(out first);

        public bool TryGetLast(out TSource last)
            => SecondEnumerable.TryGetLast(out last) || FirstEnumerable.TryGetLast(out last);

        public bool TryGetSingle(out TSource value)
        {
            var firstEnumerator = FirstEnumerable.GetEnumerator();
            var secondEnumerator = SecondEnumerable.GetEnumerator();
            if (firstEnumerator.MoveNext())
            {
                if (secondEnumerator.MoveNext())
                {
                    value = default;
                    firstEnumerator.Dispose();
                    secondEnumerator.Dispose();
                    return false;
                }
                value = firstEnumerator.Current;
                secondEnumerator.Dispose();
                firstEnumerator.Dispose();
                return true;
            }
            firstEnumerator.Dispose();
            if (secondEnumerator.MoveNext())
            {
                value = secondEnumerator.Current;
                secondEnumerator.Dispose();
                return true;
            }
            secondEnumerator.Dispose();
            value = default;
            return false;
        }

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            var firstResult = FirstEnumerable.TryGetSingle<TFirstEnumerable, TFirstEnumerator, TSource, TPredicate>(out value, predicate);
            var secondResult = SecondEnumerable.TryGetSingle<TSecondEnumerable, TSecondEnumerator, TSource, TPredicate>(out var secondValue, predicate);
            if (firstResult) return !secondResult;
            if (!secondResult) return false;
            value = secondValue;
            return true;
        }

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
        {
            var firstResult = FirstEnumerable.TryGetSingle<TFirstEnumerable, TFirstEnumerator, TSource>(out value, predicate);
            var secondResult = SecondEnumerable.TryGetSingle<TSecondEnumerable, TSecondEnumerator, TSource>(out var secondValue, predicate);
            if (firstResult) return !secondResult;
            if (!secondResult) return false;
            value = secondValue;
            return true;
        }

        public TSource[] ToArray()
        {
            var answer = new TSource[Count()];
            var index = 0;
            var firstEnumerator = FirstEnumerable.GetEnumerator();
            while (firstEnumerator.MoveNext())
                answer[index++] = firstEnumerator.Current;
            firstEnumerator.Dispose();
            var secondEnumerator = SecondEnumerable.GetEnumerator();
            while (secondEnumerator.MoveNext())
                answer[index++] = secondEnumerator.Current;
            secondEnumerator.Dispose();
            return answer;
        }

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            var answer = new NativeArray<TSource>(Count(), allocator);
            var index = 0;
            var firstEnumerator = FirstEnumerable.GetEnumerator();
            while (firstEnumerator.MoveNext())
                answer[index++] = firstEnumerator.Current;
            firstEnumerator.Dispose();
            var secondEnumerator = SecondEnumerable.GetEnumerator();
            while (secondEnumerator.MoveNext())
                answer[index++] = secondEnumerator.Current;
            secondEnumerator.Dispose();
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var answer = FirstEnumerable.ToDictionary<TFirstEnumerable, TFirstEnumerator, TSource, TKey, TElement>(keySelector, elementSelector);
            foreach (ref var source in SecondEnumerable)
                answer.Add(keySelector(source), elementSelector(source));
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
        {
            var answer = FirstEnumerable.ToDictionary<TFirstEnumerable, TFirstEnumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);
            foreach (ref var source in SecondEnumerable)
                answer.Add(keySelector.Calc(ref source), elementSelector.Calc(ref source));
            return answer;
        }

        public HashSet<TSource> ToHashSet()
        {
            var answer = FirstEnumerable.ToHashSet<TFirstEnumerable, TFirstEnumerator, TSource>();
            foreach (ref var source in SecondEnumerable)
                answer.Add(source);
            return answer;
        }

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
        {
            var answer = FirstEnumerable.ToHashSet<TFirstEnumerable, TFirstEnumerator, TSource>(comparer);
            foreach (ref var source in SecondEnumerable)
                answer.Add(source);
            return answer;
        }

        public List<TSource> ToList()
        {
            var answer = new List<TSource>(Count());
            foreach (ref var source in FirstEnumerable)
                answer.Add(source);
            foreach (ref var source in SecondEnumerable)
                answer.Add(source);
            return answer;
        }
        #endregion
    }
}