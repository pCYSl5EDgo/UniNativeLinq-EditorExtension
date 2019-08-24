using System.Linq;
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

        public static (TypeReference element, TypeReference enumerable, TypeReference enumerator) MakeGenericInstanceVariant(this TypeDefinition type, string suffix, MethodDefinition method)
        {
            var added0 = method.FromTypeToMethodParam(type.GenericParameters, suffix);
            var enumerable = type.MakeGenericInstanceType(added0);
            var enumerator = enumerable.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix);
            var element = enumerable.GetElementTypeOfCollectionType().Replace(added0, suffix);
            return (element, enumerable, enumerator);
        }

        public static (TypeReference enumerable, TypeReference enumerator, TypeReference element) MakeFromCommonType(this GenericParameter T, MethodDefinition method, TypeReference type, string suffix)
        {
            var added0 = method.FromTypeToMethodParam(type.GenericParameters, nameof(T), T, suffix);
            foreach (var parameter in added0)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            var enumerable = type.MakeGenericInstanceType(added0.Append(T));
            var enumerator = enumerable.GetEnumeratorTypeOfCollectionType().Replace(added0, nameof(T), T, suffix);
            var element = enumerable.GetElementTypeOfCollectionType().Replace(added0, nameof(T), T, suffix);

            return (enumerable, enumerator, element);
        }
    }
}