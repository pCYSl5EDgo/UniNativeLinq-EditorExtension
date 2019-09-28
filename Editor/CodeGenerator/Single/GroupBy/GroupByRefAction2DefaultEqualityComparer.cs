using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class GroupByRefAction2DefaultEqualityComparer : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public GroupByRefAction2DefaultEqualityComparer(ISingleApi api)
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
            var returnTypeDefinition = mainModule.GetType("UniNativeLinq", Api.Name + "Enumerable`8");

            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var TKey = method.DefineUnmanagedGenericParameter("TKey");
            method.GenericParameters.Add(TKey);
            TKey.Constraints.Add(new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "IEquatable`1")))
            {
                GenericArguments = { TKey }
            });

            var KeyFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefAction`2"))
            {
                GenericArguments = { T, TKey }
            };

            var TKeyFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateRefActionToStructOperatorAction`2"))
            {
                GenericArguments = { T, TKey }
            };

            var TElement = method.DefineUnmanagedGenericParameter("TElement");
            method.GenericParameters.Add(TElement);

            var ElementFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefAction`2"))
            {
                GenericArguments = { T, TElement }
            };

            var TElementFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateRefActionToStructOperatorAction`2"))
            {
                GenericArguments = { T, TElement }
            };

            var TComparer = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DefaultEqualityComparer`1"))
            {
                GenericArguments = { TKey }
            };

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, T, TKey, TKeyFunc, TElement, TElementFunc, TComparer }
                };
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, enumerable, KeyFunc, TKeyFunc, ElementFunc, TElementFunc);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, KeyFunc, TKeyFunc, ElementFunc, TElementFunc);
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
                    GenericArguments = { enumerable, enumerator, T, TKey, TKeyFunc, TElement, TElementFunc, TComparer }
                };
                GenerateNormal(method, enumerable, KeyFunc, TKeyFunc, ElementFunc, TElementFunc);
            }
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, GenericInstanceType KeyFunc, TypeReference TKeyFunc, TypeReference ElementFunc, TypeReference TElementFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("keySelector", ParameterAttributes.None, KeyFunc));
            method.Parameters.Add(new ParameterDefinition("elementSelector", ParameterAttributes.None, ElementFunc));
            method.DefineAllocatorParam();
            method.DefineGroupByDisposeOptions();

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(enumerable));
            body.Variables.Add(new VariableDefinition(TKeyFunc));
            body.Variables.Add(new VariableDefinition(TElementFunc));

            body.GetILProcessor()
                .LdLocA(0)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LoadFuncArgumentAndStoreToLocalVariableField(1, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 2)

                .LdLocA(0)
                .LdLocA(1)
                .LdLocA(2)
                .LdArg(3)
                .LdArg(4)
                .NewObj(method.ReturnType.FindMethod(".ctor", 5))
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, GenericInstanceType KeyFunc, TypeReference TKeyFunc, TypeReference ElementFunc, TypeReference TElementFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("keySelector", ParameterAttributes.None, KeyFunc));
            method.Parameters.Add(new ParameterDefinition("elementSelector", ParameterAttributes.None, ElementFunc));
            method.DefineAllocatorParam();
            method.DefineGroupByDisposeOptions();

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(enumerable));
            body.Variables.Add(new VariableDefinition(TKeyFunc));
            body.Variables.Add(new VariableDefinition(TElementFunc));

            body.GetILProcessor()
                .ArgumentNullCheck(0, Instruction.Create(OpCodes.Ldloca_S, body.Variables[0]))
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LoadFuncArgumentAndStoreToLocalVariableField(1, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 2)

                .LdLocA(0)
                .LdLocA(1)
                .LdLocA(2)
                .LdArg(3)
                .LdArg(4)
                .NewObj(method.ReturnType.FindMethod(".ctor", 5))
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, GenericInstanceType KeyFunc, TypeReference TKeyFunc, TypeReference ElementFunc, TypeReference TElementFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("keySelector", ParameterAttributes.None, KeyFunc));
            method.Parameters.Add(new ParameterDefinition("elementSelector", ParameterAttributes.None, ElementFunc));
            method.DefineAllocatorParam();
            method.DefineGroupByDisposeOptions();

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(TKeyFunc));
            body.Variables.Add(new VariableDefinition(TElementFunc));

            body.GetILProcessor()

                .LoadFuncArgumentAndStoreToLocalVariableField(1, 0)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 1)

                .LdArg(0)
                .LdLocA(0)
                .LdLocA(1)
                .LdArg(3)
                .LdArg(4)
                .NewObj(method.ReturnType.FindMethod(".ctor", 5))
                .Ret();
        }
    }
}