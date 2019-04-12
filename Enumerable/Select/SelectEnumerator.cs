using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct SelectEnumerator<TPrevEnumerator, TSource, TResult, TAction> : IRefEnumerator<TResult>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TSource : unmanaged
        where TResult : unmanaged
        where TAction : struct, IRefAction<TSource, TResult>
    {
        private TPrevEnumerator enumerator;
        private TResult* current;
        private Allocator allocator;
        private TAction action;

        internal SelectEnumerator(in TPrevEnumerator enumerator, TAction action, Allocator allocator)
        {
            this.enumerator = enumerator;
            this.current = UnsafeUtilityEx.Malloc<TResult>(1, allocator);
            this.allocator = allocator;
            this.action = action;
        }

        public ref TResult Current => ref *current;
        TResult IEnumerator<TResult>.Current => Current;
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            enumerator.Dispose();
            if (UnsafeUtility.IsValidAllocator(allocator) && current != null)
                UnsafeUtility.Free(current, allocator);
            this = default;
        }

        public bool MoveNext()
        {
            if (!enumerator.MoveNext()) return false;
            action.Execute(ref enumerator.Current, ref *current);
            return true;
        }

        public void Reset() => throw new System.InvalidOperationException();
    }
}