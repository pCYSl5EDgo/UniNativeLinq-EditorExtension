using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetLastOperator : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetLastOperator(ISingleApi api)
        {
            Api = api;
        }
        public readonly ISingleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.GenerateSingleNoEnumerable(processor, mainModule, systemModule, unityModule, GenerateEach);
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

            var predicate = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`2"))
            {
                GenericArguments = { T, mainModule.TypeSystem.Boolean }
            };

            var TPredicate = method.DefineUnmanagedGenericParameter("TPredicate");
            TPredicate.Constraints.Add(predicate);
            method.GenericParameters.Add(TPredicate);

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, TPredicate, predicate);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, enumerator, T, TPredicate, predicate);
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
                method.Parameters.Add(new ParameterDefinition("TPredicate", ParameterAttributes.In, new ByReferenceType(TPredicate))
                {
                    CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
                });

                var canIndexAccess = type.CanIndexAccess();
                var canFastCount = type.CanFastCount();

                if (canFastCount.HasValue && canFastCount.Value && canIndexAccess.HasValue && canIndexAccess.Value)
                {
                    GenerateCanIndexAccess(method, enumerable, TPredicate, predicate, T);
                }
                else
                {
                    GenerateNormal(method, enumerable, enumerator, TPredicate, predicate, T);
                }
            }
        }

        private void GenerateCanIndexAccess(MethodDefinition method, TypeReference enumerable, TypeReference TPredicate, TypeReference predicate, TypeReference T)
        {
            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int64));
            
            var fail = InstructionUtility.LoadConstant(false);
            var success = Instruction.Create(OpCodes.Ldarg_1);
            var loopStart = Instruction.Create(OpCodes.Ldarg_2);

            body.GetILProcessor()
                .LdArg(0)
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
                .Constrained(TPredicate)
                .CallVirtual(predicate.FindMethod("Calc"))
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

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference enumerator, TypeReference TPredicate, TypeReference predicate, TypeReference T)
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
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(TryMoveNext)
                .BrFalseS(@return)

                .LdArg(2)
                .LdLocA(1)
                .Constrained(TPredicate)
                .CallVirtual(predicate.FindMethod("Calc"))
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

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference enumerable, TypeReference enumerator, GenericParameter T, TypeReference TPredicate, TypeReference predicate)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("TPredicate", ParameterAttributes.In, new ByReferenceType(TPredicate))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int64));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));
            body.Variables.Add(new VariableDefinition(enumerable));

            var loopStart = Instruction.Create(OpCodes.Ldarg_2);
            var success = Instruction.Create(OpCodes.Ldarg_1);
            var fail = InstructionUtility.LoadConstant(false);

            var getLength = baseEnumerable.FindMethod("get_Length");

            method.Body.GetILProcessor()
                .LdArgA(0)
                .Call(getLength)
                .BrFalseS(fail)

                .LdLocA(2)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor"))

                .LdArgA(0)
                .Call(getLength)
                .ConvI8()
                .StLoc(0)

                .Add(loopStart)
                    .LdLocA(2)
                    .LdLoc(0)
                    .LdC(1)
                    .Sub()
                    .Dup()
                    .StLoc(0)
                    .Call(enumerable.FindMethod("get_Item"))
                    .Dup()
                    .StLoc(1)
                .Constrained(TPredicate)
                .CallVirtual(predicate.FindMethod("Calc"))
                .BrTrueS(success)

                .LdLoc(0)
                .BrTrueS(loopStart)

                .Add(fail)
                .Ret()

                .Add(success)
                .LdLoc(1)
                .CpObj(T)

                .LdC(true)
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T, TypeReference TPredicate, TypeReference predicate)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("TPredicate", ParameterAttributes.In, new ByReferenceType(TPredicate))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);
            var success = Instruction.Create(OpCodes.Ldarg_1);
            var fail = InstructionUtility.LoadConstant(false);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.IntPtr));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));

            method.Body.GetILProcessor()
                .LdArg(0)
                .LdLen()
                .BrFalseS(fail)

                .LdArg(0)
                .LdLen()
                .StLoc(0)

                .Add(loopStart)
                .LdLoc(0)
                .LdC(1)
                .Sub()
                .Dup()
                .StLoc(0)
                .LdElemA(T)
                .StLoc(1)

                .LdArg(2)
                .LdLoc(1)
                .Constrained(TPredicate)
                .CallVirtual(predicate.FindMethod("Calc"))
                .BrTrueS(success)

                .LdLoc(0)

                .BrTrueS(loopStart)

                .Add(fail)
                .Ret()

                .Add(success)
                .LdLoc(1)
                .CpObj(T)

                .LdC(true)
                .Ret();
        }
    }
}