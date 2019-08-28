using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator.Contains
{
    public sealed class ContainsRefFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public ContainsRefFunc(ISingleApi api)
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
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(ContainsRefFunc) + "Helper"));
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
            var TRefFunc = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefFunc`3"))
            {
                GenericArguments = { T, T, mainModule.TypeSystem.Boolean }
            };

            if (isSpecial)
            {
                var (enumerable, _, _) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, enumerable, T, TRefFunc);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, enumerable, T, TRefFunc);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var type = Dictionary[name];
                var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");

                GenerateNormal(method, T, enumerable, enumerator, TRefFunc);
            }
        }

        private static void GenerateNormal(MethodDefinition method, GenericParameter T, TypeReference enumerable, TypeReference enumerator, TypeReference TRefFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.In, new ByReferenceType(T))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TRefFunc));

            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var fail = InstructionUtility.LoadConstant(false);
            var dispose = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryMoveNext"))
                .BrFalseS(fail)

                .LdArg(2)
                .LdArg(1)
                .LdLocA(1)
                .CallVirtual(TRefFunc.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdC(true)
                .BrS(dispose)

                .Add(fail)
                .Add(dispose)
                .Call(enumerator.FindMethod("Dispose", 0))
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference enumerable, GenericParameter T, TypeReference TRefFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.In, new ByReferenceType(T))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TRefFunc));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(T));


            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var @return = InstructionUtility.LoadConstant(true);

            var getLength = enumerable.FindMethod("get_Length");

            body.GetILProcessor()
                .LdArg(0)
                .Call(getLength)
                .BrTrueS(loopStart)

                .LdC(false)
                .Ret()

                .Add(loopStart)
                .LdArg(1)
                .LdArg(0)
                .LdLoc(0)
                .Call(enumerable.FindMethod("get_Item"))
                .StLoc(1)
                .LdLocA(1)
                .CallVirtual(TRefFunc.FindMethod("Invoke"))
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

        private static void GenerateArray(MethodDefinition method, TypeReference enumerable, GenericParameter T, TypeReference TRefFunc)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, enumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.In, new ByReferenceType(T))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, TRefFunc));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.IntPtr));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var @return = InstructionUtility.LoadConstant(true);

            body.GetILProcessor()
                .LdArg(0)
                .LdLen()
                .BrTrueS(loopStart)

                .LdC(false)
                .Ret()

                .Add(loopStart)
                .LdArg(1)
                .LdArg(0)
                .LdLoc(0)
                .LdElemA(T)
                .CallVirtual(TRefFunc.FindMethod("Invoke"))
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