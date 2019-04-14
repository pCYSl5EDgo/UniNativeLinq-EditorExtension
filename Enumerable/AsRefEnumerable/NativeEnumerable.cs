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
        internal readonly T* Ptr;
        internal readonly int Length;

        internal NativeEnumerable(NativeArray<T> array)
        {
            if (array.IsCreated)
            {
                this.Ptr = array.GetPointer();
                this.Length = array.Length;
            }
            else
            {
                this.Ptr = null;
                this.Length = 0;
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

        public SelectIndexEnumerable<NativeEnumerable<T>, Enumerator, T, TResult, TAction> SelectIndex<TResult, TAction>(TAction action, Allocator allocator = Allocator.Temp)
            where TResult : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TResult>
#endif
            where TAction : unmanaged, ISelectIndex<T, TResult>
            => new SelectIndexEnumerable<NativeEnumerable<T>, Enumerator, T, TResult, TAction>(this, action, allocator);

        public AppendEnumerable<NativeEnumerable<T>, Enumerator, T> Append(T value, Allocator allocator = Allocator.Temp)
            => new AppendEnumerable<NativeEnumerable<T>, Enumerator, T>(this, value, allocator);

        public AppendPointerEnumerable<NativeEnumerable<T>, Enumerator, T> Append(T* value)
            => new AppendPointerEnumerable<NativeEnumerable<T>, Enumerator, T>(this, value);

        public DefaultIfEmptyEnumerable<NativeEnumerable<T>, Enumerator, T> DefaultIfEmpty(T defaultValue, Allocator allocator = Allocator.Temp)
            => new DefaultIfEmptyEnumerable<NativeEnumerable<T>, Enumerator, T>(this, defaultValue, allocator);
        #endregion

        #region function
        void ILinq<T>.Aggregate<TFunc>(ref T seed, TFunc func)
        {
            for (var i = 0; i < Length; i++)
                func.Execute(ref seed, ref Ptr[i]);
        }

        void ILinq<T>.Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
        {
            for (var i = 0; i < Length; i++)
                func.Execute(ref seed, ref Ptr[i]);
        }

        TResult ILinq<T>.Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
        {
            for (var i = 0; i < Length; i++)
                func.Execute(ref seed, ref Ptr[i]);
            return resultFunc.Calc(ref seed);
        }

        T ILinq<T>.Aggregate(T seed, Func<T, T, T> func)
        {
            for (var i = 0; i < Length; i++)
                seed = func(seed, Ptr[i]);
            return seed;
        }

        TAccumulate ILinq<T>.Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func)
        {
            for (var i = 0; i < Length; i++)
                seed = func(seed, Ptr[i]);
            return seed;
        }

        TResult ILinq<T>.Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
        {
            for (var i = 0; i < Length; i++)
                seed = func(seed, Ptr[i]);
            return resultFunc(seed);
        }

        bool ILinq<T>.All<TPredicate>(TPredicate predicate)
        {
            for (var i = 0; i < Length; i++)
                if (!predicate.Calc(ref Ptr[i]))
                    return false;
            return true;
        }

        bool ILinq<T>.All(Func<T, bool> predicate)
        {
            for (var i = 0; i < Length; i++)
                if (!predicate(Ptr[i]))
                    return false;
            return true;
        }

        bool ILinq<T>.Any() => Length != 0;

        bool ILinq<T>.Any<TPredicate>(TPredicate predicate)
        {
            for (var i = 0; i < Length; i++)
                if (predicate.Calc(ref Ptr[i]))
                    return true;
            return false;
        }

        bool ILinq<T>.Any(Func<T, bool> predicate)
        {
            for (var i = 0; i < Length; i++)
                if (predicate(Ptr[i]))
                    return true;
            return false;
        }

        bool ILinq<T>.Contains(T value)
        {
            for (var i = 0; i < Length; i++)
                if (Ptr[i].Equals(value))
                    return true;
            return false;
        }

        bool ILinq<T>.Contains(T value, IEqualityComparer<T> comparer)
        {
            for (var i = 0; i < Length; i++)
                if (comparer.Equals(value, Ptr[i]))
                    return true;
            return false;
        }

        bool ILinq<T>.Contains<TComparer>(T value, TComparer comparer)
        {
            for (var i = 0; i < Length; i++)
                if (comparer.Calc(ref Ptr[i], ref value))
                    return true;
            return false;
        }

        public int Count() => this.Length;

        public int Count(Func<T, bool> predicate)
        {
            var count = 0;
            for (var i = 0; i < Length; i++)
                if (predicate(Ptr[i]))
                    ++count;
            return count;
        }

        public int Count<TPredicate>(TPredicate predicate)
            where TPredicate : unmanaged, IRefFunc<T, bool>
        {
            var count = 0;
            for (var i = 0; i < Length; i++)
                if (predicate.Calc(ref Ptr[i]))
                    ++count;
            return count;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        long ILinq<T>.LongCount() => this.Length;

        long ILinq<T>.LongCount(Func<T, bool> predicate) => Count(predicate);

        long ILinq<T>.LongCount<TPredicate>(TPredicate predicate) => Count(predicate);

        T[] ILinq<T>.ToArray()
        {
            if (Length <= 0) return Array.Empty<T>();
            var answer = new T[Length];
            fixed (T* p = &answer[0])
                UnsafeUtility.MemCpy(p, Ptr, sizeof(T) * Length);
            return answer;
        }

        Dictionary<TKey, TElement> ILinq<T>.ToDictionary<TKey, TElement>(Func<T, TKey> keySelector, Func<T, TElement> elementSelector)
        {
            var answer = new Dictionary<TKey, TElement>(Length);
            for (var i = 0; i < Length; i++)
                answer.Add(keySelector(Ptr[i]), elementSelector(Ptr[i]));
            return answer;
        }

        Dictionary<TKey, TElement> ILinq<T>.ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
        {
            var answer = new Dictionary<TKey, TElement>(Length);
            for (var i = 0; i < Length; i++)
            {
                ref var item = ref Ptr[i];
                answer.Add(keySelector.Calc(ref item), elementSelector.Calc(ref item));
            }
            return answer;
        }

        HashSet<T> ILinq<T>.ToHashSet()
        {
            var answer = new HashSet<T>();
            for (var i = 0; i < Length; i++)
                answer.Add(Ptr[i]);
            return answer;
        }

        HashSet<T> ILinq<T>.ToHashSet(IEqualityComparer<T> comparer)
        {
            var answer = new HashSet<T>(comparer);
            for (var i = 0; i < Length; i++)
                answer.Add(Ptr[i]);
            return answer;
        }

        List<T> ILinq<T>.ToList()
        {
            var answer = new List<T>(Length);
            for (var i = 0; i < Length; i++)
                answer.Add(Ptr[i]);
            return answer;
        }

        NativeArray<T> ILinq<T>.ToNativeArray(Allocator allocator)
        {
            if (Length == 0) return default;
            var answer = new NativeArray<T>(Length, allocator, NativeArrayOptions.UninitializedMemory);
            UnsafeUtility.MemCpy(answer.GetPointer(), Ptr, sizeof(T) * Length);
            return answer;
        }

        bool ILinq<T>.TryGetElementAt(int index, out T element)
        {
            if (index < 0 || index >= Length)
            {
                element = default;
                return false;
            }
            element = Ptr[index];
            return true;
        }

        bool ILinq<T>.TryGetFirst(out T first)
        {
            if (Length == 0)
            {
                first = default;
                return false;
            }
            first = *Ptr;
            return true;
        }

        bool ILinq<T>.TryGetLast(out T last)
        {
            if (Length == 0)
            {
                last = default;
                return false;
            }
            last = Ptr[Length - 1];
            return true;
        }

        bool ILinq<T>.TryGetSingle(out T value)
        {
            if (Length != 1)
            {
                value = default;
                return false;
            }
            value = *Ptr;
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
                ptr = parent.Ptr;
                length = parent.Length;
            }

            public void Dispose() => this = default;

            public bool MoveNext() => ++index < length;

            public void Reset() => throw new InvalidOperationException();
        }
    }
}