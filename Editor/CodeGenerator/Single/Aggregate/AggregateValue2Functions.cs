using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    /*
        TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
    */
    public sealed class AggregateValue2Functions : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public AggregateValue2Functions(ISingleApi api)
        {
            Api = api;
        }

        public readonly ISingleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(AggregateValue2Functions) + "Helper"));

            if (Api.TryGetEnabled("TEnumerable", out var genericEnabled) && genericEnabled)
                GenerateGeneric(@static, mainModule, systemModule);

            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule, systemModule);
            }
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition("Aggregate", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var genericParameters = method.GenericParameters;

            var T = method.DefineUnmanagedGenericParameter();
            genericParameters.Add(T);

            var TAccumulate = new GenericParameter("TAccumulate", method);
            genericParameters.Add(TAccumulate);

            var TResult = new GenericParameter("TResult", method);
            genericParameters.Add(TResult);
            method.ReturnType = TResult;

            var Func = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`3")))
            {
                GenericArguments = { TAccumulate, T, TAccumulate }
            };

            var ResultFunc = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { TAccumulate, TResult }
            };

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, TAccumulate, Func, ResultFunc);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, enumerator, T, TAccumulate, Func, ResultFunc);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                GenerateNormal(method, Dictionary[name], T, TAccumulate, Func, ResultFunc);
            }
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference T, TypeReference TAccumulate, TypeReference TFunc, TypeReference TResultFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            var paramAccumulate = new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate);
            method.Parameters.Add(paramAccumulate);
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));
            method.Parameters.Add(new ParameterDefinition("resultFunc", ParameterAttributes.None, TResultFunc));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var condition = Instruction.Create(OpCodes.Ldloc_0);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            body.GetILProcessor()
                .ArgumentNullCheck(2, 3, Instruction.Create(OpCodes.Br_S, condition))
                .Add(loopStart)
                .LdArg(1)
                .LdArg(0)
                .LdLoc(0)
                .LdElem(T)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .StArgS(paramAccumulate)

                .LdLoc(0)
                .LdC(1)
                .Add()
                .StLoc(0)

                .Add(condition)
                .LdArg(0)
                .LdLen()
                .ConvI4()
                .BltS(loopStart)

                .LdArg(3)
                .LdArg(1)
                .CallVirtual(TResultFunc.FindMethod("Invoke"))
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference enumerable, TypeReference enumerator, TypeReference T, TypeReference TAccumulate, TypeReference TFunc, TypeReference TResultFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            var paramAccumulate = new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate);
            method.Parameters.Add(paramAccumulate);
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));
            method.Parameters.Add(new ParameterDefinition("resultFunc", ParameterAttributes.None, TResultFunc));

            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            var variables = body.Variables;
            variables.Add(enumeratorVariable);
            variables.Add(new VariableDefinition(T));
            variables.Add(new VariableDefinition(enumerable));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .ArgumentNullCheck(2, 3, Instruction.Create(OpCodes.Ldloca_S, variables[2]))
                .Dup()
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)
                .BrS(condition)
                .Add(loopStart)
                .LdArg(1)
                .LdLoc(1)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .StArgS(paramAccumulate)
                .Add(condition)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryMoveNext"))
                .BrTrueS(loopStart)
                .LdArg(3)
                .LdArg(1)
                .CallVirtual(TResultFunc.FindMethod("Invoke"))
                .Ret();
        }

        private static void GenerateGeneric(TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition("Aggregate", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var genericParameters = method.GenericParameters;

            var (T, TEnumerator, TEnumerable) = method.Define3GenericParameters();

            var TAccumulate = new GenericParameter("TAccumulate", method);
            genericParameters.Add(TAccumulate);

            var TResult = new GenericParameter("TResult", method);
            genericParameters.Add(TResult);
            method.ReturnType = TResult;

            var TFunc = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`3")))
            {
                GenericArguments = { TAccumulate, T, TAccumulate }
            };

            var TResultFunc = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { TAccumulate, TResult }
            };

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(TEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            var paramAccumulate = new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate);
            method.Parameters.Add(paramAccumulate);
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));
            method.Parameters.Add(new ParameterDefinition("resultFunc", ParameterAttributes.None, TResultFunc));

            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .ArgumentNullCheck(2, 3, Instruction.Create(OpCodes.Ldarg_0))
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)
                .BrS(condition)
                .Add(loopStart)
                .LdArg(1)
                .LdLoc(1)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .StArgS(paramAccumulate)
                .Add(condition)
                .LdLocA(1)
                .TryMoveNextEnumerator(TEnumerator)
                .BrTrueS(loopStart)
                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdArg(3)
                .LdArg(1)
                .CallVirtual(TResultFunc.FindMethod("Invoke"))
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, TypeDefinition type, TypeReference T, TypeReference TAccumulate, TypeReference TFunc, TypeReference TResultFunc)
        {
            var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            var paramAccumulate = new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate);
            method.Parameters.Add(paramAccumulate);
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));
            method.Parameters.Add(new ParameterDefinition("resultFunc", ParameterAttributes.None, TResultFunc));

            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .ArgumentNullCheck(2, 3, Instruction.Create(OpCodes.Ldarg_0))
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)
                .BrS(condition)
                .Add(loopStart)
                .LdArg(1)
                .LdLoc(1)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .StArgS(paramAccumulate)
                .Add(condition)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryMoveNext"))
                .BrTrueS(loopStart)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose", 0))
                .LdArg(3)
                .LdArg(1)
                .CallVirtual(TResultFunc.FindMethod("Invoke"))
                .Ret();
        }
    }
}