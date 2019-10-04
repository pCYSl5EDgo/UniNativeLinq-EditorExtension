using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class Range
        : ITypeDictionaryHolder,
            IApiExtensionMethodGenerator
    {
        public Range(ISingleApi api)
        {
            Api = api;
        }

        public readonly ISingleApi Api;

        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            if (!processor.TryGetEnabled(Api.Name, out var enabled) || !enabled) return;
            var names = Api.NameCollection;
            if (names.All(name => !Api.TryGetEnabled(name, out var e) || !e)) return;

            var @static = mainModule.GetType("UniNativeLinq", "Enumerable");
            if (@static is null)
            {
                @static = mainModule.DefineStatic("Enumerable");
                @static.CustomAttributes.Clear();
                mainModule.Types.Add(@static);
            }

            foreach (var name in names.Where(name => Api.TryGetEnabled(name, out var e) && e))
            {
                GenerateEach(name, @static, mainModule, systemModule, unityModule);
            }
        }

        private void GenerateEach(string name, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            var method = new MethodDefinition(Api.Description, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            @static.Methods.Add(method);

            var T = mainModule.ImportReference(systemModule.GetType("System", name));

            var TAction = mainModule.GetType("UniNativeLinq", name + "Increment");

            method.ReturnType = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RangeRepeatEnumerable`2"))
            {
                GenericArguments = { T, TAction }
            };

            method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.None, T));
            method.Parameters.Add(new ParameterDefinition("count", ParameterAttributes.None, mainModule.TypeSystem.Int64));

            method.Body.GetILProcessor()
                .LdArgA(0)
                .LdArg(1)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }
    }
}