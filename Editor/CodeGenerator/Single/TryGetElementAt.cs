using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

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
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(TryGetElementAt) + "Helper"));
            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule);
            }
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

            var T = new GenericParameter("T", method)
            {
                HasNotNullableValueTypeConstraint = true,
                CustomAttributes = { Helper.GetSystemRuntimeInteropServicesUnmanagedTypeConstraintTypeReference() }
            };
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
                switch (name)
                {
                    case "Native":
                    case "Array":
                        GenerateIndex(method, T.MakeFromCommonType(method, Dictionary[name], "0").enumerable, T);
                        break;
                    default:
                        GenerateNormal(method, Dictionary[name], T);
                        break;
                }
            }
        }

        private void GenerateNormal(MethodDefinition method, TypeDefinition type, GenericParameter T)
        {
            var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            var paramIndex = new ParameterDefinition("index", ParameterAttributes.None, method.Module.TypeSystem.Int64);
            method.Parameters.Add(paramIndex);
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

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
                .LdC(0)
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

                .Add(@return)
                .LdLocA(0)
                .Call(enumerator.FindMethod("get_Current"))
                .CpObj(T)
                .LdLocA(0)
                .Call(dispose)
                .LdC(true)
                .Ret();
        }

        private void GenerateIndex(MethodDefinition method, TypeReference enumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("index", ParameterAttributes.None, method.Module.TypeSystem.Int64));
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
                .Call(enumerable.FindMethod("LongCount"))
                .LdArg(1)
                .BleS(fail)

                .LdArg(2)
                .LdArg(0)
                .LdArg(1)
                .Call(enumerable.FindMethod("get_Item"))
                .CpObj(T)

                .LdC(true)
                .Ret();
        }

        private void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(baseEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
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

                .LdArg(2)
                .LdArg(0)
                .LdArg(1)
                .Call(baseEnumerable.FindMethod("get_Item"))
                .StObj(T)

                .LdC(true)
                .Ret();
        }

        private void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("index", ParameterAttributes.None, method.Module.TypeSystem.Int64));
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
                .LdLen()
                .LdArg(1)
                .BleS(fail)

                .LdArg(2)
                .LdArg(0)
                .LdArg(1)
                .LdElemA(T)
                .CpObj(T)

                .LdC(true)
                .Ret();
        }
    }
}