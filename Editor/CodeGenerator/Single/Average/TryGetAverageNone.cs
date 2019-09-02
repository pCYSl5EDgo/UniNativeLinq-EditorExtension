using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;
using GenericInstanceType = Mono.Cecil.GenericInstanceType;

// ReSharper disable InconsistentNaming


namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class TryGetAverageNone
        : ITypeDictionaryHolder,
            IApiExtensionMethodGenerator
    {
        public TryGetAverageNone(ISingleApi api)
        {
            Api = api;
            returnTypeName = api.Description;
        }

        public readonly ISingleApi Api;
        private readonly string returnTypeName;

        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        private bool IsNullable => returnTypeName[0] == 'N';

        private TypeReference CalcElementTypeReference(ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            switch (returnTypeName)
            {
                case "Double":
                    return mainModule.TypeSystem.Double;
                case "Single":
                    return mainModule.TypeSystem.Single;
                case "Int32":
                    return mainModule.TypeSystem.Int32;
                case "UInt32":
                    return mainModule.TypeSystem.UInt32;
                case "Int64":
                    return mainModule.TypeSystem.Int64;
                case "UInt64":
                    return mainModule.TypeSystem.UInt64;
                case "Nullable<Double>":
                    return new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Nullable`1")))
                    {
                        GenericArguments = { mainModule.TypeSystem.Double }
                    };
                case "Nullable<Single>":
                    return new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Nullable`1")))
                    {
                        GenericArguments = { mainModule.TypeSystem.Single }
                    };
                case "Nullable<Int32>":
                    return new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Nullable`1")))
                    {
                        GenericArguments = { mainModule.TypeSystem.Int32 }
                    };
                case "Nullable<UInt32>":
                    return new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Nullable`1")))
                    {
                        GenericArguments = { mainModule.TypeSystem.UInt32 }
                    };
                case "Nullable<Int64>":
                    return new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Nullable`1")))
                    {
                        GenericArguments = { mainModule.TypeSystem.Int64 }
                    };
                case "Nullable<UInt64>":
                    return new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Nullable`1")))
                    {
                        GenericArguments = { mainModule.TypeSystem.UInt64 }
                    };
                default: throw new Exception();
            }
        }

        private TypeReference CalcArgumentTypeReference(ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            switch (returnTypeName)
            {
                case "Single":
                    return mainModule.TypeSystem.Single;
                case "Double":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                    return mainModule.TypeSystem.Double;
                case "Nullable<Double>":
                case "Nullable<Int32>":
                case "Nullable<UInt32>":
                case "Nullable<Int64>":
                case "Nullable<UInt64>":
                    return new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Nullable`1")))
                    {
                        GenericArguments = { mainModule.TypeSystem.Double }
                    };
                case "Nullable<Single>":
                    return new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Nullable`1")))
                    {
                        GenericArguments = { mainModule.TypeSystem.Single }
                    };
                default: throw new Exception();
            }
        }

        private Instruction Conv => Instruction.Create(
            returnTypeName == "Single" || returnTypeName == "Nullable<Single>"
                ? OpCodes.Conv_R4
                : returnTypeName == "UInt32" || returnTypeName == "UInt64" || returnTypeName == "Nullable<UInt32>" || returnTypeName == "Nullable<UInt64>"
                  ? OpCodes.Conv_R_Un
                  : OpCodes.Conv_R8);

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(Api.Name + Api.Description + "Helper"));

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
                CustomAttributes = { Helper.ExtensionAttribute }
            };
            @static.Methods.Add(method);

            var T = CalcElementTypeReference(mainModule, systemModule);
            var TArg = CalcArgumentTypeReference(mainModule, systemModule);

            var TOperator = mainModule.GetType("UniNativeLinq.Average", (IsNullable ? "Nullable" : "") + returnTypeName + "Average");

            var Aggregate = mainModule.GetType("UniNativeLinq", "AggregateRefValue1OperatorHelper");

            var Func =
                name == "T[]"
                    ? Aggregate.Methods.First(x => x.Parameters[0].ParameterType.IsArray)
                    : name == "NativeArray<T>"
                        ? Aggregate.Methods.First(x => x.Parameters[0].ParameterType.ToDefinition().Name == "NativeArray`1")
                        : Aggregate.Methods.First(x =>
                        {
                            var parameterType = x.Parameters[0].ParameterType;
                            return !parameterType.IsArray && parameterType.ToDefinition().Name == Dictionary[name].Name;
                        });

            var genericFunc = new GenericInstanceMethod(Func)
            {
                GenericArguments = { T, TArg, TOperator }
            };

            if (isSpecial)
            {
                var (baseEnumerable, _, _) = T.MakeSpecialTypePair(name);

                method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable));
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(TArg)));
            }
            else
            {
                var typeDefinition = Dictionary[name];
                var skipIndex = typeDefinition.GenericParameters.IndexOf(typeDefinition.GenericParameters.First(x => x.Name == "T"));
                var (enumerable, _, _) = T.MakeFromCommonType(method, typeDefinition, "0");

                var genericArguments = (enumerable as GenericInstanceType).GenericArguments;
                foreach (var parameter in genericArguments.Take(skipIndex).Concat(genericArguments.Skip(skipIndex + 1)))
                {
                    genericFunc.GenericArguments.Add(parameter);
                }

                method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable))
                {
                    CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference() }
                });
                method.Parameters.Add(new ParameterDefinition("value", ParameterAttributes.Out, new ByReferenceType(TArg)));
            }



            var body = method.Body;
            body.InitLocals = true;
            body.Variables.Add(new VariableDefinition(TArg));
            body.Variables.Add(new VariableDefinition(TOperator));

            body.GetILProcessor()
                .LdArg(0)
                .LdLocA(0)
                .LdLocA(1)
                .Call(genericFunc)
                .LdLocA(1)
                .LdLoc(0)
                .LdArg(1)
                .Call(TOperator.FindMethod("TryCalculateResult"))
                .Ret();
        }
    }
}