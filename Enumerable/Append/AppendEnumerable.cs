using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct
        AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>
        : IRefEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>.Enumerator, TSource>, ILinq<TSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TSource : unmanaged
    {
        private TPrevEnumerable enumerable;
        private TSource append;
        private readonly Allocator alloc;

        public AppendEnumerable(in TPrevEnumerable enumerable, in TSource append, Allocator alloc)
        {
            this.enumerable = enumerable;
            this.append = append;
            this.alloc = alloc;
        }

        public struct Enumerator
            : IRefEnumerator<TSource>
        {
            private TPrevEnumerator enumerator;
            private readonly TSource* element;
            private readonly Allocator allocator;
            private bool isCurrentEnumerator;
            private bool hasNotAppendRead;

            public Enumerator(in TPrevEnumerator enumerator, in TSource element, Allocator allocator)
            {
                this.allocator = allocator;
                this.element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                *this.element = element;
                this.enumerator = enumerator;
                isCurrentEnumerator = true;
                hasNotAppendRead = true;
            }

            public ref TSource Current
            {
                get
                {
                    if (isCurrentEnumerator)
                        return ref enumerator.Current;
                    return ref *element;
                }
            }

            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                if (element != null && allocator != Allocator.None)
                    UnsafeUtility.Free(element, allocator);
                this = default;
            }

            public bool MoveNext()
            {
                if (isCurrentEnumerator)
                {
                    if (!enumerator.MoveNext())
                        isCurrentEnumerator = false;
                    return true;
                }
                if (!hasNotAppendRead) return false;
                hasNotAppendRead = false;
                return true;
            }

            public void Reset() => throw new InvalidOperationException();
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), append, alloc);

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> AsRefEnumerable() => this;

        public AppendEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource> Append(in TSource item, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource>(this, item, allocator);

        public DefaultIfEmptyEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource>
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                TSource,
                DefaultEqualityComparer<TSource>,
                DefaultGetHashCodeFunc<TSource>
            >
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);

        public DistinctEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                TSource,
                TEqualityComparer,
                TGetHashCodeFunc
            >
            Distinct<TEqualityComparer, TGetHashCodeFunc>(TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new DistinctEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TResult, TAction> Select<TResult, TAction>(TAction action, Allocator allocator)
            where TResult : unmanaged
            where TAction : unmanaged, IRefAction<TSource, TResult>
            => new SelectEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectIndexEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TResult, TAction> SelectIndex<TResult, TAction>(TAction action, Allocator allocator)
            where TResult : unmanaged
            where TAction : unmanaged, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectManyEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            where TResultEnumerator : struct, IRefEnumerator<TResult>
            where TResultEnumerable : struct, IRefEnumerable<TResultEnumerator, TResult>
            where TResultAction : struct, IRefAction<TSource, TResultEnumerable>
            => new SelectManyEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public
            WhereIndexEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                TSource,
                DefaultSkipIndex<TSource>
            >
            Skip(long count)
            => new WhereIndexEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, DefaultSkipIndex<TSource>>(this, new DefaultSkipIndex<TSource>(count));

        public
            WhereIndexEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                TSource,
                DefaultSkipWhileIndex<TSource, TPredicate0>
            >
            SkipWhileIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, DefaultSkipWhileIndex<TSource, TPredicate0>>(this, new DefaultSkipWhileIndex<TSource, TPredicate0>(predicate));

        public
            WhereIndexEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                TSource,
                DefaultTakeIndex<TSource>
            >
            Take(long count)
            => new WhereIndexEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, DefaultTakeIndex<TSource>>(this, new DefaultTakeIndex<TSource>(count));

        public
            WhereIndexEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                TSource,
                DefaultTakeWhileIndex<TSource, TPredicate0>
            >
            TakeWhileIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, DefaultTakeWhileIndex<TSource, TPredicate0>>(this, new DefaultTakeWhileIndex<TSource, TPredicate0>(predicate));

        public WhereEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : struct, IRefFunc<TSource, bool>
            => new WhereEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TPredicate>(this, predicate);

        public WhereIndexEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                TSource,
                TPredicate0
            >
            WhereIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TPredicate0>(this, predicate);

        public ZipEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
            Zip<TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
            (in TEnumerable0 second, TAction0 action, TSource firstDefaultValue = default, TSource0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource0>
            where TEnumerator0 : struct, IRefEnumerator<TSource0>
            where TSource0 : unmanaged
            where TAction0 : unmanaged, IRefAction<TSource, TSource0, TResult0>
            where TResult0 : unmanaged
            => new ZipEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                TEnumerable, TEnumerator,
                TSource
            >
            Concat<TEnumerable, TEnumerator>(in TEnumerable second)
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TEnumerable, TEnumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in AppendEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, AppendEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(this, second);


        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<
                    AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                    Enumerator,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(TSource[] second)
            => new ConcatEnumerable<
                    AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                    Enumerator,
                    ArrayEnumerable<TSource>,
                    ArrayEnumerable<TSource>.Enumerator,
                    TSource
                >
                (this, second.AsRefEnumerable());


        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1>
            (in DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>, DefaultIfEmptyEnumerable<TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>,
                DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator,
                TSource
            >
            Concat<TEnumerable1, TEnumerator1, TEqualityComparer, TGetHashCodeFunc>
            (in DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc> second)
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable1, TEnumerator1, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                RangeRepeatEnumerable<TSource, TAction>,
                RangeRepeatEnumerable<TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TAction>
            (in RangeRepeatEnumerable<TSource, TAction> second)
            where TAction : struct, IRefAction<TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, RangeRepeatEnumerable<TSource, TAction>, RangeRepeatEnumerable<TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            where TAction : unmanaged, ISelectIndex<TPrevSource, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectIndexEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            where TResultEnumerator0 : struct, IRefEnumerator<TSource>
            where TResultEnumerable0 : struct, IRefEnumerable<TResultEnumerator0, TSource>
            where TAction0 : struct, IRefAction<TPrevSource0, TResultEnumerable0>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
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
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>,
                WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TPrevEnumerable0, TPrevEnumerator0, TPredicate>
            (in WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate> second)
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, TSource>
            where TPrevEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate : struct, IWhereIndex<TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator,
                ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>,
                ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TAction0>
            (in ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0> second)
            where TSource0 : unmanaged
            where TEnumerator0 : struct, IRefEnumerator<TSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource0>
            where TSource1 : unmanaged
            where TEnumerator1 : struct, IRefEnumerator<TSource1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource1>
            where TAction0 : struct, IRefAction<TSource0, TSource1, TSource>
            => new ConcatEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => enumerable.CanFastCount();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource>(func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TAccumulate>(seed, func);

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TAccumulate, TResult>(seed, func, resultFunc);

        public bool Contains(TSource value)
            => append.Equals(value) || enumerable.Contains<TPrevEnumerable, TPrevEnumerator, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => comparer.Equals(append, value) || enumerable.Contains<TPrevEnumerable, TPrevEnumerator, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => comparer.Calc(ref append, ref value) || enumerable.Contains<TPrevEnumerable, TPrevEnumerator, TSource, TComparer>(value, comparer);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TAccumulate, TFunc>(ref seed, func);

        TResult ILinq<TSource>.Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public bool Contains<TEqualityComparer>(in TSource value, TEqualityComparer comparer)
            where TEqualityComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => this.Contains<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TEqualityComparer>(value, comparer);

        public int Count()
            => enumerable.Count() + 1;

        public int Count(Func<TSource, bool> predicate)
            => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource>(predicate) + 1;

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource, TPredicate>(predicate);

        public long LongCount()
            => enumerable.LongCount() + 1;

        public long LongCount(Func<TSource, bool> predicate)
            => Count(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => Count(predicate);

        public bool TryGetElementAt(long index, out TSource element)
            => this.TryGetElementAt<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource>(index, out element);

        public NativeEnumerable<TSource> ToNativeEnumerable(Allocator allocator)
            => this.ToNativeEnumerable<
                AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>,
                Enumerator, TSource>(allocator);

        public bool TryGetFirst(out TSource first)
        {
            if (enumerable.TryGetFirst(out first))
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
            => this.ToDictionary<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, Enumerator, TSource, TKey, TElement>(keySelector, elementSelector);

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