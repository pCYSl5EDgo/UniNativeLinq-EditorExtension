using Mono.Cecil;

namespace UniNativeLinq.Editor
{
    public interface IApiExtensionMethodGenerator
    {
        void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule);
    }
}