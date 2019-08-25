using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming
namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class UnionFunc : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public readonly IDoubleApi Api;
        public UnionFunc(IDoubleApi api) => Api = api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }
        const string ConcatFirstEnumerable = "FirstEnumerable";
        const string ConcatSecondEnumerable = "SecondEnumerable";

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            if (!processor.TryGetEnabled("Concat", out var enabled) || !enabled || !processor.TryGetEnabled("Distinct", out enabled) || !enabled) return;
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(UnionFunc) + "Helper"));
            var count = Api.Count;
            for (var row = 0; row < count; row++)
            {
                var rowName = Api.NameCollection[row];
                if (!processor.IsSpecialType(rowName, out var isRowSpecial)) throw new KeyNotFoundException();

                for (var column = 0; column < count; column++)
                {
                    var columnName = Api.NameCollection[column];
                    if (!processor.IsSpecialType(columnName, out var isColumnSpecial)) throw new KeyNotFoundException();

                    if (!Api.TryGetEnabled(rowName, columnName, out var apiEnabled) || !apiEnabled) continue;

                    GenerateEachPair(rowName, isRowSpecial, columnName, isColumnSpecial, @static, mainModule, systemModule);
                }
            }
        }

        private void GenerateEachPair(string rowName, bool isRowSpecial, string columnName, bool isColumnSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition("Union", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(Helper.ExtensionAttribute);
            @static.Methods.Add(method);
            if (isRowSpecial && isColumnSpecial)
            {
                GenerateSpecialSpecial(rowName, columnName, mainModule, systemModule, method);
            }
            else if (isRowSpecial)
            {
                GenerateSpecialNormal(specialName: rowName, type: Dictionary[columnName], mainModule, systemModule, method, specialIndex: 0);
            }
            else if (isColumnSpecial)
            {
                GenerateSpecialNormal(specialName: columnName, type: Dictionary[rowName], mainModule, systemModule, method, specialIndex: 1);
            }
            else
            {
                GenerateNormalNormal(Dictionary[rowName], Dictionary[columnName], mainModule, systemModule, method);
            }
        }

        private void GenerateSpecialSpecial(string rowName, string columnName, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var T = DefineT(mainModule, systemModule, method);
            var TEqualityComparer = DefineEqualityComparer(mainModule, T);
            var EqualityComparer = DefineArgumentComparer(mainModule, systemModule, T);
            var (baseEnumerable0, enumerable0, enumerator0) = T.MakeSpecialTypePair(rowName);
            var (baseEnumerable1, enumerable1, enumerator1) = T.MakeSpecialTypePair(columnName);
            var (concatEnumerable, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TEqualityComparer);
            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable0));
            method.Parameters.Add(new ParameterDefinition("second", ParameterAttributes.None, baseEnumerable1));
            DefineComparer(method, EqualityComparer);
            DefineAllocator(method);
            var body = method.Body;
            body.Variables.Add(new VariableDefinition(concatEnumerable));
            body.Variables.Add(new VariableDefinition(TEqualityComparer));

            body.GetILProcessor()
                .LdLocA(0)
                .Dup()
                .Dup()
                .LdFldA(concatEnumerable.FindField(ConcatFirstEnumerable))
                .LdArg(0)
                .Call(enumerable0.FindMethod(".ctor"))
                .LdFldA(concatEnumerable.FindField(ConcatSecondEnumerable))
                .LdArg(1)
                .Call(enumerable1.FindMethod(".ctor"))
                .LdArg(2)
                .StLoc(1)
                .LdLocA(1)
                .LdArg(3)
                .NewObj(@return.FindMethod(".ctor", 3))
                .Ret();
        }

        private TypeReference DefineArgumentComparer(ModuleDefinition mainModule, ModuleDefinition systemModule, GenericParameter T)
        {
            return new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`3")))
            {
                GenericArguments = { T, T, mainModule.TypeSystem.Boolean }
            };
        }

        private void GenerateSpecialNormal(string specialName, TypeDefinition type, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method, int specialIndex)
        {
            var T = DefineT(mainModule, systemModule, method);
            var TEqualityComparer = DefineEqualityComparer(mainModule, T);
            var EqualityComparer = DefineArgumentComparer(mainModule, systemModule, T);
            var body = method.Body;
            var processor = body.GetILProcessor();
            if (specialIndex == 0)
            {
                var (baseEnumerable0, enumerable0, enumerator0) = T.MakeSpecialTypePair(specialName);
                var (enumerable1, enumerator1, _) = T.MakeFromCommonType(method, type, "1");
                var (concatEnumerable, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TEqualityComparer);
                method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable0));
                method.Parameters.Add(new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1))
                {
                    CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
                });
                DefineComparer(method, EqualityComparer);
                DefineAllocator(method);
                body.Variables.Add(new VariableDefinition(concatEnumerable));
                body.Variables.Add(new VariableDefinition(TEqualityComparer));

                processor
                    .LdLocA(0)
                    .Dup(2)
                    .LdFldA(concatEnumerable.FindField(ConcatFirstEnumerable))
                    .LdArg(0)
                    .Call(enumerable0.FindMethod(".ctor"))
                    .CpObjFromArgumentToField(enumerable1, 1, concatEnumerable.FindField(ConcatSecondEnumerable))
                    .LdArg(2)
                    .StLoc(1)
                    .LdLocA(1)
                    .LdArg(3)
                    .NewObj(@return.FindMethod(".ctor", 3))
                    .Ret();
            }
            else
            {
                var (enumerable0, enumerator0, _) = T.MakeFromCommonType(method, type, "0");
                var (baseEnumerable1, enumerable1, enumerator1) = T.MakeSpecialTypePair(specialName);
                var (concatEnumerable, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TEqualityComparer);
                method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0))
                {
                    CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
                });
                method.Parameters.Add(new ParameterDefinition("second", ParameterAttributes.None, baseEnumerable1));
                DefineComparer(method, EqualityComparer);
                DefineAllocator(method);
                body.Variables.Add(new VariableDefinition(concatEnumerable));
                body.Variables.Add(new VariableDefinition(TEqualityComparer));

                processor
                    .LdLocA(0)
                    .Dup(2)
                    .CpObjFromArgumentToField(enumerable0, 0, concatEnumerable.FindField(ConcatFirstEnumerable))
                    .LdFldA(concatEnumerable.FindField(ConcatSecondEnumerable))
                    .LdArg(1)
                    .Call(enumerable1.FindMethod(".ctor"))
                    .LdArg(2)
                    .StLoc(1)
                    .LdLocA(1)
                    .LdArg(3)
                    .NewObj(@return.FindMethod(".ctor", 3))
                    .Ret();
            }
        }

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var T = DefineT(mainModule, systemModule, method);
            var TEqualityComparer = DefineEqualityComparer(mainModule, T);
            var EqualityComparer = DefineArgumentComparer(mainModule, systemModule, T);

            var (enumerable0, enumerator0, _) = T.MakeFromCommonType(method, type0, "0");
            var (enumerable1, enumerator1, _) = T.MakeFromCommonType(method, type1, "1");

            var (concatEnumerable, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TEqualityComparer);

            method.Parameters.Add(new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            method.Parameters.Add(new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1))
            {
                CustomAttributes = { Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference() }
            });
            DefineComparer(method, EqualityComparer);
            DefineAllocator(method);

            var body = method.Body;
            body.Variables.Add(new VariableDefinition(concatEnumerable));
            body.Variables.Add(new VariableDefinition(TEqualityComparer));
            body.GetILProcessor()
                .LdLocA(0)
                .Dup(2)
                .CpObjFromArgumentToField(enumerable0, 0, concatEnumerable.FindField(ConcatFirstEnumerable))
                .CpObjFromArgumentToField(enumerable1, 1, concatEnumerable.FindField(ConcatSecondEnumerable))
                .LdArg(2)
                .StLoc(1)
                .LdLocA(1)
                .LdArg(3)
                .NewObj(@return.FindMethod(".ctor", 3))
                .Ret();
        }

        private static TypeReference DefineEqualityComparer(ModuleDefinition mainModule, GenericParameter T)
        {
            return new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateFuncToStructOperatorFunc`3"))
            {
                GenericArguments = { T, T, mainModule.TypeSystem.Boolean }
            };
        }

        private static (GenericInstanceType ConcatEnumerable, GenericInstanceType @return) Epilogue(ModuleDefinition mainModule, MethodDefinition method, TypeReference enumerable0, TypeReference enumerator0, TypeReference enumerable1, TypeReference enumerator1, TypeReference T, TypeReference TEqualityComparer)
        {
            var ConcatEnumerable = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "ConcatEnumerable`5"))
            {
                GenericArguments =
                {
                    enumerable0,
                    enumerator0,
                    enumerable1,
                    enumerator1,
                    T,
                }
            };

            var ConcatEnumerableEnumerator = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "ConcatEnumerable`5").NestedTypes[0])
            {
                GenericArguments =
                {
                    enumerable0,
                    enumerator0,
                    enumerable1,
                    enumerator1,
                    T,
                }
            };

            var @return = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DistinctEnumerable`4"))
            {
                GenericArguments =
                {
                    ConcatEnumerable,
                    ConcatEnumerableEnumerator,
                    T,
                    TEqualityComparer,
                }
            };
            method.ReturnType = @return;
            return (ConcatEnumerable, @return);
        }

        private static GenericParameter DefineT(ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var T = new GenericParameter("T", method)
            {
                HasNotNullableValueTypeConstraint = true,
                CustomAttributes = { Helper.GetSystemRuntimeInteropServicesUnmanagedTypeConstraintTypeReference() },
                Constraints =
                {
                    new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "IEquatable`1")))
                    {
                        GenericArguments = {mainModule.TypeSystem.Boolean}
                    }
                }
            };
            ((GenericInstanceType)T.Constraints[0]).GenericArguments[0] = T;
            method.GenericParameters.Add(T);
            return T;
        }

        private static void DefineComparer(MethodDefinition method, TypeReference type)
        {
            method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.None, type));
        }

        private static void DefineAllocator(MethodDefinition method)
        {
            method.Parameters.Add(new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Helper.Allocator)
            {
                Constant = 2,
            });
        }
    }
}