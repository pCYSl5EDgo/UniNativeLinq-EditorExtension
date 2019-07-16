using System;

namespace UniNativeLinq.Editor
{
    public interface IDllGenerator : IDisposable
    {
        void Execute(IEnumerableCollectionProcessor processor, ISingleApi[] singleApis, IDoubleApi[] doubleApis, IDependency[] dependencies);
    }
}
