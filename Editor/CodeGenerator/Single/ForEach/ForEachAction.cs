using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator.ForEach
{
    public sealed class ForEachAction : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public ForEachAction(ISingleApi api)
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

            var action = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Action`1")))
            {
                GenericArguments = { T }
            };

            if (isSpecial)
            {
                var (baseEnumerable, _, _) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, action);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, action);
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
            variables.Add(new VariableDefinition(T));

            var closing = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
            var loopStart = Instruction.Create(OpCodes.Ldloca_S, variables[0]);

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryMoveNext"))
                .BrFalseS(closing)

                .LdArg(1)
                .LdLoc(1)
                .CallVirtual(action.FindMethod("Invoke"))
                .BrS(loopStart)

                .Add(closing)
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
                .LdElem(T)
                .CallVirtual(action.FindMethod("Invoke", 1))
                .BrS(loopStart);
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference action)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(baseEnumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("action", ParameterAttributes.None, action));

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(method.Module.TypeSystem.Int32));

            var @continue = Instruction.Create(OpCodes.Ldarg_1);
            var loopStart = Instruction.Create(OpCodes.Ldarg_0);

            body.GetILProcessor()
                .Add(loopStart)
                .Call(baseEnumerable.FindMethod("get_Length"))
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
                .Call(baseEnumerable.FindMethod("get_Item"))
                .CallVirtual(action.FindMethod("Invoke", 1))
                .BrS(loopStart);
        }
    }
}