using System.Collections;
using System.Collections.Generic;

namespace pcysl5edgo.Collections.LINQ
{
    public struct ConcatEnumerator<TFirstEnumerator, TSecondEnumerator, TSource> : IRefEnumerator<TSource>
        where TSource : unmanaged
        where TFirstEnumerator : struct, IRefEnumerator<TSource>
        where TSecondEnumerator : struct, IRefEnumerator<TSource>
    {
        private TFirstEnumerator first;
        private TSecondEnumerator second;
        private bool isCurrentSecond;

        public ref TSource Current => ref (isCurrentSecond ? ref second.Current : ref first.Current);
        TSource IEnumerator<TSource>.Current => Current;
        object IEnumerator.Current => Current;

        internal ConcatEnumerator(in TFirstEnumerator first, in TSecondEnumerator second)
        {
            this.first = first;
            this.second = second;
            isCurrentSecond = false;
        }

        public void Dispose()
        {
            first.Dispose();
            second.Dispose();
            this = default;
        }

        public bool MoveNext()
        {
            if (!isCurrentSecond)
            {
                if (first.MoveNext())
                    return true;
                else isCurrentSecond = true;
            }

            return second.MoveNext();
        }

        public void Reset()
        {
            first.Reset();
            second.Reset();
            isCurrentSecond = false;
        }
    }
}