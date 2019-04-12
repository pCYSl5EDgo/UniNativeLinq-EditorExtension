using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct AppendEnumerator<TPrevEnumerator, TSource> : IRefEnumerator<TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TSource : unmanaged
    {
        private TPrevEnumerator enumerator;
        private readonly TSource* element;
        private readonly Allocator allocator;
        private bool isCurrentEnumerator;
        private bool hasNotAppendRead;

        internal AppendEnumerator(in TPrevEnumerator enumerator, in TSource element, Allocator allocator)
        {
            this.allocator = allocator;
            this.element = UnsafeUtilityEx.Malloc<TSource>(1, allocator);
            *this.element = element;
            this.enumerator = enumerator;
            isCurrentEnumerator = true;
            hasNotAppendRead = true;
        }

        internal AppendEnumerator(in TPrevEnumerator enumerator, TSource* element)
        {
            this.allocator = Allocator.None;
            this.element = element;
            this.enumerator = enumerator;
            isCurrentEnumerator = true;
            hasNotAppendRead = true;
        }

        public ref TSource Current
        {
            get
            {
                if (isCurrentEnumerator)
                    return ref enumerator.Current;
                else
                    return ref *element;
            }
        }
        TSource IEnumerator<TSource>.Current => Current;
        object IEnumerator.Current => Current;

        public void Dispose()
        {
            enumerator.Dispose();
            if (element != null && allocator != Allocator.None)
                UnsafeUtility.Free(element, allocator);
            this = default;
        }

        public bool MoveNext()
        {
            if (isCurrentEnumerator)
            {
                if (!enumerator.MoveNext())
                    isCurrentEnumerator = false;
                return true;
            }
            if (!hasNotAppendRead) return false;
            hasNotAppendRead = false;
            return true;
        }

        public void Reset() => throw new InvalidOperationException();
    }
}