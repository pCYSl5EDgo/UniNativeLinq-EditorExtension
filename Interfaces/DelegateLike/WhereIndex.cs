namespace pcysl5edgo.Collections.LINQ
{
    public interface IWhereIndex<TSource>
    {
        bool Calc(ref TSource value, long index);
    }
}