using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct NativeEnumerable<T> : IRefEnumerable<NativeEnumerable<T>.Enumerator, T>, ILinq<T>
        where T : unmanaged
#if STRICT_EQUALITY
        , IEquatable<T>
#endif
    {
        private readonly T* ptr;
        private readonly int length;

        internal NativeEnumerable(NativeArray<T> array)
        {
            if (array.IsCreated)
            {
                this.ptr = array.GetPointer();
                this.length = array.Length;
            }
            else
            {
                this.ptr = null;
                this.length = 0;
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(this);

        #region Enumerable
        public NativeEnumerable<T> AsRefEnumerable() => this;

        public WhereEnumerable<NativeEnumerable<T>, Enumerator, T, TPredicate> Where<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<T, bool>
            => new WhereEnumerable<NativeEnumerable<T>, Enumerator, T, TPredicate>(this, predicate);

        public SelectEnumerable<NativeEnumerable<T>, Enumerator, T, TResult, TAction> Select<TResult, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, IRefAction<T, TResult>
            => new SelectEnumerable<NativeEnumerable<T>, Enumerator, T, TResult, TAction>(this, action, allocator);
        #endregion

        #region function
        void ILinq<T>.Aggregate<TFunc>(ref T seed, TFunc func) => this.Aggregate<NativeEnumerable<T>, Enumerator, T, TFunc>(ref seed, func);

        void ILinq<T>.Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func) => this.Aggregate<NativeEnumerable<T>, Enumerator, T, TAccumulate, TFunc>(ref seed, func);

        TResult ILinq<T>.Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            => this.Aggregate<NativeEnumerable<T>, Enumerator, T, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        T ILinq<T>.Aggregate(T seed, Func<T, T, T> func) => this.Aggregate<NativeEnumerable<T>, Enumerator, T>(seed, func);

        TAccumulate ILinq<T>.Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func) => this.Aggregate<NativeEnumerable<T>, Enumerator, T, TAccumulate>(seed, func);

        TResult ILinq<T>.Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => this.Aggregate<NativeEnumerable<T>, Enumerator, T, TAccumulate, TResult>(seed, func, resultFunc);

        bool ILinq<T>.All<TPredicate>(TPredicate predicate)
            => this.All<NativeEnumerable<T>, Enumerator, T, TPredicate>(predicate);

        bool ILinq<T>.All(Func<T, bool> predicate)
            => this.All<NativeEnumerable<T>, Enumerator, T>(predicate);

        bool ILinq<T>.Any()
            => this.Any<NativeEnumerable<T>, Enumerator, T>();

        bool ILinq<T>.Any<TPredicate>(TPredicate predicate)
            => this.Any<NativeEnumerable<T>, Enumerator, T, TPredicate>(predicate);

        bool ILinq<T>.Any(Func<T, bool> predicate) =>
            this.Any<NativeEnumerable<T>, Enumerator, T>(predicate);

        bool ILinq<T>.Contains(T value)
            => this.Contains<NativeEnumerable<T>, Enumerator, T>(value);

        bool ILinq<T>.Contains(T value, IEqualityComparer<T> comparer)
            => this.Contains<NativeEnumerable<T>, Enumerator, T>(value, comparer);

        bool ILinq<T>.Contains<TComparer>(T value, TComparer comparer)
            => this.Contains<NativeEnumerable<T>, Enumerator, T, TComparer>(value, comparer);

        int ILinq<T>.Count() => this.length;

        int ILinq<T>.Count(Func<T, bool> predicate)
            => this.Count<NativeEnumerable<T>, Enumerator, T>(predicate);

        int ILinq<T>.Count<TPredicate>(TPredicate predicate)
            => this.Count<NativeEnumerable<T>, Enumerator, T, TPredicate>(predicate);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        long ILinq<T>.LongCount() => this.length;

        long ILinq<T>.LongCount(Func<T, bool> predicate)
            => this.Count<NativeEnumerable<T>, Enumerator, T>(predicate);

        long ILinq<T>.LongCount<TPredicate>(TPredicate predicate)
            => this.Count<NativeEnumerable<T>, Enumerator, T, TPredicate>(predicate);

        T[] ILinq<T>.ToArray()
            => this.ToArray<NativeEnumerable<T>, Enumerator, T>();

        Dictionary<TKey, TElement> ILinq<T>.ToDictionary<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
            => this.ToDictionary<NativeEnumerable<T>, Enumerator, T, TKey, TElement>(keySelector, elementSelector);

        Dictionary<TKey, TElement> ILinq<T>.ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            => this.ToDictionary<NativeEnumerable<T>, Enumerator, T, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        HashSet<T> ILinq<T>.ToHashSet()
            => this.ToHashSet<NativeEnumerable<T>, Enumerator, T>();

        HashSet<T> ILinq<T>.ToHashSet(IEqualityComparer<T> comparer)
            => this.ToHashSet<NativeEnumerable<T>, Enumerator, T>(comparer);

        List<T> ILinq<T>.ToList()
        {
            var answer = new List<T>(length);
            for (var i = 0; i < length; i++)
                answer.Add(ptr[i]);
            return answer;
        }

        NativeArray<T> ILinq<T>.ToNativeArray(Allocator allocator)
        {
            if (length == 0) return default;
            var answer = new NativeArray<T>(length, allocator, NativeArrayOptions.UninitializedMemory);
            UnsafeUtility.MemCpy(answer.GetPointer(), ptr, sizeof(T) * length);
            return answer;
        }

        bool ILinq<T>.TryGetElementAt(int index, out T element)
            => this.TryGetElementAt<NativeEnumerable<T>, Enumerator, T>(index, out element);

        bool ILinq<T>.TryGetFirst(out T first)
        {
            if (length == 0)
            {
                first = default;
                return false;
            }
            first = *ptr;
            return true;
        }

        bool ILinq<T>.TryGetLast(out T last)
        {
            if (length == 0)
            {
                last = default;
                return false;
            }
            last = ptr[length - 1];
            return true;
        }

        bool ILinq<T>.TryGetSingle(out T value)
        {
            if (length != 1)
            {
                value = default;
                return false;
            }
            value = *ptr;
            return true;
        }

        bool ILinq<T>.TryGetSingle<TPredicate>(out T value, TPredicate predicate)
            => this.TryGetSingle<NativeEnumerable<T>, Enumerator, T, TPredicate>(out value, predicate);

        bool ILinq<T>.TryGetSingle(out T value, Func<T, bool> predicate)
            => this.TryGetSingle<NativeEnumerable<T>, Enumerator, T>(out value, predicate);
        #endregion

        public unsafe struct Enumerator : IRefEnumerator<T>
        {
            private readonly T* ptr;
            private readonly int length;
            private int index;

            public ref T Current => ref ptr[index];
            T IEnumerator<T>.Current => Current;
            object IEnumerator.Current => Current;

            internal Enumerator(in NativeEnumerable<T> parent)
            {
                index = -1;
                ptr = parent.ptr;
                length = parent.length;
            }

            public void Dispose() => this = default;

            public bool MoveNext() => ++index < length;

            public void Reset() => throw new InvalidOperationException();
        }
    }
}