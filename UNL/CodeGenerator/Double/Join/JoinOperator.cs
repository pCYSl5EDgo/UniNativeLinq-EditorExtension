using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class JoinOperator : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public readonly IDoubleApi Api;
        public JoinOperator(IDoubleApi api) => Api = api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            if (!processor.TryGetEnabled("Join", out var enabled) || !enabled) return;
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(nameof(JoinOperator) + "Helper"));
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
            var method = new MethodDefinition("Join", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
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
                GenerateNormalNormal(Dictionary[rowName], Dictionary[columnName], mainModule, method);
            }
        }

        private void GenerateSpecialSpecial(string rowName, string columnName, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var (key, keyEqualityComparer, T, refFunc2) = Prepare(method, mainModule);

            var (element0, baseEnumerable0, enumerable0, enumerator0, keySelector0) = DefineWithSpecial(rowName, method, refFunc2, key, 0, systemModule);
            var (element1, baseEnumerable1, enumerable1, enumerator1, keySelector1) = DefineWithSpecial(columnName, method, refFunc2, key, 1, systemModule);

            ParameterDefinition outer = new ParameterDefinition(nameof(outer), ParameterAttributes.None, baseEnumerable0);
            method.Parameters.Add(outer);

            ParameterDefinition inner = new ParameterDefinition(nameof(inner), ParameterAttributes.None, baseEnumerable1);
            method.Parameters.Add(inner);

            Epilogue(method, mainModule, element0, element1, T, enumerable0, enumerator0, enumerable1, enumerator1, key, keySelector0, keySelector1, keyEqualityComparer, out var @return);

            method.Body.GetILProcessor()
                .LdConvArg(enumerable0, 0)
                .LdConvArg(enumerable1, 1)
                .LdArgs(2, 5)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private void GenerateSpecialNormal(string specialName, TypeDefinition type0, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method, int specialIndex)
        {
            var (key, keyEqualityComparer, T, refFunc2) = Prepare(method, mainModule);

            GenericInstanceType enumerable0;
            TypeReference enumerator0;
            TypeReference element0;
            GenericParameter keySelector0;
            TypeReference element1;
            TypeReference baseEnumerable;
            GenericInstanceType enumerable1;
            TypeReference enumerator1;
            GenericParameter keySelector1;

            if (specialIndex == 0)
            {
                (element0, baseEnumerable, enumerable0, enumerator0, keySelector0) = DefineWithSpecial(specialName, method, refFunc2, key, specialIndex, systemModule);
                Routine(type0, method, "1", refFunc2, key, out enumerable1, out enumerator1, out element1, out keySelector1);

                ParameterDefinition outer = new ParameterDefinition(nameof(outer), ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(outer);

                ParameterDefinition inner = new ParameterDefinition(nameof(inner), ParameterAttributes.In, new ByReferenceType(enumerable1));
                inner.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
                method.Parameters.Add(inner);
            }
            else
            {
                Routine(type0, method, "0", refFunc2, key, out enumerable0, out enumerator0, out element0, out keySelector0);
                (element1, baseEnumerable, enumerable1, enumerator1, keySelector1) = DefineWithSpecial(specialName, method, refFunc2, key, specialIndex, systemModule);

                ParameterDefinition outer = new ParameterDefinition(nameof(outer), ParameterAttributes.In, new ByReferenceType(enumerable0));
                outer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
                method.Parameters.Add(outer);

                ParameterDefinition inner = new ParameterDefinition(nameof(inner), ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(inner);
            }

            Epilogue(method, mainModule, element0, element1, T, enumerable0, enumerator0, enumerable1, enumerator1, key, keySelector0, keySelector1, keyEqualityComparer, out var @return);

            var processor = method.Body.GetILProcessor();

            if (specialIndex == 0)
            {
                processor
                    .LdConvArg(enumerable0, 0)
                    .LdArg(1);
            }
            else
            {
                processor
                    .LdArg(0)
                    .LdConvArg(enumerable1, 1);
            }

            processor
                .LdArgs(2, 5)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private static (TypeReference element0, TypeReference baseEnumerable, GenericInstanceType enumerable0, TypeReference enumerator0, GenericParameter keySelector0) DefineWithSpecial(string specialName, MethodDefinition method, TypeDefinition refFunc2, GenericParameter key, int specialIndex, ModuleDefinition systemModule)
        {
            TypeReference element = method.DefineUnmanagedGenericParameter("TSpecial" + specialIndex);
            method.GenericParameters.Add((GenericParameter)element);
            var (baseEnumerable, enumerable, enumerator) = ((GenericParameter)element).MakeSpecialTypePair(specialName);
            var keySelector = InternalRoutine(method, specialIndex.ToString(), refFunc2, key, element);
            return (element, baseEnumerable, enumerable, enumerator, keySelector);
        }

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, ModuleDefinition mainModule, MethodDefinition method)
        {
            var (key, keyEqualityComparer, T, refFunc2) = Prepare(method, mainModule);

            Routine(type0, method, "0", refFunc2, key, out var enumerable0, out var enumerator0, out var element0, out var keySelector0);

            Routine(type1, method, "1", refFunc2, key, out var enumerable1, out var enumerator1, out var element1, out var keySelector1);

            DefineOuterInner(method, enumerable0, enumerable1);

            Epilogue(method, mainModule, element0, element1, T, enumerable0, enumerator0, enumerable1, enumerator1, key, keySelector0, keySelector1, keyEqualityComparer, out var @return);

            method.Body.GetILProcessor()
                .LdArgs(0, 7)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private static void DefineOuterInner(MethodDefinition method, GenericInstanceType enumerable0, GenericInstanceType enumerable1)
        {
            ParameterDefinition outer = new ParameterDefinition(nameof(outer), ParameterAttributes.In, new ByReferenceType(enumerable0));
            outer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            ParameterDefinition inner = new ParameterDefinition(nameof(inner), ParameterAttributes.In, new ByReferenceType(enumerable1));
            inner.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(inner);
        }

        private static (GenericParameter TKey, GenericParameter TKeyEqualityComparer, GenericParameter T, TypeDefinition IRefFunc2) Prepare(MethodDefinition method, ModuleDefinition mainModule)
        {
            GenericParameter TKey = new GenericParameter(nameof(TKey), method)
            {
                HasNotNullableValueTypeConstraint = true,
                CustomAttributes = { Helper.UnManagedAttribute }
            };
            method.GenericParameters.Add(TKey);

            GenericParameter TKeyEqualityComparer = new GenericParameter(nameof(TKeyEqualityComparer), method)
            {
                HasNotNullableValueTypeConstraint = true,
                Constraints =
                {
                    new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`3"))
                    {
                        GenericArguments = { TKey, TKey, mainModule.TypeSystem.Boolean }
                    }
                }
            };
            method.GenericParameters.Add(TKeyEqualityComparer);

            GenericParameter T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(Helper.UnManagedAttribute);
            method.GenericParameters.Add(T);

            var IRefFunc2 = mainModule.GetType("UniNativeLinq", "IRefFunc`2");
            return (TKey, TKeyEqualityComparer, T, IRefFunc2);
        }

        private static void Routine(TypeReference type, MethodDefinition method, string suffix, TypeDefinition refFunc2, GenericParameter key, out GenericInstanceType enumerable, out TypeReference enumerator, out TypeReference element, out GenericParameter keySelector)
        {
            var added0 = method.FromTypeToMethodParam(type.GenericParameters, suffix);
            enumerable = type.MakeGenericInstanceType(added0);
            enumerator = enumerable.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix);
            element = enumerable.GetElementTypeOfCollectionType().Replace(added0, suffix);

            keySelector = InternalRoutine(method, suffix, refFunc2, key, element);
        }

        private static GenericParameter InternalRoutine(MethodDefinition method, string suffix, TypeDefinition refFunc2, GenericParameter key, TypeReference element)
        {
            GenericParameter keySelector = new GenericParameter("TKeySelector" + suffix, method) { HasNotNullableValueTypeConstraint = true };
            keySelector.Constraints.Add(new GenericInstanceType(refFunc2)
            {
                GenericArguments = {
                    element,
                    key
                }
            });
            method.GenericParameters.Add(keySelector);
            return keySelector;
        }

        private static void Epilogue(MethodDefinition method, ModuleDefinition mainModule,
            TypeReference element0, TypeReference element1, GenericParameter T, GenericInstanceType enumerable0,
            TypeReference enumerator0, GenericInstanceType enumerable1, TypeReference enumerator1, GenericParameter key,
            GenericParameter tKeySelector0, GenericParameter tKeySelector1, TypeReference tKeyEqualityComparer,
            out GenericInstanceType @return)
        {
            var tSelector = new GenericParameter("TSelector", method) { HasNotNullableValueTypeConstraint = true };
            tSelector.Constraints.Add(new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`3"))
            {
                GenericArguments = {
                    element0,
                    element1,
                    T
                }
            });
            method.GenericParameters.Add(tSelector);

            @return = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "JoinEnumerable`12"))
            {
                GenericArguments =
                {
                    enumerable0,
                    enumerator0,
                    element0,
                    enumerable1,
                    enumerator1,
                    element1,
                    key,
                    tKeySelector0,
                    tKeySelector1,
                    tKeyEqualityComparer,
                    T,
                    tSelector,
                }
            };
            method.ReturnType = @return;

            ParameterDefinition outerSelector = new ParameterDefinition(nameof(outerSelector), ParameterAttributes.In, new ByReferenceType(tKeySelector0));
            outerSelector.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(outerSelector);

            ParameterDefinition innerSelector = new ParameterDefinition(nameof(innerSelector), ParameterAttributes.In, new ByReferenceType(tKeySelector1));
            innerSelector.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(innerSelector);

            ParameterDefinition comparer = new ParameterDefinition(nameof(comparer), ParameterAttributes.In, new ByReferenceType(tKeyEqualityComparer));
            comparer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(comparer);

            ParameterDefinition selector = new ParameterDefinition(nameof(selector), ParameterAttributes.In, new ByReferenceType(tSelector));
            selector.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(selector);

            ParameterDefinition allocator = new ParameterDefinition(nameof(allocator), ParameterAttributes.HasDefault | ParameterAttributes.Optional, Helper.Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
        }
    }
}