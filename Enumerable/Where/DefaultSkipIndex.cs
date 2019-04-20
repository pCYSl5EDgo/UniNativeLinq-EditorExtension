namespace pcysl5edgo.Collections.LINQ
{
    public struct DefaultSkipIndex<TSource> : IWhereIndex<TSource>
    {
        internal readonly long Count;
        public DefaultSkipIndex(long count) => this.Count = count;
        public bool Calc(ref TSource value, long index) => index >= Count;
    }
}