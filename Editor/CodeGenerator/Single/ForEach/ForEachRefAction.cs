using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator.ForEach
{
    public sealed class ForEachRefAction : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public ForEachRefAction(ISingleApi api)
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
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Void)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var action = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefAction`1"))
            {
                GenericArguments = { T }
            };

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, action);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, action);
                        break;
                }
            }
            else
            {
                var (enumerable, enumerator, _) = T.MakeFromCommonType(method, Dictionary[name], "0");
                GenerateNormal(method, enumerable, enumerator, T, action);
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference enumerator, TypeReference T, TypeReference action)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("action", ParameterAttributes.None, action));

            var body = method.Body;
            body.InitLocals = true;
            var variables = body.Variables;
            variables.Add(new VariableDefinition(enumerator));
            variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));

            var closing = Instruction.Create(OpCodes.Pop);
            var loopStart = Instruction.Create(OpCodes.Ldarg_1);

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(0)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryGetNext"))
                .LdLoc(1)
                .BrFalseS(closing)

                .CallVirtual(action.FindMethod("Invoke"))
                .BrS(loopStart)

                .Add(closing)
                .LdLocA(0)
                .Call(enumerator.FindMethod("Dispose", 0))
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference T, TypeReference action)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("action", ParameterAttributes.None, action));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.IntPtr));

            var loopStart = Instruction.Create(OpCodes.Ldarg_0);
            var @continue = Instruction.Create(OpCodes.Ldarg_1);

            body.GetILProcessor()
                .Add(loopStart)
                .LdLen()
                .LdLoc(0)
                .BneS(@continue)
                .Ret()
                .Add(@continue)
                .LdArg(0)
                .LdLoc(0)
                .Dup()
                .LdC(1)
                .Add()
                .StLoc(0)
                .LdElemA(T)
                .CallVirtual(action.FindMethod("Invoke", 1))
                .BrS(loopStart);
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference enumerable, TypeReference action)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("action", ParameterAttributes.None, action));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));
            body.Variables.Add(new VariableDefinition(enumerable));

            var @continue = Instruction.Create(OpCodes.Ldarg_1);
            var loopStart = Instruction.Create(OpCodes.Ldarga_S, method.Parameters[0]);

            body.GetILProcessor()
                .LdLocA(1)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor"))

                .Add(loopStart)
                .Call(baseEnumerable.FindMethod("get_Length"))
                .LdLoc(0)
                .BneS(@continue)

                .Ret()

                .Add(@continue)
                .LdLocA(1)
                .LdLoc(0)
                .Dup()
                .LdC(1)
                .Add()
                .StLoc(0)
                .ConvI8()
                .Call(enumerable.FindMethod("get_Item"))
                .CallVirtual(action.FindMethod("Invoke", 1))
                .BrS(loopStart);
        }
    }
}