using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetLast : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetLast(ISingleApi api)
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
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(TryGetLast) + "Helper"));
            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule);
            }
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition("TryGetLast", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
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
                var type = Dictionary[name];
                var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");
                var canIndexAccess = type.CanIndexAccess();
                var canFastCount = type.CanFastCount();

                method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
                {
                    CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
                });
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));


                if (canFastCount.HasValue && canFastCount.Value)
                {
                    if (canIndexAccess.HasValue && canIndexAccess.Value)
                    {
                        GenerateCanIndexAccess(method, T, enumerable);
                    }
                    else
                    {
                        GenerateNormalCanFastCount(method, T, enumerable, enumerator);
                    }
                }
                else
                {
                    GenerateNormal(method, T, enumerable, enumerator);
                }
            }
        }

        private void GenerateNormal(MethodDefinition method, GenericParameter T, TypeReference enumerable, TypeReference enumerator)
        {
            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));
            body.Variables.Add(new VariableDefinition(T));

            var notZero = Instruction.Create(OpCodes.Ldarg_0);
            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var success = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Cpobj, T);

            var TryMoveNext = enumerator.FindMethod("TryMoveNext");
            var Dispose = enumerator.FindMethod("Dispose", 0);
            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .LdLocA(0)
                .LdLocA(2)
                .Call(TryMoveNext)
                .BrTrueS(loopStart)

                .LdLocA(0)
                .Call(Dispose)
                .LdC(false)
                .Ret()

                .Add(loopStart)
                .LdLocA(1)
                .Call(TryMoveNext)
                .BrTrueS(success)

                .LdArg(1)
                .LdLocA(2)
                .BrS(@return)

                .Add(success)
                .LdLocA(2)
                .Call(TryMoveNext)
                .BrTrueS(loopStart)

                .LdArg(1)
                .LdLocA(1)

                .Add(@return)
                .LdLocA(0)
                .Call(Dispose)
                .LdC(true)
                .Ret();
        }

        private void GenerateNormalCanFastCount(MethodDefinition method, GenericParameter T, TypeReference enumerable, TypeReference enumerator)
        {

            var body = method.Body;

            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));
            body.Variables.Add(new VariableDefinition(T));

            var notZero = Instruction.Create(OpCodes.Call, enumerable.FindMethod("GetEnumerator", 0));
            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var success = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Cpobj, T);

            var TryMoveNext = enumerator.FindMethod("TryMoveNext");
            body.GetILProcessor()
                .LdArg(0)
                .Dup()
                .Call(enumerable.FindMethod("Any", 0))
                .BrTrueS(notZero)

                .Pop()
                .LdC(false)
                .Ret()

                .Add(notZero)
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(TryMoveNext)
                .BrTrueS(success)

                .LdArg(1)
                .LdLocA(2)
                .BrS(@return)

                .Add(success)
                .LdLocA(2)
                .Call(TryMoveNext)
                .BrTrueS(loopStart)

                .LdArg(1)
                .LdLocA(1)

                .Add(@return)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose", 0))
                .LdC(true)
                .Ret();
        }

        private void GenerateCanIndexAccess(MethodDefinition method, GenericParameter T, TypeReference enumerable)
        {
            var notZero = Instruction.Create(OpCodes.Ldarg_1);

            method.Body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("Any", 0))
                .BrTrueS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .LdArg(0)
                .Dup()
                .Call(enumerable.FindMethod("LongCount", 0))
                .LdC(1L)
                .Sub()
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
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var notZero = Instruction.Create(OpCodes.Ldarg_1);

            var getLength = baseEnumerable.FindMethod("get_Length");
            method.Body.GetILProcessor()
                .LdArg(0)
                .Call(getLength)
                .BrTrueS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .LdArg(0)
                .Dup()
                .Call(getLength)
                .LdC(1L)
                .Sub()
                .Call(baseEnumerable.FindMethod("get_Item"))
                .StObj(T)

                .LdC(true)
                .Ret();
        }

        private void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));

            var notZero = Instruction.Create(OpCodes.Ldarg_1);

            method.Body.GetILProcessor()
                .LdArg(0)
                .LdLen()
                .BrTrueS(notZero)

                .LdC(false)
                .Ret()

                .Add(notZero)
                .LdArg(0)
                .Dup()
                .LdLen()
                .LdC(1)
                .Sub()
                .LdElemA(T)
                .CpObj(T)

                .LdC(true)
                .Ret();
        }
    }
}