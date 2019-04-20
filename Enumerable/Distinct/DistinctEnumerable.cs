using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct
        DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
        : IRefEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
        where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
        where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
        where TEnumerator : struct, IRefEnumerator<TSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
    {
        private TEnumerable enumerable;
        private readonly TEqualityComparer comparer;
        private readonly TGetHashCodeFunc getHashCodeFunc;
        private readonly Allocator alloc;

        internal DistinctEnumerable(in TEnumerable enumerable, TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.comparer = comparer;
            this.getHashCodeFunc = getHashCodeFunc;
            this.alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private TSource* ptr;
            private int* codes;
            private long capacity;
            private long count;
            private readonly Allocator alloc;
            private TEqualityComparer comparer;
            private TGetHashCodeFunc getHashCodeFunc;
            private long currentIndex;

            public Enumerator(ref TEnumerable enumerable, TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator)
            {
                if (enumerable.CanFastCount())
                {
                    if ((this.capacity = enumerable.LongCount()) == 0)
                    {
                        this = default;
                        return;
                    }
                }
                else
                {
                    this.capacity = 16;
                }
                this.count = 0;
                this.enumerator = enumerable.GetEnumerator();
                this.alloc = allocator;
                this.ptr = UnsafeUtilityEx.Malloc<TSource>(this.capacity, this.alloc);
                this.codes = UnsafeUtilityEx.Malloc<int>(this.capacity, this.alloc);
                this.comparer = comparer;
                this.getHashCodeFunc = getHashCodeFunc;
                this.currentIndex = 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (ptr == null) return false;
                while (enumerator.MoveNext())
                {
                    ref var current = ref enumerator.Current;
                    var hash = getHashCodeFunc.Calc(ref current);
                    if (count == 0)
                        return InitialInsert(ref current, hash);
                    if (TryInsert(ref current, hash))
                        return true;
                }
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool TryInsert(ref TSource current, int hash)
            {
                if (hash < codes[0])
                {
                    currentIndex = 0;
                    Insert(ref current, hash);
                    return true;
                }
                if (hash > codes[count - 1])
                {
                    currentIndex = count;
                    Insert(ref current, hash);
                    return true;
                }
                var minInclusive = 0L;
                var maxInclusive = count - 1;
                while (minInclusive < maxInclusive)
                {
                    currentIndex = (minInclusive + maxInclusive) >> 1;
                    if (hash == codes[currentIndex])
                        return TryInsertAround(ref current, hash, minInclusive, maxInclusive);
                    if (hash > codes[currentIndex])
                        minInclusive = currentIndex + 1;
                    else if (hash < codes[currentIndex])
                        maxInclusive = currentIndex - 1;
                }
                if (codes[minInclusive] == hash || !comparer.Calc(ref current, ref ptr[minInclusive])) return false;
                currentIndex = minInclusive;
                Insert(ref current, hash);
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool TryInsertAround(ref TSource current, int hash, long minInclusive, long maxInclusive)
            {
                if (hash == codes[currentIndex] && comparer.Calc(ref current, ref ptr[currentIndex]))
                    return false;
                for (var i = currentIndex; --i >= minInclusive;)
                {
                    if (hash != codes[i]) break;
                    if (comparer.Calc(ref current, ref ptr[i])) return false;
                }
                for (; ++currentIndex <= maxInclusive;)
                {
                    if (hash != codes[currentIndex]) break;
                    if (comparer.Calc(ref current, ref ptr[currentIndex])) return false;
                }
                Insert(ref current, hash);
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool InitialInsert(ref TSource current, int hash)
            {
                currentIndex = 0;
                Insert(ref current, hash);
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Insert(ref TSource current, int hash)
            {
                if (capacity == count)
                {
                    InsertWithReallocation(ref current, hash, currentIndex);
                    return;
                }
                if (currentIndex < count)
                {
                    UnsafeUtilityEx.MemMove(ptr + currentIndex + 1, ptr + currentIndex, count - currentIndex);
                    UnsafeUtilityEx.MemMove(codes + currentIndex + 1, codes + currentIndex, count - currentIndex);
                }
                ptr[currentIndex] = current;
                codes[currentIndex] = hash;
                count++;
            }

            private void InsertWithReallocation(ref TSource current, int hash, long index)
            {
                var tmpPtr = UnsafeUtilityEx.Malloc<TSource>(capacity << 1, alloc);
                var tmpCodes = UnsafeUtilityEx.Malloc<int>(capacity << 1, alloc);
                capacity <<= 1;
                tmpPtr[count] = current;
                tmpCodes[count] = hash;
                if (index == 0)
                {
                    UnsafeUtilityEx.MemCpy(tmpPtr + 1, ptr, count);
                    UnsafeUtilityEx.MemCpy(tmpCodes + 1, codes, count);
                }
                else if (index == count)
                {
                    UnsafeUtilityEx.MemCpy(tmpPtr, ptr, count);
                    UnsafeUtilityEx.MemCpy(tmpCodes, codes, count);
                }
                else
                {
                    UnsafeUtilityEx.MemCpy(tmpPtr, ptr, index);
                    UnsafeUtilityEx.MemCpy(tmpPtr + index + 1, ptr + index, count - index);
                    UnsafeUtilityEx.MemCpy(tmpCodes, codes, index);
                    UnsafeUtilityEx.MemCpy(tmpCodes + index + 1, codes + index, count - index);
                }
                ++count;
                UnsafeUtility.Free(ptr, alloc);
                UnsafeUtility.Free(codes, alloc);
                ptr = tmpPtr;
                codes = tmpCodes;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref TSource Current => ref ptr[currentIndex];

            TSource IEnumerator<TSource>.Current => Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (ptr != null)
                    UnsafeUtility.Free(ptr, alloc);
                if (codes != null)
                    UnsafeUtility.Free(codes, alloc);
                this = default;
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(ref enumerable, comparer, getHashCodeFunc, alloc);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                TSource
            >
            Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource>(this, value, allocator);

        public AppendPointerEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                TSource
            >
            Append(TSource* value)
            => new AppendPointerEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource>(this, value);

        public DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
            AsRefEnumerable() => this;

        public DefaultIfEmptyEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                TSource
            >
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
            Distinct() => this;

        public SelectEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                TSource,
                TResult,
                TAction
            >
            Select<TResult, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IRefAction<TSource, TResult>
            => new SelectEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectIndexEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                TSource,
                TResult,
                TAction
            >
            SelectIndex<TResult, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectManyEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
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
            => new SelectManyEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public WhereEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                TSource,
                TPredicate
            >
            Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource, TPredicate>(this, predicate);

        public ZipEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
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
            => new ZipEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                TEnumerable0, TEnumerator0,
                TSource>
            Concat<TEnumerable0, TEnumerator0>(in TEnumerable0 second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TEnumerable0, TEnumerator0, TSource>(this, second);

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(this, second);

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeArray<TSource> second)
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second.AsRefEnumerable());

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(NativeEnumerable<TSource> second)
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(this, second);

#if UNSAFE_ARRAY_ENUMERABLE
        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(TSource[] second)
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource>(this, second.AsRefEnumerable());

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                ArrayEnumerable<TSource>,
                ArrayEnumerable<TSource>.Enumerator,
                TSource>(this, second);
#endif
        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                RangeRepeatEnumerable<TSource, TAction>,
                RangeRepeatEnumerable<TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TAction>(in RangeRepeatEnumerable<TSource, TAction> second)
            where TAction : struct, IRefAction<TSource>
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, RangeRepeatEnumerable<TSource, TAction>, RangeRepeatEnumerable<TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource, TAction>
            (in SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction> second)
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource>
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
                Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrevSource, TAction>
            (in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction> second)
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource>
            where TAction : unmanaged, ISelectIndex<TPrevSource, TSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>, SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
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
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
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
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>,
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
            => new ConcatEnumerable<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        public bool CanFastCount() => false;

        public bool Any()
        {
            var enumerator = GetEnumerator();
            if (enumerator.MoveNext())
            {
                enumerator.Dispose();
                return true;
            }
            enumerator.Dispose();
            return false;
        }

        public int Count() => (int) LongCount();

        public long LongCount()
        {
            var enumerator = GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        public bool TryGetFirst(out TSource first)
            => this.TryGetFirst<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource>(out first);

        public bool TryGetLast(out TSource last)
            => this.TryGetLast<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource>(out last);

        public bool TryGetElementAt(long index, out TSource element)
            => this.TryGetElementAt<DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>, Enumerator, TSource>(index, out element);
        #endregion
    }
}