using Mono.Cecil;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public static class DoubleApiHelper
    {
        public static bool ShouldDefine(this IDoubleApi api, string[] array)
        {
            foreach (var element0 in array)
            {
                foreach (var element1 in array)
                {
                    if (!api.TryGetEnabled(element0, element1, out var apiEnabled) || !apiEnabled) continue;
                    return true;
                }
            }
            return false;
        }


        public static TypeDefinition DefineStatic(this ModuleDefinition mainModule, string name)
            => new TypeDefinition("UniNativeLinq",
            name,
            Helper.StaticExtensionClassTypeAttributes, mainModule.TypeSystem.Object)
            {
                CustomAttributes = { Helper.ExtensionAttribute }
            };
    }
}