using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

#if UNSAFE_ARRAY_ENUMERABLE
namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct ArrayEnumerable<TSource>
        : IRefEnumerable<ArrayEnumerable<TSource>.Enumerator, TSource>, ILinq<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource>
#endif
    {
        private readonly TSource[] array;
        private readonly long offset;
        internal readonly long Length;

        internal TSource* GetPointer() => (TSource*) Unsafe.AsPointer(ref array[offset]);
        private TSource* GetPinPointer(out ulong gcHandle) => (TSource*) UnsafeUtility.PinGCArrayAndGetDataAddress(array, out gcHandle) + offset;


        public ArrayEnumerable(TSource[] array)
        {
            this.array = array ?? throw new ArgumentNullException();
            this.Length = array.LongLength;
            this.offset = 0;
        }

        public ArrayEnumerable(ArraySegment<TSource> segment)
        {
            this.array = segment.Array ?? throw new ArgumentNullException();
            this.Length = segment.Count;
            this.offset = segment.Offset;
        }

        public ArrayEnumerable(TSource[] array, long offset, long count)
        {
            this.array = array ?? throw new ArgumentNullException();
            this.Length = count;
            this.offset = offset;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private readonly TSource* ptr;
            private readonly long length;
            private readonly ulong gcHandle;
            private long index;

            internal Enumerator(TSource* ptr, long length, ulong gcHandle)
            {
                this.ptr = ptr;
                this.length = length;
                this.gcHandle = gcHandle;
                this.index = -1;
            }

            public bool MoveNext() => ++index < length;
            public void Reset() => index = -1;
            public ref TSource Current => ref ptr[index];
            TSource IEnumerator<TSource>.Current => Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (ptr != null)
                    UnsafeUtility.ReleaseGCObject(gcHandle);
            }
        }

        public Enumerator GetEnumerator()
        {
            if (array is null || array.Length == 0)
                return default;
            return new Enumerator(GetPinPointer(out var gcHandle), Length, gcHandle);
        }

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource>
            Append(TSource value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource>(this, value, allocator);

        public AppendPointerEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource>
            Append(TSource* value)
            => new AppendPointerEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource>(this, value);

        public ArrayEnumerable<TSource> AsRefEnumerable() => this;

        public DefaultIfEmptyEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource>
            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource>(this, defaultValue, allocator);

        public DistinctEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);

        public DistinctEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
            Distinct<TEqualityComparer, TGetHashCodeFunc>
            (TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
            where TGetHashCodeFunc : struct, IRefFunc<TSource, int>
            => new DistinctEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TResult, TAction>
            Select<TResult, TAction>
            (TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IRefAction<TSource, TResult>
            => new SelectEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectIndexEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TResult, TAction>
            SelectIndex<TResult, TAction>
            (TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<TSource, TResult>
            => new SelectIndexEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TResult, TAction>(this, action, allocator);

        public SelectManyEnumerable<
                ArrayEnumerable<TSource>,
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
            => new SelectManyEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TResult, TResultEnumerable, TResultEnumerator, TResultAction>(this, action);

        public WhereEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TPredicate>
            Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new WhereEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TPredicate>(this, predicate);

        public ZipEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>
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
            => new ZipEnumerable<ArrayEnumerable<TSource>, Enumerator, TSource, TEnumerable0, TEnumerator0, TSource0, TResult0, TAction0>(this, second, action, firstDefaultValue, secondDefaultValue, allocator);
        #endregion

        #region Concat
        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                TEnumerable0, TEnumerator0,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in TEnumerable0 second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    TEnumerable0, TEnumerator0,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeEnumerable<TSource> second)
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    NativeEnumerable<TSource>,
                    NativeEnumerable<TSource>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat(in NativeArray<TSource> second)
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    NativeEnumerable<TSource>,
                    NativeEnumerable<TSource>.Enumerator,
                    TSource>
                (this, second.AsRefEnumerable());

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                    AppendEnumerator<TEnumerator0, TSource>,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                    AppendEnumerator<TEnumerator0, TSource>,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                ArrayEnumerable<TSource>, Enumerator,
                TSource
            >
            Concat(in ArrayEnumerable<TSource> second)
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    ArrayEnumerable<TSource>, Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
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
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0>
            (in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, TSource>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
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
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable0, TEnumerator0, TSource, TEqualityComparer, TGetHashCodeFunc>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                RangeRepeatEnumerable<TSource, TAction>,
                RangeRepeatEnumerable<TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TAction>
            (in RangeRepeatEnumerable<TSource, TAction> second)
            where TAction : struct, IRefAction<TSource>
            => new ConcatEnumerable<
                    ArrayEnumerable<TSource>, Enumerator,
                    RangeRepeatEnumerable<TSource, TAction>,
                    RangeRepeatEnumerable<TSource, TAction>.Enumerator,
                    TSource
                >
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrev, TAction>
            (in SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction> second)
            where TPrev : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrev>
#endif
            where TAction : unmanaged, IRefAction<TPrev, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TPrev>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev>
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction>,
                    SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPrev, TAction>
            (in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction> second)
            where TPrev : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrev>
#endif
            where TAction : unmanaged, ISelectIndex<TPrev, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TPrev>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev>
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction>,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, TSource, TAction>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
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
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>, SelectManyEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TResultEnumerable0, TResultEnumerator0, TAction0>.Enumerator, TSource>(this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>, Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TPredicate>
            (in WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate> second)
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator,
                    TSource>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<TSource>,
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
            => new ConcatEnumerable<ArrayEnumerable<TSource>, Enumerator, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>, ZipEnumerable<TEnumerable0, TEnumerator0, TSource0, TEnumerable1, TEnumerator1, TSource1, TSource, TAction0>.Enumerator, TSource>(this, second);
        #endregion

        #region Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        public bool Any() => Length != 0;

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate.Calc(ref *ptr))
                    return true;
            return false;
        }

        public bool Any(Func<TSource, bool> predicate)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate(*ptr))
                    return true;
            return false;
        }

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (!predicate.Calc(ref *ptr))
                    return false;
            return true;
        }

        public bool All(Func<TSource, bool> predicate)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (!predicate(*ptr))
                    return false;
            return true;
        }

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                func.Execute(ref seed, ref *ptr);
        }

        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                func.Execute(ref seed, ref *ptr);
            return resultFunc.Calc(ref seed);
        }

        public TSource Aggregate(Func<TSource, TSource, TSource> func)
        {
            if (Length == 0) throw new InvalidOperationException();
            var ptr = GetPointer();
            var seed = *ptr++;
            for (var i = 1L; i < Length; i++, ptr++)
                seed = func(seed, *ptr);
            return seed;
        }

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                seed = func(seed, *ptr);
            return seed;
        }

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                seed = func(seed, *ptr);
            return resultFunc(seed);
        }

        public bool Contains(TSource value)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (value.Equals(*ptr))
                    return true;
            return false;
        }

        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (comparer.Equals(value, *ptr))
                    return true;
            return false;
        }

        public bool Contains<TComparer>(TSource value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (comparer.Calc(ref value, ref *ptr))
                    return true;
            return false;
        }

        public int Count() => (int) Length;

        public int Count(Func<TSource, bool> predicate)
        {
            var count = 0;
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate(*ptr))
                    ++count;
            return count;
        }

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            var count = 0;
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate.Calc(ref *ptr))
                    ++count;
            return count;
        }

        public long LongCount() => Length;

        public long LongCount(Func<TSource, bool> predicate)
        {
            var count = 0L;
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate(*ptr))
                    ++count;
            return count;
        }

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
        {
            var count = 0L;
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate.Calc(ref *ptr))
                    ++count;
            return count;
        }

        public bool TryGetElementAt(long index, out TSource element)
        {
            if (index < 0 || index >= Length)
            {
                element = default;
                return false;
            }
            element = array[offset + index];
            return true;
        }

        public bool TryGetFirst(out TSource first)
        {
            if (Length == 0)
            {
                first = default;
                return false;
            }
            first = array[offset];
            return true;
        }

        public bool TryGetLast(out TSource last)
        {
            if (Length == 0)
            {
                last = default;
                return false;
            }
            last = array[offset + Length - 1];
            return true;
        }

        public bool TryGetSingle(out TSource value)
        {
            if (Length == 1)
            {
                value = array[offset];
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => this.TryGetSingle<ArrayEnumerable<TSource>, Enumerator, TSource, TPredicate>(out value, predicate);

        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
            => this.TryGetSingle<ArrayEnumerable<TSource>, Enumerator, TSource>(out value, predicate);

        public TSource[] ToArray()
        {
            if (Length == 0)
                return Array.Empty<TSource>();
            var answer = new TSource[Length];
            var src = Unsafe.AsPointer(ref array[offset]);
            var dest = Unsafe.AsPointer(ref answer[0]);
            UnsafeUtility.MemCpy(dest, src, sizeof(TSource) * Length);
            return answer;
        }

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
        {
            if (Length == 0) return default;
            var answer = new NativeArray<TSource>((int) Length, allocator);
            var src = Unsafe.AsPointer(ref array[offset]);
            var dest = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
            UnsafeUtility.MemCpy(dest, src, sizeof(TSource) * Length);
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var answer = new Dictionary<TKey, TElement>((int) Length);
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                answer.Add(keySelector(*ptr), elementSelector(*ptr));
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
        {
            var answer = new Dictionary<TKey, TElement>((int) Length);
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
            {
                ref var item = ref *ptr;
                answer.Add(keySelector.Calc(ref item), elementSelector.Calc(ref item));
            }
            return answer;
        }

        public HashSet<TSource> ToHashSet()
        {
            var answer = new HashSet<TSource>();
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                answer.Add(*ptr);
            return answer;
        }

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
        {
            var answer = new HashSet<TSource>(comparer);
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                answer.Add(*ptr);
            return answer;
        }

        public List<TSource> ToList()
        {
            var answer = new List<TSource>();
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                answer.Add(*ptr);
            return answer;
        }
        #endregion
    }
}
#endif