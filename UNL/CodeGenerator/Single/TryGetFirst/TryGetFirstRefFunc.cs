using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetFirstRefFunc : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetFirstRefFunc(ISingleApi api)
        {
            Api = api;
        }
        public readonly ISingleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.GenerateSingleNoEnumerable(processor, mainModule, GenerateEach, GenerateGeneric);
        }

        private void GenerateGeneric(TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var (T, TEnumerator, TEnumerable) = method.Define3GenericParameters();

            var TPredicate = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefFunc`2"))
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
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var fail = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);


            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .GetEnumeratorEnumerable(TEnumerable)
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .TryGetNextEnumerator(TEnumerator)
                .StLoc(2)

                .LdLoc(1)
                .BrFalseS(fail)

                .LdArg(2)
                .LdLoc(2)
                .CallVirtual(TPredicate.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdArg(1)
                .LdLoc(2)
                .CpObj(T)

                .LdLocA(0)
                .DisposeEnumerator(TEnumerator)

                .LdC(true)
                .Ret()

                .Add(fail)
                .DisposeEnumerator(TEnumerator)
                .LdC(false)
                .Ret();
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule)
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

            var TPredicate = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefFunc`2"))
            {
                GenericArguments = { T, mainModule.TypeSystem.Boolean }
            };

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, _) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, TPredicate);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, T, TPredicate);
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

                GenerateNormal(method, enumerable, enumerator, TPredicate, T);
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference enumerator, TypeReference TPredicate, TypeReference T)
        {
            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(enumerator);
            body.Variables.Add(enumeratorVariable);
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var fail = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryGetNext"))
                .StLoc(2)

                .LdLoc(1)
                .BrFalseS(fail)

                .LdArg(2)
                .LdLoc(2)
                .CallVirtual(TPredicate.FindMethod("Invoke"))
                .BrFalseS(loopStart)

                .LdArg(1)
                .LdLoc(2)
                .CpObj(T)

                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose", 0))

                .LdC(true)
                .Ret()

                .Add(fail)
                .Call(enumerator.FindMethod("Dispose", 0))
                .LdC(false)
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference enumerable, GenericParameter T, TypeReference TPredicate)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("predicate", ParameterAttributes.None, TPredicate));

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
                .ArgumentNullCheck(2, Instruction.Create(OpCodes.Ldarg_0))
                .Call(getLength)
                .BrFalseS(fail)

                .LdLocA(2)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor"))

                .Add(loopStart)
                    .LdLocA(2)
                    .LdLoc(0)
                    .Call(enumerable.FindMethod("get_Item"))
                    .Dup()
                    .StLoc(1)
                .CallVirtual(TPredicate.FindMethod("Invoke"))
                .BrTrueS(success)

                .LdLoc(0)
                .LdC(1)
                .Add()
                .Dup()
                .StLoc(0)

                .LdArgA(0)
                .Call(getLength)
                .ConvI8()

                .BltS(loopStart)

                .Add(fail)
                .Ret()

                .Add(success)
                .LdLoc(1)
                .CpObj(T)

                .LdC(true)
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T, TypeReference TPredicate)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(T)));
            method.Parameters.Add(new ParameterDefinition("predicate", ParameterAttributes.None, TPredicate));

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);
            var success = Instruction.Create(OpCodes.Ldarg_1);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.IntPtr));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));

            method.Body.GetILProcessor()
                .ArgumentNullCheck(0, 2, Instruction.Create(OpCodes.Ldarg_0))
                .LdLen()
                .BrTrueS(loopStart)

                .LdC(false)
                .Ret()

                .Add(loopStart)
                .LdLoc(0)
                .LdElemA(T)
                .StLoc(1)

                .LdArg(2)
                .LdLoc(1)
                .CallVirtual(TPredicate.FindMethod("Invoke"))
                .BrTrueS(success)

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

                .Add(success)
                .LdLoc(1)
                .CpObj(T)

                .LdC(true)
                .Ret();
        }
    }
}