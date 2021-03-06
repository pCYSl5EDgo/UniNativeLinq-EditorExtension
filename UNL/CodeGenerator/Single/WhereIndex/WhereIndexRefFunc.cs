﻿using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class WhereIndexRefFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public WhereIndexRefFunc(ISingleApi api)
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

            var method = new MethodDefinition("Where", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var func = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefWhereIndex`1"))
            {
                GenericArguments = { T }
            };

            var TPredicate = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateRefFuncToWhereIndexStructOperator`1"))
            {
                GenericArguments = { T }
            };

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, T, TPredicate }
                };
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, enumerable, TPredicate, func);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, TPredicate, func);
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
                    GenericArguments = { enumerable, enumerator, T, TPredicate }
                };
                GenerateNormal(method, enumerable, TPredicate, func);
            }
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, GenericInstanceType predicate, GenericInstanceType func)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("predicate", ParameterAttributes.None, func));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(enumerable));
            body.Variables.Add(new VariableDefinition(predicate));

            body.GetILProcessor()
                .ArgumentNullCheck(0, Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]))
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LoadFuncArgumentAndStoreToLocalVariableField(1, 1)

                .LdLocA(0)
                .LdLocA(1)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, GenericInstanceType predicate, GenericInstanceType func)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("predicate", ParameterAttributes.None, func));

            var body = method.Body;

            body.Variables.Add(new VariableDefinition(predicate));

            body.GetILProcessor()

                .LoadFuncArgumentAndStoreToLocalVariableField(1, 0)

                .LdArg(0)
                .LdLocA(0)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, GenericInstanceType predicate, GenericInstanceType func)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("predicate", ParameterAttributes.None, func));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(enumerable));
            body.Variables.Add(new VariableDefinition(predicate));

            body.GetILProcessor()
                .LdLocA(0)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LoadFuncArgumentAndStoreToLocalVariableField(1, 1)

                .LdLocA(0)
                .LdLocA(1)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }
    }
}