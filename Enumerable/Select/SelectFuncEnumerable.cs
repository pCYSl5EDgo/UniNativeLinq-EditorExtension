using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct
        SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>
        : IRefEnumerable<SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>.Enumerator, TSource>
        where TPrevSource : unmanaged
        where TSource : unmanaged
        where TEnumerator : struct, IRefEnumerator<TPrevSource>
        where TEnumerable : struct, IRefEnumerable<TEnumerator, TPrevSource>
    {
        private TEnumerable enumerable;
        private readonly Func<TPrevSource, TSource> func;
        private readonly Func<TPrevSource, long, TSource> funcIndex;
        private readonly Allocator alloc;

        public SelectFuncEnumerable(in TEnumerable enumerable, Func<TPrevSource, TSource> func, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.func = func;
            this.funcIndex = null;
            this.alloc = allocator;
        }

        public SelectFuncEnumerable(in TEnumerable enumerable, Func<TPrevSource, long, TSource> funcIndex, Allocator allocator)
        {
            this.enumerable = enumerable;
            this.func = null;
            this.funcIndex = funcIndex;
            this.alloc = allocator;
        }

        public struct Enumerator : IRefEnumerator<TSource>
        {
            private TEnumerator enumerator;
            private readonly Func<TPrevSource, TSource> func;
            private readonly Func<TPrevSource, long, TSource> funcIndex;
            private readonly TSource* current;
            private readonly Allocator allocator;
            private long index;

            public Enumerator(in TEnumerator enumerator, Func<TPrevSource, TSource> func, Func<TPrevSource, long, TSource> funcIndex, Allocator allocator)
            {
                this.enumerator = enumerator;
                this.func = func;
                this.funcIndex = funcIndex;
                this.current = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
                this.allocator = allocator;
                this.index = -1;
            }

            public bool MoveNext()
            {
                if (!enumerator.MoveNext())
                    return false;
                ++index;
                if (func is null)
                {
                    if (funcIndex is null) return false;
                    *current = funcIndex(enumerator.Current, index);
                }
                else
                {
                    *current = func(enumerator.Current);
                }
                return true;
            }

            public void Reset() => throw new InvalidOperationException();

            public ref TSource Current => ref *current;

            TSource IEnumerator<TSource>.Current => Current;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                enumerator.Dispose();
                if (current != null)
                    UnsafeUtility.Free(current, allocator);
                this = default;
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(enumerable.GetEnumerator(), func, funcIndex, alloc);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();

        public bool CanFastCount() => enumerable.CanFastCount();

        public bool Any() => enumerable.Any();

        public int Count() => enumerable.Count();

        public long LongCount() => enumerable.LongCount();

        public bool TryGetFirst(out TSource first)
            => this.TryGetFirst<SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>, Enumerator, TSource>(out first);

        public bool TryGetLast(out TSource last)
            => this.TryGetLast<SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>, Enumerator, TSource>(out last);

        public bool TryGetElementAt(long index, out TSource element)
            => this.TryGetElementAt<SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>, Enumerator, TSource>(index, out element);

        public NativeEnumerable<TSource> ToNativeEnumerable(Allocator allocator)
            => this.ToNativeEnumerable<SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>, Enumerator, TSource>(allocator);

        public NativeArray<TSource> ToNativeArray(Allocator allocator)
            => this.ToNativeArray<SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>, Enumerator, TSource>(allocator);

        public TSource[] ToArray()
            => this.ToArray<SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>, Enumerator, TSource>();

        public HashSet<TSource> ToHashSet()
            => this.ToHashSet<SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>, Enumerator, TSource>();

        public HashSet<TSource> ToHashSet(IEqualityComparer<TSource> comparer)
            => this.ToHashSet<SelectFuncEnumerable<TEnumerable, TEnumerator, TPrevSource, TSource>, Enumerator, TSource>(comparer);
    }
}