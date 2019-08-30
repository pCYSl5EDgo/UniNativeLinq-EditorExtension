using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class SelectOperator : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public SelectOperator(ISingleApi api)
        {
            Api = api;
        }
        public readonly ISingleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.GenerateSingleWithEnumerable(processor, mainModule, systemModule, unityModule, GenerateEach);
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var returnTypeDefinition = mainModule.GetType("UniNativeLinq", "SelectEnumerable`5");

            var method = new MethodDefinition("Select", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);


            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var TResult = method.DefineUnmanagedGenericParameter("TResult");
            method.GenericParameters.Add(TResult);

            var IRefAction = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefAction`2"))
            {
                GenericArguments = { T, TResult }
            };
            var TSelector = new GenericParameter("TSelector", method)
            {
                HasNotNullableValueTypeConstraint = true,
                Constraints = { IRefAction }
            };
            method.GenericParameters.Add(TSelector);

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, T, TResult, TSelector }
                };
                switch (name)
                {
                    case "T[]":
                    case "NativeArray<T>":
                        GenerateSpecial(method, baseEnumerable, enumerable, TSelector);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var type = Dictionary[name];
                var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, T, TResult, TSelector }
                };
                GenerateNormal(method, enumerable, TSelector);
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, GenericParameter selector)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("selector", ParameterAttributes.In, new ByReferenceType(selector))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });

            method.Body.GetILProcessor()
                .LdArgs(0, 2)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }

        private static void GenerateSpecial(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, GenericParameter selector)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("selector", ParameterAttributes.In, new ByReferenceType(selector))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(enumerable));

            body.GetILProcessor()
                .LdLocA(0)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LdLocA(0)
                .LdArg(1)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }
    }
}