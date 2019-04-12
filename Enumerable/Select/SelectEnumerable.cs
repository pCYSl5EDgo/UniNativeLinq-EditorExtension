using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public struct SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction> : IRefEnumerable<SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>, ILinq<TResult>
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

        public SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction> GetEnumerator()
            => new SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>(enumerable.GetEnumerator(), action, alloc);

        internal SelectEnumerable(in TPrevEnumerable enumerable, TAction action, Allocator alloc)
        {
            this.enumerable = enumerable;
            this.action = action;
            this.alloc = alloc;
        }

        IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction> AsRefEnumerable() => this;

        public SelectEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TNextResult, TNextAction> Select<TNextResult, TNextAction>(TNextAction nextAction, Allocator allocator = Allocator.Temp)
            where TNextAction : struct, IRefAction<TResult, TNextResult>
            where TNextResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextResult>
#endif
            => new SelectEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TNextResult, TNextAction>(this, nextAction, allocator);

        public WhereEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => new WhereEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TPredicate>(this, predicate);

        public AppendEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult> Append(TResult value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(this, value, allocator);
        
        public unsafe AppendPointerEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult> Append(TResult* value)
            => new AppendPointerEnumerable<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(this, value);
            

        public bool Any() => enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource>();

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

        public void Aggregate<TFunc>(ref TResult seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TResult, TResult>
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TFunc>(ref seed, func);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TResult>
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TAccumulate, TFunc>(ref seed, func);

        public TNextResult Aggregate<TAccumulate, TNextResult, TFunc, TNextResultFunc>(ref TAccumulate seed, TFunc func, TNextResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TResult>
            where TNextResultFunc : unmanaged, IRefFunc<TAccumulate, TNextResult>
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TAccumulate, TNextResult, TFunc, TNextResultFunc>(ref seed, func, resultFunc);

        public TResult Aggregate(TResult seed, Func<TResult, TResult, TResult> func)
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(seed, func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func)
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TAccumulate>(seed, func);

        public TResult1 Aggregate<TAccumulate, TResult1>(TAccumulate seed, Func<TAccumulate, TResult, TAccumulate> func, Func<TAccumulate, TResult1> resultFunc)
            => this.Aggregate<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TAccumulate, TResult1>(seed, func, resultFunc);

        public bool Contains(TResult value)
            => this.Contains<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(value);

        public bool Contains(TResult value, IEqualityComparer<TResult> comparer)
            => this.Contains<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(value, comparer);

        public bool Contains<TComparer>(TResult value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TResult, TResult, bool>
            => this.Contains<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TComparer>(value, comparer);

        public int Count()
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>();

        public int Count(Func<TResult, bool> predicate)
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TPredicate>(predicate);

        public long LongCount()
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>();

        public long LongCount(Func<TResult, bool> predicate)
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.Count<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TPredicate>(predicate);

        public bool TryGetElementAt(int index, out TResult element)
            => this.TryGetElementAt<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(index, out element);

        public bool TryGetFirst(out TResult first)
            => this.TryGetFirst<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(out first);

        public bool TryGetLast(out TResult last)
            => this.TryGetLast<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(out last);

        public bool TryGetSingle(out TResult value)
            => this.TryGetSingle<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(out value);

        public bool TryGetSingle<TPredicate>(out TResult value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TResult, bool>
            => this.TryGetSingle<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TResult value, Func<TResult, bool> predicate)
            => this.TryGetSingle<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(out value, predicate);

        public TResult[] ToArray()
            => this.ToArray<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>();

        public NativeArray<TResult> ToNativeArray(Allocator allocator)
            => this.ToNativeArray<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(allocator);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TResult, TKey> keySelector, Func<TResult, TElement> elementSelector)
            => this.ToDictionary<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TResult, TKey>
            where TElementFunc : unmanaged, IRefFunc<TResult, TElement>
            => this.ToDictionary<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TResult> ToHashSet()
            => this.ToHashSet<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>();

        public HashSet<TResult> ToHashSet(IEqualityComparer<TResult> comparer)
            => this.ToHashSet<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>(comparer);

        public List<TResult> ToList()
            => this.ToList<SelectEnumerable<TPrevEnumerable, TPrevEnumerator, TSource, TResult, TAction>, SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction>, TResult>();
    }
}