using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetFirstNone : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetFirstNone(ISingleApi api)
        {
            Api = api;
        }
        public readonly ISingleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.GenerateSingleNoEnumerable(processor, mainModule, GenerateEach, GenerateGeneric);
        }

        private void GenerateGeneric(TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var (T, TEnumerator, TEnumerable) = method.Define3GenericParameters();

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(TEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var body = method.Body;

            body.Variables.Add(new VariableDefinition(TEnumerator));

            body.GetILProcessor()
                .LdArg(0)
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)
                .LdLocA(0)
                .LdArg(1)
                .TryMoveNextEnumerator(TEnumerator)
                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .Ret();
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            if (isSpecial)
            {
                var (baseEnumerable, _, _) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, T);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var type = Dictionary[name];
                var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");
                var canIndexAccess = type.CanIndexAccess();
                var canFastCount = type.CanFastCount();

                method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
                {
                    CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
                });
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

                if (canFastCount.HasValue && canFastCount.Value)
                {
                    if (canIndexAccess.HasValue && canIndexAccess.Value)
                    {
                        GenerateCanIndexAccess(method, T, enumerable);
                    }
                    else
                    {
                        GenerateNormalCanFastCount(method, enumerable, enumerator);
                    }
                }
                else
                {
                    GenerateNormal(method, enumerable, enumerator);
                }
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference enumerator)
        {
            var body = method.Body;

            body.Variables.Add(new VariableDefinition(enumerator));

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)
                .LdLocA(0)
                .LdArg(1)
                .Call(enumerator.FindMethod("TryMoveNext"))
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose", 0))
                .Ret();
        }

        private static void GenerateNormalCanFastCount(MethodDefinition method, TypeReference enumerable, TypeReference enumerator)
        {
            var notZero = Instruction.Create(OpCodes.Call, enumerable.FindMethod("GetEnumerator", 0));

            var body = method.Body;

            body.Variables.Add(new VariableDefinition(enumerator));

            body.GetILProcessor()
                .LdArg(0)
                .Dup()
                .Call(enumerable.FindMethod("Any", 0))
                .BrTrueS(notZero)

                .Pop()
                .LdC(false)
                .Ret()

                .Add(notZero)
                .StLoc(0)
                .LdLocA(0)
                .LdArg(1)
                .Call(enumerator.FindMethod("TryMoveNext"))
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose", 0))
                .Ret();
        }

        private static void GenerateCanIndexAccess(MethodDefinition method, GenericParameter T, TypeReference enumerable)
        {
            var notZero = Instruction.Create(OpCodes.Ldarg_1);

            method.Body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("Any", 0))
                .BrTrueS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .LdArg(0)
                .LdC(0L)
                .Call(enumerable.FindMethod("get_Item"))
                .CpObj(T)

                .LdC(true)
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(baseEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var notZero = Instruction.Create(OpCodes.Ldarg_1);

            method.Body.GetILProcessor()
                .LdArg(0)
                .Call(baseEnumerable.FindMethod("get_Length"))
                .BrTrueS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .LdArg(0)
                .LdC(0)
                .Call(baseEnumerable.FindMethod("get_Item"))
                .StObj(T)

                .LdC(true)
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var notZero = Instruction.Create(OpCodes.Ldarg_1);

            method.Body.GetILProcessor()
                .ArgumentNullCheck(0, Instruction.Create(OpCodes.Ldarg_0))
                .LdLen()
                .BrTrueS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .LdArg(0)
                .LdC(0)
                .LdElemA(T)
                .CpObj(T)

                .LdC(true)
                .Ret();
        }
    }
}