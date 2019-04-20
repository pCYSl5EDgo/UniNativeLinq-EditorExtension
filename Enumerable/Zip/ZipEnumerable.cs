using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct
        ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>
        : IRefEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>.Enumerator, TSource>, ILinq<TSource>
        where TFirstSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TFirstSource>
#endif
        where TSecondSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSecondSource>
#endif
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TFirstEnumerator : struct, IRefEnumerator<TFirstSource>
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TFirstSource>
        where TSecondEnumerator : struct, IRefEnumerator<TSecondSource>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSecondSource>
        where TAction : struct, IRefAction<TFirstSource, TSecondSource, TSource>
    {
        private TFirstEnumerable firstCollection;
        private TSecondEnumerable secondCollection;
        private readonly Allocator alloc;
        private readonly TAction acts;
        private readonly TFirstSource firstDefault;
        private readonly TSecondSource secondDefault;

        public ZipEnumerable(in TFirstEnumerable firstCollection, in TSecondEnumerable secondCollection, TAction acts, in TFirstSource firstDefault, in TSecondSource secondDefault, Allocator alloc)
        {
            this.firstCollection = firstCollection;
            this.secondCollection = secondCollection;
            this.acts = acts;
            this.firstDefault = firstDefault;
            this.secondDefault = secondDefault;
            this.alloc = alloc;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TFirstEnumerator firstEnumerator;
            private TSecondEnumerator secondEnumerator;
            private readonly TSource* current;
            private readonly Allocator allocator;
            private TAction action;
            private TFirstSource firstDefaultValue;
            private TSecondSource secondDefaultValue;
            private bool isFirstAlive, isSecondAlive;

            internal Enumerator(in TFirstEnumerator firstEnumerator, in TSecondEnumerator secondEnumerator, TAction action, TFirstSource firstDefaultValue, TSecondSource secondDefaultValue, Allocator allocator)
            {
                this.firstEnumerator = firstEnumerator;
                this.secondEnumerator = secondEnumerator;
                this.current = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                this.allocator = allocator;
                this.action = action;
                this.firstDefaultValue = firstDefaultValue;
                this.secondDefaultValue = secondDefaultValue;
                this.isFirstAlive = this.isSecondAlive = true;
            }

            public bool MoveNext()
            {
                if (isFirstAlive && !firstEnumerator.MoveNext())
                    isFirstAlive = false;
                if (isSecondAlive && !secondEnumerator.MoveNext())
                    isSecondAlive = false;
                if (isFirstAlive)
                {
                    if (isSecondAlive)
                        action.Execute(ref firstEnumerator.Current, ref secondEnumerator.Current, ref *current);
                    else
                        action.Execute(ref firstEnumerator.Current, ref secondDefaultValue, ref *current);
                }
                else
                {
                    if (isSecondAlive)
                        action.Execute(ref firstDefaultValue, ref secondEnumerator.Current, ref *current);
                    else
                        return false;
                }
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref TSource Current => ref *current;

            TSource IEnumerator<TSource>.Current => Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (current != null)
                    UnsafeUtility.Free(current, allocator);
                this = default;
            }
        }


        public Enumerator GetEnumerator() => new Enumerator(firstCollection.GetEnumerator(), secondCollection.GetEnumerator(), acts, firstDefault, secondDefault, alloc);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TSource
            >
            Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(this, value, allocator);

        public AppendPointerEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TSource
            >
            Append(TSource* value)
            => new AppendPointerEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(this, value);

        public ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>
            AsRefEnumerable() => this;

        public DefaultIfEmptyEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TSource
            >
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TSource,
                DefaultEqualityComparer<TSource>,
                DefaultGetHashCodeFunc<TSource>
            >
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);

        public DistinctEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TSource,
                TEqualityComparer,
                TGetHashCodeFunc
            >
            Distinct<TEqualityComparer, TGetHashCodeFunc>
            (TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new DistinctEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TSource,
                TResult,
                TFunc
            >
            Select<TResult, TFunc>(TFunc func, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TFunc : struct, IRefAction<TSource, TResult>
            => new SelectEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TResult, TFunc>(this, func, allocator);

        public SelectIndexEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TSource,
                TResult,
                TFunc
            >
            SelectIndex<TResult, TFunc>(TFunc func, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TFunc : struct, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TResult, TFunc>(this, func, allocator);

        public WhereEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TSource,
                TPredicate
            >
            Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TPredicate>(this, predicate);

        public ZipEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TSource,
                TEnumerable0,
                TEnumerator0,
                TSource0,
                TResult,
                TAction0
            >
            Zip<TEnumerable0, TEnumerator0, TSource0, TResult, TAction0>
            (in TEnumerable0 second, TAction0 action, TSource firstDefaultValue = default, TSource0 secondDefaultValue = default, Allocator allocator = Allocator.Temp)
            where TSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource0>
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction0 : struct, IRefAction<TSource, TSource0, TResult>
            => new ZipEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                TEnumerable0,
                TEnumerator0,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>(in TEnumerable0 second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TEnumerable0, TEnumerator0, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(TSource[] second)
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());
#endif

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
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
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer0, TGetHashCodeFunc0>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEqualityComparer0, TGetHashCodeFunc0>
            (in DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer0, TGetHashCodeFunc0> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEqualityComparer0 : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc0 : struct, IRefFunc<TSource, int>
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer0, TGetHashCodeFunc0>, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer0, TGetHashCodeFunc0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource0, TAction0>
            (in SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> second)
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TAction0 : unmanaged, IRefAction<TPrevSource0, TSource>
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource0, TAction0>
            (in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> second)
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TAction0 : unmanaged, ISelectIndex<TPrevSource0, TSource>
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
                Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPredicate0>
            (in WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TPredicate0 : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>,
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
            => new ConcatEnumerable<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        public bool CanFastCount() => firstCollection.CanFastCount() && secondCollection.CanFastCount();

        public bool Any() => firstCollection.Any() || secondCollection.Any();

        public int Count()
        {
            var firstCount = firstCollection.Count();
            var secondCount = secondCollection.Count();
            return firstCount > secondCount ? firstCount : secondCount;
        }

        public long LongCount()
        {
            var firstCount = firstCollection.LongCount();
            var secondCount = secondCollection.LongCount();
            return firstCount > secondCount ? firstCount : secondCount;
        }

        public bool TryGetFirst(out TSource first)
            => this.TryGetFirst<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(out first);

        public bool TryGetLast(out TSource last)
            => this.TryGetLast<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(out last);

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.Any<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public bool Any(Func<TSource, bool> predicate)
            => this.Any<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.All<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public bool All(Func<TSource, bool> predicate)
            => this.All<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(predicate);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            => this.Aggregate<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TAccumulate, TFunc>(ref seed, func);

        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
            => this.Aggregate<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
            => this.Aggregate<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => this.Aggregate<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TAccumulate>(seed, func);

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => this.Aggregate<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TAccumulate, TResult>(seed, func, resultFunc);

        public bool Contains(TSource value)
            => this.Contains<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => this.Contains<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => this.Contains<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TComparer>(value, comparer);

        public int Count(Func<TSource, bool> predicate)
            => this.Count<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.Count<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public long LongCount(Func<TSource, bool> predicate)
            => this.LongCount<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.LongCount<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public bool TryGetElementAt(long index, out TSource element)
            => this.TryGetElementAt<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(index, out element);

        public bool TryGetSingle(out TSource value)
            => this.TryGetSingle<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(out value);

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.TryGetSingle<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
            => this.TryGetSingle<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(out value, predicate);

        public TSource[] ToArray()
            => this.ToArray<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>();

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
            => this.ToNativeArray<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(allocator);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            => this.ToDictionary<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
            => this.ToDictionary<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TSource> ToHashSet()
            => this.ToHashSet<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>();

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
            => this.ToHashSet<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>(comparer);

        public List<TSource> ToList()
            => this.ToList<ZipEnumerable<TFirstEnumerable, TFirstEnumerator, TFirstSource, TSecondEnumerable, TSecondEnumerator, TSecondSource, TSource, TAction>, Enumerator, TSource>();
        #endregion
    }
}