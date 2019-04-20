namespace LINQ2NativeArray.Interfaces.DelegateLike
{
    public interface IWhereIndex<TSource>
    {
        bool Calc(ref TSource value, long index);
    }
}