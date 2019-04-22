namespace pcysl5edgo.Collections.LINQ
{
    public struct OrderByDescending<TSource, TComparer>
        : IRefFunc<TSource, TSource, int>
        where TComparer : struct, IRefFunc<TSource, TSource, int>
    {
        private TComparer comparer;
        public OrderByDescending(TComparer comparer) => this.comparer = comparer;
        public int Calc(ref TSource arg0, ref TSource arg1) => comparer.Calc(ref arg1, ref arg0);
    }
}