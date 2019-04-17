using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// ReSharper disable All

namespace pcysl5edgo.Collections.LINQ
{
    public struct SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>
        : IRefEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>.Enumerator, TResult>, ILinq<TResult>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TResult : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TResult>
#endif
        where TAction : struct, IRefAction<TSource, TResult>
    {
        private TPrevEnumerable enumerable;
        private readonly TAction action;
        private readonly Allocator alloc;

        internal SelectEnumerable(in TPrevEnumerable enumerable, TAction action, Allocator alloc)
        {
            this.enumerable = enumerable;
            this.action = action;
            this.alloc = alloc;
        }

        public Enumerator GetEnumerator()
            => new Enumerator(enumerable.GetEnumerator(), action, alloc);

        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult> Append(TResult value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(this, value, allocator);

        public unsafe AppendPointerEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult> Append(TResult* value)
            => new AppendPointerEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(this, value);

        public SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction> AsRefEnumerable() => this;

        public DefaultIfEmptyEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>
            DefaultIfEmpty(TResult defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(this, defaultValue, allocator);

        public DistinctEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator, TResult, DefaultEqualityComparer<TResult>, DefaultGetHashCodeFunc<TResult>>
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, DefaultEqualityComparer<TResult>, DefaultGetHashCodeFunc<TResult>>(this, default, default, allocator);

        public DistinctEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator, TResult, TEqualityComparer, TGetHashCodeFunc>
            Distinct<TEqualityComparer, TGetHashCodeFunc>(TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TResult, TResult, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TResult, int>
            => new DistinctEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction> Select<TNextResult, TNextAction>(TNextAction nextAction, Allocator allocator = Allocator.Temp)
            where TNextAction : unmanaged, IRefAction<TResult, TNextResult>
            where TNextResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextResult>
#endif
            => new SelectEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>(this, nextAction, allocator);

        public SelectIndexEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                TResult,
                TNextResult,
                TNextAction
            >
            SelectIndex<TNextResult, TNextAction>(TNextAction nextAction, Allocator allocator = Allocator.Temp)
            where TNextResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextResult>
#endif
            where TNextAction : unmanaged, ISelectIndex<TResult, TNextResult>
            => new SelectIndexEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>(this, nextAction, allocator);

        public WhereEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => new WhereEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(this, predicate);
        #endregion

        #region Concat
        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                TEnumerable1, TEnumerator1,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1>
            (in TEnumerable1 second)
            where TEnumerator1 : struct, IRefEnumerator<TResult>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TResult>
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    TEnumerable1, TEnumerator1,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TResult>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TResult>.Enumerator,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2>
            (in ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TResult> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TResult>
            where TEnumerator1 : struct, IRefEnumerator<TResult>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TResult>
            where TEnumerator2 : struct, IRefEnumerator<TResult>
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TResult>,
                    ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TResult>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                NativeEnumerable<TResult>,
                NativeEnumerable<TResult>.Enumerator,
                TResult
            >
            Concat(NativeArray<TResult> second)
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    NativeEnumerable<TResult>,
                    NativeEnumerable<TResult>.Enumerator,
                    TResult>
                (this, second.AsRefEnumerable());

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                NativeEnumerable<TResult>,
                NativeEnumerable<TResult>.Enumerator,
                TResult
            >
            Concat(in NativeEnumerable<TResult> second)
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    NativeEnumerable<TResult>,
                    NativeEnumerable<TResult>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                AppendPointerEnumerable<TEnumerable1, TEnumerator1, TResult>,
                AppendEnumerator<TEnumerator1, TResult>,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendPointerEnumerable<TEnumerable1, TEnumerator1, TResult> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TResult>
            where TEnumerator1 : struct, IRefEnumerator<TResult>
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    AppendPointerEnumerable<TEnumerable1, TEnumerator1, TResult>,
                    AppendEnumerator<TEnumerator1, TResult>,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                AppendEnumerable<TEnumerable1, TEnumerator1, TResult>,
                AppendEnumerator<TEnumerator1, TResult>,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendEnumerable<TEnumerable1, TEnumerator1, TResult> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TResult>
            where TEnumerator1 : struct, IRefEnumerator<TResult>
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    AppendEnumerable<TEnumerable1, TEnumerator1, TResult>,
                    AppendEnumerator<TEnumerator1, TResult>,
                    TResult>
                (this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                ArrayEnumerable<TResult>,
                ArrayEnumerable<TResult>.Enumerator,
                TResult
            >
            Concat
            (in ArrayEnumerable<TResult> second)
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    ArrayEnumerable<TResult>,
                    ArrayEnumerable<TResult>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                ArrayEnumerable<TResult>,
                ArrayEnumerable<TResult>.Enumerator,
                TResult
            >
            Concat(TResult[] second)
            => new ConcatEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, ArrayEnumerable<TResult>, ArrayEnumerable<TResult>.Enumerator, TResult>(this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TResult>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TResult>.Enumerator,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1>
            (in DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TResult> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TResult>
            where TEnumerator1 : struct, IRefEnumerator<TResult>
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TResult>,
                    DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TResult>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                DistinctEnumerable<TEnumerable1, TEnumerator1, TResult, TEqualityComparer, TGetHashCodeFunc>,
                DistinctEnumerable<TEnumerable1, TEnumerator1, TResult, TEqualityComparer, TGetHashCodeFunc>.Enumerator,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1, TEqualityComparer, TGetHashCodeFunc>
            (in DistinctEnumerable<TEnumerable1, TEnumerator1, TResult, TEqualityComparer, TGetHashCodeFunc> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TResult>
            where TEnumerator1 : struct, IRefEnumerator<TResult>
            where TEqualityComparer : struct, IRefFunc<TResult, TResult, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TResult, int>
            => new ConcatEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, DistinctEnumerable<TEnumerable1, TEnumerator1, TResult, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable1, TEnumerator1, TResult, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TResult>(this, second);

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                RangeRepeatEnumerable<TResult, TAction1>,
                RangeRepeatEnumerable<TResult, TAction1>.Enumerator,
                TResult
            >
            Concat<TAction1>
            (in RangeRepeatEnumerable<TResult, TAction1> second)
            where TAction1 : struct, IRefAction<TResult>
            => new ConcatEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, RangeRepeatEnumerable<TResult, TAction1>, RangeRepeatEnumerable<TResult, TAction1>.Enumerator, TResult>(this, second);

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>.Enumerator,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1, TPrevSource1, TAction1>
            (in SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1> second)
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource1>
            where TAction1 : unmanaged, IRefAction<TPrevSource1, TResult>
            where TPrevSource1 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource1>
#endif
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>,
                    SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>,
                SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>.Enumerator,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1, TPrevSource1, TAction1>
            (in SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1> second)
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource1>
            where TAction1 : unmanaged, ISelectIndex<TPrevSource1, TResult>
            where TPrevSource1 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource1>
#endif
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>,
                    SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>.Enumerator,
                    TResult>
                (this, second);


        public ConcatEnumerable<
                SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                WhereEnumerable<TEnumerable1, TEnumerator1, TResult, TPredicate>,
                WhereEnumerable<TEnumerable1, TEnumerator1, TResult, TPredicate>.Enumerator,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1, TPredicate>(in WhereEnumerable<TEnumerable1, TEnumerator1, TResult, TPredicate> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TResult>
            where TEnumerator1 : struct, IRefEnumerator<TResult>
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => new ConcatEnumerable<
                    SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    WhereEnumerable<TEnumerable1, TEnumerator1, TResult, TPredicate>,
                    WhereEnumerable<TEnumerable1, TEnumerator1, TResult, TPredicate>.Enumerator,
                    TResult>
                (this, second);
        #endregion

        #region Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => enumerable.Any();

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!predicate.Calc(ref enumerator.Current)) continue;
                enumerator.Dispose();
                return true;
            }
            enumerator.Dispose();
            return false;
        }

        public bool Any(Func<TResult, bool> predicate)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!predicate(enumerator.Current)) continue;
                enumerator.Dispose();
                return true;
            }
            enumerator.Dispose();
            return false;
        }

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate.Calc(ref enumerator.Current)) continue;
                enumerator.Dispose();
                return false;
            }
            enumerator.Dispose();
            return true;
        }

        public bool All(Func<TResult, bool> predicate)
        {
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current)) continue;
                enumerator.Dispose();
                return false;
            }
            enumerator.Dispose();
            return true;
        }

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TResult>
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate, TFunc>(ref seed, func);

        public TNextResult Aggregate<TAccumulate, TNextResult, TFunc, TNextResultFunc>(ref TAccumulate seed, TFunc func, TNextResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TResult>
            where TNextResultFunc : unmanaged, IRefFunc<TAccumulate, TNextResult>
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate, TNextResult, TFunc, TNextResultFunc>(ref seed, func, resultFunc);

        public TResult Aggregate(Func<TResult, TResult, TResult> func)
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func)
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate>(seed, func);

        public TResult1 Aggregate<TAccumulate, TResult1>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func, Func<TAccumulate, TResult1> resultFunc)
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate, TResult1>(seed, func, resultFunc);

        public bool Contains(TResult value)
            => this.Contains<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(value);

        public bool Contains(TResult value, IEqualityComparer<TResult> comparer)
            => this.Contains<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(value, comparer);

        public bool Contains<TComparer>(TResult value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TResult, TResult, bool>
            => this.Contains<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TComparer>(value, comparer);

        public int Count() => enumerable.Count();

        public int Count(Func<TResult, bool> predicate)
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(predicate);

        public long LongCount() => enumerable.LongCount();

        public long LongCount(Func<TResult, bool> predicate)
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(predicate);

        public bool TryGetElementAt(int index, out TResult element)
            => this.TryGetElementAt<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(index, out element);

        public bool TryGetFirst(out TResult first)
            => this.TryGetFirst<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out first);

        public bool TryGetLast(out TResult last)
            => this.TryGetLast<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out last);

        public bool TryGetSingle(out TResult value)
            => this.TryGetSingle<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out value);

        public bool TryGetSingle<TPredicate>(out TResult value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.TryGetSingle<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TResult value, Func<TResult, bool> predicate)
            => this.TryGetSingle<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out value, predicate);

        public TResult[] ToArray()
            => this.ToArray<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>();

        public NativeArray<TResult> ToNativeArray(Allocator allocator)
            => this.ToNativeArray<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(allocator);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TResult, TKey> keySelector, Func<TResult, TElement> elementSelector)
            => this.ToDictionary<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TResult, TKey>
            where TElementFunc : unmanaged, IRefFunc<TResult, TElement>
            => this.ToDictionary<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TResult> ToHashSet()
            => this.ToHashSet<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>();

        public HashSet<TResult> ToHashSet(IEqualityComparer<TResult> comparer)
            => this.ToHashSet<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(comparer);

        public List<TResult> ToList()
            => this.ToList<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>();
        #endregion

        public unsafe struct Enumerator : IRefEnumerator<TResult>
        {
            private TPrevEnumerator enumerator;
            private readonly TResult* current;
            private readonly Allocator allocator;
            private TAction action;

            internal Enumerator(in TPrevEnumerator enumerator, TAction action, Allocator allocator)
            {
                this.enumerator = enumerator;
                this.current = UnsafeUtilityEx.Malloc<TResult>(1, allocator);
                this.allocator = allocator;
                this.action = action;
            }

            public ref TResult Current => ref *current;
            TResult IEnumerator<TResult>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                if (UnsafeUtility.IsValidAllocator(allocator) && current != null)
                    UnsafeUtility.Free(current, allocator);
                this = default;
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext()) return false;
                action.Execute(ref enumerator.Current, ref *current);
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
        }
    }
}