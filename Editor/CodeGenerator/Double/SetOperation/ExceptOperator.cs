using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming
namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class ExceptOperator : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public readonly IDoubleApi Api;
        public ExceptOperator(IDoubleApi api) => Api = api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.HelpExceptIntersect(processor, mainModule, GenerateEachPair);
        }

        private void GenerateEachPair(string rowName, bool isRowSpecial, string columnName, bool isColumnSpecial, TypeDefinition @static, ModuleDefinition mainModule)
        {
            var method = new MethodDefinition(Api.Name, Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
            {
                DeclaringType = @static,
                AggressiveInlining = true,
            };
            method.CustomAttributes.Add(Helper.ExtensionAttribute);
            @static.Methods.Add(method);
            if (isRowSpecial && isColumnSpecial)
            {
                GenerateSpecialSpecial(rowName, columnName, mainModule, method);
            }
            else if (isRowSpecial)
            {
                GenerateSpecialNormal(specialName: rowName, type: Dictionary[columnName], mainModule, method, specialIndex: 0);
            }
            else if (isColumnSpecial)
            {
                GenerateSpecialNormal(specialName: columnName, type: Dictionary[rowName], mainModule, method, specialIndex: 1);
            }
            else
            {
                GenerateNormalNormal(Dictionary[rowName], Dictionary[columnName], mainModule, method);
            }
        }

        private void GenerateSpecialSpecial(string rowName, string columnName, ModuleDefinition mainModule, MethodDefinition method)
        {
            var T = DefineT(method);
            var (baseEnumerable0, enumerable0, enumerator0) = T.MakeSpecialTypePair(rowName);
            var (baseEnumerable1, enumerable1, enumerator1) = T.MakeSpecialTypePair(columnName);
            var TComparer = DefineTComparer(mainModule, method, T);
            var (TSetOperation, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TComparer);

            var param0 = new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable0);
            method.Parameters.Add(param0);
            var param1 = new ParameterDefinition("second", ParameterAttributes.None, baseEnumerable1);
            method.Parameters.Add(param1);
            DefineComparer(method, TComparer);
            DefineAllocator(method);

            var body = method.Body;
            body.Variables.Add(new VariableDefinition(TSetOperation));

            body.GetILProcessor()
                .LdConvArg(enumerable0, 0)
                .LdConvArg(enumerable1, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                .LdLocA(0)
                .LdArg(3)
                .NewObj(@return.FindMethod(".ctor", 4))
                .Ret();
        }

        private void GenerateSpecialNormal(string specialName, TypeDefinition type, ModuleDefinition mainModule, MethodDefinition method, int specialIndex)
        {
            var T = DefineT(method);
            var TComparer = DefineTComparer(mainModule, method, T);
            var body = method.Body;

            if (specialIndex == 0)
            {
                var (baseEnumerable, enumerable0, enumerator0) = T.MakeSpecialTypePair(specialName);
                var (enumerable1, enumerator1, _) = T.MakeFromCommonType(method, type, "1");
                var (TSetOperation, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TComparer);

                var param0 = new ParameterDefinition("@this", ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(param0);
                var param1 = new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1));
                param1.CustomAttributes.Add(Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference());
                method.Parameters.Add(param1);
                DefineComparer(method, TComparer);
                DefineAllocator(method);
                body.Variables.Add(new VariableDefinition(TSetOperation));

                body.GetILProcessor()
                    .LdConvArg(enumerable0, 0)
                    .LdArg(1)
                    .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                    .LdLocA(0)
                    .LdArg(3)
                    .NewObj(@return.FindMethod(".ctor", 4))
                    .Ret();
            }
            else
            {
                var (enumerable0, enumerator0, _) = T.MakeFromCommonType(method, type, "0");
                var (baseEnumerable, enumerable1, enumerator1) = T.MakeSpecialTypePair(specialName);
                var (TSetOperation, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TComparer);

                var param0 = new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0));
                param0.CustomAttributes.Add(Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference());
                method.Parameters.Add(param0);
                var param1 = new ParameterDefinition("second", ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(param1);
                DefineComparer(method, TComparer);
                DefineAllocator(method);
                body.Variables.Add(new VariableDefinition(TSetOperation));

                body.GetILProcessor()
                    .LdArg(0)
                    .LdConvArg(enumerable1, 1)
                    .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                    .LdLocA(0)
                    .LdArg(3)
                    .NewObj(@return.FindMethod(".ctor", 4))
                    .Ret();
            }
        }

        private static GenericParameter DefineTComparer(ModuleDefinition mainModule, MethodDefinition method, GenericParameter T)
        {
            var TComparer = new GenericParameter("TComparer", method)
            {
                HasNotNullableValueTypeConstraint = true,
                Constraints =
                {
                    new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`3"))
                    {
                        GenericArguments = {T, T, mainModule.TypeSystem.Int32}
                    }
                }
            };
            method.GenericParameters.Add(TComparer);
            return TComparer;
        }

        private static void DefineAllocator(MethodDefinition method)
        {
            ParameterDefinition allocator = new ParameterDefinition(nameof(allocator), ParameterAttributes.HasDefault | ParameterAttributes.Optional, Helper.Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
        }

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, ModuleDefinition mainModule, MethodDefinition method)
        {
            var T = DefineT(method);

            var (enumerable0, enumerator0, _) = T.MakeFromCommonType(method, type0, "0");
            var (enumerable1, enumerator1, _) = T.MakeFromCommonType(method, type1, "1");
            var TComparer = DefineTComparer(mainModule, method, T);
            var (TSetOperation, @return) = Epilogue(mainModule, method, enumerable0, enumerator0, enumerable1, enumerator1, T, TComparer);
            var systemRuntimeCompilerServicesReadonlyAttributeTypeReference = Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference();
            var param0 = new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0));
            param0.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(param0);
            var param1 = new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1));
            param1.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(param1);
            DefineComparer(method, TComparer);
            DefineAllocator(method);

            var body = method.Body;
            body.Variables.Add(new VariableDefinition(TSetOperation));

            body.GetILProcessor()
                .LdArgs(0, 2)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                .LdLocA(0)
                .LdArg(3)
                .NewObj(@return.FindMethod(".ctor", 4))
                .Ret();
        }

        private static void DefineComparer(MethodDefinition method, GenericParameter TComparer)
        {
            method.Parameters.Add(new ParameterDefinition("comparer", ParameterAttributes.None, TComparer));
        }

        private (GenericInstanceType TSetOperation, GenericInstanceType @return) Epilogue(ModuleDefinition mainModule, IMethodSignature method, TypeReference enumerable0, TypeReference enumerator0, TypeReference enumerable1, TypeReference enumerator1, TypeReference T, TypeReference TComparer)
        {
            var TSetOperation = new GenericInstanceType(mainModule.GetType("UniNativeLinq", Api.Name + "Operation`6"))
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

        private static GenericParameter DefineT(MethodDefinition method)
        {
            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);
            return T;
        }
    }
}