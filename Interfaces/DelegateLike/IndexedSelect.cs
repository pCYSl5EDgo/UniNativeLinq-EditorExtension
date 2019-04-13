namespace pcysl5edgo.Collections.LINQ
{
    public interface IIndexedSelect<TSource, TResult>
    {
        void Execute(ref TSource source, int index, ref TResult result);
    }
}