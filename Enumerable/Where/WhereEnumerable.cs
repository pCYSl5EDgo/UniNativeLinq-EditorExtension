using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public struct WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate> : IRefEnumerable<WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>, ILinq<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TPredicate : struct, IRefFunc<TSource, bool>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
    {
        private TPrevEnumerable enumerable;
        private readonly TPredicate predicts;

        internal WhereEnumerable(in TPrevEnumerable enumerable, TPredicate predicts)
        {
            this.enumerable = enumerable;
            this.predicts = predicts;
        }

        public WhereEnumerator<TPrevEnumerator, TSource, TPredicate> GetEnumerator()
            => new WhereEnumerator<TPrevEnumerator, TSource, TPredicate>(enumerable.GetEnumerator(), predicts);

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate> AsRefEnumerable() => this;

        public WhereEnumerable<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TNextPredicate> Where<TNextPredicate>(TNextPredicate predicate)
            where TNextPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TNextPredicate>(this, predicate);

        public SelectEnumerable<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TResult, TAction> Select<TResult, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IRefAction<TSource, TResult>
            => new SelectEnumerable<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TResult, TAction>(this, action, allocator);

        public AppendEnumerable<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource> Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(this, value, allocator);

        public unsafe AppendPointerEnumerable<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource> Append(TSource* value)
            => new AppendPointerEnumerable<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(this, value);

        public bool Any()
            => this.Any<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>();

        public bool Any<TAnotherPredicate>(TAnotherPredicate predicate)
            where TAnotherPredicate : struct, IRefFunc<TSource, bool>
            => this.Any<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TAnotherPredicate>(predicate);

        public bool Any(Func<TSource, bool> predicate)
            => this.Any<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(predicate);

        public bool All<TAnotherPredicate>(TAnotherPredicate predicate)
            where TAnotherPredicate : struct, IRefFunc<TSource, bool>
            => this.All<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TAnotherPredicate>(predicate);

        public bool All(Func<TSource, bool> predicate)
            => this.All<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(predicate);

        public void Aggregate<TFunc>(ref TSource seed, TFunc func)
            where TFunc : struct, IRefAction<TSource, TSource>
            => this.Aggregate<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TFunc>(ref seed, func);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : struct, IRefAction<TAccumulate, TSource>
            => this.Aggregate<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TAccumulate, TFunc>(ref seed, func);

        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : struct, IRefAction<TAccumulate, TSource>
            where TResultFunc : struct, IRefFunc<TAccumulate, TResult>
            => this.Aggregate<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TSource Aggregate(TSource seed, Func<TSource, TSource, TSource> func)
            => this.Aggregate<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(seed, func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => this.Aggregate<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TAccumulate>(seed, func);

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => this.Aggregate<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TAccumulate, TResult>(seed, func, resultFunc);

        public bool Contains(TSource value)
            => this.Contains<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => this.Contains<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : struct, IRefFunc<TSource, TSource, bool>
            => this.Contains<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TComparer>(value, comparer);

        public int Count()
            => this.Count<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>();

        public int Count(Func<TSource, bool> predicate)
            => this.Count<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(predicate);

        public int Count<TAnotherPredicate>(TAnotherPredicate predicate)
            where TAnotherPredicate : struct, IRefFunc<TSource, bool>
            => this.Count<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TAnotherPredicate>(predicate);

        public long LongCount()
            => this.Count<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>();

        public long LongCount(Func<TSource, bool> predicate)
            => this.Count<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(predicate);

        public long LongCount<TAnotherPredicate>(TAnotherPredicate predicate)
            where TAnotherPredicate : struct, IRefFunc<TSource, bool>
            => this.Count<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TAnotherPredicate>(predicate);

        public bool TryGetElementAt(int index, out TSource element)
            => this.TryGetElementAt<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(index, out element);

        public bool TryGetFirst(out TSource first)
            => this.TryGetFirst<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(out first);

        public bool TryGetLast(out TSource last)
            => this.TryGetLast<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(out last);

        public bool TryGetSingle(out TSource value)
            => this.TryGetSingle<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(out value);

        public bool TryGetSingle<TAnotherPredicate>(out TSource value, TAnotherPredicate predicate)
            where TAnotherPredicate : struct, IRefFunc<TSource, bool>
            => this.TryGetSingle<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TAnotherPredicate>(out value, predicate);

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
            => this.TryGetSingle<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(out value, predicate);

        public TSource[] ToArray()
            => this.ToArray<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>();

        public NativeArray<TSource> ToNativeArray(Allocator alloc)
            => this.ToNativeArray<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(alloc);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            => this.ToDictionary<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : struct, IRefFunc<TSource, TKey>
            where TElementFunc : struct, IRefFunc<TSource, TElement>
            => this.ToDictionary<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TSource> ToHashSet()
            => this.ToHashSet<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>();

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
            => this.ToHashSet<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>(comparer);

        public List<TSource> ToList()
            => this.ToList<WhereEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>, WhereEnumerator<TPrevEnumerator, TSource, TPredicate>, TSource>();
    }
}