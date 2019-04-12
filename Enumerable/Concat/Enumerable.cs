using System.Collections;
using System.Collections.Generic;

namespace pcysl5edgo.Collections.LINQ
{
    public struct
        ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource> :
            IRefEnumerable<ConcatEnumerator<TFirstEnumerator, TSecondEnumerator, TSource>, TSource>
        where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TSource>
        where TFirstEnumerator : struct, IRefEnumerator<TSource>
        where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSource>
        where TSecondEnumerator : struct, IRefEnumerator<TSource>
        where TSource : unmanaged
    {
        private TFirstEnumerable first;
        private TSecondEnumerable second;

        internal ConcatEnumerable(in TFirstEnumerable first, in TSecondEnumerable second)
        {
            this.first = first;
            this.second = second;
        }

        public ConcatEnumerator<TFirstEnumerator, TSecondEnumerator, TSource> GetEnumerator() =>
            new ConcatEnumerator<TFirstEnumerator, TSecondEnumerator, TSource>(first.GetEnumerator(),
                second.GetEnumerator());

        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}