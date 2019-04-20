using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public struct
        SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>
        : IRefEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>.Enumerator, TSource>, ILinq<TSource>
        where TPrevSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TPrevSource>
#endif
        where TEnumerator : struct, IRefEnumerator<TPrevSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TPrevSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TSourceEnumerator : struct, IRefEnumerator<TSource>
        where TSourceEnumerable : struct, IRefEnumerable<TSourceEnumerator, TSource>
        where TAction : struct, IRefAction<TPrevSource, TSourceEnumerable>
    {
        private TEnumerable enumerable;
        private readonly TAction acts;

        internal SelectManyEnumerable(in TEnumerable enumerable, TAction action)
        {
            this.enumerable = enumerable;
            this.acts = action;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private TSourceEnumerable resultEnumerable;
            private TSourceEnumerator resultEnumerator;
            private TAction action;
            private bool isNotFirst;

            internal Enumerator(in TEnumerator enumerator, TAction action)
            {
                this.enumerator = enumerator;
                this.resultEnumerable = default;
                this.resultEnumerator = default;
                this.action = action;
                this.isNotFirst = false;
            }


            public bool MoveNext()
            {
                if (isNotFirst)
                {
                    if (resultEnumerator.MoveNext())
                        return true;
                    resultEnumerator.Dispose();
                    while (enumerator.MoveNext())
                    {
                        action.Execute(ref enumerator.Current, ref resultEnumerable);
                        resultEnumerator = resultEnumerable.GetEnumerator();
                        if (resultEnumerator.MoveNext())
                            return true;
                        resultEnumerator.Dispose();
                    }
                    return false;
                }
                isNotFirst = true;
                while (enumerator.MoveNext())
                {
                    action.Execute(ref enumerator.Current, ref resultEnumerable);
                    resultEnumerator = resultEnumerable.GetEnumerator();
                    if (resultEnumerator.MoveNext())
                        return true;
                    resultEnumerator.Dispose();
                }
                return false;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref TSource Current => ref resultEnumerator.Current;

            TSource IEnumerator<TSource>.Current => Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                resultEnumerator.Dispose();
                enumerator.Dispose();
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), acts);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource
            >
            Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(this, value, allocator);

        public SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>
            AsRefEnumerable()
            => this;

        public DefaultIfEmptyEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource
            >
            DefaultIfEmpty(TSource value, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(this, value, allocator);

        public SelectEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource,
                TResult,
                TAction0
            >
            Select<TResult, TAction0>
            (TAction0 action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction0 : struct, IRefAction<TSource, TResult>
            => new SelectEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TResult, TAction0>(this, action, allocator);

        public SelectIndexEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource,
                TResult,
                TAction0
            >
            SelectIndex<TResult, TAction0>
            (TAction0 action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction0 : struct, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TResult, TAction0>(this, action, allocator);

        public SelectManyEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
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
            => new SelectManyEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public
            WhereIndexEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource,
                DefaultSkipIndex<TSource>
            >
            Skip(long count)
            => new WhereIndexEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, DefaultSkipIndex<TSource>>(this, new DefaultSkipIndex<TSource>(count));

        public
            WhereIndexEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource,
                DefaultSkipWhileIndex<TSource, TPredicate0>
            >
            SkipWhileIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, DefaultSkipWhileIndex<TSource, TPredicate0>>(this, new DefaultSkipWhileIndex<TSource, TPredicate0>(predicate));

        public
            WhereIndexEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource,
                DefaultTakeIndex<TSource>
            >
            Take(long count)
            => new WhereIndexEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, DefaultTakeIndex<TSource>>(this, new DefaultTakeIndex<TSource>(count));

        public
            WhereIndexEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource,
                DefaultTakeWhileIndex<TSource, TPredicate0>
            >
            TakeWhileIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, DefaultTakeWhileIndex<TSource, TPredicate0>>(this, new DefaultTakeWhileIndex<TSource, TPredicate0>(predicate));

        public WhereEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource,
                TPredicate
            >
            Where<TPredicate>
            (TPredicate predicate)
            where TPredicate : struct, IRefFunc<TSource, bool>
            => new WhereEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TPredicate>(this, predicate);

        public WhereIndexEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                TSource,
                TPredicate0
            >
            WhereIndex<TPredicate0>(TPredicate0 predicate)
            where TPredicate0 : struct, IWhereIndex<TSource>
            => new WhereIndexEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TPredicate0>(this, predicate);

        public ZipEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>
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
            => new ZipEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second);


        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeArray<TSource> second)
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(TSource[] second)
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, ArrayEnumerable<TSource>, ArrayEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());
#endif
        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>, DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEqualityComparer, TGetHashCodeFunc>
            (in DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc> second)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                RangeRepeatEnumerable<TSource, TAction0>,
                RangeRepeatEnumerable<TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TAction0>
            (in RangeRepeatEnumerable<TSource, TAction0> second)
            where TAction0 : struct, IRefAction<TSource>
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, RangeRepeatEnumerable<TSource, TAction0>, RangeRepeatEnumerable<TSource, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource0, TAction0>
            (in SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> second)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TAction0 : unmanaged, IRefAction<TPrevSource0, TSource>
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource0, TAction0>
            (in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> second)
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TAction0 : unmanaged, ISelectIndex<TPrevSource0, TSource>
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
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
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
                Enumerator,
                WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>,
                WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TPrevEnumerable0, TPrevEnumerator0, TPredicate>
            (in WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate> second)
            where TPrevEnumerable0 : struct, IRefEnumerable<TPrevEnumerator0, TSource>
            where TPrevEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate : struct, IRefFunc<TSource, bool>
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>, WhereEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
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
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>, WhereIndexEnumerable<TPrevEnumerable0, TPrevEnumerator0, TSource, TPredicate>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>,
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
            => new ConcatEnumerable<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
        #endregion

        #region Function        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => false;

        public bool Any()
            => this.Any<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>();

        public int Count()
            => this.Count<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>();

        public long LongCount()
            => this.LongCount<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>();

        public bool TryGetFirst(out TSource first)
            => this.TryGetFirst<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(out first);

        public bool TryGetLast(out TSource last)
            => this.TryGetLast<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(out last);

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.Any<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public bool Any(Func<TSource, bool> predicate)
            => this.Any<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(predicate);

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.All<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public bool All(Func<TSource, bool> predicate)
            => this.All<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(predicate);

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            => this.Aggregate<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TAccumulate, TFunc>(ref seed, func);

        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
            => this.Aggregate<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
            => this.Aggregate<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(func);

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            => this.Aggregate<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TAccumulate>(seed, func);

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => this.Aggregate<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TAccumulate, TResult>(seed, func, resultFunc);

        public bool Contains(TSource value)
            => this.Contains<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(value);

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
            => this.Contains<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(value, comparer);

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
            => this.Contains<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TComparer>(value, comparer);

        public int Count(Func<TSource, bool> predicate)
            => this.Count<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(predicate);

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.Count<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public long LongCount(Func<TSource, bool> predicate)
            => this.LongCount<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(predicate);

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.LongCount<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TPredicate>(predicate);

        public bool TryGetElementAt(long index, out TSource element)
            => this.TryGetElementAt<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(index, out element);

        public NativeEnumerable<TSource> ToNativeEnumerable(Allocator allocator)
            => this.ToNativeEnumerable<
                SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>
                ,Enumerator, TSource>(allocator);

        public bool TryGetSingle(out TSource value)
            => this.TryGetSingle<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(out value);

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.TryGetSingle<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
            => this.TryGetSingle<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(out value, predicate);

        public TSource[] ToArray()
            => this.ToArray<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>();

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
            => this.ToNativeArray<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(allocator);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            => this.ToDictionary<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TKey, TElement>(keySelector, elementSelector);

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
            => this.ToDictionary<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        public HashSet<TSource> ToHashSet()
            => this.ToHashSet<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>();

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
            => this.ToHashSet<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>(comparer);

        public List<TSource> ToList()
            => this.ToList<SelectManyEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource, TSourceEnumerable, TSourceEnumerator, TAction>, Enumerator, TSource>();
        #endregion
    }
}