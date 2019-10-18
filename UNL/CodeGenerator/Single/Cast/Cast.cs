using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class Cast
        : ITypeDictionaryHolder,
            IApiExtensionMethodGenerator
    {
        public Cast(ISingleApi api)
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
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = {Helper.ExtensionAttribute}
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var TTo = method.DefineUnmanagedGenericParameter("TTo");
            method.GenericParameters.Add(TTo);

            var type = Dictionary[name];
            if (name != "Native") throw new NotSupportedException(name);
            var typeDefinition = method.Module.GetType("UniNativeLinq", "NativeEnumerable`1");
            var enumerable = new GenericInstanceType(typeDefinition)
            {
                GenericArguments = { T }
            };
            method.ReturnType = new GenericInstanceType(typeDefinition)
            {
                GenericArguments = { TTo }
            };
            GenerateNativeEnumerable(method, enumerable, T, TTo, systemModule);
        }

        private static void GenerateNativeEnumerable(MethodDefinition method, TypeReference enumerable, GenericParameter TFrom, TypeReference TTo, ModuleDefinition systemModule)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = {Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference()}
            });

            var create = method.ReturnType.FindMethod("Create", 2);

            var invalidCastException = method.Module.ImportReference(systemModule.GetType("System", nameof(InvalidCastException)));
            
            var success = Instruction.Create(OpCodes.Call, create);

            method.Body.GetILProcessor()
                .LdArg(0)
                .LdFld(enumerable.FindField("Ptr"))
                .LdArg(0)
                .LdFld(enumerable.FindField("Length"))
                .SizeOf(TFrom)
                .ConvI8()
                .Add(Instruction.Create(OpCodes.Mul)) // byte長さ
                .Dup() // 比較用ベース
                .Dup() // 比較用計算後
                .SizeOf(TTo)
                .Add(Instruction.Create(OpCodes.Div))
                .SizeOf(TTo)
                .Add(Instruction.Create(OpCodes.Mul))
                .BeqS(success)
                .Pop()
                .Pop()
                .NewObj(invalidCastException.FindMethod(".ctor", 0))
                .Throw()
                .Add(success)
                .Ret();
        }
    }
}