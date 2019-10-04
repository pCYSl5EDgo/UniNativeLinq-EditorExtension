using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class DefaultIfEmptyNone : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public DefaultIfEmptyNone(ISingleApi api)
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
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var returnTypeDefinition = mainModule.GetType("UniNativeLinq", Api.Name + "Enumerable`3");

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, T }
                };
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, enumerable, T);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, T);
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
                    GenericArguments = { enumerable, enumerator, T }
                };
                GenerateNormal(method, enumerable, T);
            }
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));

            var body = method.Body;
            body.InitLocals = true;
            var variables = body.Variables;
            variables.Add(new VariableDefinition(enumerable));
            variables.Add(new VariableDefinition(T));

            body.GetILProcessor()
                .ArgumentNullCheck(0, Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]))
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LdLocA(0)
                .LdLocA(1)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));

            var body = method.Body;
            body.InitLocals = true;
            var variables = body.Variables;
            variables.Add(new VariableDefinition(enumerable));
            variables.Add(new VariableDefinition(T));

            body.GetILProcessor()
                .LdLocA(0)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LdLocA(0)
                .LdLocA(1)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });

            var body = method.Body;
            body.InitLocals = true;
            var variables = body.Variables;
            variables.Add(new VariableDefinition(T));

            method.Body.GetILProcessor()
                .LdArg(0)
                .LdLocA(0)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }
    }
}