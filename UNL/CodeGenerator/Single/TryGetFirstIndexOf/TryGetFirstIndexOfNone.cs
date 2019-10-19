using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetFirstIndexOfNone : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public TryGetFirstIndexOfNone(ISingleApi api)
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
            mainModule.Types.Add(@static = mainModule.DefineStatic(Api.Name + Api.Description + "Helper"));

            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule, systemModule);
            }
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
            var IEquatable = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "IEquatable`1")))
            {
                GenericArguments = { T }
            };
            T.Constraints.Add(IEquatable);
            method.GenericParameters.Add(T);

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, _) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, IEquatable);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, T, IEquatable);
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
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(method.Module.TypeSystem.Int64)));
                method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.None, T));

                GenerateNormal(method, enumerable, enumerator, T, IEquatable);
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference enumerator, TypeReference T, TypeReference IEquatable)
        {
            var body = method.Body;
            body.InitLocals = true;
            var enumeratorVariable = new VariableDefinition(enumerator);
            var variables = body.Variables;
            variables.Add(enumeratorVariable);
            variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            variables.Add(new VariableDefinition(new ByReferenceType(T)));
            variables.Add(new VariableDefinition(method.Module.TypeSystem.Int64));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);
            var fail = Instruction.Create(OpCodes.Ldloca_S, enumeratorVariable);

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryGetNext"))
                .StLoc(2)

                .LdLoc(1)
                .BrFalseS(fail)

                .LdLoc(2)
                .LdArg(2)
                .Constrained(T)
                .CallVirtual(IEquatable.FindMethod("Equals"))
                .LdLoc(3)
                .LdC(1L)
                .Add()
                .StLoc(3)
                .BrFalseS(loopStart)

                .LdArg(1)
                .LdLoc(3)
                .LdC(-1L)
                .Add()
                .StObj(method.Module.TypeSystem.Int64)

                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose", 0))

                .LdC(true)
                .Ret()

                .Add(fail)
                .Call(enumerator.FindMethod("Dispose", 0))
                .LdC(false)
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference enumerable, GenericParameter T, TypeReference IEquatable)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(method.Module.TypeSystem.Int32)));
            method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.None, T));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));
            body.Variables.Add(new VariableDefinition(enumerable));

            var loopStart = Instruction.Create(OpCodes.Ldloca_S, body.Variables[2]);
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

                .Add(loopStart)
                    .LdLoc(0)
                    .Call(enumerable.FindMethod("get_Item"))
                    .Dup()
                    .StLoc(1)
                .LdArg(2)
                .Constrained(T)
                .CallVirtual(IEquatable.FindMethod("Equals"))
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
                .LdLoc(0)
                .StObj(method.Module.TypeSystem.Int32)

                .LdC(true)
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericParameter T, TypeReference IEquatable)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(method.Module.TypeSystem.Int64)));
            method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.None, T));

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);
            var success = Instruction.Create(OpCodes.Ldarg_1);

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.IntPtr));
            body.Variables.Add(new VariableDefinition(new ByReferenceType(T)));

            method.Body.GetILProcessor()
                .ArgumentNullCheck(0, Instruction.Create(OpCodes.Ldarg_0))
                .LdLen()
                .BrTrueS(loopStart)

                .LdC(false)
                .Ret()

                .Add(loopStart)
                .LdLoc(0)
                .LdElemA(T)
                .StLoc(1)

                .LdLocA(1)
                .LdArg(2)
                .Constrained(T)
                .CallVirtual(IEquatable.FindMethod("Equals"))
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
                .LdLoc(0)
                .StObj(method.Module.TypeSystem.Int64)

                .LdC(true)
                .Ret();
        }
    }
}