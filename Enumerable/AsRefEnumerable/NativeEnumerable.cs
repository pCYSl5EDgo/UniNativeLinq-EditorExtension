using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct
        NativeEnumerable<TSource>
        : IRefEnumerable<NativeEnumerable<TSource>.Enumerator, TSource>, ILinq<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
    {
        internal readonly TSource* Ptr;
        internal readonly int Length;

        internal NativeEnumerable(NativeArray<TSource> array)
        {
            if (array.IsCreated)
            {
                this.Ptr = array.GetPointer();
                this.Length = array.Length;
            }
            else
            {
                this.Ptr = null;
                this.Length = 0;
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        #region Enumerable
        public AppendEnumerable<NativeEnumerable<TSource>, Enumerator, TSource> Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<NativeEnumerable<TSource>, Enumerator, TSource>(this, value, allocator);

        public AppendPointerEnumerable<NativeEnumerable<TSource>, Enumerator, TSource> Append(TSource* value)
            => new AppendPointerEnumerable<NativeEnumerable<TSource>, Enumerator, TSource>(this, value);

        public NativeEnumerable<TSource> AsRefEnumerable() => this;

        public DefaultIfEmptyEnumerable<NativeEnumerable<TSource>, Enumerator, TSource> DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<NativeEnumerable<TSource>, Enumerator, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);

        public DistinctEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
            Distinct<TEqualityComparer, TGetHashCodeFunc>
            (TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new DistinctEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TResult, TAction> Select<TResult, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IRefAction<TSource, TResult>
            => new SelectEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectIndexEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TResult, TAction> SelectIndex<TResult, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectManyEnumerable<
                NativeEnumerable<TSource>,
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
            => new SelectManyEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public WhereEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TPredicate>(this, predicate);

        public ZipEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
            Zip<TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
            (in TEnumerable0 second, TAction0 action, TSource firstDefaultValue = default, TSource0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource0>
            where TEnumerator0 : struct, IRefEnumerator<TSource0>
            where TSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource0>
#endif
            where TAction0 : unmanaged, IRefAction<TSource, TSource0, TResult0>
            where TResult0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult0>
#endif
            => new ZipEnumerable<NativeEnumerable<TSource>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                TEnumerable, TEnumerator,
                TSource
            >
            Concat<TEnumerable, TEnumerator>(in TEnumerable second)
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                TEnumerable, TEnumerator, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                NativeEnumerable<TSource>,
                Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, NativeEnumerable<TSource>, Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                NativeEnumerable<TSource>,
                Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, NativeEnumerable<TSource>, Enumerator, TSource>(this, second.AsRefEnumerable());

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1>
            (in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<
                    NativeEnumerable<TSource>,
                    Enumerator,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in TSource[] second)
            => new ConcatEnumerable<
                    NativeEnumerable<TSource>,
                    Enumerator,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>, Enumerator,
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
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>, Enumerator,
                RangeRepeatEnumerable<TSource, TAction>,
                RangeRepeatEnumerable<TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TAction>
            (in RangeRepeatEnumerable<TSource, TAction> second)
            where TAction : struct, IRefAction<TSource>
            => new ConcatEnumerable<
                    NativeEnumerable<TSource>, Enumerator,
                    RangeRepeatEnumerable<TSource, TAction>,
                    RangeRepeatEnumerable<TSource, TAction>.Enumerator,
                    TSource
                >
                (this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource, TAction>
            (in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction> second)
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TAction : unmanaged, ISelectIndex<TPrevSource, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource, TAction>
            (in SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction> second)
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
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
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPredicate>
            (in WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                NativeEnumerable<TSource>,
                Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TAction0>
            (in ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0> second)
            where TSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource0>
            where TSource1 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource1>
#endif
            where TEnumerator1 : struct, IRefEnumerator<TSource1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource1>
            where TAction0 : struct, IRefAction<TSource0, TSource1, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        void ILinq<TSource>.Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
        {
            for (var i = 0; i < Length; i++)
                func.Execute(ref seed, ref Ptr[i]);
        }

        TResult ILinq<TSource>.Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
        {
            for (var i = 0; i < Length; i++)
                func.Execute(ref seed, ref Ptr[i]);
            return resultFunc.Calc(ref seed);
        }

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
        {
            if (Length == 0) throw new InvalidOperationException();
            var seed = Ptr[0];
            for (var i = 1L; i < Length; i++)
                seed = func(seed, Ptr[i]);
            return seed;
        }

        TAccumulate ILinq<TSource>.Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            for (var i = 0; i < Length; i++)
                seed = func(seed, Ptr[i]);
            return seed;
        }

        TResult ILinq<TSource>.Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
        {
            for (var i = 0; i < Length; i++)
                seed = func(seed, Ptr[i]);
            return resultFunc(seed);
        }

        bool ILinq<TSource>.All<TPredicate>(TPredicate predicate)
        {
            for (var i = 0; i < Length; i++)
                if (!predicate.Calc(ref Ptr[i]))
                    return false;
            return true;
        }

        bool ILinq<TSource>.All(Func<TSource, bool> predicate)
        {
            for (var i = 0; i < Length; i++)
                if (!predicate(Ptr[i]))
                    return false;
            return true;
        }

        public bool Any() => Length != 0;

        bool ILinq<TSource>.Any<TPredicate>(TPredicate predicate)
        {
            for (var i = 0; i < Length; i++)
                if (predicate.Calc(ref Ptr[i]))
                    return true;
            return false;
        }

        public bool Any(Func<TSource, bool> predicate)
        {
            for (var i = 0; i < Length; i++)
                if (predicate(Ptr[i]))
                    return true;
            return false;
        }

        bool ILinq<TSource>.Contains(TSource value)
        {
            for (var i = 0; i < Length; i++)
                if (Ptr[i].Equals(value))
                    return true;
            return false;
        }

        bool ILinq<TSource>.Contains(TSource value, IEqualityComparer<TSource> comparer)
        {
            for (var i = 0; i < Length; i++)
                if (comparer.Equals(value, Ptr[i]))
                    return true;
            return false;
        }

        bool ILinq<TSource>.Contains<TComparer>(TSource value, TComparer comparer)
        {
            for (var i = 0; i < Length; i++)
                if (comparer.Calc(ref Ptr[i], ref value))
                    return true;
            return false;
        }

        public int Count() => this.Length;

        public int Count(Func<TSource, bool> predicate)
        {
            var count = 0;
            for (var i = 0; i < Length; i++)
                if (predicate(Ptr[i]))
                    ++count;
            return count;
        }

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            var count = 0;
            for (var i = 0; i < Length; i++)
                if (predicate.Calc(ref Ptr[i]))
                    ++count;
            return count;
        }

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public long LongCount() => this.Length;

        long ILinq<TSource>.LongCount(Func<TSource, bool> predicate) => Count(predicate);

        long ILinq<TSource>.LongCount<TPredicate>(TPredicate predicate) => Count(predicate);

        TSource[] ILinq<TSource>.ToArray()
        {
            if (Length <= 0) return Array.Empty<TSource>();
            var answer = new TSource[Length];
            fixed (TSource* p = &answer[0])
                UnsafeUtility.MemCpy(p, Ptr, sizeof(TSource) * Length);
            return answer;
        }

        Dictionary<TKey, TElement> ILinq<TSource>.ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var answer = new Dictionary<TKey, TElement>(Length);
            for (var i = 0; i < Length; i++)
                answer.Add(keySelector(Ptr[i]), elementSelector(Ptr[i]));
            return answer;
        }

        Dictionary<TKey, TElement> ILinq<TSource>.ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
        {
            var answer = new Dictionary<TKey, TElement>(Length);
            for (var i = 0; i < Length; i++)
            {
                ref var item = ref Ptr[i];
                answer.Add(keySelector.Calc(ref item), elementSelector.Calc(ref item));
            }
            return answer;
        }

        HashSet<TSource> ILinq<TSource>.ToHashSet()
        {
            var answer = new HashSet<TSource>();
            for (var i = 0; i < Length; i++)
                answer.Add(Ptr[i]);
            return answer;
        }

        HashSet<TSource> ILinq<TSource>.ToHashSet(IEqualityComparer<TSource> comparer)
        {
            var answer = new HashSet<TSource>(comparer);
            for (var i = 0; i < Length; i++)
                answer.Add(Ptr[i]);
            return answer;
        }

        List<TSource> ILinq<TSource>.ToList()
        {
            var answer = new List<TSource>(Length);
            for (var i = 0; i < Length; i++)
                answer.Add(Ptr[i]);
            return answer;
        }

        NativeArray<TSource> ILinq<TSource>.ToNativeArray(Allocator allocator)
        {
            if (Length == 0) return default;
            var answer = new NativeArray<TSource>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            UnsafeUtility.MemCpy(answer.GetPointer(), Ptr, sizeof(TSource) * Length);
            return answer;
        }

        bool ILinq<TSource>.TryGetElementAt(int index, out TSource element)
        {
            if (index < 0 || index >= Length)
            {
                element = default;
                return false;
            }
            element = Ptr[index];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetFirst(out TSource first)
        {
            if (Length == 0)
            {
                first = default;
                return false;
            }
            first = *Ptr;
            return true;
        }

        public bool TryGetLast(out TSource last)
        {
            if (Length == 0)
            {
                last = default;
                return false;
            }
            last = Ptr[Length - 1];
            return true;
        }

        bool ILinq<TSource>.TryGetSingle(out TSource value)
        {
            if (Length != 1)
            {
                value = default;
                return false;
            }
            value = *Ptr;
            return true;
        }

        bool ILinq<TSource>.TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            => this.TryGetSingle<NativeEnumerable<TSource>, Enumerator, TSource, TPredicate>(out value, predicate);

        bool ILinq<TSource>.TryGetSingle(out TSource value, Func<TSource, bool> predicate)
            => this.TryGetSingle<NativeEnumerable<TSource>, Enumerator, TSource>(out value, predicate);
        #endregion

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private readonly TSource* ptr;
            private readonly int length;
            private int index;

            public ref TSource Current => ref ptr[index];
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(in NativeEnumerable<TSource> parent)
            {
                index = -1;
                ptr = parent.Ptr;
                length = parent.Length;
            }

            public void Dispose() => this = default;

            public bool MoveNext() => ++index < length;

            public void Reset() => throw new InvalidOperationException();
        }
    }
}