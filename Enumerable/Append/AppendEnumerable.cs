using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> : IRefEnumerable<AppendEnumerator<TPrevEnumerator, TSource>, TSource>, ILinq<TSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
    {
        private TPrevEnumerable enumerable;
        private TSource append;
        private readonly Allocator alloc;

        internal AppendEnumerable(in TPrevEnumerable enumerable, in TSource append, Allocator alloc)
        {
            this.enumerable = enumerable;
            this.append = append;
            this.alloc = alloc;
        }

        public AppendEnumerator<TPrevEnumerator, TSource> GetEnumerator() => new AppendEnumerator<TPrevEnumerator, TSource>(enumerable.GetEnumerator(), append, alloc);
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> AsRefEnumerable() => this;

        public AppendPointerEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(TSource* item)
            => new AppendPointerEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item);

        public AppendEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(in TSource item, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item, allocator);

        public SelectEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TResult, TAction> Select<TResult, TAction>(TAction action, Allocator allocator)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IRefAction<TSource, TResult>
            => new SelectEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TResult, TAction>(this, action, allocator);

        public SelectIndexEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TResult, TAction> SelectIndex<TResult, TAction>(TAction action, Allocator allocator)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TResult, TAction>(this, action, allocator);

        public WhereEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate>(this, predicate);

        public DefaultIfEmptyEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, defaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                TEnumerable, TEnumerator,
                TSource
            >
            Concat<TEnumerable, TEnumerator>(in TEnumerable second)
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TEnumerable, TEnumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, AppendEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<
                    AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                    AppendEnumerator<TPrevEnumerator, TSource>,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second);
        
        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(TSource[] second)
            => new ConcatEnumerable<
                    AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                    AppendEnumerator<TPrevEnumerator, TSource>,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                AppendEnumerator<TPrevEnumerator, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>, DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        public bool Any() => true;

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => predicate.Calc(ref append) || enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(predicate);

        public bool Any(Func<TSource, bool> predicate)
            => predicate(append) || enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => predicate.Calc(ref append) && enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(predicate);

        public bool All(Func<TSource, bool> predicate)
            => predicate(append) && enumerable.Any<TPrevEnumerable, TPrevEnumerator, TSource>(predicate);

        public void Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TSource Aggregate(TSource seed, Func<TSource, TSource, TSource> func)
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(seed, func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate>(seed, func);

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TResult>(seed, func, resultFunc);

        public bool Contains(TSource value)
            => append.Equals(value) || enumerable.Contains<TPrevEnumerable, TPrevEnumerator, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => comparer.Equals(append, value) || enumerable.Contains<TPrevEnumerable, TPrevEnumerator, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => comparer.Calc(ref append, ref value) || enumerable.Contains<TPrevEnumerable, TPrevEnumerator, TSource, TComparer>(value, comparer);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TFunc>(ref seed, func);

        TResult ILinq<TSource>.Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public void Aggregate<TFunc>(ref TSource seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TSource, TSource>
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TFunc>(ref seed, func);

        public bool Contains<TEqualityComparer>(in TSource value, TEqualityComparer comparer)
            where TEqualityComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => this.Contains<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TEqualityComparer>(value, comparer);

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
            => this.TryGetElementAt<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(index, out element);

        public bool TryGetFirst(out TSource first)
        {
            if (enumerable.TryGetFirst<TPrevEnumerable, TPrevEnumerator, TSource>(out first))
                return true;
            first = append;
            return true;
        }

        public bool TryGetLast(out TSource last)
        {
            last = append;
            return true;
        }

        public bool TryGetSingle(out TSource value)
        {
            if (enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource>(out value))
                return false;
            value = append;
            return true;
        }

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            if (!predicate.Calc(ref append))
                return enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(out value, predicate);
            if (enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(out value, predicate))
                return false;
            value = append;
            return true;
        }

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
        {
            if (!predicate(append))
                return enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource>(out value, predicate);
            if (enumerable.TryGetSingle<TPrevEnumerable, TPrevEnumerator, TSource>(out value, predicate))
                return false;
            value = append;
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
            answer[answer.LongLength - 1] = append;
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
            answer[answer.Length - 1] = append;
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            => this.ToDictionary<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
        {
            var answer = enumerable.ToDictionary<TPrevEnumerable, TPrevEnumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);
            answer.Add(keySelector.Calc(ref append), elementSelector.Calc(ref append));
            return answer;
        }

        public HashSet<TSource> ToHashSet()
        {
            var answer = enumerable.ToHashSet<TPrevEnumerable, TPrevEnumerator, TSource>();
            answer.Add(append);
            return answer;
        }

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
        {
            var answer = enumerable.ToHashSet<TPrevEnumerable, TPrevEnumerator, TSource>();
            answer.Add(append);
            return answer;
        }

        public List<TSource> ToList()
        {
            var answer = enumerable.ToList<TPrevEnumerable, TPrevEnumerator, TSource>();
            answer.Add(append);
            return answer;
        }
        #endregion
    }
}