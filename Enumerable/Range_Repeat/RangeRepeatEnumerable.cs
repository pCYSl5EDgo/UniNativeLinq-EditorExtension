using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct
        RangeRepeatEnumerable<TSource, TAction>
        : IRefEnumerable<RangeRepeatEnumerable<TSource, TAction>.Enumerator, TSource>, ILinq<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TAction : struct, IRefAction<TSource>
    {
        private readonly TSource start;
        private readonly long length;
        private TAction acts;
        private readonly Allocator alloc;

        internal RangeRepeatEnumerable(TSource start, long length, TAction acts, Allocator allocator)
        {
            this.start = start;
            this.length = length;
            this.acts = acts;
            this.alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private readonly Allocator allocator;
            private readonly TSource* current;
            private readonly long count;
            private long index;
            private TAction action;

            internal Enumerator(TSource start, long count, TAction action, Allocator allocator)
            {
                this.current = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                *this.current = start;
                this.count = count;
                this.index = -1;
                this.action = action;
                this.allocator = allocator;
            }

            public bool MoveNext()
            {
                if (++index >= count) return false;
                if (index > 0) action.Execute(ref *current);
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
            public ref TSource Current => ref *current;
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;
            public void Dispose() => UnsafeUtility.Free(current, allocator);
        }

        public Enumerator GetEnumerator() => new Enumerator(start, length, acts, alloc);
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<
            RangeRepeatEnumerable<TSource, TAction>,
            Enumerator,
            TSource
        > Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource>(this, value, allocator);

        public AppendPointerEnumerable<
            RangeRepeatEnumerable<TSource, TAction>,
            Enumerator,
            TSource
        > Append(TSource* value)
            => new AppendPointerEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource>(this, value);

        public RangeRepeatEnumerable<TSource, TAction> AsRefEnumerable() => this;

        public DefaultIfEmptyEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                TSource
            >
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);

        public DistinctEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
            Distinct<TEqualityComparer, TGetHashCodeFunc>(TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new DistinctEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectIndexEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                TSource,
                TResult,
                TAction1
            >
            SelectIndex<TResult, TAction1>(TAction1 action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction1 : unmanaged, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, TResult, TAction1>(this, action, allocator);

        public SelectEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                TSource,
                TResult,
                TAction1
            >
            Select<TResult, TAction1>(TAction1 action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction1 : unmanaged, IRefAction<TSource, TResult>
            => new SelectEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, TResult, TAction1>(this, action, allocator);

        public SelectManyEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                TSource,
                TResult,
                TResultEnumerable,
                TResultEnumerator,
                TResultAction
            >
            SelectMany<TResult, TResultEnumerable, TResultEnumerator, TResultAction>(TResultAction action)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TResultEnumerator : struct, IRefEnumerator<TResult>
            where TResultEnumerable : struct, IRefEnumerable<TResultEnumerator, TResult>
            where TResultAction : struct, IRefAction<TSource, TResultEnumerable>
            => new SelectManyEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public WhereEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                TSource,
                TPredicate
            >
            Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, TPredicate>(this, predicate);

        public ZipEnumerable<
                RangeRepeatEnumerable<TSource, TAction>
                , Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
            Zip<TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
            (in TEnumerable0 second, TAction0 action, TSource firstDefaultValue = default, TSource0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource0>
            where TEnumerator0 : struct, IRefEnumerator<TSource0>
            where TSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource0>
#endif
            where TResult0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult0>
#endif
            where TAction0 : unmanaged, IRefAction<TSource, TSource0, TResult0>
            => new ZipEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                TEnumerable0,
                TEnumerator0,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>(in TEnumerable0 second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TEnumerable0, TEnumerator0, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(TSource[] second)
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
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
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(this, default);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
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
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                TSource
            >
            Concat(in RangeRepeatEnumerable<TSource, TAction> second)
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource, TAction0>
            (in SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0> second)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource>
            where TAction0 : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
                Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource, TAction0>
            (in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0> second)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource>
            where TAction0 : unmanaged, ISelectIndex<TPrevSource, TSource>
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
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
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
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
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator, TSource>(this, second);
        
        public ConcatEnumerable<
                RangeRepeatEnumerable<TSource, TAction>,
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
            => new ConcatEnumerable<RangeRepeatEnumerable<TSource, TAction>, Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        public bool CanFastCount() => true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => length > 0;

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            var element = start;
            if (predicate.Calc(ref element)) return true;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (predicate.Calc(ref element)) return true;
            }
            return false;
        }

        public bool Any(Func<TSource, bool> predicate)
        {
            if (predicate(start)) return true;
            var element = start;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (predicate(element)) return true;
            }
            return false;
        }

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            var element = start;
            if (!predicate.Calc(ref element)) return false;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (!predicate.Calc(ref element)) return false;
            }
            return true;
        }

        public bool All(Func<TSource, bool> predicate)
        {
            if (!predicate(start)) return false;
            var element = start;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (!predicate(element)) return false;
            }
            return true;
        }

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
        {
            if (length == 0) return;
            var element = start;
            func.Execute(ref seed, ref element);
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                func.Execute(ref seed, ref element);
            }
        }

        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
        {
            if (length == 0) goto RETURN;
            var element = start;
            func.Execute(ref seed, ref element);
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                func.Execute(ref seed, ref element);
            }
            RETURN:
            return resultFunc.Calc(ref seed);
        }

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (length == 0) throw new InvalidOperationException();
            if (length == 1) return start;
            TSource seed;
            var element = seed = start;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                seed = func(seed, element);
            }
            return seed;
        }

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (length == 0) goto RETURN;
            var element = start;
            seed = func(seed, element);
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                seed = func(seed, element);
            }
            RETURN:
            return seed;
        }

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
        {
            if (length == 0) goto RETURN;
            var element = start;
            seed = func(seed, element);
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                seed = func(seed, element);
            }
            RETURN:
            return resultFunc(seed);
        }

        public bool Contains(TSource value)
        {
            if (length == 0) return false;
            if (value.Equals(start)) return true;
            var element = start;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (value.Equals(element)) return true;
            }
            return false;
        }

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
        {
            if (length == 0) return false;
            if (comparer.Equals(value, start)) return true;
            var element = start;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (comparer.Equals(value, start)) return true;
            }
            return false;
        }

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
        {
            if (length == 0) return false;
            var element = start;
            if (comparer.Calc(ref value, ref element)) return true;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (comparer.Calc(ref value, ref element)) return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count() => (int) length;

        public int Count(Func<TSource, bool> predicate)
        {
            if (length == 0) return 0;
            var count = 0;
            if (predicate(start)) ++count;
            var element = start;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (predicate(element)) ++count;
            }
            return count;
        }

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            if (length == 0) return 0;
            var count = 0;
            var element = start;
            if (predicate.Calc(ref element)) ++count;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (predicate.Calc(ref element)) ++count;
            }
            return count;
        }

        public long LongCount() => length;

        public long LongCount(Func<TSource, bool> predicate)
        {
            if (length == 0) return 0;
            var count = 0L;
            if (predicate(start)) ++count;
            var element = start;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (predicate(element)) ++count;
            }
            return count;
        }

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            if (length == 0) return 0;
            var count = 0L;
            var element = start;
            if (predicate.Calc(ref element)) ++count;
            for (var i = 1; i < length; i++)
            {
                acts.Execute(ref element);
                if (predicate.Calc(ref element)) ++count;
            }
            return count;
        }

        public bool TryGetElementAt(int index, out TSource element)
        {
            element = start;
            if (index < 0 || index >= length)
                return false;
            for (var i = 1; i < index; i++)
                acts.Execute(ref element);
            return true;
        }

        public bool TryGetFirst(out TSource first)
        {
            first = start;
            return length != 0;
        }

        public bool TryGetLast(out TSource last)
        {
            last = start;
            for (var i = 1; i < length; i++)
                acts.Execute(ref last);
            return length != 0;
        }

        public bool TryGetSingle(out TSource value)
        {
            value = start;
            return length == 1;
        }

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.TryGetSingle<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
            => this.TryGetSingle<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource>(out value, predicate);

        public TSource[] ToArray()
        {
            if (length == 0) return Array.Empty<TSource>();
            var answer = new TSource[length];
            var element = answer[0] = start;
            for (var i = 1L; i < answer.LongLength; i++)
            {
                acts.Execute(ref element);
                answer[i] = element;
            }
            return answer;
        }

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            if (length == 0) return default;
            var answer = new NativeArray<TSource>((int) length, allocator, NativeArrayOptions.UninitializedMemory);
            var element = answer[0] = start;
            for (var i = 1; i < answer.Length; i++)
            {
                acts.Execute(ref element);
                answer[i] = element;
            }
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            => this.ToDictionary<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
            => this.ToDictionary<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TSource> ToHashSet()
            => this.ToHashSet<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource>();

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
            => this.ToHashSet<RangeRepeatEnumerable<TSource, TAction>, Enumerator, TSource>(comparer);


        public List<TSource> ToList()
        {
            if (length == 0) return new List<TSource>();
            var answer = new List<TSource>((int) length) {start};
            var element = start;
            for (var i = 1L; i < length; i++)
            {
                acts.Execute(ref element);
                answer.Add(element);
            }
            return answer;
        }
        #endregion
    }
}