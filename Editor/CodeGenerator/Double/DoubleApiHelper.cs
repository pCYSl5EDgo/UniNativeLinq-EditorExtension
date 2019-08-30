using Mono.Cecil;
using Mono.Cecil.Cil;

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

        public static ILProcessor CpObjFromArgumentToField(this ILProcessor processor, TypeReference type, int variableIndex, FieldReference to)
        {
            return processor
                .LdFldA(to)
                .LdArg(variableIndex)
                .CpObj(type);
        }
    }
}