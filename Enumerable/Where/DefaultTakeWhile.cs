namespace pcysl5edgo.Collections.LINQ
{
    public struct DefaultTakeWhile<TSource, TPredicate>
        : IRefFunc<TSource, bool>
        where TPredicate : struct, IRefFunc<TSource, bool>
    {
        private bool isTakeEnd;
        private TPredicate predicate;

        public DefaultTakeWhile(TPredicate predicate)
        {
            this.isTakeEnd = false;
            this.predicate = predicate;
        }

        public bool Calc(ref TSource value)
        {
            if (isTakeEnd) return false;
            if (predicate.Calc(ref value)) return true;
            isTakeEnd = true;
            return false;
        }
    }
}