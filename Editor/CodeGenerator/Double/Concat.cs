using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using UnityEngine;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public class Concat : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public Concat(IDoubleApi api)
        {
            Api = api;
        }
        public IDoubleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            if (!processor.TryGetEnabled("Concat", out var enabled) || !enabled) return;
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic("ConcatHelper"));
            var count = Api.Count;
            for (var row = 0; row < count; row++)
            {
                var rowName = Api.NameCollection[row];
                if (!processor.IsSpecialType(rowName, out var isRowSpecial)) throw new KeyNotFoundException();

                for (var column = 0; column < count; column++)
                {
                    var columnName = Api.NameCollection[column];
                    if (!processor.IsSpecialType(columnName, out var isColumnSpecial)) throw new KeyNotFoundException();

                    if (!Api.TryGetEnabled(rowName, columnName, out var apiEnabled) || !apiEnabled) continue;

                    GenerateEachPair(rowName, isRowSpecial, columnName, isColumnSpecial, @static, mainModule, systemModule);
                }
            }
        }

        private void GenerateEachPair(string rowName, bool isRowSpecial, string columnName, bool isColumnSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition(nameof(Concat), Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(Helper.ExtensionAttribute);
            @static.Methods.Add(method);
            if (isRowSpecial && isColumnSpecial)
            {
                //GenerateSpecialSpecial(rowName, columnName, @static, mainModule, method);
            }
            else if (isRowSpecial)
            {
                //GenerateSpecialNormal(rowName, Dictionary[columnName], @static, mainModule, method, specialIndex 0);
            }
            else if (isColumnSpecial)
            {
                //GenerateSpecialNormal(columnName, Dictionary[rowName], @static, mainModule, method, 1);
            }
            else
            {
                GenerateNormalNormal(Dictionary[rowName], Dictionary[columnName], @static, mainModule, method, systemModule);
            }
        }

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, TypeDefinition @static, ModuleDefinition mainModule, MethodDefinition method, ModuleDefinition systemModule)
        {
            Prepare(type0, mainModule, method, out GenericParameter T, out GenericInstanceType Enumerable0, out TypeReference Enumerator0, out TypeReference Element0);

            const string suffix1 = "1";

            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, nameof(T), T, suffix1);
            foreach (var parameter in added1)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            var Enumerable1 = type1.MakeGenericInstanceType(added1.Append(T));

            var Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);
            var Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);

            if (!Element0.Equals(Element1))
            {
                Debug.LogWarning(Element0.FullName + "  is different from " + Element1.FullName);
                @static.Methods.Remove(method);
                return;
            }

            var @return = mainModule.GetType("UniNativeLinq", "ConcatEnumerable`5").MakeGenericInstanceType(new[]
            {
                Enumerable0,
                Enumerator0,
                Enumerable1,
                Enumerator1,
                T
            });
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, Enumerable0.MakeByReferenceType());
            var systemRuntimeCompilerServicesReadonlyAttributeTypeReference = mainModule.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference();
            thisParam.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(thisParam);

            var secondParam = new ParameterDefinition("second", ParameterAttributes.In, Enumerable1.MakeByReferenceType());
            secondParam.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(secondParam);

            var processor = method.Body.GetILProcessor();
            processor
                .LdArg(0)
                .LdArg(1)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private void Prepare(TypeDefinition type0, ModuleDefinition mainModule, MethodDefinition method, out GenericParameter T, out GenericInstanceType Enumerable0, out TypeReference Enumerator0, out TypeReference Element0)
        {
            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(mainModule.GetSystemRuntimeInteropServicesUnmanagedTypeConstraintTypeReference());
            method.GenericParameters.Add(T);

            const string suffix0 = "0";

            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, nameof(T), T, suffix0);
            foreach (var parameter in added0)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            Enumerable0 = type0.MakeGenericInstanceType(added0.Append(T));

            Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);
            Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);
        }
    }
}