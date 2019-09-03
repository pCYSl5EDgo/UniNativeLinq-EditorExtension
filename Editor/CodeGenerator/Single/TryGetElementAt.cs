using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetElementAt : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetElementAt(ISingleApi api)
        {
            Api = api;
        }
        public readonly ISingleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.GenerateSingleNoEnumerable(processor, mainModule, GenerateEach, GenerateGeneric);
        }

        private static void GenerateGeneric(TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition("TryGetElementAt", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
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
            var paramIndex = new ParameterDefinition("index", ParameterAttributes.None, method.Module.TypeSystem.Int64);
            method.Parameters.Add(paramIndex);
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var body = method.Body;
            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);

            var notZero = Instruction.Create(OpCodes.Ldarg_0);
            var success = Instruction.Create(OpCodes.Ldarg_1);
            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Ldarg_2);

            body.GetILProcessor()
                .LdArg(1)
                .LdC(0L)
                .BgeS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)

                .Add(loopStart)
                .MoveNextEnumerator(TEnumerator)
                .BrTrueS(success)

                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdC(false)
                .Ret()

                .Add(success)
                .LdC(0L)
                .BeqS(@return)

                .LdArg(1)
                .LdC(1L)
                .Sub()
                .StArgS(paramIndex)
                .BrS(loopStart)

                .Add(@return)
                .LdLocA(0)
                .GetCurrentEnumerator(TEnumerator)
                .CpObj(T)
                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdC(true)
                .Ret();
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition("TryGetElementAt", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
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
                var paramIndex = new ParameterDefinition("index", ParameterAttributes.None, method.Module.TypeSystem.Int64);
                method.Parameters.Add(paramIndex);
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));


                if (canFastCount.HasValue && canFastCount.Value)
                {
                    if (canIndexAccess.HasValue && canIndexAccess.Value)
                    {
                        GenerateCanIndexAccess(method, T, enumerable);
                    }
                    else
                    {
                        GenerateNormalCanFastCount(method, T, enumerable, enumerator, paramIndex);
                    }
                }
                else
                {
                    GenerateNormal(method, T, enumerable, enumerator, paramIndex);
                }
            }
        }

        private static void GenerateNormalCanFastCount(MethodDefinition method, GenericParameter T, TypeReference enumerable, TypeReference enumerator, ParameterDefinition paramIndex)
        {
            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Ldarg_2);
            var fail = InstructionUtility.LoadConstant(false);

            var dispose = enumerator.FindMethod("Dispose", 0);
            body.GetILProcessor()
                .LdArg(1)
                .LdC(0L)
                .BltS(fail)

                .LdArg(1)
                .LdArg(0)
                .Call(enumerable.FindMethod("LongCount"))
                .BgeS(fail)

                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .Call(enumerator.FindMethod("MoveNext"))
                .Pop()

                .LdArg(1)
                .LdC(0L)
                .BeqS(@return)

                .LdArg(1)
                .LdC(1L)
                .Sub()
                .StArgS(paramIndex)
                .BrS(loopStart)

                .Add(Instruction.Create(OpCodes.Nop))
                .Add(@return)
                .LdLocA(0)
                .Call(enumerator.FindMethod("get_Current"))
                .LdObj(T)
                .StObj(T)
                .LdLocA(0)
                .Call(dispose)
                .LdC(true)
                .Ret()

                .Add(fail)
                .Ret();
        }

        private static void GenerateCanIndexAccess(MethodDefinition method, GenericParameter T, TypeReference enumerable)
        {
            var fail = Instruction.Create(OpCodes.Ldc_I4_0);

            var body = method.Body;

            body.GetILProcessor()
                .LdArg(1)
                .LdC(0L)
                .BltS(fail)

                .LdArg(1)
                .LdArg(0)
                .Call(enumerable.FindMethod("LongCount", 0))
                .BgeS(fail)
                .Add(Instruction.Create(OpCodes.Nop))
                .LdArg(2)
                .LdArg(0)
                .LdArg(1)
                .Call(enumerable.FindMethod("get_Item"))
                .LdObj(T)
                .StObj(T)
                .LdC(true)
                .Ret()

                .Add(fail)
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, GenericParameter T, TypeReference enumerable, TypeReference enumerator, ParameterDefinition paramIndex)
        {
            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);

            var notZero = Instruction.Create(OpCodes.Ldarg_0);
            var success = Instruction.Create(OpCodes.Ldarg_1);
            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Ldarg_2);

            var dispose = enumerator.FindMethod("Dispose", 0);
            body.GetILProcessor()
                .LdArg(1)
                .LdC(0L)
                .BgeS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .Call(enumerator.FindMethod("MoveNext"))
                .BrTrueS(success)

                .LdLocA(0)
                .Call(dispose)
                .LdC(false)
                .Ret()

                .Add(success)
                .LdC(0L)
                .BeqS(@return)

                .LdArg(1)
                .LdC(1L)
                .Sub()
                .StArgS(paramIndex)
                .BrS(loopStart)
                .Add(Instruction.Create(OpCodes.Nop))
                .Add(@return)
                .LdLocA(0)
                .Call(enumerator.FindMethod("get_Current"))
                .LdObj(T)
                .StObj(T)
                .LdLocA(0)
                .Call(dispose)
                .LdC(true)
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(baseEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("index", ParameterAttributes.None, method.Module.TypeSystem.Int32));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var fail = InstructionUtility.LoadConstant(false);
            var notMinus = Instruction.Create(OpCodes.Ldarg_0);

            method.Body.GetILProcessor()
                .LdArg(1)
                .LdC(0)
                .BgeS(notMinus)

                .Add(fail)
                .Ret()

                .Add(notMinus)
                .Call(baseEnumerable.FindMethod("get_Length"))
                .LdArg(1)
                .BleS(fail)
                .Add(Instruction.Create(OpCodes.Nop))
                .LdArg(2)
                .LdArg(0)
                .LdArg(1)
                .Call(baseEnumerable.FindMethod("get_Item"))
                .StObj(T)

                .LdC(true)
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("index", ParameterAttributes.None, method.Module.TypeSystem.Int64));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var fail = InstructionUtility.LoadConstant(false);

            method.Body.GetILProcessor()
                .ArgumentNullCheck(0, Instruction.Create(OpCodes.Ldarg_1))
                .LdC(0L)
                .BltS(fail)

                .LdArg(0)
                .LdLen()
                .ConvI8()
                .LdArg(1)
                .BleS(fail)

                .LdArg(2)
                .LdArg(0)
                .LdArg(1)
                .LdElemA(T)
                .CpObj(T)

                .LdC(true)
                .Ret()

                .Add(fail)
                .Ret();
        }
    }
}