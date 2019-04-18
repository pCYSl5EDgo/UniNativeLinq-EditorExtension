using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>
        : IRefEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>.Enumerator, TSource>, ILinq<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
        where TEnumerator : struct, IRefEnumerator<TSource>
    {
        private TEnumerable enumerable;
        private readonly TSource val;
        private readonly Allocator alloc;

        internal DefaultIfEmptyEnumerable(in TEnumerable enumerable, in TSource val, Allocator alloc)
        {
            this.enumerable = enumerable;
            this.val = val;
            this.alloc = alloc;
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), val, alloc);
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private readonly TSource* ptr;
            private readonly Allocator allocator;
            private bool isFirst;
            private bool isDefault;

            internal Enumerator(in TEnumerator enumerator, in TSource value, Allocator allocator)
            {
                this.enumerator = enumerator;
                this.ptr = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                *this.ptr = value;
                this.allocator = allocator;
                isFirst = true;
                isDefault = false;
            }

            internal Enumerator(in TEnumerator enumerator, TSource* ptr)
            {
                this.enumerator = enumerator;
                this.ptr = ptr;
                this.allocator = Allocator.None;
                isFirst = true;
                isDefault = false;
            }

            public ref TSource Current
            {
                get
                {
                    if (isDefault)
                        return ref *ptr;
                    else
                        return ref enumerator.Current;
                }
            }

            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                if (ptr != null && allocator != Allocator.None)
                    UnsafeUtility.Free(ptr, allocator);
                this = default;
            }

            public bool MoveNext()
            {
                if (!isFirst) return !isDefault && enumerator.MoveNext();
                isFirst = false;
                isDefault = !enumerator.MoveNext();
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
        }

        public DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>
            AsRefEnumerable()
            => this;

        #region Enumerable
        public AppendEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>
            Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(this, value, allocator);

        public AppendPointerEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>
            Append(TSource* value)
            => new AppendPointerEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(this, value);

        public DefaultIfEmptyEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);

        public DistinctEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
            Distinct<TEqualityComparer, TGetHashCodeFunc>(TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new DistinctEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TNextSource, TAction>
            Select<TNextSource, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TNextSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextSource>
#endif
            where TAction : unmanaged, IRefAction<TSource, TNextSource>
            => new SelectEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TNextSource, TAction>(this, action, allocator);

        public SelectIndexEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TNextSource, TAction>
            SelectIndex<TNextSource, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TNextSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextSource>
#endif
            where TAction : unmanaged, ISelectIndex<TSource, TNextSource>
            => new SelectIndexEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TNextSource, TAction>(this, action, allocator);

        public SelectManyEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
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
            => new SelectManyEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public WhereEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TPredicate>
            Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TPredicate>(this, predicate);
        #endregion

        #region Concat
        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                TEnumerable1, TEnumerator1,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>(in TEnumerable1 second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                TEnumerable1, TEnumerator1, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2>(in ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, AppendEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                    Enumerator,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(TSource[] second)
            => new ConcatEnumerable<
                    DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                    Enumerator,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>, DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEqualityComparer, TGetHashCodeFunc>
            (in DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                RangeRepeatEnumerable<TSource, TAction>,
                RangeRepeatEnumerable<TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TAction>
            (in RangeRepeatEnumerable<TSource, TAction> second)
            where TAction : struct, IRefAction<TSource>
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, RangeRepeatEnumerable<TSource, TAction>, RangeRepeatEnumerable<TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
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
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
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
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
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
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>,
                Enumerator,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TPredicate>
            (in WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => enumerable.CanFastCount();

        public bool Any() => true;

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.Any<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TPredicate>(predicate);

        public bool Any(Func<TSource, bool> predicate)
            => this.Any<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.All<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TPredicate>(predicate);

        public bool All(Func<TSource, bool> predicate)
            => this.All<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(predicate);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            => this.Aggregate<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TAccumulate, TFunc>(ref seed, func);

        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
            => this.Aggregate<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
            => this.Aggregate<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => this.Aggregate<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TAccumulate>(seed, func);

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => this.Aggregate<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TAccumulate, TResult>(seed, func, resultFunc);

        public bool Contains(TSource value)
            => this.Contains<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => this.Contains<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => this.Contains<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TComparer>(value, comparer);

        public int Count()
        {
            int count;
            if (enumerable.CanFastCount())
            {
                count = enumerable.Count();
                return count == 0 ? 1 : count;
            }
            var enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                return 1;
            }
            count = 1;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        public int Count(Func<TSource, bool> predicate)
            => this.Count<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.Count<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TPredicate>(predicate);

        public long LongCount()
        {
            long count;
            if (enumerable.CanFastCount())
            {
                count = enumerable.LongCount();
                return count == 0 ? 1 : count;
            }
            var enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                return 1;
            }
            count = 1;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        public long LongCount(Func<TSource, bool> predicate)
            => this.Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.Count(predicate);

        public bool TryGetElementAt(int index, out TSource element)
            => this.TryGetElementAt<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(index, out element);

        public bool TryGetFirst(out TSource first)
        {
            var enumerator = enumerable.GetEnumerator();
            first = enumerator.MoveNext() ? enumerator.Current : val;
            enumerator.Dispose();
            return true;
        }

        public bool TryGetLast(out TSource last)
        {
            if (!enumerable.TryGetLast<TEnumerable, TEnumerator, TSource>(out last))
                last = val;
            return true;
        }

        public bool TryGetSingle(out TSource value)
        {
            var enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                value = val;
                enumerator.Dispose();
                return true;
            }
            value = enumerator.Current;
            if (enumerator.MoveNext())
            {
                enumerator.Dispose();
                return false;
            }
            enumerator.Dispose();
            return true;
        }

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.TryGetSingle<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
            => this.TryGetSingle<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(out value, predicate);

        public TSource[] ToArray()
        {
            var length = enumerable.Count();
            if (length == 0)
                return new[] {val};
            var answer = new TSource[length];
            var enumerator = enumerable.GetEnumerator();
            var index = 0;
            while (enumerator.MoveNext())
                answer[index++] = enumerator.Current;
            enumerator.Dispose();
            return answer;
        }

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            var length = enumerable.Count();
            NativeArray<TSource> answer;
            if (length == 0)
            {
                answer = new NativeArray<TSource>(1, allocator) {[0] = val};
                return answer;
            }
            answer = new NativeArray<TSource>(length, allocator);
            var enumerator = enumerable.GetEnumerator();
            var index = 0;
            while (enumerator.MoveNext())
                answer[index++] = enumerator.Current;
            enumerator.Dispose();
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            => this.ToDictionary<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
            => this.ToDictionary<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TSource> ToHashSet()
            => this.ToHashSet<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>();

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
            => this.ToHashSet<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>(comparer);

        public List<TSource> ToList()
            => this.ToList<DefaultIfEmptyEnumerable<TEnumerable, TEnumerator, TSource>, Enumerator, TSource>();
        #endregion
    }
}