using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public static class DoubleApiHelper
    {
        public static bool ShouldDefine(this IDoubleApi api, string[] array)
        {
            foreach (var element0 in array)
            {
                foreach (var element1 in array)
                {
                    if (!api.TryGetEnabled(element0, element1, out var apiEnabled) || !apiEnabled) continue;
                    return true;
                }
            }
            return false;
        }

        public static TypeDefinition DefineStatic(this ModuleDefinition mainModule, string name)
            => new TypeDefinition("UniNativeLinq",
            name,
            Helper.StaticExtensionClassTypeAttributes, mainModule.TypeSystem.Object)
            {
                CustomAttributes = { Helper.ExtensionAttribute }
            };

        public static ILProcessor CpObjFromArgumentToField(this ILProcessor processor, TypeReference type, int variableIndex, FieldReference to)
        {
            return processor
                .LdFldA(to)
                .LdArg(variableIndex)
                .CpObj(type);
        }

        public static void HelpWithGenerate(this IDoubleApi Api, IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, Action<string, bool, string, bool, TypeDefinition, ModuleDefinition> GenerateEachPair)
        {
            if (!processor.TryGetEnabled(Api.Name, out var enabled) || !enabled) return;
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(Api.Name + Api.Description + "Helper"));
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

                    GenerateEachPair(rowName, isRowSpecial, columnName, isColumnSpecial, @static, mainModule);
                }
            }
        }

        public static void HelpWithGenerate(this IDoubleApi Api, IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, Action<string, bool, string, bool, TypeDefinition, ModuleDefinition, ModuleDefinition> GenerateEachPair)
        {
            if (!processor.TryGetEnabled(Api.Name, out var enabled) || !enabled) return;
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(Api.Name + Api.Description + "Helper"));
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
    }
}