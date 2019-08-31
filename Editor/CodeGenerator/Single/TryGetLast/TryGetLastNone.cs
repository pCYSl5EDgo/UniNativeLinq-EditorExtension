using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetLastNone : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetLastNone(ISingleApi api)
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
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .LdArg(0)
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .TryMoveNextEnumerator(TEnumerator)
                .BrFalseS(@return)

                .LdC(true)
                .StLoc(2)

                .LdArg(1)
                .LdLocA(1)
                .CpObj(T)
                .BrS(loopStart)

                .Add(@return)
                .DisposeEnumerator(TEnumerator)
                .LdLoc(2)
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


                if (canFastCount.HasValue && canFastCount.Value && canIndexAccess.HasValue && canIndexAccess.Value)
                {
                    GenerateCanIndexAccess(method, T, enumerable);
                }
                else
                {
                    GenerateNormal(method, T, enumerable, enumerator);
                }
            }
        }

        private static void GenerateNormal(MethodDefinition method, GenericParameter T, TypeReference enumerable, TypeReference enumerator)
        {
            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            var TryMoveNext = enumerator.FindMethod("TryMoveNext");
            var Dispose = enumerator.FindMethod("Dispose", 0);
            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(TryMoveNext)
                .BrFalseS(@return)

                .LdC(true)
                .StLoc(2)

                .LdArg(1)
                .LdLocA(1)
                .CpObj(T)
                .BrS(loopStart)

                .Add(@return)
                .Call(Dispose)
                .LdLoc(2)
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
                .Dup()
                .Call(enumerable.FindMethod("LongCount", 0))
                .LdC(1L)
                .Sub()
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

            var getLength = baseEnumerable.FindMethod("get_Length");
            method.Body.GetILProcessor()
                .LdArg(0)
                .Call(getLength)
                .BrTrueS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .LdArg(0)
                .Dup()
                .Call(getLength)
                .LdC(1)
                .Sub()
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
                .LdArg(0)
                .LdLen()
                .BrTrueS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .LdArg(0)
                .Dup()
                .LdLen()
                .LdC(1)
                .Sub()
                .LdElemA(T)
                .CpObj(T)

                .LdC(true)
                .Ret();
        }
    }
}