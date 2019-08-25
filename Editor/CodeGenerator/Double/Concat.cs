using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
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
            DefineT(method, out var T);

            var (baseSpecialTypeReference0, enumerable0, enumerator0) = T.MakeSpecialTypePair(rowName);
            var (baseSpecialTypeReference1, enumerable1, enumerator1) = T.MakeSpecialTypePair(columnName);

            var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T);

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseSpecialTypeReference0));
            method.Parameters.Add(new ParameterDefinition("second", ParameterAttributes.None, baseSpecialTypeReference1));

            method.Body.GetILProcessor()
                .LdConvArg(enumerable0, 0)
                .LdConvArg(enumerable1, 1)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private void GenerateSpecialNormal(string columnName, TypeDefinition type, TypeDefinition @static, ModuleDefinition mainModule, MethodDefinition method, int specialIndex)
        {
            DefineT(method, out var T);

            var body = method.Body;
            if (specialIndex == 0)
            {
                var (specialType, enumerable0, enumerator0) = T.MakeSpecialTypePair(columnName);
                var (enumerable1, enumerator1, _) = T.MakeFromCommonType(method, type, "1");

                var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T);

                method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, specialType));
                method.Parameters.Add(new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1))
                {
                    CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
                });

                body.Variables.Add(new VariableDefinition(enumerable0));

                body.GetILProcessor()
                    .LdConvArg(enumerable0, 0)
                    .LdArg(1)
                    .NewObj(@return.FindMethod(".ctor"))
                    .Ret();
            }
            else
            {
                var (enumerable0, enumerator0, _) = T.MakeFromCommonType(method, type, "0");
                var (specialType, enumerable1, enumerator1) = T.MakeSpecialTypePair(columnName);

                var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T);

                method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0))
                {
                    CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
                });
                method.Parameters.Add(new ParameterDefinition("second", ParameterAttributes.None, specialType));

                body.Variables.Add(new VariableDefinition(enumerable0));

                body.GetILProcessor()
                    .LdArg(0)
                    .LdConvArg(enumerable1, 1)
                    .NewObj(@return.FindMethod(".ctor"))
                    .Ret();
            }
        }

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, TypeDefinition @static, ModuleDefinition mainModule, MethodDefinition method)
        {
            DefineT(method, out var T);

            var (enumerable0, enumerator0, element0) = T.MakeFromCommonType(method, type0, "0");

            var (enumerable1, enumerator1, element1) = T.MakeFromCommonType(method, type1, "1");

            if (!element0.Equals(element1))
            {
                Debug.LogWarning(element0.FullName + "  is different from " + element1.FullName);
                @static.Methods.Remove(method);
                return;
            }

            var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T);

            var systemRuntimeCompilerServicesReadonlyAttributeTypeReference = Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference();

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0))
            {
                CustomAttributes = { systemRuntimeCompilerServicesReadonlyAttributeTypeReference }
            });

            method.Parameters.Add(new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1))
            {
                CustomAttributes = { systemRuntimeCompilerServicesReadonlyAttributeTypeReference }
            });

            method.Body.GetILProcessor()
                .LdArgs(0, 2)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private static TypeReference DefineReturn(ModuleDefinition mainModule, MethodDefinition method, TypeReference enumerable0, TypeReference enumerator0, TypeReference enumerable1, TypeReference enumerator1, GenericParameter T)
        {
            return method.ReturnType = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "ConcatEnumerable`5"))
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
        }

        private static void DefineT(MethodDefinition method, out GenericParameter T)
        {
            T = new GenericParameter(nameof(T), method)
            {
                HasNotNullableValueTypeConstraint = true,
                CustomAttributes = { Helper.GetSystemRuntimeInteropServicesUnmanagedTypeConstraintTypeReference() }
            };
            method.GenericParameters.Add(T);
        }
    }
}