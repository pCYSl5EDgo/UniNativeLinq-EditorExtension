using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetSingleNone : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetSingleNone(ISingleApi api)
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
            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);

            var fail = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .LdArg(0)
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)

                .LdLocA(0)
                .LdArg(1)
                .TryMoveNextEnumerator(TEnumerator)
                .BrFalseS(fail)

                .LdLocA(0)
                .MoveNextEnumerator(TEnumerator)
                .BrTrueS(fail)

                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdC(true)
                .Ret()

                .Add(fail)
                .DisposeEnumerator(TEnumerator)
                .LdC(false)
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

                GenerateNormal(method, enumerable, enumerator, T);
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference enumerator, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var body = method.Body;
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);

            var fail = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            var Dispose = enumerator.FindMethod("Dispose", 0);
            var TryMoveNext = enumerator.FindMethod("TryMoveNext");
            var MoveNext = enumerator.FindMethod("MoveNext");
            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .LdLocA(0)
                .LdArg(1)
                .Call(TryMoveNext)
                .BrFalseS(fail)

                .LdLocA(0)
                .Call(MoveNext)
                .BrTrueS(fail)

                .LdLocA(0)
                .Call(Dispose)
                .LdC(true)
                .Ret()

                .Add(fail)
                .Call(Dispose)
                .LdC(false)
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference enumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var fail = InstructionUtility.LoadConstant(false);

            method.Body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("get_Length"))
                .LdC(1)
                .BneS(fail)

                .LdArg(1)
                .LdArg(0)
                .LdC(0)
                .Call(enumerable.FindMethod("get_Item"))
                .StObj(T)

                .Add(fail)
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference enumerable, TypeReference T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, enumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var fail = InstructionUtility.LoadConstant(false);

            method.Body.GetILProcessor()
                .ArgumentNullCheck(0, Instruction.Create(OpCodes.Ldarg_0))
                .LdLen()
                .LdC(1)
                .BneS(fail)

                .LdArg(1)
                .LdArg(0)
                .LdC(0)
                .LdElemA(T)
                .CpObj(T)

                .Add(fail)
                .Ret();
        }
    }
}