using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class OrderByFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public OrderByFunc(ISingleApi api)
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
            var returnTypeDefinition = mainModule.GetType("UniNativeLinq", Api.Name + "Enumerable`4");

            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var Func = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`3")))
            {
                GenericArguments = { T, T, mainModule.TypeSystem.Int32 }
            };

            var TComparer = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateFuncToStructOperatorFunc`3"))
            {
                GenericArguments = { T, T, mainModule.TypeSystem.Int32 }
            };

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, T, TComparer }
                };
                switch (name)
                {
                    case "T[]":
                    case "NativeArray<T>":
                        GenerateSpecial(method, baseEnumerable, enumerable, TComparer, Func);
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
                    GenericArguments = { enumerable, enumerator, T, TComparer }
                };
                GenerateNormal(method, enumerable, TComparer, Func);
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference comparer, TypeReference func)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.None, func));
            method.DefineAllocatorParam();

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(comparer));

            body.GetILProcessor()
                .LdArg(1)
                .StLoc(0)

                .LdArg(0)
                .LdLocA(0)
                .LdArg(2)
                .NewObj(method.ReturnType.FindMethod(".ctor", 3))
                .Ret();
        }

        private static void GenerateSpecial(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, TypeReference comparer, TypeReference func)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.None, func));
            method.DefineAllocatorParam();

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(enumerable));
            body.Variables.Add(new VariableDefinition(comparer));

            body.GetILProcessor()
                .LdLocA(0)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LdArg(1)
                .StLoc(1)

                .LdLocA(0)
                .LdLocA(1)
                .LdArg(2)
                .NewObj(method.ReturnType.FindMethod(".ctor", 3))
                .Ret();
        }
    }
}