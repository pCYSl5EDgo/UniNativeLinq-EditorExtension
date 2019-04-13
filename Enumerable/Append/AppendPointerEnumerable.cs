using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> : IRefEnumerable<AppendEnumerator<TPrevEnumerator, TSource>, TSource>, ILinq<TSource>
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

        public AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> AsRefEnumerable() => this;

        public AppendPointerEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(TSource* item)
            => new AppendPointerEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item);

        public AppendEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(in TSource item, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item, allocator);

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
        
        public WhereEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate>(this, predicate);

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

        public TSource Aggregate(TSource seed, Func<TSource, TSource, TSource> func)
            => this.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(seed, func);

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

        public void Aggregate<TFunc>(ref TSource seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TSource, TSource>
            => this.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TFunc>(ref seed, func);

        public bool Contains<TEqualityComparer>(in TSource value, TEqualityComparer comparer)
            where TEqualityComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => this.Contains<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TEqualityComparer>(value, comparer);

        public int Count()
            => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource>() + 1;

        public int Count(Func<TSource, bool> predicate)
            => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource>(predicate) + 1;

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(predicate);

        public long LongCount()
            => Count();

        public long LongCount(Func<TSource, bool> predicate)
            => Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => Count(predicate);

        public bool TryGetElementAt(int index, out TSource element)
            => this.TryGetElementAt<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(index, out element);

        public bool TryGetFirst(out TSource first)
        {
            if (enumerable.TryGetFirst<TPrevEnumerable, TPrevEnumerator, TSource>(out first))
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
    }
}