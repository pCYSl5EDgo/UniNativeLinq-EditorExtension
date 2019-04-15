using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>
        : IRefEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>.Enumerator, TResult>, ILinq<TResult>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TResult : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TResult>
#endif
        where TAction : unmanaged, ISelectIndex<TSource, TResult>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
    {
        private TPrevEnumerable enumerable;
        private TAction acts;
        private readonly Allocator alloc;

        internal SelectIndexEnumerable(in TPrevEnumerable enumerable, TAction acts, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.acts = acts;
            this.alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TResult>
        {
            private TPrevEnumerator enumerator;
            private readonly TResult* current;
            private readonly Allocator allocator;
            private TAction action;
            private int index;

            internal Enumerator(in TPrevEnumerator enumerator, TAction action, Allocator allocator)
            {
                this.enumerator = enumerator;
                this.current = UnsafeUtilityEx.Malloc<TResult>(1, allocator);
                this.action = action;
                this.allocator = allocator;
                index = -1;
            }

            public bool MoveNext()
            {
                ++index;
                if (!enumerator.MoveNext()) return false;
                action.Execute(ref enumerator.Current, index, ref *current);
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
            public ref TResult Current => ref *current;
            TResult IEnumerator<TResult>.Current => Current;
            object IEnumerator.Current => Current;
            public void Dispose() => UnsafeUtility.Free(this.current, this.allocator);
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), acts, alloc);
        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction> AsRefEnumerable() => this;

        public SelectIndexEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>
            SelectIndex<TNextResult, TNextAction>(TNextAction action, Allocator allocator = Allocator.Temp)
            where TNextResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextResult>
#endif
            where TNextAction : unmanaged, ISelectIndex<TResult, TNextResult>
            => new SelectIndexEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>(this, action, allocator);

        public SelectEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>
            Select<TNextResult, TNextAction>(TNextAction action, Allocator allocator = Allocator.Temp)
            where TNextResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextResult>
#endif
            where TNextAction : unmanaged, IRefAction<TResult, TNextResult>
            => new SelectEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>(this, action, allocator);

        public AppendEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>
            Append(TResult value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(this, value, allocator);

        public AppendPointerEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>
            Append(TResult* value)
            => new AppendPointerEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(this, value);

        public DefaultIfEmptyEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>
            DefaultIfEmpty(TResult defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(this, defaultValue, allocator);

        public WhereEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>
            Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => new WhereEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(this, predicate);
        #endregion

        #region Concat
        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                TEnumerable1,
                TEnumerator1,
                TResult
            >
            Concat<TEnumerable1, TEnumerator1>
            (in TEnumerable1 second)
            where TEnumerator1 : struct, IRefEnumerator<TResult>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TResult>
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    TEnumerable1, TEnumerator1,
                    TResult
                >
                (this, second);


        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
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
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>,
                    SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
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
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>,
                    SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TResult, TAction1>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
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
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TResult>,
                    ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TResult>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                NativeEnumerable<TResult>,
                NativeEnumerable<TResult>.Enumerator,
                TResult
            >
            Concat(NativeArray<TResult> second)
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    NativeEnumerable<TResult>,
                    NativeEnumerable<TResult>.Enumerator,
                    TResult>
                (this, second.AsRefEnumerable());

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                NativeEnumerable<TResult>,
                NativeEnumerable<TResult>.Enumerator,
                TResult
            >
            Concat(in NativeEnumerable<TResult> second)
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    NativeEnumerable<TResult>,
                    NativeEnumerable<TResult>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
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
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    AppendPointerEnumerable<TEnumerable1, TEnumerator1, TResult>,
                    AppendEnumerator<TEnumerator1, TResult>,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
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
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    AppendEnumerable<TEnumerable1, TEnumerator1, TResult>,
                    AppendEnumerator<TEnumerator1, TResult>,
                    TResult>
                (this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                ArrayEnumerable<TResult>,
                ArrayEnumerable<TResult>.Enumerator,
                TResult
            >
            Concat(in ArrayEnumerable<TResult> second)
            => new ConcatEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, ArrayEnumerable<TResult>, ArrayEnumerable<TResult>.Enumerator, TResult>(this, second);
        
        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                Enumerator,
                ArrayEnumerable<TResult>,
                ArrayEnumerable<TResult>.Enumerator,
                TResult
            >
            Concat(in TResult[] second)
            => new ConcatEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, ArrayEnumerable<TResult>, ArrayEnumerable<TResult>.Enumerator, TResult>(this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
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
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TResult>,
                    DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TResult>.Enumerator,
                    TResult>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
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
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>,
                    Enumerator,
                    WhereEnumerable<TEnumerable1, TEnumerator1, TResult, TPredicate>,
                    WhereEnumerable<TEnumerable1, TEnumerator1, TResult, TPredicate>.Enumerator,
                    TResult>
                (this, second);
        #endregion

        #region Function
        public bool Any() => enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource>();

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.Any<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(predicate);

        public bool Any(Func<TResult, bool> predicate)
            => this.Any<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.All<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(predicate);

        public bool All(Func<TResult, bool> predicate)
            => this.All<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(predicate);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TResult>
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate, TFunc>(ref seed, func);

        public TNextResult Aggregate<TAccumulate, TNextResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TResult>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TNextResult>
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate, TNextResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TResult Aggregate(Func<TResult, TResult, TResult> func)
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func)
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate>(seed, func);

        public TNextResult Aggregate<TAccumulate, TNextResult>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func, Func<TAccumulate, TNextResult> resultFunc)
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate, TNextResult>(seed, func, resultFunc);

        public bool Contains(TResult value)
            => this.Contains<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(value);

        public bool Contains(TResult value, IEqualityComparer<TResult> comparer)
            => this.Contains<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(value, comparer);

        public bool Contains<TComparer>(TResult value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TResult, TResult, bool>
            => this.Contains<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TComparer>(value, comparer);

        public int Count()
            => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource>();

        public int Count(Func<TResult, bool> predicate)
            => this.Count<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.Count<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(predicate);

        public long LongCount()
            => Count();

        public long LongCount(Func<TResult, bool> predicate)
            => Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => Count(predicate);

        public bool TryGetElementAt(int index, out TResult element)
            => this.TryGetElementAt<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(index, out element);

        public bool TryGetFirst(out TResult first)
        {
            first = default;
            var enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                return false;
            }
            acts.Execute(ref enumerator.Current, 0, ref first);
            enumerator.Dispose();
            return true;
        }

        public bool TryGetLast(out TResult last)
            => this.TryGetLast<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out last);

        public bool TryGetSingle(out TResult value)
            => this.TryGetSingle<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out value);

        public bool TryGetSingle<TPredicate>(out TResult value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.TryGetSingle<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TResult value, Func<TResult, bool> predicate)
            => this.TryGetSingle<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out value, predicate);

        public TResult[] ToArray()
            => this.ToArray<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>();

        public NativeArray<TResult> ToNativeArray(Allocator allocator)
            => this.ToNativeArray<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(allocator);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TResult, TKey> keySelector, Func<TResult, TElement> elementSelector)
            => this.ToDictionary<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TResult, TKey>
            where TElementFunc : unmanaged, IRefFunc<TResult, TElement>
            => this.ToDictionary<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TResult> ToHashSet()
            => this.ToHashSet<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>();

        public HashSet<TResult> ToHashSet(IEqualityComparer<TResult> comparer)
            => this.ToHashSet<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(comparer);

        public List<TResult> ToList()
            => this.ToList<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, Enumerator, TResult>();
        #endregion
    }
}