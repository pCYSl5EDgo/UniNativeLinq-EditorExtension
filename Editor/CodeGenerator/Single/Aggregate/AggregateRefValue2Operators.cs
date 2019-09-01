using System.Collections.Generic;
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
            if (!Api.TryGetEnabled("TEnumerable", out var genericEnabled) || !genericEnabled) return;
            var @static = mainModule.DefineStatic(nameof(AggregateRefValue2Operators) + "Helper");
            @static.CustomAttributes.Clear();
            mainModule.Types.Add(@static);
            GenerateGeneric(@static, mainModule);
        }
        private static void GenerateGeneric(TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition("Aggregate", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            @static.Methods.Add(method);

            var genericParameters = method.GenericParameters;

            var (T, TEnumerator, TEnumerable) = method.Define3GenericParameters();

            var TAccumulate = new GenericParameter("TAccumulate", method);
            genericParameters.Add(TAccumulate);

            var TResult = new GenericParameter("TResult", method);
            genericParameters.Add(TResult);
            method.ReturnType = TResult;

            var IRefAction = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefAction`2"))
            {
                GenericArguments = { TAccumulate, T }
            };
            var TFunc = new GenericParameter("TFunc", method)
            {
                Constraints =
                {
                    IRefAction
                }
            };
            genericParameters.Add(TFunc);

            var IRefFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`2"))
            {
                GenericArguments = { TAccumulate, TResult }
            };
            var TResultFunc = new GenericParameter("TResultFunc", method)
            {
                Constraints =
                {
                    IRefFunc
                }
            };
            genericParameters.Add(TResultFunc);

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(TEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("accumulate", ParameterAttributes.None, new ByReferenceType(TAccumulate)));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.In, new ByReferenceType(TFunc))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("resultFunc", ParameterAttributes.In, new ByReferenceType(TResultFunc))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });


            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var condition = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .LdArg(0)
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)
                .BrS(condition)
                .Add(loopStart)
                .LdArg(1)
                .LdLoc(2)
                .Constrained(TFunc)
                .CallVirtual(IRefAction.FindMethod("Execute"))
                .Add(condition)
                .LdLocA(1)
                .TryGetNextEnumerator(TEnumerator)
                .StLoc(2)
                .LdLoc(1)
                .BrTrueS(loopStart)
                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdArg(3)
                .LdArg(1)
                .Constrained(TResultFunc)
                .CallVirtual(IRefFunc.FindMethod("Calc"))
                .Ret();
        }
    }
}