using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct
        AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>
        : IRefEnumerable<AppendEnumerator<TPrevEnumerator, TSource>, TSource>, ILinq<TSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
    {
        private TPrevEnumerable enumerable;
        private readonly TSource* append;

        internal AppendPointerEnumerable(in TPrevEnumerable enumerable, TSource* append)
        {
            this.enumerable = enumerable;
            this.append = append;
        }

        public AppendEnumerator<TPrevEnumerator, TSource> GetEnumerator() => new AppendEnumerator<TPrevEnumerator, TSource>(enumerable.GetEnumerator(), append);
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> AsRefEnumerable() => this;

        public AppendPointerEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(TSource* item)
            => new AppendPointerEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item);

        public AppendEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(in TSource item, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item, allocator);

        public DefaultIfEmptyEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                TSource,
                DefaultEqualityComparer<TSource>,
                DefaultGetHashCodeFunc<TSource>
            >
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);

        public DistinctEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                TSource,
                TEqualityComparer,
                TGetHashCodeFunc
            >
            Distinct<TEqualityComparer, TGetHashCodeFunc>(TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new DistinctEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TResult, TAction> Select<TResult, TAction>(TAction action, Allocator allocator)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IRefAction<TSource, TResult>
            => new SelectEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TResult, TAction>(this, action, allocator);

        public SelectIndexEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TResult, TAction> SelectIndex<TResult, TAction>(TAction action, Allocator allocator)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TResult, TAction>(this, action, allocator);

        public SelectManyEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
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
            => new SelectManyEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public WhereEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate>(this, predicate);


        public ZipEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
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
            => new ZipEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                TEnumerable, TEnumerator,
                TSource
            >
            Concat<TEnumerable, TEnumerator>(in TEnumerable second)
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                TEnumerable, TEnumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2>(in ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, AppendEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<
                    AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                    AppendEnumerator<TPrevEnumerator, TSource>,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(TSource[] second)
            => new ConcatEnumerable<
                    AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                    AppendEnumerator<TPrevEnumerator, TSource>,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>, DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>,
                DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TEqualityComparer, TGetHashCodeFunc>
            (in DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                RangeRepeatEnumerable<TSource, TAction>,
                RangeRepeatEnumerable<TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TAction>
            (in RangeRepeatEnumerable<TSource, TAction> second)
            where TAction : struct, IRefAction<TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, RangeRepeatEnumerable<TSource, TAction>, RangeRepeatEnumerable<TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>,
                SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TPrevSource, TAction>
            (SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction> second)
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TAction : unmanaged, ISelectIndex<TPrevSource, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TPrevSource, TAction>
            (SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction> second)
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
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
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TPredicate>
            (in WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
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
            => new ConcatEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
            #endregion

            #region Function
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => enumerable.CanFastCount();

        public bool Any() => true;

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => predicate.Calc(ref *append) || enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(predicate);

        public bool Any(Func<TSource, bool> predicate)
            => predicate(*append) || enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => predicate.Calc(ref *append) && enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(predicate);

        public bool All(Func<TSource, bool> predicate)
            => predicate(*append) && enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource>(predicate);

        public void Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
            => this.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
            => this.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => this.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate>(seed, func);

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => this.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TResult>(seed, func, resultFunc);

        public bool Contains(TSource value)
            => append->Equals(value) || enumerable.Contains<TPrevEnumerable, TPrevEnumerator, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => comparer.Equals(*append, value) || enumerable.Contains<TPrevEnumerable, TPrevEnumerator, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => comparer.Calc(ref *append, ref value) || enumerable.Contains<TPrevEnumerable, TPrevEnumerator, TSource, TComparer>(value, comparer);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            => this.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TFunc>(ref seed, func);

        TResult ILinq<TSource>.Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            => this.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public bool Contains<TEqualityComparer>(in TSource value, TEqualityComparer comparer)
            where TEqualityComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => this.Contains<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TEqualityComparer>(value, comparer);

        public int Count() => enumerable.Count() + 1;

        public int Count(Func<TSource, bool> predicate)
            => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource>(predicate) + 1;

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(predicate);

        public long LongCount() => enumerable.LongCount();

        public long LongCount(Func<TSource, bool> predicate)
            => Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => Count(predicate);

        public bool TryGetElementAt(long index, out TSource element)
            => this.TryGetElementAt<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(index, out element);

        public bool TryGetFirst(out TSource first)
        {
            if (enumerable.TryGetFirst(out first))
                return true;
            first = *append;
            return true;
        }

        public bool TryGetLast(out TSource last)
        {
            last = *append;
            return true;
        }

        public bool TryGetSingle(out TSource value)
        {
            if (enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource>(out value))
                return false;
            value = *append;
            return true;
        }

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            if (!predicate.Calc(ref *append))
                return enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(out value, predicate);
            if (enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(out value, predicate))
                return false;
            value = *append;
            return true;
        }

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
        {
            if (!predicate(*append))
                return enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource>(out value, predicate);
            if (enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource>(out value, predicate))
                return false;
            value = *append;
            return true;
        }

        public TSource[] ToArray()
        {
            var answer = new TSource[LongCount()];
            var enumerator = enumerable.GetEnumerator();
            fixed (TSource* fixedPtr = &answer[0])
            {
                var ptr = fixedPtr;
                while (enumerator.MoveNext())
                    *ptr++ = enumerator.Current;
            }
            enumerator.Dispose();
            answer[answer.LongLength - 1] = *append;
            return answer;
        }

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            var answer = new NativeArray<TSource>(Count(), allocator);
            var enumerator = enumerable.GetEnumerator();
            var ptr = answer.GetPointer();
            while (enumerator.MoveNext())
                *ptr++ = enumerator.Current;
            enumerator.Dispose();
            answer[answer.Length - 1] = *append;
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            => this.ToDictionary<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
        {
            var answer = enumerable.ToDictionary<TPrevEnumerable, TPrevEnumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);
            answer.Add(keySelector.Calc(ref *append), elementSelector.Calc(ref *append));
            return answer;
        }

        public HashSet<TSource> ToHashSet()
        {
            var answer = enumerable.ToHashSet<TPrevEnumerable, TPrevEnumerator, TSource>();
            answer.Add(*append);
            return answer;
        }

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
        {
            var answer = enumerable.ToHashSet<TPrevEnumerable, TPrevEnumerator, TSource>();
            answer.Add(*append);
            return answer;
        }

        public List<TSource> ToList()
        {
            var answer = enumerable.ToList<TPrevEnumerable, TPrevEnumerator, TSource>();
            answer.Add(*append);
            return answer;
        }
        #endregion
    }
}