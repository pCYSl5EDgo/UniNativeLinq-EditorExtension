using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource> : IRefEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>.Enumerator, TSource>, ILinq<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TSource>
        where TFirstEnumerator : struct, IRefEnumerator<TSource>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSource>
        where TSecondEnumerator : struct, IRefEnumerator<TSource>
    {
        private TFirstEnumerable first;
        private TSecondEnumerable second;

        internal ConcatEnumerable(in TFirstEnumerable first, in TSecondEnumerable second)
        {
            this.first = first;
            this.second = second;
        }

        public Enumerator GetEnumerator() => new Enumerator(first.GetEnumerator(), second.GetEnumerator());

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource> AsRefEnumerable() => this;

        public AppendEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource> Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource>(this, value, allocator);

        public AppendPointerEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource> Append(TSource* value)
            => new AppendPointerEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource>(this, value);

        public WhereEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource, TPredicate>(this, predicate);

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

        public DefaultIfEmptyEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource>
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, TSource>(this, defaultValue, allocator);


        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TFirstEnumerator first;
            private TSecondEnumerator second;
            private bool isCurrentSecond;

            public ref TSource Current => ref (isCurrentSecond ? ref second.Current : ref first.Current);
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(in TFirstEnumerator first, in TSecondEnumerator second)
            {
                this.first = first;
                this.second = second;
                isCurrentSecond = false;
            }

            public void Dispose()
            {
                first.Dispose();
                second.Dispose();
            }

            public bool MoveNext()
            {
                if (isCurrentSecond) return second.MoveNext();
                if (first.MoveNext()) return true;
                isCurrentSecond = true;
                return second.MoveNext();
            }

            public void Reset() => throw new InvalidOperationException();
        }

        public bool Any()
            => first.Any<TFirstEnumerable, TFirstEnumerator, TSource>() || second.Any<TSecondEnumerable, TSecondEnumerator, TSource>();

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => first.Any<TFirstEnumerable, TFirstEnumerator, TSource, TPredicate>(predicate) || second.Any<TSecondEnumerable, TSecondEnumerator, TSource, TPredicate>(predicate);

        public bool Any(Func<TSource, bool> predicate)
            => first.Any<TFirstEnumerable, TFirstEnumerator, TSource>(predicate) || second.Any<TSecondEnumerable, TSecondEnumerator, TSource>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => first.All<TFirstEnumerable, TFirstEnumerator, TSource, TPredicate>(predicate) || second.All<TSecondEnumerable, TSecondEnumerator, TSource, TPredicate>(predicate);

        public bool All(Func<TSource, bool> predicate)
            => first.All<TFirstEnumerable, TFirstEnumerator, TSource>(predicate) || second.All<TSecondEnumerable, TSecondEnumerator, TSource>(predicate);

        public void Aggregate<TFunc>(ref TSource seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TSource, TSource>
        {
            first.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TFunc>(ref seed, func);
            second.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TFunc>(ref seed, func);
        }

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
        {
            first.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TAccumulate, TFunc>(ref seed, func);
            second.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TAccumulate, TFunc>(ref seed, func);
        }

        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
        {
            first.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TAccumulate, TFunc>(ref seed, func);
            return second.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);
        }

        public TSource Aggregate(TSource seed, Func<TSource, TSource, TSource> func)
            => second.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource>(first.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource>(seed, func), func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => second.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TAccumulate>(first.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TAccumulate>(seed, func), func);

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => second.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TAccumulate, TResult>(first.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TAccumulate>(seed, func), func, resultFunc);

        public bool Contains(TSource value)
            => first.Contains<TFirstEnumerable, TFirstEnumerator, TSource>(value) || second.Contains<TSecondEnumerable, TSecondEnumerator, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => first.Contains<TFirstEnumerable, TFirstEnumerator, TSource>(value, comparer) || second.Contains<TSecondEnumerable, TSecondEnumerator, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => first.Contains<TFirstEnumerable, TFirstEnumerator, TSource, TComparer>(value, comparer) || second.Contains<TSecondEnumerable, TSecondEnumerator, TSource, TComparer>(value, comparer);

        public int Count()
            => first.Count<TFirstEnumerable, TFirstEnumerator, TSource>() + second.Count<TSecondEnumerable, TSecondEnumerator, TSource>();

        public int Count(Func<TSource, bool> predicate)
            => first.Count<TFirstEnumerable, TFirstEnumerator, TSource>(predicate) + second.Count<TSecondEnumerable, TSecondEnumerator, TSource>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => first.Count<TFirstEnumerable, TFirstEnumerator, TSource, TPredicate>(predicate) + second.Count<TSecondEnumerable, TSecondEnumerator, TSource, TPredicate>(predicate);

        public long LongCount()
            => Count();

        public long LongCount(Func<TSource, bool> predicate)
            => Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => Count(predicate);

        public bool TryGetElementAt(int index, out TSource element)
        {
            var firstEnumerator = first.GetEnumerator();
            while (firstEnumerator.MoveNext())
            {
                if (--index >= 0) continue;
                element = firstEnumerator.Current;
                firstEnumerator.Dispose();
                return true;
            }
            firstEnumerator.Dispose();
            var secondEnumerator = second.GetEnumerator();
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

        // ReSharper disable once ParameterHidesMember
        public bool TryGetFirst(out TSource first)
            => this.first.TryGetFirst<TFirstEnumerable, TFirstEnumerator, TSource>(out first) || second.TryGetFirst<TSecondEnumerable, TSecondEnumerator, TSource>(out first);

        public bool TryGetLast(out TSource last)
            => second.TryGetLast<TSecondEnumerable, TSecondEnumerator, TSource>(out last) || first.TryGetLast<TFirstEnumerable, TFirstEnumerator, TSource>(out last);

        public bool TryGetSingle(out TSource value)
        {
            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();
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
            else
            {
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
        }

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            var firstResult = first.TryGetSingle<TFirstEnumerable, TFirstEnumerator, TSource, TPredicate>(out value, predicate);
            var secondResult = second.TryGetSingle<TSecondEnumerable, TSecondEnumerator, TSource, TPredicate>(out var secondValue, predicate);
            if (firstResult) return !secondResult;
            if (!secondResult) return false;
            value = secondValue;
            return true;
        }

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
        {
            var firstResult = first.TryGetSingle<TFirstEnumerable, TFirstEnumerator, TSource>(out value, predicate);
            var secondResult = second.TryGetSingle<TSecondEnumerable, TSecondEnumerator, TSource>(out var secondValue, predicate);
            if (firstResult) return !secondResult;
            if (!secondResult) return false;
            value = secondValue;
            return true;
        }

        public TSource[] ToArray()
        {
            var answer = new TSource[Count()];
            var index = 0;
            var firstEnumerator = first.GetEnumerator();
            while (firstEnumerator.MoveNext())
                answer[index++] = firstEnumerator.Current;
            firstEnumerator.Dispose();
            var secondEnumerator = second.GetEnumerator();
            while (secondEnumerator.MoveNext())
                answer[index++] = secondEnumerator.Current;
            secondEnumerator.Dispose();
            return answer;
        }

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            var answer = new NativeArray<TSource>(Count(), allocator);
            var index = 0;
            var firstEnumerator = first.GetEnumerator();
            while (firstEnumerator.MoveNext())
                answer[index++] = firstEnumerator.Current;
            firstEnumerator.Dispose();
            var secondEnumerator = second.GetEnumerator();
            while (secondEnumerator.MoveNext())
                answer[index++] = secondEnumerator.Current;
            secondEnumerator.Dispose();
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var answer = first.ToDictionary<TFirstEnumerable, TFirstEnumerator, TSource, TKey, TElement>(keySelector, elementSelector);
            foreach (ref var source in second)
                answer.Add(keySelector(source), elementSelector(source));
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
        {
            var answer = first.ToDictionary<TFirstEnumerable, TFirstEnumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);
            foreach (ref var source in second)
                answer.Add(keySelector.Calc(ref source), elementSelector.Calc(ref source));
            return answer;
        }

        public HashSet<TSource> ToHashSet()
        {
            var answer = first.ToHashSet<TFirstEnumerable, TFirstEnumerator, TSource>();
            foreach (ref var source in second)
                answer.Add(source);
            return answer;
        }

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
        {
            var answer = first.ToHashSet<TFirstEnumerable, TFirstEnumerator, TSource>(comparer);
            foreach (ref var source in second)
                answer.Add(source);
            return answer;
        }

        public List<TSource> ToList()
        {
            var answer = new List<TSource>(Count());
            foreach (ref var source in first)
                answer.Add(source);
            foreach (ref var source in second)
                answer.Add(source);
            return answer;
        }
    }
}