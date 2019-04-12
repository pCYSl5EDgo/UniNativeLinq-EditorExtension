using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public struct NativeEnumerable<T> : IRefEnumerable<NativeEnumerable<T>.Enumerator, T>, ILinq<T>
        where T : unmanaged
#if STRICT_EQUALITY
        , IEquatable<T>
#endif
    {
        private NativeArray<T> array;

        public NativeEnumerable(NativeArray<T> array)
        {
            this.array = array;
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
        void ILinq<T>.Aggregate<TFunc>(ref T seed, TFunc func) => array.Aggregate<T, TFunc>(ref seed, func);

        void ILinq<T>.Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func) => array.Aggregate(ref seed, func);

        TResult ILinq<T>.Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            => array.Aggregate<T, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);

        T ILinq<T>.Aggregate(T seed, Func<T, T, T> func) => array.Aggregate<T, T>(seed, func);

        TAccumulate ILinq<T>.Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func) => array.Aggregate(seed, func);

        TResult ILinq<T>.Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            => array.Aggregate(seed, func, resultFunc);

        bool ILinq<T>.All<TPredicate>(TPredicate predicate) => array.All(predicate);

        bool ILinq<T>.All(Func<T, bool> predicate) => array.All(predicate);

        bool ILinq<T>.Any() => array.Any();

        bool ILinq<T>.Any<TPredicate>(TPredicate predicate) => array.Any(predicate);

        bool ILinq<T>.Any(Func<T, bool> predicate) => array.Any(predicate);

        bool ILinq<T>.Contains(T value) => array.Contains(value);

        bool ILinq<T>.Contains(T value, IEqualityComparer<T> comparer) => array.Contains(value, comparer);

        bool ILinq<T>.Contains<TComparer>(T value, TComparer comparer) => array.Contains(value, comparer);

        int ILinq<T>.Count() => array.Length;

        int ILinq<T>.Count(Func<T, bool> predicate) => array.Count(predicate);

        int ILinq<T>.Count<TPredicate>(TPredicate predicate) => array.Count(predicate);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        long ILinq<T>.LongCount() => array.Length;

        long ILinq<T>.LongCount(Func<T, bool> predicate) => array.Count(predicate);

        long ILinq<T>.LongCount<TPredicate>(TPredicate predicate) => array.Count(predicate);

        T[] ILinq<T>.ToArray() => array.ToArray();

        Dictionary<TKey, TElement> ILinq<T>.ToDictionary<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
            => array.ToDictionary(keySelector, elementSelector);

        Dictionary<TKey, TElement> ILinq<T>.ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
            => array.ToDictionary<T, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);

        HashSet<T> ILinq<T>.ToHashSet() => array.ToHashSet();

        HashSet<T> ILinq<T>.ToHashSet(IEqualityComparer<T> comparer) => array.ToHashSet(comparer);

        List<T> ILinq<T>.ToList() => array.ToList();

        NativeArray<T> ILinq<T>.ToNativeArray(Allocator allocator)
        {
            var answer = new NativeArray<T>(array.Length, allocator, NativeArrayOptions.UninitializedMemory);
            answer.CopyFrom(array);
            return answer;
        }

        bool ILinq<T>.TryGetElementAt(int index, out T element) => array.TryGetElementAt(index, out element);

        bool ILinq<T>.TryGetFirst(out T first)
        {
            if (!array.IsCreated || array.Length < 1)
            {
                first = default;
                return false;
            }
            first = array[0];
            return true;
        }

        bool ILinq<T>.TryGetLast(out T last)
        {
            if (!array.IsCreated || array.Length == 0)
            {
                last = default;
                return false;
            }
            last = array[array.Length - 1];
            return true;
        }

        bool ILinq<T>.TryGetSingle(out T value) => array.TryGetSingle(out value);

        bool ILinq<T>.TryGetSingle<TPredicate>(out T value, TPredicate predicate) => array.TryGetSingle(out value, predicate);

        bool ILinq<T>.TryGetSingle(out T value, Func<T, bool> predicate) => array.TryGetSingle(out value, predicate);
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
                if (parent.array.IsCreated)
                {
                    ptr = parent.array.GetPointer();
                    length = parent.array.Length;
                }
                else
                {
                    ptr = null;
                    length = 0;
                }
            }

            public void Dispose() => this = default;

            public bool MoveNext() => ++index < length;

            public void Reset() => throw new InvalidOperationException();
        }
    }
}