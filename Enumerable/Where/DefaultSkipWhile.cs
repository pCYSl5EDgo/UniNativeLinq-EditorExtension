namespace pcysl5edgo.Collections.LINQ
{
    public struct DefaultSkipWhile<TSource, TPredicate>
        : IRefFunc<TSource, bool>
        where TPredicate : struct, IRefFunc<TSource, bool>
    {
        private bool isSkipEnd;
        private TPredicate predicate;

        public DefaultSkipWhile(TPredicate predicate)
        {
            this.isSkipEnd = false;
            this.predicate = predicate;
        }

        public bool Calc(ref TSource value)
        {
            if (isSkipEnd) return true;
            if (!predicate.Calc(ref value)) return false;
            return isSkipEnd = true;
        }
    }
}