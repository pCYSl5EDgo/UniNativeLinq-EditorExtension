using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class SelectFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public SelectFunc(ISingleApi api)
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
            var returnTypeDefinition = mainModule.GetType("UniNativeLinq", Api.Name + "Enumerable`5");

            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var TPrev = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(TPrev);

            var T = method.DefineUnmanagedGenericParameter("T");
            method.GenericParameters.Add(T);

            var func = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { TPrev, T }
            };

            var TSelector = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateFuncToStructOperatorAction`2"))
            {
                GenericArguments = { TPrev, T }
            };

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = TPrev.MakeSpecialTypePair(name);
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, TPrev, T, TSelector }
                };
                switch (name)
                {
                    case "T[]":
                    case "NativeArray<T>":
                        GenerateSpecial(method, baseEnumerable, enumerable, TSelector, func);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var type = Dictionary[name];
                var (enumerable, enumerator, _) = TPrev.MakeFromCommonType(method, type, "0");
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, TPrev, T, TSelector }
                };
                GenerateNormal(method, enumerable, TSelector, func);
            }
        }

        private void GenerateNormal(MethodDefinition method, TypeReference enumerable, GenericInstanceType selector, GenericInstanceType func)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("selector", ParameterAttributes.None, func));

            var body = method.Body;

            body.Variables.Add(new VariableDefinition(selector));

            body.GetILProcessor()
                .LdArg(0)
                .LdArg(1)
                .StLoc(0)
                .LdLocA(0)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }

        private void GenerateSpecial(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, GenericInstanceType selector, GenericInstanceType func)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("selector", ParameterAttributes.None, func));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(enumerable));
            body.Variables.Add(new VariableDefinition(selector));

            body.GetILProcessor()
                .LdLocA(0)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LdArg(1)
                .StLoc(1)

                .LdLocA(0)
                .LdLocA(1)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }
    }
}