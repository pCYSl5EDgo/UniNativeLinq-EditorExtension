using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class GroupJoinOperator : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public readonly IDoubleApi Api;
        public GroupJoinOperator(IDoubleApi api) => Api = api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
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
                GenerateSpecialNormal(rowName, Dictionary[columnName], mainModule, method, specialIndex: 0);
            }
            else if (isColumnSpecial)
            {
                GenerateSpecialNormal(columnName, Dictionary[rowName], mainModule, method, specialIndex: 1);
            }
            else
            {
                GenerateNormalNormal(Dictionary[rowName], Dictionary[columnName], mainModule, method);
            }
        }

        private static void GenerateSpecialSpecial(string rowName, string columnName, ModuleDefinition mainModule, MethodDefinition method)
        {
            var (key, keyEqualityComparer, T, refFunc2) = Prepare(method, mainModule);
            var (Element0, baseEnumerable0, Enumerable0, Enumerator0) = DefineSpecial(rowName, method, 0);
            var TOuterKeySelector = InternalOuterRoutine(method, refFunc2, Element0, key);
            var (Element1, baseEnumerable1, Enumerable1, Enumerator1) = DefineSpecial(columnName, method, 1);
            var TInnerKeySelector = InternalInnerRoutine(method, refFunc2, Element1, key, mainModule, keyEqualityComparer, out var WhereIndexEnumerable);

            var TSelector = Epilogue(method, mainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, key, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.None, baseEnumerable0);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.None, baseEnumerable1);
            method.Parameters.Add(inner);

            DefineParameters(TOuterKeySelector, method, TInnerKeySelector, keyEqualityComparer, TSelector);

            method.Body.GetILProcessor()
                .LdConvArg(Enumerable0, 0)
                .LdConvArg(Enumerable1, 1)
                .LdArgs(2, 5)
                .NewObj(@return.FindMethod(".ctor", 7))
                .Ret();
        }

        private static void GenerateSpecialNormal(string specialName, TypeDefinition type, ModuleDefinition mainModule, MethodDefinition method, int specialIndex)
        {
            var (key, keyEqualityComparer, T, refFunc2) = Prepare(method, mainModule);


            if (specialIndex == 0)
            {
                var (Element0, baseEnumerable, Enumerable0, Enumerator0) = DefineSpecial(specialName, method, specialIndex);
                var TOuterKeySelector = InternalOuterRoutine(method, refFunc2, Element0, key);

                var Enumerable1 = InnerRoutine(type, method, refFunc2, key, mainModule, keyEqualityComparer, out var Enumerator1, out var Element1, out var TInnerKeySelector, out var WhereIndexEnumerable);

                var TSelector = Epilogue(method, mainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, key, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, out var @return);

                var outer = new ParameterDefinition("outer", ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(outer);

                var inner = new ParameterDefinition("inner", ParameterAttributes.In, new ByReferenceType(Enumerable1));
                inner.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
                method.Parameters.Add(inner);

                DefineParameters(TOuterKeySelector, method, TInnerKeySelector, keyEqualityComparer, TSelector);

                method.Body.GetILProcessor()
                    .LdConvArg(Enumerable0, 0)
                    .LdArgs(1, 6)
                    .NewObj(@return.FindMethod(".ctor", 7))
                    .Ret();
            }
            else
            {
                var Enumerable0 = OuterRoutine(type, method, refFunc2, key, out var Enumerator0, out var Element0, out var TOuterKeySelector);

                var (Element1, baseEnumerable, Enumerable1, Enumerator1) = DefineSpecial(specialName, method, specialIndex);
                var TInnerKeySelector = InternalInnerRoutine(method, refFunc2, Element1, key, mainModule, keyEqualityComparer, out var WhereIndexEnumerable);

                var TSelector = Epilogue(method, mainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, key, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, out var @return);

                var outer = new ParameterDefinition("outer", ParameterAttributes.In, new ByReferenceType(Enumerable0));
                outer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
                method.Parameters.Add(outer);

                var inner = new ParameterDefinition("inner", ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(inner);

                DefineParameters(TOuterKeySelector, method, TInnerKeySelector, keyEqualityComparer, TSelector);

                method.Body.GetILProcessor()
                    .LdArg(0)
                    .LdConvArg(Enumerable1, 1)
                    .LdArgs(2, 5)
                    .NewObj(@return.FindMethod(".ctor", 7))
                    .Ret();
            }
        }

        private static (TypeReference specialTypeReference, TypeReference baseEnumerable, GenericInstanceType enumerable, TypeReference enumerator) DefineSpecial(string specialName, MethodDefinition method, int specialIndex)
        {
            var specialTypeReference = method.DefineUnmanagedGenericParameter("TSpecial" + specialIndex);
            method.GenericParameters.Add(specialTypeReference);
            var (baseEnumerable, enumerable, enumerator) = specialTypeReference.MakeSpecialTypePair(specialName);
            return (specialTypeReference, baseEnumerable, enumerable, enumerator);
        }

        private static void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, ModuleDefinition mainModule, MethodDefinition method)
        {
            var (key, keyEqualityComparer, T, refFunc2) = Prepare(method, mainModule);

            var Enumerable0 = OuterRoutine(type0, method, refFunc2, key, out var Enumerator0, out var Element0, out var TOuterKeySelector);

            var Enumerable1 = InnerRoutine(type1, method, refFunc2, key, mainModule, keyEqualityComparer, out var Enumerator1, out var Element1, out var TInnerKeySelector, out var WhereIndexEnumerable);

            var TSelector = Epilogue(method, mainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, key, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, new ByReferenceType(Enumerable0));
            outer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, new ByReferenceType(Enumerable1));
            inner.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            DefineParameters(TOuterKeySelector, method, TInnerKeySelector, keyEqualityComparer, TSelector);

            method.Body.GetILProcessor()
                .LdArgs(0, 7)
                .NewObj(@return.FindMethod(".ctor", 7))
                .Ret();
        }

        private static void DefineParameters(TypeReference TOuterKeySelector, MethodDefinition method, TypeReference TInnerKeySelector, TypeReference TKeyEqualityComparer, TypeReference TSelector)
        {
            var outerSelector = new ParameterDefinition("outerSelector", ParameterAttributes.In, new ByReferenceType(TOuterKeySelector));
            outerSelector.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(outerSelector);

            var innerSelector = new ParameterDefinition("innerSelector", ParameterAttributes.In, new ByReferenceType(TInnerKeySelector));
            innerSelector.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(innerSelector);

            var comparer = new ParameterDefinition("comparer", ParameterAttributes.In, new ByReferenceType(TKeyEqualityComparer));
            comparer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(comparer);

            var selector = new ParameterDefinition("selector", ParameterAttributes.In, new ByReferenceType(TSelector));
            selector.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(selector);

            var allocator = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Helper.Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
        }

        private static GenericParameter Epilogue(MethodDefinition method, ModuleDefinition MainModule, TypeReference Element0, TypeReference WhereIndexEnumerable, TypeReference T, TypeReference Enumerable0, TypeReference Enumerator0, GenericInstanceType Enumerable1, TypeReference Enumerator1, TypeReference Element1, TypeReference TKey, TypeReference TOuterKeySelector, TypeReference TInnerKeySelector, TypeReference TKeyEqualityComparer, out GenericInstanceType @return)
        {
            GenericParameter TSelector = new GenericParameter(nameof(TSelector), method) { HasNotNullableValueTypeConstraint = true };
            TSelector.Constraints.Add(MainModule.GetType("UniNativeLinq", "IRefFunc`3").MakeGenericInstanceType(new[]
            {
                Element0,
                WhereIndexEnumerable,
                T
            }));
            method.GenericParameters.Add(TSelector);

            @return = MainModule.GetType("UniNativeLinq", "GroupJoinEnumerable`12").MakeGenericInstanceType(new[]
            {
                Enumerable0,
                Enumerator0,
                Element0,
                Enumerable1,
                Enumerator1,
                Element1,
                TKey,
                TOuterKeySelector,
                TInnerKeySelector,
                TKeyEqualityComparer,
                T,
                TSelector,
            });
            method.ReturnType = @return;
            return TSelector;
        }

        private static GenericInstanceType InnerRoutine(TypeReference type1, MethodDefinition method, TypeDefinition IRefFunc2, GenericParameter TKey, ModuleDefinition MainModule, GenericParameter TKeyEqualityComparer, out TypeReference Enumerator1, out TypeReference Element1, out GenericParameter TInnerKeySelector, out GenericInstanceType WhereIndexEnumerable)
        {
            const string suffix1 = "1";
            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, suffix1);
            var Enumerable1 = type1.MakeGenericInstanceType(added1);
            Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, suffix1);
            Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, suffix1);

            TInnerKeySelector = InternalInnerRoutine(method, IRefFunc2, Element1, TKey, MainModule, TKeyEqualityComparer, out WhereIndexEnumerable);
            return Enumerable1;
        }

        private static GenericInstanceType OuterRoutine(TypeReference type0, MethodDefinition method, TypeDefinition IRefFunc2, GenericParameter TKey, out TypeReference Enumerator0, out TypeReference Element0, out GenericParameter TOuterKeySelector)
        {
            const string suffix0 = "0";
            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, suffix0);
            var Enumerable0 = type0.MakeGenericInstanceType(added0);
            Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix0);
            Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, suffix0);

            TOuterKeySelector = InternalOuterRoutine(method, IRefFunc2, Element0, TKey);
            return Enumerable0;
        }

        private static GenericParameter InternalOuterRoutine(MethodDefinition method, TypeReference IRefFunc2, TypeReference Element0, TypeReference TKey)
        {
            GenericParameter TOuterKeySelector = new GenericParameter(nameof(TOuterKeySelector), method) { HasNotNullableValueTypeConstraint = true };
            TOuterKeySelector.Constraints.Add(IRefFunc2.MakeGenericInstanceType(new[]
            {
                Element0,
                TKey
            }));
            method.GenericParameters.Add(TOuterKeySelector);
            return TOuterKeySelector;
        }

        private static GenericParameter InternalInnerRoutine(MethodDefinition method, TypeDefinition IRefFunc2, TypeReference Element1, GenericParameter TKey, ModuleDefinition MainModule, GenericParameter TKeyEqualityComparer, out GenericInstanceType WhereIndexEnumerable)
        {
            GenericParameter TInnerKeySelector = new GenericParameter(nameof(TInnerKeySelector), method) { HasNotNullableValueTypeConstraint = true };
            TInnerKeySelector.Constraints.Add(IRefFunc2.MakeGenericInstanceType(new[]
            {
                Element1,
                TKey
            }));
            method.GenericParameters.Add(TInnerKeySelector);

            var NativeEnumerable = MainModule.GetType("UniNativeLinq", "NativeEnumerable`1");
            var InnerNativeEnumerable = new GenericInstanceType(NativeEnumerable)
            {
                GenericArguments = { Element1 }
            };
            var InnerNativeEnumerator = new GenericInstanceType(NativeEnumerable.NestedTypes.First(x => x.Name == "Enumerator"))
            {
                GenericArguments = { Element1 }
            };

            var GroupJoinPredicate = new GenericInstanceType(MainModule.GetType("UniNativeLinq", "GroupJoinPredicate`3"))
            {
                GenericArguments =
                {
                    Element1,
                    TKey,
                    TKeyEqualityComparer
                }
            };

            WhereIndexEnumerable = new GenericInstanceType(MainModule.GetType("UniNativeLinq", "WhereIndexEnumerable`4"))
            {
                GenericArguments =
                {
                    InnerNativeEnumerable,
                    InnerNativeEnumerator,
                    Element1,
                    GroupJoinPredicate,
                }
            };
            return TInnerKeySelector;
        }

        private static (GenericParameter TKey, GenericParameter TKeyEqualityComparer, GenericParameter T, TypeDefinition IRefFunc2) Prepare(MethodDefinition method, ModuleDefinition mainModule)
        {
            var TKey = method.DefineUnmanagedGenericParameter("TKey");
            method.GenericParameters.Add(TKey);

            GenericParameter TKeyEqualityComparer = new GenericParameter(nameof(TKeyEqualityComparer), method) { HasNotNullableValueTypeConstraint = true };
            TKeyEqualityComparer.Constraints.Add(new GenericInstanceType(mainModule.GetType("UniNativeLinq", "IRefFunc`3"))
            {
                GenericArguments = { TKey, TKey, mainModule.TypeSystem.Boolean }
            });
            method.GenericParameters.Add(TKeyEqualityComparer);

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var IRefFunc2 = mainModule.GetType("UniNativeLinq", "IRefFunc`2");
            return (TKey, TKeyEqualityComparer, T, IRefFunc2);
        }
    }
}