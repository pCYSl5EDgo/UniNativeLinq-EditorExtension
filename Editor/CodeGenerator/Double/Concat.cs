using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using UnityEngine;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class Concat : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public Concat(IDoubleApi api)
        {
            Api = api;
        }
        public readonly IDoubleApi Api;
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

                    GenerateEachPair(rowName, isRowSpecial, columnName, isColumnSpecial, @static, mainModule);
                }
            }
        }

        private void GenerateEachPair(string rowName, bool isRowSpecial, string columnName, bool isColumnSpecial, TypeDefinition @static, ModuleDefinition mainModule)
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
                GenerateSpecialSpecial(rowName, columnName, mainModule, method);
            }
            else if (isRowSpecial)
            {
                GenerateSpecialNormal(rowName, Dictionary[columnName], @static, mainModule, method, 0);
            }
            else if (isColumnSpecial)
            {
                GenerateSpecialNormal(columnName, Dictionary[rowName], @static, mainModule, method, 1);
            }
            else
            {
                GenerateNormalNormal(Dictionary[rowName], Dictionary[columnName], @static, mainModule, method);
            }
        }

        private void GenerateSpecialSpecial(string rowName, string columnName, ModuleDefinition mainModule, MethodDefinition method)
        {
            GenericParameter T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(Helper.GetSystemRuntimeInteropServicesUnmanagedTypeConstraintTypeReference());
            method.GenericParameters.Add(T);

            var (baseSpecialTypeReference0, enumerableTypeReference0, enumeratorTypeReference0) = T.MakeSpecialTypePair(rowName);
            var (baseSpecialTypeReference1, enumerableTypeReference1, enumeratorTypeReference1) = T.MakeSpecialTypePair(columnName);

            var @return = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "ConcatEnumerable`5"))
            {
                GenericArguments = {
                    enumerableTypeReference0,
                    enumeratorTypeReference0,
                    enumerableTypeReference1,
                    enumeratorTypeReference1,
                    T
                }
            };
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.None, baseSpecialTypeReference0);
            method.Parameters.Add(thisParam);

            var secondParam = new ParameterDefinition("second", ParameterAttributes.None, baseSpecialTypeReference1);
            method.Parameters.Add(secondParam);

            method.Body.Variables.Add(new VariableDefinition(enumerableTypeReference0));
            method.Body.Variables.Add(new VariableDefinition(enumerableTypeReference1));

            var constructor0 = enumerableTypeReference0.FindMethod(".ctor", x => x.Parameters.Count == 1);
            var constructor1 = enumerableTypeReference1.FindMethod(".ctor", x => x.Parameters.Count == 1);

            method.Body.GetILProcessor()
                .LdLocA(0)
                .Dup()
                .LdArg(0)
                .Call(constructor0)
                .LdLocA(1)
                .Dup()
                .LdArg(1)
                .Call(constructor1)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private void GenerateSpecialNormal(string columnName, TypeDefinition type0, TypeDefinition @static, ModuleDefinition mainModule, MethodDefinition method, int specialIndex)
        {
            Prepare(type0, method, out var T, out var enumerable0, out var enumerator0, out var element0);

            if (!element0.Equals(T))
            {
                Debug.LogWarning(element0.FullName + "  is different from " + nameof(T));
                @static.Methods.Remove(method);
                return;
            }

            var (baseSpecialTypeReference, enumerableTypeReference, enumeratorTypeReference) = T.MakeSpecialTypePair(columnName);

            var types = new[]
            {
                enumerable0,
                enumerator0,
                enumerable0,
                enumerator0,
                T,
            };
            types[specialIndex << 1] = enumerableTypeReference;
            types[(specialIndex << 1) + 1] = enumeratorTypeReference;
            var @return = mainModule.GetType("UniNativeLinq", "ConcatEnumerable`5").MakeGenericInstanceType(types);
            method.ReturnType = @return;

            var paramNormal = new ParameterDefinition("@this", ParameterAttributes.In, enumerable0.MakeByReferenceType());
            var systemRuntimeCompilerServicesReadonlyAttributeTypeReference = Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference();
            paramNormal.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);

            var paramSpecial = new ParameterDefinition("second", ParameterAttributes.None, baseSpecialTypeReference);

            var processor = method.Body.GetILProcessor();

            var constructor = enumerableTypeReference.FindMethod(".ctor", x => x.Parameters.Count == 1);

            method.Body.Variables.Add(new VariableDefinition(enumerableTypeReference));

            if (specialIndex == 0)
            {
                method.Parameters.Add(paramSpecial);
                method.Parameters.Add(paramNormal);

                processor
                    .LdLocA(0)
                    .Dup()
                    .LdArg(specialIndex)
                    .Call(constructor)
                    .LdArg(1);
            }
            else
            {
                method.Parameters.Add(paramNormal);
                method.Parameters.Add(paramSpecial);

                processor
                    .LdLocA(0)
                    .LdArg(specialIndex)
                    .Call(constructor)
                    .LdArg(0)
                    .LdLocA(0);
            }

            processor
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, TypeDefinition @static, ModuleDefinition mainModule, MethodDefinition method)
        {
            Prepare(type0, method, out var T, out var enumerable0, out var enumerator0, out var element0);

            const string suffix1 = "1";

            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, nameof(T), T, suffix1);
            foreach (var parameter in added1)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            var enumerable1 = type1.MakeGenericInstanceType(added1.Append(T));

            var enumerator1 = enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);
            var element1 = enumerable1.GetElementTypeOfCollectionType().Replace(added1, nameof(T), T, suffix1);

            if (!element0.Equals(element1))
            {
                Debug.LogWarning(element0.FullName + "  is different from " + element1.FullName);
                @static.Methods.Remove(method);
                return;
            }

            var @return = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "ConcatEnumerable`5"))
            {
                GenericArguments =
                {
                    enumerable0,
                    enumerator0,
                    enumerable1,
                    enumerator1,
                    T
                }
            };
            method.ReturnType = @return;

            var thisParam = new ParameterDefinition("@this", ParameterAttributes.In, enumerable0.MakeByReferenceType());
            var systemRuntimeCompilerServicesReadonlyAttributeTypeReference = Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference();
            thisParam.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(thisParam);

            var secondParam = new ParameterDefinition("second", ParameterAttributes.In, enumerable1.MakeByReferenceType());
            secondParam.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(secondParam);

            var processor = method.Body.GetILProcessor();
            processor
                .LdArg(0)
                .LdArg(1)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private void Prepare(TypeDefinition type0, MethodDefinition method, out GenericParameter T, out GenericInstanceType enumerable0, out TypeReference enumerator0, out TypeReference element0)
        {
            T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(Helper.GetSystemRuntimeInteropServicesUnmanagedTypeConstraintTypeReference());
            method.GenericParameters.Add(T);

            const string suffix0 = "0";

            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, nameof(T), T, suffix0);
            foreach (var parameter in added0)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }
            enumerable0 = type0.MakeGenericInstanceType(added0.Append(T));

            enumerator0 = enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);
            element0 = enumerable0.GetElementTypeOfCollectionType().Replace(added0, nameof(T), T, suffix0);
        }
    }
}