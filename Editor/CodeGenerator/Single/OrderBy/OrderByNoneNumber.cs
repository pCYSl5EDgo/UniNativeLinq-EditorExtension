using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class OrderByNoneNumber : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public OrderByNoneNumber(ISingleApi api)
        {
            Api = api;
            elementTypeName = api.Description.Substring(4);
        }

        private readonly string elementTypeName;
        public readonly ISingleApi Api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            if (!processor.TryGetEnabled(Api.Name, out var enabled) || !enabled) return;
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(Api.Name + Api.Description + "Helper"));
            TypeReference elementTypeReference;
            switch (elementTypeName)
            {
                case "Double":
                    elementTypeReference = mainModule.TypeSystem.Double;
                    break;
                case "Single":
                    elementTypeReference = mainModule.TypeSystem.Single;
                    break;
                case "Int32":
                    elementTypeReference = mainModule.TypeSystem.Int32;
                    break;
                case "UInt32":
                    elementTypeReference = mainModule.TypeSystem.UInt32;
                    break;
                case "Int64":
                    elementTypeReference = mainModule.TypeSystem.Int64;
                    break;
                case "UInt64":
                    elementTypeReference = mainModule.TypeSystem.UInt64;
                    break;
                default: throw new ArgumentException();
            }
            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                GenerateEach(name, isSpecial, @static, mainModule, systemModule, elementTypeReference);
            }
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule, TypeReference elementTypeReference)
        {
            var returnTypeDefinition = mainModule.GetType("UniNativeLinq", Api.Name + "Enumerable`4");

            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var TComparer = mainModule.GetType("UniNativeLinq", "DefaultOrderByAscending" + elementTypeName);

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = elementTypeReference.MakeSpecialTypePair(name);
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, elementTypeReference, TComparer }
                };
                switch (name)
                {
                    case "T[]":
                    case "NativeArray<T>":
                        GenerateSpecial(method, baseEnumerable, enumerable);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var type = Dictionary[name];
                var (enumerable, enumerator, _) = elementTypeReference.MakeFromCommonType(method, type, "0");
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, elementTypeReference, TComparer }
                };
                GenerateNormal(method, enumerable);
            }
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.DefineAllocatorParam();
            method.Body.GetILProcessor()
                .LdArgs(0, 2)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }

        public void GenerateSpecial(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.DefineAllocatorParam();

            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(enumerable));

            body.GetILProcessor()
                .LdLocA(0)
                .LdArg(0)
                .Call(enumerable.FindMethod(".ctor", 1))

                .LdLocA(0)
                .LdArg(1)
                .NewObj(method.ReturnType.FindMethod(".ctor", 2))
                .Ret();
        }
    }
}