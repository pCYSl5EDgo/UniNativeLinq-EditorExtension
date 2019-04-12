using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct DefaultIfEmptyEnumerator<TEnumerator, T> : IRefEnumerator<T>
        where TEnumerator : struct, IRefEnumerator<T>
        where T : unmanaged
    {
        private TEnumerator enumerator;
        private readonly T* ptr;
        private readonly Allocator allocator;
        private bool isFirst;
        private bool isDefault;

        internal DefaultIfEmptyEnumerator(in TEnumerator enumerator, in T value, Allocator allocator)
        {
            this.enumerator = enumerator;
            this.ptr = UnsafeUtilityEx.Malloc<T>(1, allocator);
            *this.ptr = value;
            this.allocator = allocator;
            isFirst = true;
            isDefault = false;
        }
        internal DefaultIfEmptyEnumerator(in TEnumerator enumerator, T* ptr)
        {
            this.enumerator = enumerator;
            this.ptr = ptr;
            this.allocator = Allocator.None;
            isFirst = true;
            isDefault = false;
        }
        public ref T Current
        {
            get
            {
                if (isDefault)
                    return ref *ptr;
                else
                    return ref enumerator.Current;
            }
        }
        T IEnumerator<T>.Current => Current;
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            enumerator.Dispose();
            if (ptr != null && allocator != Allocator.None)
                UnsafeUtility.Free(ptr, allocator);
            this = default;
        }

        public bool MoveNext()
        {
            if (!isFirst) return !isDefault && enumerator.MoveNext();
            isFirst = false;
            isDefault = !enumerator.MoveNext();
            return true;
        }

        public void Reset() => throw new System.InvalidOperationException();
    }
}