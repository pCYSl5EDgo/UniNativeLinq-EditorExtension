namespace pcysl5edgo.Collections.LINQ
{
    public struct DefaultTakeWhileIndex<TSource, TPredicate>
        : IWhereIndex<TSource>
        where TPredicate : struct, IWhereIndex<TSource>
    {
        private bool isTakeEnd;
        private TPredicate predicate;

        public DefaultTakeWhileIndex(TPredicate predicate)
        {
            this.isTakeEnd = false;
            this.predicate = predicate;
        }

        public bool Calc(ref TSource value, long index)
        {
            if (isTakeEnd) return false;
            if (predicate.Calc(ref value, index)) return true;
            isTakeEnd = true;
            return false;
        }
    }
}