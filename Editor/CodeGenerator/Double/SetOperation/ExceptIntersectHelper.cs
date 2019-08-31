using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace UniNativeLinq.Editor.CodeGenerator
{
    public static class ExceptIntersectHelper
    {
        // ReSharper disable once InconsistentNaming
        public static void HelpExceptIntersect(this IDoubleApi Api, IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, Action<string, bool, string, bool, TypeDefinition, ModuleDefinition> GenerateEachPair)
        {
            if (!processor.TryGetEnabled("SetOperation", out var enabled) || !enabled) return;
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

        // ReSharper disable once InconsistentNaming
        public static void HelpExceptIntersect(this IDoubleApi Api, IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, Action<string, bool, string, bool, TypeDefinition, ModuleDefinition, ModuleDefinition> GenerateEachPair)
        {
            if (!processor.TryGetEnabled("SetOperation", out var enabled) || !enabled) return;
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