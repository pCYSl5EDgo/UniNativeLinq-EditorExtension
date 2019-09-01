using System.Collections.Generic;
using Mono.Cecil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class Repeat : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public Repeat(ISingleApi api)
        {
            Api = api;
        }

        public readonly ISingleApi Api;

        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            if (!processor.TryGetEnabled(Api.Name, out var enabled) || !enabled || !Api.TryGetEnabled("", out enabled) || !enabled) return;
            var @static = mainModule.GetType("UniNativeLinq", "Enumerable");
            if (@static is null)
            {
                @static = mainModule.DefineStatic("Enumerable");
                @static.CustomAttributes.Clear();
                mainModule.Types.Add(@static);
            }

            var method = new MethodDefinition(Api.Description, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var NoAction = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "NoAction`1"))
            {
                GenericArguments = { T }
            };

            method.ReturnType = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RangeRepeatEnumerable`2"))
            {
                GenericArguments = { T, NoAction }
            };

            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.In, new ByReferenceType(T))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("count", ParameterAttributes.None, mainModule.TypeSystem.Int64));

            method.Body.GetILProcessor()
                .LdArgs(0, 2)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }
    }
}