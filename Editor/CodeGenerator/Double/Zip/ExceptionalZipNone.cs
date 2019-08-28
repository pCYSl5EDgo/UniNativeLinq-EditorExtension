using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class ExceptionalZipNone : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public readonly IDoubleApi Api;
        public ExceptionalZipNone(IDoubleApi api) => Api = api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            if (!processor.TryGetEnabled("ExceptionalZip", out var enabled) || !enabled) return;
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(ExceptionalZipNone) + "Helper"));
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
            var method = new MethodDefinition("ExceptionalZip", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
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
                GenerateSpecialNormal(rowName, Dictionary[columnName], mainModule, systemModule, method, 0);
            }
            else if (isColumnSpecial)
            {
                GenerateSpecialNormal(columnName, Dictionary[rowName], mainModule, systemModule, method, 1);
            }
            else
            {
                GenerateNormalNormal(Dictionary[rowName], Dictionary[columnName], mainModule, systemModule, method);
            }
        }

        private void GenerateSpecialSpecial(string rowName, string columnName, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var (element0, enumerable0, enumerator0, baseTypeReference0) = DefineWithSpecial(rowName, method, 0, systemModule);
            var (element1, enumerable1, enumerator1, baseTypeReference1) = DefineWithSpecial(columnName, method, 1, systemModule);
            var (T, TAction) = Prepare(element0, element1, mainModule, systemModule);
            var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, element0, enumerable1, enumerator1, element1, T, TAction);
            var param0 = new ParameterDefinition("@this", ParameterAttributes.None, baseTypeReference0);
            method.Parameters.Add(param0);
            var param1 = new ParameterDefinition("second", ParameterAttributes.None, baseTypeReference1);
            method.Parameters.Add(param1);

            method.Body
                .GetILProcessor()
                .LdConvArg(enumerable0, 0)
                .LdConvArg(enumerable1, 1)
                .NewObj(@return.FindMethod(".ctor", 2))
                .Ret();
        }

        private void GenerateSpecialNormal(string specialName, TypeDefinition type0, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method, int specialIndex)
        {
            TypeReference element0;
            TypeReference enumerable0;
            TypeReference enumerator0;
            TypeReference baseTypeReference;
            TypeReference element1;
            TypeReference enumerable1;
            TypeReference enumerator1;
            if (specialIndex == 0)
            {
                (element0, enumerable0, enumerator0, baseTypeReference) = DefineWithSpecial(specialName, method, 0, systemModule);
                (element1, enumerable1, enumerator1) = type0.MakeGenericInstanceVariant("1", method);
            }
            else
            {
                (element0, enumerable0, enumerator0) = type0.MakeGenericInstanceVariant("0", method);
                (element1, enumerable1, enumerator1, baseTypeReference) = DefineWithSpecial(specialName, method, 1, systemModule);
            }
            var (T, TAction) = Prepare(element0, element1, mainModule, systemModule);

            var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, element0, enumerable1, enumerator1, element1, T, TAction);

            if (specialIndex == 0)
            {
                var param0 = new ParameterDefinition("@this", ParameterAttributes.None, baseTypeReference);
                method.Parameters.Add(param0);

                var param1 = new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1));
                param1.CustomAttributes.Add(Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference());
                method.Parameters.Add(param1);

                method.Body
                    .GetILProcessor()
                    .LdConvArg(enumerable0, 0)
                    .LdArg(1)
                    .NewObj(@return.FindMethod(".ctor", 2))
                    .Ret();
            }
            else
            {
                var param0 = new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0));
                param0.CustomAttributes.Add(Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference());
                method.Parameters.Add(param0);

                var param1 = new ParameterDefinition("second", ParameterAttributes.None, baseTypeReference);
                method.Parameters.Add(param1);

                method.Body
                    .GetILProcessor()
                    .LdArg(0)
                    .LdConvArg(enumerable1, 1)
                    .NewObj(@return.FindMethod(".ctor", 2))
                    .Ret();
            }
        }

        private static (TypeReference elemenet, TypeReference enumerable, TypeReference enumerator, TypeReference baseTypeReference) DefineWithSpecial(string specialName, MethodDefinition method, int specialIndex, ModuleDefinition systemModule)
        {
            var element = method.DefineUnmanagedGenericParameter("TSpecial" + specialIndex);
            method.GenericParameters.Add(element);
            var (baseEnumerable, enumerable, enumerator) = element.MakeSpecialTypePair(specialName);
            return (element, enumerable, enumerator, baseEnumerable);
        }

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var (element0, enumerable0, enumerator0) = type0.MakeGenericInstanceVariant("0", method);
            var (element1, enumerable1, enumerator1) = type1.MakeGenericInstanceVariant("1", method);

            var (T, TAction) = Prepare(element0, element1, mainModule, systemModule);

            var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, element0, enumerable1, enumerator1, element1, T, TAction);

            var systemRuntimeCompilerServicesReadonlyAttributeTypeReference = Helper.GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference();

            var param0 = new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0));
            param0.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(param0);

            var param1 = new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1));
            param1.CustomAttributes.Add(systemRuntimeCompilerServicesReadonlyAttributeTypeReference);
            method.Parameters.Add(param1);

            method.Body
                .GetILProcessor()
                .LdArgs(0, 2)
                .NewObj(@return.FindMethod(".ctor", 2))
                .Ret();
        }

        private static TypeReference DefineReturn(ModuleDefinition mainModule, MethodDefinition method, TypeReference enumerable0, TypeReference enumerator0, TypeReference element0, TypeReference enumerable1, TypeReference enumerator1, TypeReference element1, TypeReference T, TypeReference TAction)
        {
            return method.ReturnType = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "ExceptionalZipEnumerable`8"))
            {
                GenericArguments =
                {
                    enumerable0,
                    enumerator0,
                    element0,
                    enumerable1,
                    enumerator1,
                    element1,
                    T,
                    TAction,
                }
            };
        }

        private (TypeReference, TypeReference) Prepare(TypeReference element0, TypeReference element1, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var T = new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "ValueTuple`2")))
            {
                GenericArguments = { element0, element1 }
            };

            var TAction = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "ZipValueTuple`2"))
            {
                GenericArguments = { element0, element1 }
            };

            return (T, TAction);
        }
    }
}