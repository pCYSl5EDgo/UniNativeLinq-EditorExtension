namespace pcysl5edgo.Collections.LINQ
{
    public struct CompoundOrderBy<TSource, TComparer0, TComparer1>
        : IRefFunc<TSource, TSource, int>
        where TComparer0 : struct, IRefFunc<TSource, TSource, int>
        where TComparer1 : struct, IRefFunc<TSource, TSource, int>
    {
        private TComparer0 comparer0;
        private TComparer1 comparer1;

        public CompoundOrderBy(in TComparer0 comparer0, in TComparer1 comparer1)
        {
            this.comparer0 = comparer0;
            this.comparer1 = comparer1;
        }

        public int Calc(ref TSource arg0, ref TSource arg1)
        {
            var comp = comparer0.Calc(ref arg0, ref arg1);
            return comp == 0 ? comparer1.Calc(ref arg0, ref arg1) : comp;
        }
    }
}