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
                AppendEnumerator<TEnumerator2, TSource>,
                TSource
            >
            Concat<TEnumerable2, TEnumerator2>
            (in AppendEnumerable<TEnumerable2, TEnumerator2, TSource> second)
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, AppendEnumerable<TEnumerable2, TEnumerator2, TSource>, AppendEnumerator<TEnumerator2, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>,
                Enumerator,
                AppendPointerEnumerable<TEnumerable2, TEnumerator2, TSource>,
                AppendEnumerator<TEnumerator2, TSource>,
                TSource
            >
            Concat<TEnumerable2, TEnumerator2>
            (in AppendPointerEnumerable<TEnumerable2, TEnumerator2, TSource> second)
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>, Enumerator, AppendPointerEnumerable<TEnumerable2, TEnumerator2, TSource>, AppendEnumerator<TEnumerator2, TSource>, TSource>(this, second);

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
        #endregion

        #region Function
        public bool Any()
            => FirstEnumerable.Any<TFirstEnumerable, TFirstEnumerator, TSource>() || SecondEnumerable.Any<TSecondEnumerable, TSecondEnumerator, TSource>();

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

        public void Aggregate<TFunc>(ref TSource seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TSource, TSource>
        {
            FirstEnumerable.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource, TFunc>(ref seed, func);
            SecondEnumerable.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource, TFunc>(ref seed, func);
        }

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

        public TSource Aggregate(TSource seed, Func<TSource, TSource, TSource> func)
            => SecondEnumerable.Aggregate<TSecondEnumerable, TSecondEnumerator, TSource>(FirstEnumerable.Aggregate<TFirstEnumerable, TFirstEnumerator, TSource>(seed, func), func);

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

        public int Count()
            => FirstEnumerable.Count<TFirstEnumerable, TFirstEnumerator, TSource>() + SecondEnumerable.Count<TSecondEnumerable, TSecondEnumerator, TSource>();

        public int Count(Func<TSource, bool> predicate)
            => FirstEnumerable.Count<TFirstEnumerable, TFirstEnumerator, TSource>(predicate) + SecondEnumerable.Count<TSecondEnumerable, TSecondEnumerator, TSource>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => FirstEnumerable.Count<TFirstEnumerable, TFirstEnumerator, TSource, TPredicate>(predicate) + SecondEnumerable.Count<TSecondEnumerable, TSecondEnumerator, TSource, TPredicate>(predicate);

        public long LongCount()
            => Count();

        public long LongCount(Func<TSource, bool> predicate)
            => Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => Count(predicate);

        public bool TryGetElementAt(int index, out TSource element)
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

        // ReSharper disable once ParameterHidesMember
        public bool TryGetFirst(out TSource first)
            => this.FirstEnumerable.TryGetFirst<TFirstEnumerable, TFirstEnumerator, TSource>(out first) || SecondEnumerable.TryGetFirst<TSecondEnumerable, TSecondEnumerator, TSource>(out first);

        public bool TryGetLast(out TSource last)
            => SecondEnumerable.TryGetLast<TSecondEnumerable, TSecondEnumerator, TSource>(out last) || FirstEnumerable.TryGetLast<TFirstEnumerable, TFirstEnumerator, TSource>(out last);

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