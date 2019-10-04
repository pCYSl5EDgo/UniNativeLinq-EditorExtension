using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetLastFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetLastFunc(ISingleApi api)
        {
            Api = api;
        }
        public readonly ISingleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.GenerateSingleNoEnumerable(processor, mainModule, systemModule, GenerateEach, GenerateGeneric);
        }

        private void GenerateGeneric(TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var (T, TEnumerator, TEnumerable) = method.Define3GenericParameters();

            var TPredicate = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { T, mainModule.TypeSystem.Boolean }
            };

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(TEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("predicate", ParameterAttributes.None, TPredicate));

            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .TryMoveNextEnumerator(TEnumerator)
                .BrFalseS(@return)

                .LdArg(2)
                .LdLoc(1)
                .CallVirtual(TPredicate.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdC(true)
                .StLoc(2)

                .LdArg(1)
                .LdLocA(1)
                .CpObj(T)
                .BrS(loopStart)

                .Add(@return)
                .DisposeEnumerator(TEnumerator)
                .LdLoc(2)
                .Ret();
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var TPredicate = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { T, mainModule.TypeSystem.Boolean }
            };

            if (isSpecial)
            {
                var (baseEnumerable, _, _) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, TPredicate);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, T, TPredicate);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var type = Dictionary[name];
                var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");

                method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
                {
                    CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
                });
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
                method.Parameters.Add(new ParameterDefinition("predicate", ParameterAttributes.None, TPredicate));

                var canIndexAccess = type.CanIndexAccess();
                var canFastCount = type.CanFastCount();

                if (canFastCount.HasValue && canFastCount.Value && canIndexAccess.HasValue && canIndexAccess.Value)
                {
                    GenerateCanIndexAccess(method, enumerable, TPredicate, T);
                }
                else
                {
                    GenerateNormal(method, enumerable, enumerator, TPredicate, T);
                }
            }
        }

        private static void GenerateCanIndexAccess(MethodDefinition method, TypeReference enumerable, TypeReference TPredicate, TypeReference T)
        {
            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int64));

            var fail = InstructionUtility.LoadConstant(false);
            var success = Instruction.Create(OpCodes.Ldarg_1);
            var loopStart = Instruction.Create(OpCodes.Ldarg_2);

            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .Call(enumerable.FindMethod("LongCount", 0))
                .Dup()
                .StLoc(0)
                .BrFalseS(fail)

                .Add(loopStart)
                .LdArg(0)
                .LdLoc(0)
                .LdC(1)
                .Sub()
                .Dup()
                .StLoc(0)
                .Call(enumerable.FindMethod("get_Item"))
                .LdObj(T)
                .CallVirtual(TPredicate.FindMethod("Invoke"))
                .BrTrueS(success)

                .LdLoc(0)
                .BrTrueS(loopStart)

                .Add(fail)
                .Ret()

                .Add(success)
                .LdArg(0)
                .LdLoc(0)
                .Call(enumerable.FindMethod("get_Item"))
                .CpObj(T)
                .LdC(true)
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference enumerator, TypeReference TPredicate, TypeReference T)
        {
            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(T));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            var TryMoveNext = enumerator.FindMethod("TryMoveNext");
            var Dispose = enumerator.FindMethod("Dispose", 0);
            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(TryMoveNext)
                .BrFalseS(@return)

                .LdArg(2)
                .LdLoc(1)
                .CallVirtual(TPredicate.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdC(true)
                .StLoc(2)

                .LdArg(1)
                .LdLocA(1)
                .CpObj(T)
                .BrS(loopStart)

                .Add(@return)
                .Call(Dispose)
                .LdLoc(2)
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T, TypeReference TPredicate)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(baseEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("predicate", ParameterAttributes.None, TPredicate));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(T));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var success = Instruction.Create(OpCodes.Ldarg_1);
            var fail = InstructionUtility.LoadConstant(false);

            var getLength = baseEnumerable.FindMethod("get_Length");

            method.Body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .Call(getLength)
                .Dup()
                .StLoc(0)
                .BrFalseS(fail)

                .Add(loopStart)
                    .LdArg(0)
                    .LdLoc(0)
                    .LdC(1)
                    .Sub()
                    .Dup()
                    .StLoc(0)
                    .Call(baseEnumerable.FindMethod("get_Item"))
                    .Dup()
                    .StLoc(1)
                .CallVirtual(TPredicate.FindMethod("Invoke"))
                .BrTrueS(success)

                .LdLoc(0)
                .BrTrueS(loopStart)

                .Add(fail)
                .Ret()

                .Add(success)
                .LdLocA(1)
                .CpObj(T)

                .LdC(true)
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T, TypeReference TPredicate)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("predicate", ParameterAttributes.None, TPredicate));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var success = InstructionUtility.LoadConstant(true);
            var fail = InstructionUtility.LoadConstant(false);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.IntPtr));

            method.Body.GetILProcessor()
                .ArgumentNullCheck(0, 2, Instruction.Create(OpCodes.Ldarg_0))
                .LdLen()
                .BrFalseS(fail)

                .LdArg(0)
                .LdLen()
                .StLoc(0)

                .Add(loopStart)
                .LdArg(1)
                .LdArg(0)
                .LdLoc(0)
                .LdC(1)
                .Sub()
                .Dup()
                .StLoc(0)
                .LdElemA(T)
                .CpObj(T)
                .LdArg(1)
                .LdObj(T)
                .CallVirtual(TPredicate.FindMethod("Invoke"))
                .BrTrueS(success)

                .LdLoc(0)

                .BrTrueS(loopStart)

                .Add(fail)
                .Ret()

                .Add(success)
                .Ret();
        }
    }
}