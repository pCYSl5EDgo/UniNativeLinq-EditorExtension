using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetSingleFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetSingleFunc(ISingleApi api)
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

            var Func = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { T, mainModule.TypeSystem.Boolean }
            };

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(TEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, Func));

            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(TEnumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var fail = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);


            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(3)
                .TryGetNextEnumerator(TEnumerator)
                .StLoc(1)
                .LdLoc(3)
                .BrFalseS(@return)

                .LdArg(2)
                .LdLoc(1)
                .LdObj(T)
                .CallVirtual(Func.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdLoc(2)
                .BrTrueS(fail)

                .LdArg(1)
                .LdLoc(1)
                .CpObj(T)
                .LdC(true)
                .StLoc(2)
                .BrS(loopStart)

                .Add(fail)
                .DisposeEnumerator(TEnumerator)
                .LdC(false)
                .Ret()

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
            var Func = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { T, mainModule.TypeSystem.Boolean }
            };

            if (isSpecial)
            {
                var (baseEnumerable, _, _) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, Func);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, T, Func);
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
                method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, Func));

                GenerateNormal(method, enumerable, enumerator, T, Func);
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference enumerator, GenericParameter T, GenericInstanceType Func)
        {
            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var @return = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var fail = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            var Dispose = enumerator.FindMethod("Dispose", 0);
            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(3)
                .Call(enumerator.FindMethod("TryGetNext"))
                .StLoc(1)
                .LdLoc(3)
                .BrFalseS(@return)

                .LdArg(2)
                .LdLoc(1)
                .LdObj(T)
                .CallVirtual(Func.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdLoc(2)
                .BrTrueS(fail)

                .LdArg(1)
                .LdLoc(1)
                .CpObj(T)
                .LdC(true)
                .StLoc(2)
                .BrS(loopStart)

                .Add(fail)
                .Call(Dispose)
                .LdC(false)
                .Ret()

                .Add(@return)
                .Call(Dispose)
                .LdLoc(2)
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference enumerable, GenericParameter T, GenericInstanceType Func)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, Func));

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);
            var @return = Instruction.Create(OpCodes.Ldloc_2);
            var fail = InstructionUtility.LoadConstant(false);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(T));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            var getLength = enumerable.FindMethod("get_Length");

            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .Call(getLength)
                .BrTrueS(loopStart)

                .LdC(false)
                .Ret()

                .Add(loopStart)
                .Call(getLength)
                .LdLoc(0)
                .BleS(@return)

                .LdArg(2)
                .LdArg(0)
                .LdLoc(0)
                .Dup()
                .LdC(1)
                .Add()
                .StLoc(0)
                .Call(enumerable.FindMethod("get_Item"))
                .Dup()
                .StLoc(1)
                .CallVirtual(Func.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdLoc(2)
                .BrTrueS(fail)

                .LdArg(1)
                .LdLocA(1)
                .CpObj(T)
                .LdC(true)
                .StLoc(2)
                .BrS(loopStart)

                .Add(fail)
                .Ret()

                .Add(@return)
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference enumerable, TypeReference T, TypeReference Func)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, enumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("func", ParameterAttributes.None, Func));

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);
            var @return = Instruction.Create(OpCodes.Ldloc_2);
            var fail = InstructionUtility.LoadConstant(false);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.IntPtr));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            body.GetILProcessor()
                .ArgumentNullCheck(0, 2, Instruction.Create(OpCodes.Ldarg_0))
                .LdLen()
                .BrTrueS(loopStart)

                .LdC(false)
                .Ret()

                .Add(loopStart)
                .LdLen()
                .LdLoc(0)
                .BleS(@return)

                .LdArg(2)
                .LdArg(0)
                .LdLoc(0)
                .Dup()
                .LdC(1)
                .Add()
                .StLoc(0)
                .LdElemA(T)
                .Dup()
                .StLoc(1)
                .LdObj(T)
                .CallVirtual(Func.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdLoc(2)
                .BrTrueS(fail)

                .LdArg(1)
                .LdLoc(1)
                .CpObj(T)
                .LdC(true)
                .StLoc(2)
                .BrS(loopStart)

                .Add(fail)
                .Ret()

                .Add(@return)
                .Ret();
        }
    }
}