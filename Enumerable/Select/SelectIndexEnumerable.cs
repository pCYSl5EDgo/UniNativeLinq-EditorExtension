using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct
        SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>
        : IRefEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>.Enumerator, TSource>, ILinq<TSource>
        where TPrevSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TPrevSource>
#endif
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TAction : struct, ISelectIndex<TPrevSource, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TPrevSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TPrevSource>
    {
        private TPrevEnumerable enumerable;
        private TAction acts;
        private readonly Allocator alloc;

        internal SelectIndexEnumerable(in TPrevEnumerable enumerable, TAction acts, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.acts = acts;
            this.alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TPrevEnumerator enumerator;
            private readonly TSource* current;
            private readonly Allocator allocator;
            private TAction action;
            private int index;

            internal Enumerator(in TPrevEnumerator enumerator, TAction action, Allocator allocator)
            {
                this.enumerator = enumerator;
                this.current = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
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
            public ref TSource Current => ref *current;
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;
            public void Dispose() => UnsafeUtility.Free(this.current, this.allocator);
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), acts, alloc);
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>
            Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(this, value, allocator);

        public AppendPointerEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>
            Append(TSource* value)
            => new AppendPointerEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(this, value);

        public SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction> AsRefEnumerable() => this;

        public DefaultIfEmptyEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);

        public DistinctEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
            Distinct<TEqualityComparer, TGetHashCodeFunc>(TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new DistinctEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectIndexEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TNextResult, TNextAction>
            SelectIndex<TNextResult, TNextAction>(TNextAction action, Allocator allocator = Allocator.Temp)
            where TNextResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextResult>
#endif
            where TNextAction : unmanaged, ISelectIndex<TSource, TNextResult>
            => new SelectIndexEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TNextResult, TNextAction>(this, action, allocator);

        public SelectEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TNextResult, TNextAction>
            Select<TNextResult, TNextAction>(TNextAction action, Allocator allocator = Allocator.Temp)
            where TNextResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TNextResult>
#endif
            where TNextAction : unmanaged, IRefAction<TSource, TNextResult>
            => new SelectEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TNextResult, TNextAction>(this, action, allocator);

        public SelectManyEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
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
            => new SelectManyEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public WhereEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TPredicate>
            Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TPredicate>(this, predicate);

        public ZipEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>
                , Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
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
            => new ZipEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
#endif
        #endregion

        #region Concat
        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                TEnumerable1,
                TEnumerator1,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in TEnumerable1 second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    TEnumerable1, TEnumerator1,
                    TSource
                >
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2>
            (in ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>,
                    ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    NativeEnumerable<TSource>,
                    NativeEnumerable<TSource>.Enumerator,
                    TSource>
                (this, second.AsRefEnumerable());

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    NativeEnumerable<TSource>,
                    NativeEnumerable<TSource>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>,
                    AppendEnumerator<TEnumerator1, TSource>,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                    AppendEnumerator<TEnumerator1, TSource>,
                    TSource>
                (this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in TSource[] second)
            => new ConcatEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>,
                    DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>,
                DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TEqualityComparer, TGetHashCodeFunc>
            (in DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new ConcatEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                RangeRepeatEnumerable<TSource, TAction1>,
                RangeRepeatEnumerable<TSource, TAction1>.Enumerator,
                TSource
            >
            Concat<TAction1>
            (in RangeRepeatEnumerable<TSource, TAction1> second)
            where TAction1 : struct, IRefAction<TSource>
            => new ConcatEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, RangeRepeatEnumerable<TSource, TAction1>, RangeRepeatEnumerable<TSource, TAction1>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TPrevSource1, TAction1>
            (in SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1> second)
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource1>
            where TAction1 : unmanaged, IRefAction<TPrevSource1, TSource>
            where TPrevSource1 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource1>
#endif
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>,
                    SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>,
                SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TPrevSource1, TAction1>
            (in SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1> second)
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource1>
            where TAction1 : unmanaged, ISelectIndex<TPrevSource1, TSource>
            where TPrevSource1 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource1>
#endif
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>,
                    SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
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
            => new ConcatEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                Enumerator,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TPredicate>(in WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate> second)
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<
                    SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
                    Enumerator,
                    WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>,
                    WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>.Enumerator,
                    TSource>
                (this, second);
        
        public ConcatEnumerable<
                SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>,
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
            => new ConcatEnumerable<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Any() => enumerable.Any();

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.Any<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public bool Any(Func<TSource, bool> predicate)
            => this.Any<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.All<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public bool All(Func<TSource, bool> predicate)
            => this.All<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(predicate);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TAccumulate, TFunc>(ref seed, func);

        public TNextResult Aggregate<TAccumulate, TNextResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TNextResult>
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TAccumulate, TNextResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TAccumulate>(seed, func);

        public TNextResult Aggregate<TAccumulate, TNextResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TNextResult> resultFunc)
            => this.Aggregate<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TAccumulate, TNextResult>(seed, func, resultFunc);

        public bool Contains(TSource value)
            => this.Contains<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => this.Contains<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => this.Contains<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TComparer>(value, comparer);

        public int Count() => enumerable.Count();

        public int Count(Func<TSource, bool> predicate)
            => this.Count<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.Count<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public long LongCount() => enumerable.LongCount();

        public long LongCount(Func<TSource, bool> predicate)
            => Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => Count(predicate);

        public bool TryGetElementAt(long index, out TSource element)
            => this.TryGetElementAt<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(index, out element);

        public bool TryGetFirst(out TSource first)
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

        public bool TryGetLast(out TSource last)
            => this.TryGetLast<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(out last);

        public bool TryGetSingle(out TSource value)
            => this.TryGetSingle<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(out value);

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.TryGetSingle<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
            => this.TryGetSingle<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(out value, predicate);

        public TSource[] ToArray()
            => this.ToArray<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>();

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
            => this.ToNativeArray<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(allocator);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            => this.ToDictionary<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
            => this.ToDictionary<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TSource> ToHashSet()
            => this.ToHashSet<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>();

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
            => this.ToHashSet<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>(comparer);

        public List<TSource> ToList()
            => this.ToList<SelectIndexEnumerable<TPrevEnumerable, TPrevEnumerator, TPrevSource, TSource, TAction>, Enumerator, TSource>();
        #endregion
    }
}