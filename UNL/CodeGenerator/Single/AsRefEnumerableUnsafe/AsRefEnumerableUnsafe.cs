using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class AsRefEnumerableUnsafe
        : ITypeDictionaryHolder,
            IApiExtensionMethodGenerator
    {
        public AsRefEnumerableUnsafe(ISingleApi api)
        {
            Api = api;
        }

        public readonly ISingleApi Api;

        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            var @static = mainModule.GetType("UniNativeLinq", "NativeEnumerable");
            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule, systemModule);
            }
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition("AsRefEnumerableUnsafe", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = {Helper.ExtensionAttribute}
            };
            @static.Methods.Add(method);

            var T = new GenericParameter("T", method)
            {
                HasNotNullableValueTypeConstraint = true,
                HasDefaultConstructorConstraint = true,
                HasReferenceTypeConstraint = false,
            };
            method.GenericParameters.Add(T);

            var (baseEnumerable, enumerable, _) = T.MakeSpecialTypePair(name);
            method.ReturnType = enumerable;
            switch (name)
            {
                case "T[]":
                    GenerateArray(method, baseEnumerable, enumerable);
                    break;
                case "NativeArray<T>":
                    GenerateNativeArray(method, baseEnumerable, enumerable);
                    break;
                default: throw new NotSupportedException(name);
            }
        }

        private static void GenerateArray(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));

            var body = method.Body;
            
            body.GetILProcessor()
                .ArgumentNullCheck(0, Instruction.Create(OpCodes.Ldarg_0))
                .NewObj(enumerable.FindMethod(".ctor", 1))
                .Ret();
        }

        private static void GenerateNativeArray(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            
            method.Body.GetILProcessor()
                .LdArg(0)
                .NewObj(enumerable.FindMethod(".ctor", 1))
                .Ret();
        }
    }
}