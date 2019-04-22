//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.Collections;
//using Unity.Collections.LowLevel.Unsafe;
//
//namespace pcysl5edgo.Collections.LINQ
//{
//    public unsafe struct
//        OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>
//        : IRefOrderedEnumerable<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>.Enumerator, TSource>, ILinq<TSource>
//        where TSource : unmanaged
//#if STRICT_EQUALITY
//        , IEquatable<TSource>
//
//        where TEnumerator : struct, IRefEnumerator<TSource>
//        where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
//        where TComparer : struct, IRefFunc<TSource, TSource, int>
//    {
//        private TEnumerable enumerable;
//        private TComparer orderComparer;
//        private readonly Allocator alloc;
//
//        public OrderByEnumerable(in TEnumerable enumerable, TComparer comparer, Allocator allocator)
//        {
//            this.enumerable = enumerable;
//            this.orderComparer = comparer;
//            this.alloc = allocator;
//        }
//
//        public struct Enumerator : IRefEnumerator<TSource>
//        {
//            internal TSource* Ptr;
//            private long capacity;
//            internal long Count;
//            private long index;
//            private readonly Allocator allocator;
//
//            internal Enumerator(ref TEnumerable enumerable, TComparer orderComparer, Allocator allocator)
//            {
//                this.allocator = allocator;
//                this.index = -1;
//                this.Count = 0;
//                this.capacity = 16;
//                if (enumerable.CanFastCount() && (this.capacity = enumerable.Count()) == 0)
//                {
//                    this = default;
//                    return;
//                }
//                this.Ptr = UnsafeUtilityEx.Malloc<TSource>(this.capacity, allocator);
//                var enumerator = enumerable.GetEnumerator();
//                if (!enumerator.MoveNext())
//                {
//                    enumerator.Dispose();
//                    UnsafeUtility.Free(this.Ptr, allocator);
//                    this = default;
//                    return;
//                }
//                Ptr[0] = enumerator.Current;
//                Count = 1;
//                Sort(ref enumerator, ref orderComparer);
//                enumerator.Dispose();
//            }
//
//            private void Sort(ref TEnumerator enumerator, ref TComparer comparer)
//            {
//                while (enumerator.MoveNext())
//                {
//                    var minInclusive = 0L;
//                    var maxInclusive = Count - 1;
//                    ref var current = ref enumerator.Current;
//                    while (minInclusive < maxInclusive)
//                    {
//                        var insertIndex = (minInclusive + maxInclusive) >> 1;
//                        var comp = comparer.Calc(ref current, ref Ptr[insertIndex]);
//                        if (comp == 0)
//                        {
//                            minInclusive = insertIndex + 1;
//                            break;
//                        }
//                        if (comp > 0)
//                            minInclusive = insertIndex + 1;
//                        else
//                            maxInclusive = insertIndex - 1;
//                    }
//                    Insert(ref current, minInclusive);
//                }
//            }
//
//            private void Insert(ref TSource current, long insertIndex)
//            {
//                if (Count == capacity)
//                {
//                    ReAllocAndInsert(ref current, insertIndex);
//                    return;
//                }
//                if (insertIndex == 0)
//                    UnsafeUtilityEx.MemMove(Ptr + 1, Ptr, Count);
//                else if (insertIndex != Count)
//                    UnsafeUtilityEx.MemMove(Ptr + insertIndex + 1, Ptr + insertIndex, Count - insertIndex);
//                Ptr[insertIndex] = current;
//                ++Count;
//            }
//
//            private void ReAllocAndInsert(ref TSource current, long insertIndex)
//            {
//                capacity += capacity >> 1;
//                var tmp = UnsafeUtilityEx.Malloc<TSource>(capacity, allocator);
//                if (insertIndex == 0)
//                {
//                    UnsafeUtilityEx.MemCpy(tmp + 1, Ptr, Count);
//                }
//                else if (insertIndex == Count)
//                {
//                    UnsafeUtilityEx.MemCpy(tmp, Ptr, Count);
//                }
//                else
//                {
//                    UnsafeUtilityEx.MemCpy(tmp, Ptr, insertIndex);
//                    UnsafeUtilityEx.MemCpy(tmp + insertIndex + 1, Ptr + insertIndex, Count - insertIndex);
//                }
//                tmp[insertIndex] = current;
//                UnsafeUtility.Free(Ptr, allocator);
//                Ptr = tmp;
//                ++Count;
//            }
//
//            public bool MoveNext() => ++index < Count;
//
//            public void Reset() => index = -1;
//
//            public ref TSource Current => ref Ptr[index];
//
//            TSource IEnumerator<TSource>.Current => Current;
//
//            object IEnumerator.Current => Current;
//
//            public void Dispose()
//            {
//                if (Ptr != null)
//                    UnsafeUtility.Free(Ptr, allocator);
//                this = default;
//            }
//        }
//
//
//        public TEnumerable0 CreateRefOrderedEnumerable<TEnumerable0, TEnumerator0, TKey, TKeySelector, TComparer1>(TKeySelector keySelector, TComparer1 comparer)
//            where TEnumerable0 : struct, IRefOrderedEnumerable<TEnumerator0, TKey>
//            where TEnumerator0 : struct, IRefEnumerator<TKey>
//            where TKey : unmanaged
//#if STRICT_EQUALITY
//            , IEquatable<TKey>
//
//            where TKeySelector : struct, IRefAction<TSource, TKey>
//            where TComparer1 : struct, IRefFunc<TKey, TKey, int>
//        {
//            throw new NotImplementedException();
//        }
//
//        public IOrderedEnumerable<TSource> CreateOrderedEnumerable<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer, bool @descending)
//        {
//            
//        }
//
//        public Enumerator GetEnumerator() => new Enumerator(ref enumerable, orderComparer, alloc);
//        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
//
//        #region Enumerable
//        public
//            AppendEnumerable<
//                OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>,
//                Enumerator,
//                TSource
//            >
//            Append(TSource value, Allocator allocator = Allocator.Temp)
//            => new AppendEnumerable<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(this, value, allocator);
//
//        public
//            OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>
//            AsRefEnumerable()
//            => this;
//
//        public
//            DefaultIfEmptyEnumerable<
//                OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>,
//                Enumerator,
//                TSource
//            >
//            DefaultIfEmpty(TSource defaultValue, Allocator allocator = Allocator.Temp)
//            => new DefaultIfEmptyEnumerable<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(this, defaultValue, allocator);
//
//        public
//            DistinctEnumerable<
//                OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>,
//                Enumerator,
//                TSource,
//                TEqualityComparer0,
//                TGetHashCodeFunc0
//            >
//            Distinct<TEqualityComparer0, TGetHashCodeFunc0>(TEqualityComparer0 comparer, TGetHashCodeFunc0 getHashCodeFunc, Allocator allocator = Allocator.Temp)
//            where TEqualityComparer0 : struct, IRefFunc<TSource, TSource, bool>
//            where TGetHashCodeFunc0 : struct, IRefFunc<TSource, int>
//            => new DistinctEnumerable<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TEqualityComparer0, TGetHashCodeFunc0>(this, comparer, getHashCodeFunc, allocator);
//
//        public
//            DistinctEnumerable<
//                OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>,
//                Enumerator,
//                TSource,
//                DefaultEqualityComparer<TSource>,
//                DefaultGetHashCodeFunc<TSource>
//            >
//            Distinct(Allocator allocator = Allocator.Temp)
//            => new DistinctEnumerable<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, DefaultEqualityComparer<TSource>, DefaultGetHashCodeFunc<TSource>>(this, default, default, allocator);
//
//        public
//            OrderByEnumerable<
//                OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>,
//                Enumerator,
//                TSource,
//                TComparer0
//            >
//            OrderBy<TComparer0>(TComparer0 comparer, Allocator allocator = Allocator.Temp)
//            where TComparer0 : struct, IRefFunc<TSource, TSource, int>
//            => new OrderByEnumerable<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TComparer0>(this, comparer, allocator);
//        #endregion
//
//        #region Concat
//        public
//            ConcatEnumerable<
//                OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>,
//                Enumerator,
//                TEnumerable0,
//                TEnumerator0,
//                TSource>
//            Concat<TEnumerable0, TEnumerator0>(in TEnumerable0 second)
//            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
//            where TEnumerator0 : struct, IRefEnumerator<TSource>
//            => new ConcatEnumerable<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TEnumerable0, TEnumerator0, TSource>(this, second);
//        #endregion
//
//        #region Function
//        public bool CanFastCount() => enumerable.CanFastCount();
//        public bool Any() => enumerable.Any();
//        public int Count() => enumerable.Count();
//        public long LongCount() => enumerable.LongCount();
//
//        public bool TryGetFirst(out TSource first)
//        {
//            var enumerator = enumerable.GetEnumerator();
//            if (!enumerator.MoveNext())
//            {
//                first = default;
//                enumerator.Dispose();
//                return false;
//            }
//            first = enumerator.Current;
//            while (enumerator.MoveNext())
//            {
//                ref var current = ref enumerator.Current;
//                if (orderComparer.Calc(ref first, ref current) > 0)
//                    first = current;
//            }
//            enumerator.Dispose();
//            return true;
//        }
//
//        public bool TryGetLast(out TSource last)
//        {
//            var enumerator = enumerable.GetEnumerator();
//            if (!enumerator.MoveNext())
//            {
//                last = default;
//                enumerator.Dispose();
//                return false;
//            }
//            last = enumerator.Current;
//            while (enumerator.MoveNext())
//            {
//                ref var current = ref enumerator.Current;
//                if (orderComparer.Calc(ref last, ref current) < 0)
//                    last = current;
//            }
//            enumerator.Dispose();
//            return true;
//        }
//
//        public bool TryGetElementAt(long index, out TSource element)
//            => this.TryGetElementAt<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(index, out element);
//
//        public NativeEnumerable<TSource> ToNativeEnumerable(Allocator allocator)
//        {
//            var enumerator = GetEnumerator();
//            if (allocator == alloc)
//                return new NativeEnumerable<TSource>(enumerator.Ptr, enumerator.Count);
//            var count = enumerator.Count;
//            var answer = UnsafeUtilityEx.Malloc<TSource>(count, allocator);
//            UnsafeUtilityEx.MemCpy(answer, enumerator.Ptr, count);
//            enumerator.Dispose();
//            return new NativeEnumerable<TSource>(answer, count);
//        }
//
//        public NativeArray<TSource> ToNativeArray(Allocator allocator)
//        {
//            var enumerator = GetEnumerator();
//            NativeArray<TSource> answer;
//            if (allocator == alloc)
//            {
//                answer = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TSource>(enumerator.Ptr, (int) (sizeof(TSource) * enumerator.Count), allocator);
//#if ENABLE_UNITY_COLLECTIONS_CHECKS
//                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref answer, AtomicSafetyHandle.Create());
//
//                return answer;
//            }
//            answer = new NativeArray<TSource>((int) enumerator.Count, allocator);
//            UnsafeUtilityEx.MemCpy(answer.GetPointer(), enumerator.Ptr, enumerator.Count);
//            enumerator.Dispose();
//            return answer;
//        }
//
//        public TSource[] ToArray()
//        {
//            var enumerator = GetEnumerator();
//            var answer = new TSource[enumerator.Count];
//            fixed (TSource* dest = &answer[0])
//            {
//                UnsafeUtilityEx.MemCpy(dest, enumerator.Ptr, enumerator.Count);
//            }
//            enumerator.Dispose();
//            return answer;
//        }
//
//        public HashSet<TSource> ToHashSet()
//            => this.ToHashSet<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>();
//
//        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
//            => this.ToHashSet<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(comparer);
//
//        public bool Any<TPredicate>(TPredicate predicate)
//            where TPredicate : unmanaged, IRefFunc<TSource, bool>
//            => this.Any<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TPredicate>(predicate);
//
//        public bool Any(Func<TSource, bool> predicate)
//            => this.Any<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(predicate);
//
//        public bool All<TPredicate>(TPredicate predicate)
//            where TPredicate : unmanaged, IRefFunc<TSource, bool>
//            => this.All<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TPredicate>(predicate);
//
//        public bool All(Func<TSource, bool> predicate)
//            => this.All<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(predicate);
//
//        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
//            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
//            => this.Aggregate<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TAccumulate, TFunc>(ref seed, func);
//
//        public TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
//            where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
//            where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>
//            => this.Aggregate<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);
//
//        public TSource Aggregate(Func<TSource, TSource, TSource> func)
//            => this.Aggregate<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(func);
//
//        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
//            => this.Aggregate<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TAccumulate>(seed, func);
//
//        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
//            => this.Aggregate<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TAccumulate, TResult>(seed, func, resultFunc);
//
//        public bool Contains(TSource value)
//            => this.Contains<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(value);
//
//        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
//            => this.Contains<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(value, comparer);
//
//        public bool Contains<TComparer1>(TSource value, TComparer1 comparer)
//            where TComparer1 : unmanaged, IRefFunc<TSource, TSource, bool>
//            => this.Contains<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TComparer1>(value, comparer);
//
//        public int Count(Func<TSource, bool> predicate)
//            => this.Count<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(predicate);
//
//
//        public int Count<TPredicate>(TPredicate predicate)
//            where TPredicate : unmanaged, IRefFunc<TSource, bool>
//            => this.Count<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TPredicate>(predicate);
//
//        public long LongCount(Func<TSource, bool> predicate)
//            => this.LongCount<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(predicate);
//
//        public long LongCount<TPredicate>(TPredicate predicate)
//            where TPredicate : unmanaged, IRefFunc<TSource, bool>
//            => this.LongCount<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TPredicate>(predicate);
//
//        public bool TryGetSingle(out TSource value)
//            => this.TryGetSingle<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(out value);
//
//        public bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate)
//            where TPredicate : unmanaged, IRefFunc<TSource, bool>
//            => this.TryGetSingle<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TPredicate>(out value, predicate);
//
//        public bool TryGetSingle(out TSource value, Func<TSource, bool> predicate)
//            => this.TryGetSingle<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>(out value, predicate);
//
//        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
//            => this.ToDictionary<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TKey, TElement>(keySelector, elementSelector);
//
//        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
//            where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
//            where TElementFunc : unmanaged, IRefFunc<TSource, TElement>
//            => this.ToDictionary<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource, TKey, TElement, TKeyFunc, TElementFunc>(keySelector, elementSelector);
//
//        public List<TSource> ToList()
//            => this.ToList<OrderByEnumerable<TEnumerable, TEnumerator, TSource, TComparer>, Enumerator, TSource>();
//        #endregion
//    }
//}