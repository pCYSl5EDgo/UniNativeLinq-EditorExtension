using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class ContainsFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public ContainsFunc(ISingleApi api)
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
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(ContainsFunc) + "Helper"));
            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule, systemModule);
            }
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition("Contains", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);
            var TFunc = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`3")))
            {
                GenericArguments = { T, T, mainModule.TypeSystem.Boolean }
            };

            if (isSpecial)
            {
                var (enumerable, _, _) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, enumerable, T, TFunc);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, enumerable, T, TFunc);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var type = Dictionary[name];
                var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");

                GenerateNormal(method, T, enumerable, enumerator, TFunc);
            }
        }

        private static void GenerateNormal(MethodDefinition method, GenericParameter T, TypeReference enumerable, TypeReference enumerator, TypeReference TFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.In, new ByReferenceType(T))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));

            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));
            body.Variables.Add(new VariableDefinition(T));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var fail = InstructionUtility.LoadConstant(false);
            var dispose = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .LdArg(1)
                .LdObj(T)
                .StLoc(2)

                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryMoveNext"))
                .BrFalseS(fail)

                .LdArg(2)
                .LdLoc(1)
                .LdLoc(2)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdC(true)
                .BrS(dispose)

                .Add(fail)
                .Add(dispose)
                .Call(enumerator.FindMethod("Dispose", 0))
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference enumerable, GenericParameter T, TypeReference TFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.In, new ByReferenceType(T))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(T));


            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var @return = InstructionUtility.LoadConstant(true);
            var success = Instruction.Create(OpCodes.Ldarg_1);

            var getLength = enumerable.FindMethod("get_Length");

            body.GetILProcessor()
                .LdArg(0)
                .Call(getLength)
                .BrTrueS(success)

                .LdC(false)
                .Ret()

                .Add(success)
                .LdObj(T)
                .StLoc(1)

                .Add(loopStart)
                .LdArg(0)
                .LdLoc(0)
                .Call(enumerable.FindMethod("get_Item"))
                .LdLoc(1)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .BrTrueS(@return)

                .LdLoc(0)
                .LdC(1)
                .Add()
                .Dup()
                .StLoc(0)
                .LdArg(0)
                .Call(getLength)
                .BltS(loopStart)

                .LdC(false)
                .Ret()

                .Add(@return)
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference enumerable, GenericParameter T, TypeReference TFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, enumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.In, new ByReferenceType(T))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TFunc));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.IntPtr));
            body.Variables.Add(new VariableDefinition(T));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var @return = InstructionUtility.LoadConstant(true);
            var success = Instruction.Create(OpCodes.Ldarg_1);

            body.GetILProcessor()
                .LdArg(0)
                .LdLen()
                .BrTrueS(success)

                .LdC(false)
                .Ret()

                .Add(success)
                .LdObj(T)
                .StLoc(1)

                .Add(loopStart)
                .LdArg(0)
                .LdLoc(0)
                .LdElem(T)
                .LdLoc(1)
                .CallVirtual(TFunc.FindMethod("Invoke"))
                .BrTrueS(@return)

                .LdLoc(0)
                .LdC(1)
                .Add()
                .Dup()
                .StLoc(0)
                .LdArg(0)
                .LdLen()
                .BltS(loopStart)

                .LdC(false)
                .Ret()

                .Add(@return)
                .Ret();
        }
    }
}