namespace pcysl5edgo.Collections.LINQ
{
    public interface ISelectIndex<TSource, TResult>
    {
        void Execute(ref TSource source, long index, ref TResult result);
    }
}