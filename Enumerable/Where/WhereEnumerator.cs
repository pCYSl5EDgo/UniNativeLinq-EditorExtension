using System.Collections;
using System.Collections.Generic;

namespace pcysl5edgo.Collections.LINQ
{
    public struct WhereEnumerator<TPrevEnumerator, TSource, TPredicate> : IRefEnumerator<TSource>
        where TSource : unmanaged
        where  TPrevEnumerator : struct, IRefEnumerator<TSource>
        where  TPredicate : struct, IRefFunc<TSource, bool>
    {
        private TPrevEnumerator enumerator;
        private TPredicate predicate;

        internal WhereEnumerator(in TPrevEnumerator enumerator, TPredicate predicate)
        {
            this.enumerator = enumerator;
            this.predicate = predicate;
        }
        
        public bool MoveNext()
        {
            while (enumerator.MoveNext())
                if (predicate.Calc(ref enumerator.Current))
                    return true;
            return false;
        }

        public void Reset() => throw new System.InvalidOperationException();

        public ref TSource Current => ref enumerator.Current;

        TSource IEnumerator<TSource>.Current => Current;

        object IEnumerator.Current => Current;

        public void Dispose() => enumerator.Dispose();
    }
}