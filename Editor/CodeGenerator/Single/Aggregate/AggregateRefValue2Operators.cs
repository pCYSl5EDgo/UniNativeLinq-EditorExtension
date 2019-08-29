using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    /*
        TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, in TFunc func, in TResultFunc resultFunc)
            where TFunc : IRefAction<T, TAccumulate>
            where TResultFunc : IRefFunc<TAccumulate, TResult>
    */
    public sealed class AggregateRefValue2Operators : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public AggregateRefValue2Operators(ISingleApi api)
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
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(AggregateRefValue2Operators) + "Helper"));
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

            var IRefAction = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefAction`2"))
            {
                GenericArguments = { T, TAccumulate }
            };
            var Func = new GenericParameter("TFunc", method)
            {
                Constraints =
                {
                    IRefAction
                }
            };
            genericParameters.Add(Func);

            var IRefFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`2"))
            {
                GenericArguments = { TAccumulate, TResult }
            };
            var ResultFunc = new GenericParameter("TResultFunc", method)
            {
                Constraints =
                {
                    IRefFunc
                }
            };
            genericParameters.Add(ResultFunc);

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, TAccumulate, Func, ResultFunc, IRefAction, IRefFunc);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, enumerator, T, TAccumulate, Func, ResultFunc, IRefAction, IRefFunc);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                GenerateNormal(method, Dictionary[name], T, TAccumulate, Func, ResultFunc, IRefAction, IRefFunc);
            }
        }

        private void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference T, TypeReference TAccumulate, TypeReference TFunc, TypeReference TResultFunc, TypeReference IRefAction, TypeReference IRefFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("accumulate", ParameterAttributes.None, new ByReferenceType(TAccumulate)));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.In, new ByReferenceType(TFunc))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("resultFunc", ParameterAttributes.In, new ByReferenceType(TResultFunc))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var condition = Instruction.Create(OpCodes.Ldloc_0);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            
            body.GetILProcessor()
                .BrS(condition)
                .Add(loopStart)
                .LdArg(0)
                .LdLoc(0)
                .LdElemA(T)
                .LdArg(1)
                .Constrained(TFunc)
                .CallVirtual(IRefAction.FindMethod("Execute"))

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
                .Constrained(TResultFunc)
                .CallVirtual(IRefFunc.FindMethod("Calc"))
                .Ret();
        }

        private void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference enumerable, TypeReference enumerator, TypeReference T, TypeReference TAccumulate, TypeReference TFunc, TypeReference TResultFunc, TypeReference IRefAction, TypeReference IRefFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("accumulate", ParameterAttributes.None, new ByReferenceType(TAccumulate)));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.In, new ByReferenceType(TFunc))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("resultFunc", ParameterAttributes.In, new ByReferenceType(TResultFunc))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });

            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));
            body.Variables.Add(new VariableDefinition(enumerable));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .LdLocA(3)
                .Dup()
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)
                .BrS(condition)
                .Add(loopStart)
                .LdLoc(2)
                .LdArg(1)
                .Constrained(TFunc)
                .CallVirtual(IRefAction.FindMethod("Execute"))
                .Add(condition)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryGetNext"))
                .StLoc(2)
                .LdLoc(1)
                .BrTrueS(loopStart)
                .LdArg(3)
                .LdArg(1)
                .Constrained(TResultFunc)
                .CallVirtual(IRefFunc.FindMethod("Calc"))
                .Ret();
        }

        private void GenerateNormal(MethodDefinition method, TypeDefinition type, TypeReference T, TypeReference TAccumulate, TypeReference TFunc, TypeReference TResultFunc, TypeReference IRefAction, TypeReference IRefFunc)
        {
            var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("accumulate", ParameterAttributes.None, new ByReferenceType(TAccumulate)));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.In, new ByReferenceType(TFunc))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("resultFunc", ParameterAttributes.In, new ByReferenceType(TResultFunc))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });


            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)
                .BrS(condition)
                .Add(loopStart)
                .LdLoc(2)
                .LdArg(1)
                .Constrained(TFunc)
                .CallVirtual(IRefAction.FindMethod("Execute"))
                .Add(condition)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryGetNext"))
                .StLoc(2)
                .LdLoc(1)
                .BrTrueS(loopStart)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose", 0))
                .LdArg(3)
                .LdArg(1)
                .Constrained(TResultFunc)
                .CallVirtual(IRefFunc.FindMethod("Calc"))
                .Ret();
        }
    }
}