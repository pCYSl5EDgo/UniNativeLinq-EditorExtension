using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator.ForEach
{
    public sealed class ForEachOperator : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public ForEachOperator(ISingleApi api)
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

            var action = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefAction`1"))
            {
                GenericArguments = { T }
            };

            var TAction = new GenericParameter("TAction", method)
            {
                HasNotNullableValueTypeConstraint = true,
                Constraints = { action }
            };
            method.GenericParameters.Add(TAction);

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        GenerateArray(method, baseEnumerable, T, TAction, action);
                        break;
                    case "NativeArray<T>":
                        GenerateNativeArray(method, baseEnumerable, enumerable, TAction, action);
                        break;
                }
            }
            else
            {
                var (enumerable, enumerator, _) = T.MakeFromCommonType(method, Dictionary[name], "0");
                GenerateNormal(method, enumerable, enumerator, T, TAction, action);
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, TypeReference enumerator, TypeReference T, TypeReference TAction, TypeReference action)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("action", ParameterAttributes.In, new ByReferenceType(TAction))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });

            var body = method.Body;
            body.InitLocals = true;
            var variables = body.Variables;
            variables.Add(new VariableDefinition(enumerator));
            variables.Add(new VariableDefinition(method.Module.TypeSystem.Boolean));
            variables.Add(new VariableDefinition(new ByReferenceType(T)));

            var closing = Instruction.Create(OpCodes.Ldloca_S, variables[0]);
            var loopStart = Instruction.Create(OpCodes.Ldloca_S, variables[0]);

            body.GetILProcessor()
                .LdArg(0)
                .Call(enumerable.FindMethod("GetEnumerator", 0))
                .StLoc(0)

                .Add(loopStart)
                .LdLocA(1)
                .Call(enumerator.FindMethod("TryGetNext"))
                .StLoc(2)
                .LdLoc(1)
                .BrFalseS(closing)

                .LdArg(1)
                .LdLoc(2)
                .Constrained(TAction)
                .CallVirtual(action.FindMethod("Execute"))
                .BrS(loopStart)

                .Add(closing)
                .Call(enumerator.FindMethod("Dispose", 0))
                .Ret();
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference T, TypeReference TAction, TypeReference action)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("action", ParameterAttributes.In, new ByReferenceType(TAction))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });

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
                .Constrained(TAction)
                .CallVirtual(action.FindMethod("Execute", 1))
                .BrS(loopStart);
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, TypeReference enumerable, TypeReference TAction, TypeReference action)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("action", ParameterAttributes.In, new ByReferenceType(TAction))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });

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
                .Constrained(TAction)
                .CallVirtual(action.FindMethod("Execute", 1))
                .BrS(loopStart);
        }
    }
}