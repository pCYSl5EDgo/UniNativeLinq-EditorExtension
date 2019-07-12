namespace UniNativeLinq.Editor
{
    public interface IDllGenerator
    {
        void Execute(IEnumerableCollectionProcessor processor, ISingleApi[] singleApis, IDoubleApi[] doubleApis);
    }
}
