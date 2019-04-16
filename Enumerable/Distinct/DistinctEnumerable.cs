using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct DistinctEnumerable<TEnumerable, TEnumerator, TSource, TEqualityComparer, TGetHashCodeFunc>
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
        private TEqualityComparer comparer;
        private TGetHashCodeFunc getHashCodeFunc;
        private Allocator alloc;

        internal DistinctEnumerable(in TEnumerable enumerable, TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.comparer = comparer;
            this.getHashCodeFunc = getHashCodeFunc;
            this.alloc = allocator;
        }

        public struct Enumerator
        {
            private readonly TSource* ptr;
            private readonly long count;
            private readonly Allocator alloc;
            private long index;

            internal Enumerator(TEnumerator enumerator, TEqualityComparer comparer, TGetHashCodeFunc getHashCodeFunc, Allocator allocator)
            {
                if (!enumerator.MoveNext())
                {
                    enumerator.Dispose();
                    this = default;
                    return;
                }
                index = -1;
                alloc = allocator;
                count = 1L;
                var capacity = 16L;
                var hashCodes = UnsafeUtilityEx.Malloc<int>(capacity, Allocator.Temp);
                ptr = UnsafeUtilityEx.Malloc<TSource>(capacity, alloc);
                ref var current = ref enumerator.Current;
                *ptr = current;
                *hashCodes = getHashCodeFunc.Calc(ref current);
                while (enumerator.MoveNext())
                {
                    if (count == capacity)
                    {
                        UnsafeUtilityEx.ReAlloc(ref hashCodes, count, count << 1, Allocator.Temp);
                        UnsafeUtilityEx.ReAlloc(ref ptr, ref capacity, allocator);
                    }
                    current = ref enumerator.Current;
                    var hash = getHashCodeFunc.Calc(ref current);
                    Insert(ref current, hash, hashCodes, ptr, ref count, ref comparer);
                }
                enumerator.Dispose();
                UnsafeUtility.Free(hashCodes, Allocator.Temp);
            }

            private static void Insert(ref TSource current, int hash, int* codes, TSource* source, ref long count, ref TEqualityComparer comparer)
            {
                if (hash < codes[0])
                {
                    UnsafeUtilityEx.MemMove(source + 1, source, count);
                    source[0] = current;
                    codes[0] = hash;
                    count++;
                    return;
                }
                if (hash > codes[count - 1])
                {
                    source[count] = current;
                    codes[count] = hash;
                    count++;
                    return;
                }
                var minIndexInclusive = 0;
                var maxIndexInclusive = count - 1;
                var index = (minIndexInclusive + maxIndexInclusive) >> 1;
            }
        }
    }
}