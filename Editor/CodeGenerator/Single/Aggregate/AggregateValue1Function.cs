using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    /*
        TAccumulate Aggregate(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func)
    */
    public sealed class AggregateValue1Function : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public AggregateValue1Function(ISingleApi api)
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
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(AggregateValue1Function) + "Helper"));
            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule, systemModule);
            }
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
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

            var Func = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`3")))
            {
                GenericArguments = { TAccumulate, T, TAccumulate }
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
                        GenerateNativeArray(method, baseEnumerable, TAccumulate, Func);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var typeDefinition = Dictionary[name];
                if (typeDefinition is null)
                {
                    throw new NullReferenceException(name);
                }
                GenerateNormal(method, typeDefinition, T, TAccumulate, Func);
            }
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference T, TypeReference TAccumulate, TypeReference TFunc)
        {
            var parameters = method.Parameters;
            parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            parameters.Add(new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate));
            parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.IntPtr));

            var end = Instruction.Create(OpCodes.Ldarg_1);

            body.GetILProcessor()

                .Add(loopStart)
                .LdLen()
                .LdLoc(0)

                .BeqS(end)

                .LdArg(2)
                .LdArg(1)
                .LdArg(0)
                .LdLoc(0)
                .Dup()
                .LdC(1)
                .Add()
                .StLoc(0)
                .LdElem(T)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .StArgS(parameters[1])
                .BrS(loopStart)

                .Add(end)
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference TAccumulate, TypeReference TFunc)
        {
            var parameters = method.Parameters;
            parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(baseEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            parameters.Add(new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate));
            parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));

            var end = Instruction.Create(OpCodes.Ldarg_1);

            body.GetILProcessor()

                .Add(loopStart)
                .Call(baseEnumerable.FindMethod("get_Length"))
                .LdLoc(0)

                .BeqS(end)

                .LdArg(2)
                .LdArg(1)
                .LdArg(0)
                .LdLoc(0)
                .Dup()
                .LdC(1)
                .Add()
                .StLoc(0)
                .Call(baseEnumerable.FindMethod("get_Item"))
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .StArgS(parameters[1])
                .BrS(loopStart)

                .Add(end)
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, TypeDefinition type, TypeReference T, TypeReference TAccumulate, TypeReference TFunc)
        {
            var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");
            var parameters = method.Parameters;
            parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            parameters.Add(new ParameterDefinition("accumulate", ParameterAttributes.None, TAccumulate));
            parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));

            var body = method.Body;

            var variables = body.Variables;
            variables.Add(new VariableDefinition(enumerator));
            variables.Add(new VariableDefinition(T));

            var end = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
            var loopStart = Instruction.Create(OpCodes.Ldloca_S, variables[0]);

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryMoveNext"))
                .BrFalseS(end)

                .LdArg(2)
                .LdArg(1)
                .LdLoc(1)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .StArgS(parameters[1])
                .BrS(loopStart)

                .Add(end)
                .Call(enumerator.FindMethod("Dispose", 0))
                .LdArg(1)
                .Ret();
        }
    }
}