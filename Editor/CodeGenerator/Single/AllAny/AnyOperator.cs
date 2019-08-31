using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class AnyOperator : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public AnyOperator(ISingleApi api)
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

            var IRefFunc2 = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`2"))
            {
                GenericArguments = { T, mainModule.TypeSystem.Boolean }
            };
            var TOperator = method.DefineUnmanagedGenericParameter("TOperator");
            TOperator.Constraints.Add(IRefFunc2);
            method.GenericParameters.Add(TOperator);

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(TEnumerable))
            {
                CustomAttributes = { Helper.IsReadOnlyAttribute }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.In, new ByReferenceType(TOperator))
            {
                CustomAttributes = { Helper.IsReadOnlyAttribute }
            });

            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);

            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var loopStart = Instruction.Create(OpCodes.Ldarg_1);

            
            body.GetILProcessor()
                .LdArg(0)
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)
                .BrS(condition)
                .Add(loopStart)
                .LdLocA(0)
                .GetCurrentEnumerator(TEnumerator)
                .Constrained(TOperator)
                .CallVirtual(IRefFunc2.FindMethod("Calc"))
                .BrFalseS(condition)
                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdC(true)
                .Ret()
                .Add(condition)
                .MoveNextEnumerator(TEnumerator)
                .BrTrueS(loopStart)
                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdC(false)
                .Ret();
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition("Any", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var IRefFunc2 = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`2"))
            {
                GenericArguments = { T, mainModule.TypeSystem.Boolean }
            };
            var TOperator = method.DefineUnmanagedGenericParameter("TOperator");
            TOperator.Constraints.Add(IRefFunc2);
            method.GenericParameters.Add(TOperator);

            if (isSpecial)
            {
                switch (name)
                {
                    case "T[]":
                        GenerateArray(mainModule, method, TOperator, T, IRefFunc2, T.MakeSpecialTypePair(name));
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, TOperator, IRefFunc2, T.MakeSpecialTypePair(name));
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                GenerateNormal(method, Dictionary[name], TOperator, T, IRefFunc2);
            }
        }

        private static void GenerateNativeArray(MethodDefinition method, GenericParameter TOperator, GenericInstanceType IRefFunc2, (TypeReference baseEnumerable, GenericInstanceType specialEnumerable, GenericInstanceType specialEnumerator) tuple)
        {
            var (baseEnumerable, enumerable, enumerator) = tuple;
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.In, new ByReferenceType(TOperator))
            {
                CustomAttributes = { Helper.IsReadOnlyAttribute }
            });
            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(enumerable));

            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var loopStart = Instruction.Create(OpCodes.Ldarg_1);

            body.GetILProcessor()
                .LdLocA(1)
                .Dup()
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor"))
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)
                .BrS(condition)
                .Add(loopStart)
                .LdLocA(0)
                .Call(enumerator.FindMethod("get_Current", 0))
                .Constrained(TOperator)
                .CallVirtual(IRefFunc2.FindMethod("Calc"))
                .BrFalseS(condition)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose"))
                .LdC(true)
                .Ret()
                .Add(condition)
                .Call(enumerator.FindMethod("MoveNext"))
                .BrTrueS(loopStart)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose"))
                .LdC(false)
                .Ret();
        }

        private static void GenerateArray(ModuleDefinition mainModule, MethodDefinition method, GenericParameter TOperator, GenericParameter T, GenericInstanceType IRefFunc2, (TypeReference baseEnumerable, GenericInstanceType specialEnumerable, GenericInstanceType specialEnumerator) makeSpecialTypePair)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, makeSpecialTypePair.baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.In, new ByReferenceType(TOperator))
            {
                CustomAttributes = { Helper.IsReadOnlyAttribute }
            });
            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(mainModule.TypeSystem.Int32));

            var loopStart = Instruction.Create(OpCodes.Ldloc_0);
            var next = Instruction.Create(OpCodes.Ldloc_0);
            var retFalse = InstructionUtility.LoadConstant(false);
            body.GetILProcessor()
                .Add(loopStart)
                .LdArg(0)
                .LdLen()
                .ConvI4()
                .BgeS(retFalse)
                .LdArg(1)
                .LdArg(0)
                .LdLoc(0)
                .LdElemA(T)
                .Constrained(TOperator)
                .CallVirtual(IRefFunc2.FindMethod("Calc"))
                .BrFalseS(next)
                .LdC(true)
                .Ret()
                .Add(next)
                .LdC(1)
                .Add()
                .StLoc(0)
                .BrS(loopStart)
                .Add(retFalse)
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, TypeDefinition type, TypeReference TOperator, TypeReference T, TypeReference IRefFunc2)
        {
            var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.IsReadOnlyAttribute }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.In, new ByReferenceType(TOperator))
            {
                CustomAttributes = { Helper.IsReadOnlyAttribute }
            });

            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);

            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var loopStart = Instruction.Create(OpCodes.Ldarg_1);

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)
                .BrS(condition)
                .Add(loopStart)
                .LdLocA(0)
                .Call(enumerator.FindMethod("get_Current", 0))
                .Constrained(TOperator)
                .CallVirtual(IRefFunc2.FindMethod("Calc"))
                .BrFalseS(condition)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose"))
                .LdC(true)
                .Ret()
                .Add(condition)
                .Call(enumerator.FindMethod("MoveNext"))
                .BrTrueS(loopStart)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose"))
                .LdC(false)
                .Ret();
        }
    }
}