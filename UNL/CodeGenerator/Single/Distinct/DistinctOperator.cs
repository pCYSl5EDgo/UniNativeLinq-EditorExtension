using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class DistinctOperator : ITypeDictionaryHolder, IApiExtensionMethodGenerator
    {
        public DistinctOperator(ISingleApi api)
        {
            Api = api;
        }

        public readonly ISingleApi Api;

        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.GenerateSingleWithEnumerable(processor, mainModule, systemModule, unityModule, GenerateEach);
        }

        private void GenerateEach(string name, bool isSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var returnTypeDefinition = mainModule.GetType("UniNativeLinq", Api.Name + "Enumerable`4");

            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = method.DefineUnmanagedGenericParameter();
            T.Constraints.Add(new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "IEquatable`1")))
            {
                GenericArguments = { T }
            });
            method.GenericParameters.Add(T);


            var EqualityComparer = new GenericParameter("TEqualityComparer", method)
            {
                HasNotNullableValueTypeConstraint = true,
                Constraints =
                {
                    new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`3"))
                    {
                        GenericArguments = { T, T, mainModule.TypeSystem.Boolean }
                    }
                }
            };
            method.GenericParameters.Add(EqualityComparer);

            if (isSpecial)
            {
                var (baseEnumerable, enumerable, enumerator) = T.MakeSpecialTypePair(name);
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, T, EqualityComparer }
                };
                switch (name)
                {
                    case "T[]":
                    case "NativeArray<T>":
                        GenerateSpecial(method, baseEnumerable, enumerable, T, EqualityComparer);
                        break;
                    default: throw new NotSupportedException(name);
                }
            }
            else
            {
                var type = Dictionary[name];
                var (enumerable, enumerator, _) = T.MakeFromCommonType(method, type, "0");
                method.ReturnType = new GenericInstanceType(returnTypeDefinition)
                {
                    GenericArguments = { enumerable, enumerator, T, EqualityComparer }
                };
                GenerateNormal(method, enumerable, T, EqualityComparer);
            }
        }

        private static void GenerateSpecial(MethodDefinition method, TypeReference baseEnumerable, GenericInstanceType enumerable, GenericParameter T, TypeReference equalityComparer)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
            method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.In, new ByReferenceType(equalityComparer))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
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
                .LdArg(2)
                .NewObj(method.ReturnType.FindMethod(".ctor", 3))
                .Ret();
        }

        private static void GenerateNormal(MethodDefinition method, TypeReference enumerable, GenericParameter T, TypeReference equalityComparer)
        {
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.In, new ByReferenceType(equalityComparer))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
            });
            method.DefineAllocatorParam();

            method.Body.GetILProcessor()
                .LdArgs(0, 3)
                .NewObj(method.ReturnType.FindMethod(".ctor", 3))
                .Ret();
        }
    }
}