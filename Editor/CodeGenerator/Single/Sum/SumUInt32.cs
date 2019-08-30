using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class SumUInt32 : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public SumUInt32(ISingleApi api)
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
            var returnType = mainModule.TypeSystem.UInt32;
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