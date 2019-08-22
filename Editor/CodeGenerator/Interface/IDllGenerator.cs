using System;

namespace UniNativeLinq.Editor
{
    public interface IDllGenerator : IDisposable
    {
        void Execute(IEnumerableCollectionProcessor processor, IApiExtensionMethodGenerator[] generators, IDependency[] dependencies);
    }
}
