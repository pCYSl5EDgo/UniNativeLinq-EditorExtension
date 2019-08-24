using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class IntersectNone : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public readonly IDoubleApi Api;
        public IntersectNone(IDoubleApi api) => Api = api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            if (!processor.TryGetEnabled("SetOperation", out var enabled) || !enabled) return;
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(IntersectNone) + "Helper"));
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
            var method = new MethodDefinition("Intersect", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
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
            var (baseEnumerable0, enumerable0, enumerator0) = T.MakeSpecialTypePair(rowName);
            var (baseEnumerable1, enumerable1, enumerator1) = T.MakeSpecialTypePair(columnName);
            var TComparer = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DefaultOrderByAscending`1"))
            {
                GenericArguments = { T }
            };
            var (TSetOperation, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TComparer);

            var param0 = new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable0);
            method.Parameters.Add(param0);
            var param1 = new ParameterDefinition("second", ParameterAttributes.None, baseEnumerable1);
            method.Parameters.Add(param1);
            DefineAllocator(method);

            var body = method.Body;

            body.Variables.Add(new VariableDefinition(TSetOperation));

            body.GetILProcessor()
                .LdConvArg(enumerable0, 0)
                .LdConvArg(enumerable1, 1)
                .LdLocA(0)
                .LdArg(2)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private void GenerateSpecialNormal(string specialName, TypeDefinition type, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method, int specialIndex)
        {
            var T = DefineT(mainModule, systemModule, method);
            var (enumerable0, enumerator0, _) = T.MakeFromCommonType(method, type, "0");
            var (baseEnumerable, enumerable1, enumerator1) = T.MakeSpecialTypePair(specialName);

            var TComparer = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DefaultOrderByAscending`1"))
            {
                GenericArguments = { T }
            };
            var (TSetOperation, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TComparer);

            if (specialIndex == 0)
            {
                var param0 = new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(param0);
                var param1 = new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(type));
                param1.CustomAttributes.Add(Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference());
                method.Parameters.Add(param1);
                DefineAllocator(method);

                var body = method.Body;

                body.Variables.Add(new VariableDefinition(TSetOperation));

                body.GetILProcessor()
                    .LdConvArg(enumerable1, 0)
                    .LdArg(1)
                    .LdLocA(0)
                    .LdArg(2)
                    .NewObj(@return.FindMethod(".ctor"))
                    .Ret();
            }
            else
            {
                var param0 = new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(type));
                param0.CustomAttributes.Add(Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference());
                method.Parameters.Add(param0);
                var param1 = new ParameterDefinition("second", ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(param1);
                DefineAllocator(method);

                var body = method.Body;

                body.Variables.Add(new VariableDefinition(TSetOperation));

                body.GetILProcessor()
                    .LdArg(0)
                    .LdConvArg(enumerable1, 1)
                    .LdLocA(0)
                    .LdArg(2)
                    .NewObj(@return.FindMethod(".ctor"))
                    .Ret();
            }
        }

        private static void DefineAllocator(MethodDefinition method)
        {
            ParameterDefinition allocator = new ParameterDefinition(nameof(allocator), ParameterAttributes.HasDefault | ParameterAttributes.Optional, Helper.Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
        }

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var T = DefineT(mainModule, systemModule, method);

            var (enumerable0, enumerator0, _) = T.MakeFromCommonType(method, type0, "0");
            var (enumerable1, enumerator1, _) = T.MakeFromCommonType(method, type1, "1");

            var TComparer = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DefaultOrderByAscending`1"))
            {
                GenericArguments = { T }
            };
            var (TSetOperation, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TComparer);
            var systemRuntimeCompilerServicesReadonlyAttributeTypeReference = Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference();
            var param0 = new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(type0));
            param0.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(param0);
            var param1 = new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(type1));
            param1.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(param1);
            DefineAllocator(method);

            var body = method.Body;

            body.Variables.Add(new VariableDefinition(TSetOperation));

            body.GetILProcessor()
                .LdArgs(0, 2)
                .LdLocA(0)
                .LdArg(2)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private static (GenericInstanceType TSetOperation, GenericInstanceType @return) Epilogue(ModuleDefinition mainModule, MethodDefinition method, TypeReference enumerable0, TypeReference enumerator0, TypeReference enumerable1, TypeReference enumerator1, TypeReference T, TypeReference TComparer)
        {
            var TSetOperation = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IntersectOperation`6"))
            {
                GenericArguments =
                {
                    enumerable0,
                    enumerator0,
                    enumerable1,
                    enumerator1,
                    T,
                    TComparer,
                }
            };
            var @return = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "SetOperationEnumerable`6"))
            {
                GenericArguments =
                {
                    enumerable0,
                    enumerator0,
                    enumerable1,
                    enumerator1,
                    T,
                    TSetOperation,
                }
            };
            method.ReturnType = @return;
            return (TSetOperation, @return);
        }

        private static GenericParameter DefineT(ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var T = new GenericParameter("T", method)
            {
                HasNotNullableValueTypeConstraint = true,
                CustomAttributes = { Helper.GetSystemRuntimeInteropServicesUnmanagedTypeConstraintTypeReference() },
                Constraints =
                {
                    new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "IComparable`1")))
                    {
                        GenericArguments = {mainModule.TypeSystem.Boolean}
                    }
                }
            };
            ((GenericInstanceType)T.Constraints[0]).GenericArguments[0] = T;
            method.GenericParameters.Add(T);
            return T;
        }
    }
}