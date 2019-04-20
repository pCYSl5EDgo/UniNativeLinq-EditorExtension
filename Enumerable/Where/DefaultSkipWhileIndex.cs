namespace pcysl5edgo.Collections.LINQ
{
    public struct DefaultSkipWhileIndex<TSource, TPredicate>
        : IWhereIndex<TSource>
        where TPredicate : struct, IWhereIndex<TSource>
    {
        private bool isSkipEnd;
        private TPredicate predicate;

        public DefaultSkipWhileIndex(TPredicate predicate)
        {
            this.isSkipEnd = false;
            this.predicate = predicate;
        }

        public bool Calc(ref TSource value, long index)
        {
            if (isSkipEnd) return true;
            if (!predicate.Calc(ref value, index)) return false;
            return isSkipEnd = true;
        }
    }
}