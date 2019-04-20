namespace pcysl5edgo.Collections.LINQ
{
    public struct DefaultTakeIndex<TSource> : IWhereIndex<TSource>
    {
        internal readonly long Count;
        public DefaultTakeIndex(long count) => this.Count = count;
        public bool Calc(ref TSource value, long index) => index < Count;
    }
}