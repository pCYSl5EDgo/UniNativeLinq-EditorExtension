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
        void ILinq<T>.Aggregate<TFunc>(ref T seed, TFunc func)
        {
            for (var i = 0; i < length; i++)
                func.Execute(ref seed, ref ptr[i]);
        }

        void ILinq<T>.Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
        {
            for (var i = 0; i < length; i++)
                func.Execute(ref seed, ref ptr[i]);
        }

        TResult ILinq<T>.Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
        {
            for (var i = 0; i < length; i++)
                func.Execute(ref seed, ref ptr[i]);
            return resultFunc.Calc(ref seed);
        }

        T ILinq<T>.Aggregate(T seed, Func<T, T, T> func)
        {
            for (var i = 0; i < length; i++)
                seed = func(seed, ptr[i]);
            return seed;
        }

        TAccumulate ILinq<T>.Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func)
        {
            for (var i = 0; i < length; i++)
                seed = func(seed, ptr[i]);
            return seed;
        }

        TResult ILinq<T>.Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
        {
            for (var i = 0; i < length; i++)
                seed = func(seed, ptr[i]);
            return resultFunc(seed);
        }

        bool ILinq<T>.All<TPredicate>(TPredicate predicate)
        {
            for (var i = 0; i < length; i++)
                if (!predicate.Calc(ref ptr[i]))
                    return false;
            return true;
        }

        bool ILinq<T>.All(Func<T, bool> predicate)
        {
            for (var i = 0; i < length; i++)
                if (!predicate(ptr[i]))
                    return false;
            return true;
        }

        bool ILinq<T>.Any() => length != 0;

        bool ILinq<T>.Any<TPredicate>(TPredicate predicate)
        {
            for (var i = 0; i < length; i++)
                if (predicate.Calc(ref ptr[i]))
                    return true;
            return false;
        }

        bool ILinq<T>.Any(Func<T, bool> predicate)
        {
            for (var i = 0; i < length; i++)
                if (predicate(ptr[i]))
                    return true;
            return false;
        }

        bool ILinq<T>.Contains(T value)
        {
            for (var i = 0; i < length; i++)
                if (ptr[i].Equals(value))
                    return true;
            return false;
        }

        bool ILinq<T>.Contains(T value, IEqualityComparer<T> comparer)
        {
            for (var i = 0; i < length; i++)
                if (comparer.Equals(value, ptr[i]))
                    return true;
            return false;
        }

        bool ILinq<T>.Contains<TComparer>(T value, TComparer comparer)
        {
            for (var i = 0; i < length; i++)
                if (comparer.Calc(ref ptr[i], ref value))
                    return true;
            return false;
        }

        public int Count() => this.length;

        public int Count(Func<T, bool> predicate)
        {
            var count = 0;
            for (var i = 0; i < length; i++)
                if (predicate(ptr[i]))
                    ++count;
            return count;
        }

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<T, bool>
        {
            var count = 0;
            for (var i = 0; i < length; i++)
                if (predicate.Calc(ref ptr[i]))
                    ++count;
            return count;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        long ILinq<T>.LongCount() => this.length;

        long ILinq<T>.LongCount(Func<T, bool> predicate) => Count(predicate);

        long ILinq<T>.LongCount<TPredicate>(TPredicate predicate) => Count(predicate);

        T[] ILinq<T>.ToArray()
        {
            if (length <= 0)return Array.Empty<T>();
            var answer = new T[length];
            fixed(T* p = &answer[0])
                UnsafeUtility.MemCpy(p, ptr,sizeof(T) * length);
            return answer;
        }

        Dictionary<TKey, TElement> ILinq<T>.ToDictionary<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
        {
            var answer = new Dictionary<TKey, TElement>(length);
            for (var i = 0; i < length; i++)
                answer.Add(keySelector(ptr[i]), elementSelector(ptr[i]));
            return answer;
        }

        Dictionary<TKey, TElement> ILinq<T>.ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
        {
            var answer = new Dictionary<TKey, TElement>(length);
            for (var i = 0; i < length; i++)
            {
                ref var item = ref ptr[i];
                answer.Add(keySelector.Calc(ref item), elementSelector.Calc(ref item));
            }
            return answer;
        }

        HashSet<T> ILinq<T>.ToHashSet()
        { 
            var answer = new HashSet<T>();
            for (var i = 0; i < length; i++)
                answer.Add(ptr[i]);
            return answer;
        }

        HashSet<T> ILinq<T>.ToHashSet(IEqualityComparer<T> comparer)
        {
            var answer = new HashSet<T>(comparer);
            for (var i = 0; i < length; i++)
                answer.Add(ptr[i]);
            return answer;
        }

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
        {
            if (index < 0 || index >= length)
            {
                element = default;
                return false;
            }
            element = ptr[index];
            return true;
        }

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

        public struct Enumerator : IRefEnumerator<T>
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