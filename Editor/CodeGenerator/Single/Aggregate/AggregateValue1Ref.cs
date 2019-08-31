using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    /*
        TAccumulate Aggregate<TAccumulate, TResult>(ref TAccumulate seed, RefAction<T, TAccumulate> func)
    */
    public sealed class AggregateValue1Ref : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public AggregateValue1Ref(ISingleApi api)
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
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(AggregateValue1Ref) + "Helper"));

            if (Api.TryGetEnabled("TEnumerable", out var genericEnabled) && genericEnabled)
                GenerateGeneric(@static, mainModule);

            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule);
            }
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition("Aggregate", Helper.StaticMethodAttributes, mainModule.TypeSystem.Void)
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
            method.ReturnType = TAccumulate;

            var Func = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefAction`2"))
            {
                GenericArguments = { T, TAccumulate }
            };

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, TAccumulate, Func);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, enumerator, T, TAccumulate, Func);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                GenerateNormal(method, Dictionary[name], T, TAccumulate, Func);
            }
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference T, TypeReference TAccumulate, TypeReference TFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            var paramAccumulate = new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate);
            method.Parameters.Add(paramAccumulate);
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));

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
                .LdArgAs(paramAccumulate)
                .CallVirtual(TFunc.FindMethod("Invoke"))

                .LdLoc(0)
                .LdC(1)
                .Add()
                .StLoc(0)

                .Add(condition)
                .LdArg(0)
                .LdLen()
                .ConvI4()
                .BltS(loopStart)
                .LdArg(1)
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference enumerable, TypeReference enumerator, TypeReference T, TypeReference TAccumulate, TypeReference TFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            var paramAccumulate = new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate);
            method.Parameters.Add(paramAccumulate);
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));

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
                .LdArgAs(paramAccumulate)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .Add(condition)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryGetNext"))
                .StLoc(2)
                .LdLoc(1)
                .BrTrueS(loopStart)
                .LdArg(1)
                .Ret();
        }
        private static void GenerateGeneric(TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition("Aggregate", Helper.StaticMethodAttributes, mainModule.TypeSystem.Void)
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
            method.ReturnType = TAccumulate;

            var TFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefAction`2"))
            {
                GenericArguments = { T, TAccumulate }
            };

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(TEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            var paramAccumulate = new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate);
            method.Parameters.Add(paramAccumulate);
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));


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
                .LdLoc(2)
                .LdArgAs(paramAccumulate)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .Add(condition)
                .LdLocA(1)
                .TryGetNextEnumerator(TEnumerator)
                .StLoc(2)
                .LdLoc(1)
                .BrTrueS(loopStart)
                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)
                .LdArg(1)
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, TypeDefinition type, TypeReference T, TypeReference TAccumulate, TypeReference TFunc)
        {
            var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            var paramAccumulate = new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate);
            method.Parameters.Add(paramAccumulate);
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));


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
                .LdArgAs(paramAccumulate)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .Add(condition)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryGetNext"))
                .StLoc(2)
                .LdLoc(1)
                .BrTrueS(loopStart)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose", 0))
                .LdArg(1)
                .Ret();
        }
    }
}