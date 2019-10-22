using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming
using UniNativeLinq.Editor;
using UniNativeLinq.Editor.CodeGenerator;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class Create
        : ITypeDictionaryHolder,
            IApiExtensionMethodGenerator
    {
        public Create(ISingleApi api)
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

            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            @static.Methods.Add(method);

            var T = new GenericParameter("T", method)
            {
                HasNotNullableValueTypeConstraint = true,
                HasDefaultConstructorConstraint = true,
            };
            method.GenericParameters.Add(T);

            var typeDefinition = method.Module.GetType("UniNativeLinq", "NativeEnumerable`1");
            method.ReturnType = new GenericInstanceType(typeDefinition)
            {
                GenericArguments = {T}
            };
            GenerateNativeEnumerable(method, method.ReturnType, T, systemModule);
        }

        private static void GenerateNativeEnumerable(MethodDefinition method, TypeReference enumerable, TypeReference T, ModuleDefinition systemModule)
        {
            method.Parameters.Add(new ParameterDefinition("length", ParameterAttributes.None, method.Module.TypeSystem.Int64));
            method.Parameters.Add(new ParameterDefinition("allocator", ParameterAttributes.None, Helper.Allocator));
            var malloc = new GenericInstanceMethod(method.Module.GetType("UniNativeLinq", "UnsafeUtilityEx").FindMethod("Malloc", 2))
            {
                GenericArguments = {T}
            };
            var create = method.ReturnType.FindMethod("Create", 2);

            method.Body.GetILProcessor()
                .LdArg(0)
                .LdArg(1)
                .Call(malloc)
                .LdArg(0)
                .Call(create)
                .Ret();
        }
    }
}