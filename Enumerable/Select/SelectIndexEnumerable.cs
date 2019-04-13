using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>
        : IRefEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>.Enumerator, TResult>, ILinq<TResult>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TResult : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TResult>
#endif
        where TAction : unmanaged, ISelectIndex<TSource, TResult>
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
    {
        private TEnumerable enumerable;
        private TAction acts;
        private readonly Allocator alloc;

        internal SelectIndexEnumerable(in TEnumerable enumerable, TAction acts, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.acts = acts;
            this.alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TResult>
        {
            private TEnumerator enumerator;
            private readonly TResult* current;
            private readonly Allocator allocator;
            private TAction action;
            private int index;

            internal Enumerator(in TEnumerator enumerator, TAction action, Allocator allocator)
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

        public SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction> AsRefEnumerable() => this;

        public SelectIndexEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>
            SelectIndex<TNextResult, TNextAction>(TNextAction action, Allocator allocator = Allocator.Temp)
            where TNextResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextResult>
#endif
            where TNextAction : unmanaged, ISelectIndex<TResult, TNextResult>
            => new SelectIndexEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>(this, action, allocator);

        public SelectEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>
            Select<TNextResult, TNextAction>(TNextAction action, Allocator allocator = Allocator.Temp)
            where TNextResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextResult>
#endif
            where TNextAction : unmanaged, IRefAction<TResult, TNextResult>
            => new SelectEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TNextResult, TNextAction>(this, action, allocator);

        public AppendEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>
            Append(TResult value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(this, value, allocator);

        public AppendPointerEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>
            Append(TResult* value)
            => new AppendPointerEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(this, value);

        public DefaultIfEmptyEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>
            DefaultIfEmpty(TResult defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(this, defaultValue, allocator);

        public WhereEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>
            Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => new WhereEnumerable<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(this, predicate);

        public bool Any() => enumerable.Any<TEnumerable, TEnumerator, TSource>();

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.Any<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(predicate);

        public bool Any(Func<TResult, bool> predicate)
            => this.Any<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.All<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(predicate);

        public bool All(Func<TResult, bool> predicate)
            => this.All<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(predicate);

        public void Aggregate<TFunc>(ref TResult seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TResult, TResult>
            => this.Aggregate<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TFunc>(ref seed, func);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TResult>
            => this.Aggregate<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate, TFunc>(ref seed, func);

        public TNextResult Aggregate<TAccumulate, TNextResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TResult>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TNextResult>
            => this.Aggregate<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate, TNextResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TResult Aggregate(TResult seed, Func<TResult, TResult, TResult> func)
            => this.Aggregate<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(seed, func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func)
            => this.Aggregate<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate>(seed, func);

        public TNextResult Aggregate<TAccumulate, TNextResult>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func, Func<TAccumulate, TNextResult> resultFunc)
            => this.Aggregate<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TAccumulate, TNextResult>(seed, func, resultFunc);

        public bool Contains(TResult value)
            => this.Contains<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(value);

        public bool Contains(TResult value, IEqualityComparer<TResult> comparer)
            => this.Contains<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(value, comparer);

        public bool Contains<TComparer>(TResult value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TResult, TResult, bool>
            => this.Contains<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TComparer>(value, comparer);

        public int Count()
            => enumerable.Count<TEnumerable, TEnumerator, TSource>();

        public int Count(Func<TResult, bool> predicate)
            => this.Count<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.Count<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(predicate);

        public long LongCount()
            => Count();

        public long LongCount(Func<TResult, bool> predicate)
            => Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => Count(predicate);

        public bool TryGetElementAt(int index, out TResult element)
            => this.TryGetElementAt<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(index, out element);

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
            => this.TryGetLast<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out last);

        public bool TryGetSingle(out TResult value)
            => this.TryGetSingle<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out value);

        public bool TryGetSingle<TPredicate>(out TResult value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.TryGetSingle<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TResult value, Func<TResult, bool> predicate)
            => this.TryGetSingle<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(out value, predicate);

        public TResult[] ToArray()
            => this.ToArray<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>();

        public NativeArray<TResult> ToNativeArray(Allocator allocator)
            => this.ToNativeArray<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(allocator);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TResult, TKey> keySelector, Func<TResult, TElement> elementSelector)
            => this.ToDictionary<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TResult, TKey>
            where TElementFunc : unmanaged, IRefFunc<TResult, TElement>
            => this.ToDictionary<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TResult> ToHashSet()
            => this.ToHashSet<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>();

        public HashSet<TResult> ToHashSet(IEqualityComparer<TResult> comparer)
            => this.ToHashSet<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>(comparer);

        public List<TResult> ToList()
            => this.ToList<SelectIndexEnumerable<TEnumerable, TEnumerator, TSource, TResult, TAction>, Enumerator, TResult>();
    }
}