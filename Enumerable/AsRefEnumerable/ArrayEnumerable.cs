using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

#if UNSAFE_ARRAY_ENUMERABLE
namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct ArrayEnumerable<T> : IRefEnumerable<ArrayEnumerable<T>.Enumerator, T>, ILinq<T>
        where T : unmanaged
#if STRICT_EQUALITY
        , IEquatable<T>
#endif
    {
        private readonly T[] array;
        private readonly long offset;
        internal readonly long Length;

        internal T* GetPointer() => (T*) Unsafe.AsPointer(ref array[offset]);
        private T* GetPinPointer(out ulong gcHandle) => (T*) UnsafeUtility.PinGCArrayAndGetDataAddress(array, out gcHandle) + offset;


        public ArrayEnumerable(T[] array)
        {
            this.array = array ?? throw new ArgumentNullException();
            this.Length = array.LongLength;
            this.offset = 0;
        }

        public ArrayEnumerable(ArraySegment<T> segment)
        {
            this.array = segment.Array ?? throw new ArgumentNullException();
            this.Length = segment.Count;
            this.offset = segment.Offset;
        }

        public ArrayEnumerable(T[] array, long offset, long count)
        {
            this.array = array ?? throw new ArgumentNullException();
            this.Length = count;
            this.offset = offset;
        }

        public struct Enumerator : IRefEnumerator<T>
        {
            private readonly T* ptr;
            private readonly long length;
            private readonly ulong gcHandle;
            private long index;

            internal Enumerator(T* ptr, long length, ulong gcHandle)
            {
                this.ptr = ptr;
                this.length = length;
                this.gcHandle = gcHandle;
                this.index = -1;
            }

            public bool MoveNext() => ++index < length;
            public void Reset() => index = -1;
            public ref T Current => ref ptr[index];
            T IEnumerator<T>.Current => Current;
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

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Enumerable
        public AppendEnumerable<ArrayEnumerable<T>, Enumerator, T>
            Append(T value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<ArrayEnumerable<T>, Enumerator, T>(this, value, allocator);

        public AppendPointerEnumerable<ArrayEnumerable<T>, Enumerator, T>
            Append(T* value)
            => new AppendPointerEnumerable<ArrayEnumerable<T>, Enumerator, T>(this, value);

        public ArrayEnumerable<T> AsRefEnumerable() => this;

        public DefaultIfEmptyEnumerable<ArrayEnumerable<T>, Enumerator, T>
            DefaultIfEmpty(T defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<ArrayEnumerable<T>, Enumerator, T>(this, defaultValue, allocator);

        public DistinctEnumerable<ArrayEnumerable<T>, Enumerator, T, DefaultEqualityComparer<T>, DefaultGetHashCodeFunc<T>>
            Distinct(Allocator allocator = Allocator.Temp)
            => new DistinctEnumerable<ArrayEnumerable<T>, Enumerator, T, DefaultEqualityComparer<T>, DefaultGetHashCodeFunc<T>>(this, default, default, allocator);

        public DistinctEnumerable<ArrayEnumerable<T>, Enumerator, T, TEqualityComparer, TGetHashCodeFunc>
            Distinct<TEqualityComparer, TGetHashCodeFunc>
            (TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator = Allocator.Temp)
            where TEqualityComparer : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc : struct, IRefFunc<T, int>
            => new DistinctEnumerable<ArrayEnumerable<T>, Enumerator, T, TEqualityComparer, TGetHashCodeFunc>(this, comparer, getHashCodeFunc, allocator);

        public SelectEnumerable<ArrayEnumerable<T>, Enumerator, T, TResult, TAction>
            Select<TResult, TAction>
            (TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IRefAction<T, TResult>
            => new SelectEnumerable<ArrayEnumerable<T>, Enumerator, T, TResult, TAction>(this, action, allocator);

        public SelectIndexEnumerable<ArrayEnumerable<T>, Enumerator, T, TResult, TAction>
            SelectIndex<TResult, TAction>
            (TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<T, TResult>
            => new SelectIndexEnumerable<ArrayEnumerable<T>, Enumerator, T, TResult, TAction>(this, action, allocator);

        public WhereEnumerable<ArrayEnumerable<T>, Enumerator, T, TPredicate>
            Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => new WhereEnumerable<ArrayEnumerable<T>, Enumerator, T, TPredicate>(this, predicate);
        #endregion

        #region Concat
        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                TEnumerable0, TEnumerator0,
                T
            >
            Concat<TEnumerable0, TEnumerator0>
            (in TEnumerable0 second)
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    TEnumerable0, TEnumerator0,
                    T>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T
            >
            Concat(in NativeEnumerable<T> second)
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                NativeEnumerable<T>,
                NativeEnumerable<T>.Enumerator,
                T
            >
            Concat(in NativeArray<T> second)
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    NativeEnumerable<T>,
                    NativeEnumerable<T>.Enumerator,
                    T>
                (this, second.AsRefEnumerable());

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerator<TEnumerator0, T>,
                T
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendEnumerable<TEnumerable0, TEnumerator0, T> second)
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    AppendEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerator<TEnumerator0, T>,
                    T>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, T>,
                AppendEnumerator<TEnumerator0, T>,
                T
            >
            Concat<TEnumerable0, TEnumerator0>
            (in AppendPointerEnumerable<TEnumerable0, TEnumerator0, T> second)
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    AppendPointerEnumerable<TEnumerable0, TEnumerator0, T>,
                    AppendEnumerator<TEnumerator0, T>,
                    T>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                ArrayEnumerable<T>, Enumerator,
                T
            >
            Concat(in ArrayEnumerable<T> second)
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    ArrayEnumerable<T>, Enumerator,
                    T>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1>
            (in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T> second)
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEnumerator1 : struct, IRefEnumerator<T>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, T>
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>,
                    ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, T>.Enumerator,
                    T>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0>
            (in DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T> second)
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>,
                    DefaultIfEmptyEnumerable<TEnumerable0, TEnumerator0, T>.Enumerator,
                    T>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer, TGetHashCodeFunc>,
                DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer, TGetHashCodeFunc>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TEqualityComparer, TGetHashCodeFunc>
            (in DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer, TGetHashCodeFunc> second)
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TEqualityComparer : struct, IRefFunc<T, T, bool>
            where TGetHashCodeFunc : struct, IRefFunc<T, int>
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer, TGetHashCodeFunc>, DistinctEnumerable<TEnumerable0, TEnumerator0, T, TEqualityComparer, TGetHashCodeFunc>.Enumerator, T>(this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                RangeRepeatEnumerable<T, TAction>,
                RangeRepeatEnumerable<T, TAction>.Enumerator,
                T
            >
            Concat<TAction>
            (in RangeRepeatEnumerable<T, TAction> second)
            where TAction : struct, IRefAction<T>
            => new ConcatEnumerable<
                    ArrayEnumerable<T>, Enumerator,
                    RangeRepeatEnumerable<T, TAction>,
                    RangeRepeatEnumerable<T, TAction>.Enumerator,
                    T
                >
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TPrev, TAction>
            (in SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction> second)
            where TPrev : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrev>
#endif
            where TAction : unmanaged, IRefAction<TPrev, T>
            where TEnumerator0 : struct, IRefEnumerator<TPrev>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev>
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction>,
                    SelectEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction>.Enumerator,
                    T>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction>,
                SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TPrev, TAction>
            (in SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction> second)
            where TPrev : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrev>
#endif
            where TAction : unmanaged, ISelectIndex<TPrev, T>
            where TEnumerator0 : struct, IRefEnumerator<TPrev>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrev>
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction>,
                    SelectIndexEnumerable<TEnumerable0, TEnumerator0, TPrev, T, TAction>.Enumerator,
                    T>
                (this, second);

        public ConcatEnumerable<
                ArrayEnumerable<T>, Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate>,
                WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate>.Enumerator,
                T
            >
            Concat<TEnumerable0, TEnumerator0, TPredicate>
            (in WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate> second)
            where TEnumerator0 : struct, IRefEnumerator<T>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, T>
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => new ConcatEnumerable<ArrayEnumerable<T>, Enumerator,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate>,
                    WhereEnumerable<TEnumerable0, TEnumerator0, T, TPredicate>.Enumerator,
                    T>
                (this, second);
        #endregion

        #region Function
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFastCount() => true;

        public bool Any() => Length != 0;

        public bool Any<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<T, bool>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate.Calc(ref *ptr))
                    return true;
            return false;
        }

        public bool Any(Func<T, bool> predicate)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate(*ptr))
                    return true;
            return false;
        }

        public bool All<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<T, bool>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (!predicate.Calc(ref *ptr))
                    return false;
            return true;
        }

        public bool All(Func<T, bool> predicate)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (!predicate(*ptr))
                    return false;
            return true;
        }

        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
            where TFunc : unmanaged, IRefAction<TAccumulate, T>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                func.Execute(ref seed, ref *ptr);
        }

        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : unmanaged, IRefAction<TAccumulate, T>
            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                func.Execute(ref seed, ref *ptr);
            return resultFunc.Calc(ref seed);
        }

        public T Aggregate(Func<T, T, T> func)
        {
            if (Length == 0) throw new InvalidOperationException();
            var ptr = GetPointer();
            var seed = *ptr++;
            for (var i = 1L; i < Length; i++, ptr++)
                seed = func(seed, *ptr);
            return seed;
        }

        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                seed = func(seed, *ptr);
            return seed;
        }

        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                seed = func(seed, *ptr);
            return resultFunc(seed);
        }

        public bool Contains(T value)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (value.Equals(*ptr))
                    return true;
            return false;
        }

        public bool Contains(T value, IEqualityComparer<T> comparer)
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (comparer.Equals(value, *ptr))
                    return true;
            return false;
        }

        public bool Contains<TComparer>(T value, TComparer comparer)
            where TComparer : unmanaged, IRefFunc<T, T, bool>
        {
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (comparer.Calc(ref value, ref *ptr))
                    return true;
            return false;
        }

        public int Count() => (int) Length;

        public int Count(Func<T, bool> predicate)
        {
            var count = 0;
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate(*ptr))
                    ++count;
            return count;
        }

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<T, bool>
        {
            var count = 0;
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate.Calc(ref *ptr))
                    ++count;
            return count;
        }

        public long LongCount() => Length;

        public long LongCount(Func<T, bool> predicate)
        {
            var count = 0L;
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate(*ptr))
                    ++count;
            return count;
        }

        public long LongCount<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<T, bool>
        {
            var count = 0L;
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                if (predicate.Calc(ref *ptr))
                    ++count;
            return count;
        }

        public bool TryGetElementAt(int index, out T element)
        {
            if (index < 0 || index >= Length)
            {
                element = default;
                return false;
            }
            element = array[offset + index];
            return true;
        }

        public bool TryGetFirst(out T first)
        {
            if (Length == 0)
            {
                first = default;
                return false;
            }
            first = array[offset];
            return true;
        }

        public bool TryGetLast(out T last)
        {
            if (Length == 0)
            {
                last = default;
                return false;
            }
            last = array[offset + Length - 1];
            return true;
        }

        public bool TryGetSingle(out T value)
        {
            if (Length == 1)
            {
                value = array[offset];
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetSingle<TPredicate>(out T value, TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => this.TryGetSingle<ArrayEnumerable<T>, Enumerator, T, TPredicate>(out value, predicate);

        public bool TryGetSingle(out T value, Func<T, bool> predicate)
            => this.TryGetSingle<ArrayEnumerable<T>, Enumerator, T>(out value, predicate);

        public T[] ToArray()
        {
            if (Length == 0)
                return Array.Empty<T>();
            var answer = new T[Length];
            var src = Unsafe.AsPointer(ref array[offset]);
            var dest = Unsafe.AsPointer(ref answer[0]);
            UnsafeUtility.MemCpy(dest, src, sizeof(T) * Length);
            return answer;
        }

        public NativeArray<T> ToNativeArray(Allocator allocator)
        {
            if (Length == 0) return default;
            var answer = new NativeArray<T>((int) Length, allocator);
            var src = Unsafe.AsPointer(ref array[offset]);
            var dest = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(answer);
            UnsafeUtility.MemCpy(dest, src, sizeof(T) * Length);
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
        {
            var answer = new Dictionary<TKey, TElement>((int) Length);
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                answer.Add(keySelector(*ptr), elementSelector(*ptr));
            return answer;
        }

        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            where TKeyFunc : unmanaged, IRefFunc<T, TKey>
            where TElementFunc : unmanaged, IRefFunc<T, TElement>
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

        public HashSet<T> ToHashSet()
        {
            var answer = new HashSet<T>();
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                answer.Add(*ptr);
            return answer;
        }

        public HashSet<T> ToHashSet(IEqualityComparer<T> comparer)
        {
            var answer = new HashSet<T>(comparer);
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                answer.Add(*ptr);
            return answer;
        }

        public List<T> ToList()
        {
            var answer = new List<T>();
            var ptr = GetPointer();
            for (var i = 0L; i < Length; i++, ptr++)
                answer.Add(*ptr);
            return answer;
        }
        #endregion
    }
}
#endif