using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class SumInt32 : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public SumInt32(ISingleApi api)
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
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(SumInt32) + "Helper"));
            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule);
            }
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule)
        {
            var returnType = mainModule.TypeSystem.Int32;
            var method = new MethodDefinition("Sum", Helper.StaticMethodAttributes, returnType)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);
            if (isSpecial)
            {
                var (baseEnumerable, _, _) = returnType.MakeSpecialTypePair(name);
                switch (name)
                {
                    case "T[]":
                        returnType.GenerateSum_Array(method, baseEnumerable);
                        break;
                    case "NativeArray<T>":
                        returnType.GenerateSum_NativeArray(method, baseEnumerable);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var (enumerable, enumerator, _) = returnType.MakeFromCommonType(method, Dictionary[name], "0");
                returnType.GenerateSum_IRefEnumerable(method, enumerable, enumerator);
            }
        }
    }
}