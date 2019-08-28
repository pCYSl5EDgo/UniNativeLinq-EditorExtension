using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class AllFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public AllFunc(ISingleApi api)
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
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(AllFunc) + "Helper"));
            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule, systemModule);
            }
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition("All", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var Func = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { T, mainModule.TypeSystem.Boolean }
            };

            if (isSpecial)
            {
                switch (name)
                {
                    case "T[]":
                        GenerateArray(mainModule, method, Func, T, T.MakeSpecialTypePair(name));
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, Func, T, T.MakeSpecialTypePair(name));
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                GenerateNormal(method, Dictionary[name], Func, T);
            }
        }

        private void GenerateNativeArray(MethodDefinition method, TypeReference TFunc, TypeReference T, (TypeReference baseEnumerable, GenericInstanceType specialEnumerable, GenericInstanceType specialEnumerator) tuple)
        {
            var (baseEnumerable, enumerable, enumerator) = tuple;
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(baseEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));
            var body = method.Body;
            body.InitLocals = true;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));

            var loopStart = Instruction.Create(OpCodes.Ldloc_0);
            var next = Instruction.Create(OpCodes.Ldloc_0);
            var retTrue = Instruction.Create(OpCodes.Ldc_I4_1);
            body.GetILProcessor()
                .Add(loopStart)
                    .LdArg(0)
                    .Call(baseEnumerable.FindMethod("get_Length"))
                .BgeS(retTrue)
                .LdArg(1)
                    .LdArg(0)
                    .LdLoc(0)
                    .Call(baseEnumerable.FindMethod("get_Item", 1))
                .CallVirtual(TFunc.FindMethod("Invoke", 1))
                .BrTrueS(next)
                .LdC(false)
                .Ret()
                .Add(next)
                .LdC(1)
                .Add()
                .StLoc(0)
                .BrS(loopStart)
                .Add(retTrue)
                .Ret();
        }

        private void GenerateArray(ModuleDefinition mainModule, MethodDefinition method, TypeReference TFunc, GenericParameter T, (TypeReference baseEnumerable, GenericInstanceType specialEnumerable, GenericInstanceType specialEnumerator) makeSpecialTypePair)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, makeSpecialTypePair.baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));
            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(mainModule.TypeSystem.Int32));

            var loopStart = Instruction.Create(OpCodes.Ldloc_0);
            var next = Instruction.Create(OpCodes.Ldloc_0);
            var retTrue = Instruction.Create(OpCodes.Ldc_I4_1);
            body.GetILProcessor()
                .Add(loopStart)
                .LdArg(0)
                .LdLen()
                .ConvI4()
                .BgeS(retTrue)
                .LdArg(1)
                .LdArg(0)
                .LdLoc(0)
                .LdElemAny(T)
                .CallVirtual(TFunc.FindMethod("Invoke", 1))
                .BrTrueS(next)
                .LdC(false)
                .Ret()
                .Add(next)
                .LdC(1)
                .Add()
                .StLoc(0)
                .BrS(loopStart)
                .Add(retTrue)
                .Ret();
        }

        private void GenerateNormal(MethodDefinition method, TypeDefinition type, TypeReference TFunc, TypeReference T)
        {
            var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.IsReadOnlyAttribute }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));

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
                .LdObj(T)
                .CallVirtual(TFunc.FindMethod("Invoke", 1))
                .BrTrueS(condition)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose"))
                .LdC(false)
                .Ret()
                .Add(condition)
                .Call(enumerator.FindMethod("MoveNext"))
                .BrTrueS(loopStart)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose"))
                .LdC(true)
                .Ret();
        }
    }
}