namespace pcysl5edgo.Collections.LINQ
{
    public interface ISelectIndex<TSource, TResult>
    {
        void Execute(ref TSource source, int index, ref TResult result);
    }
}