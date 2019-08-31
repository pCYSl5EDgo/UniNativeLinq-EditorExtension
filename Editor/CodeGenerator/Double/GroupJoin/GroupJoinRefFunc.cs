using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class GroupJoinRefFunc : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public readonly IDoubleApi Api;
        public GroupJoinRefFunc(IDoubleApi api) => Api = api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.HelpWithGenerate(processor, mainModule, systemModule, GenerateEachPair);
        }

        private void GenerateEachPair(string rowName, bool isRowSpecial, string columnName, bool isColumnSpecial, TypeDefinition @static, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var method = new MethodDefinition("GroupJoin", Helper.StaticMethodAttributes, mainModule.TypeSystem.Boolean)
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

        private static void DefineVariables(MethodBody body, TypeReference keySelector0, TypeReference keySelector1, TypeReference keyEqualityComparer, TypeReference tSelector)
        {
            var variableDefinitions = body.Variables;
            variableDefinitions.Add(new VariableDefinition(keySelector0));
            variableDefinitions.Add(new VariableDefinition(keySelector1));
            variableDefinitions.Add(new VariableDefinition(keyEqualityComparer));
            variableDefinitions.Add(new VariableDefinition(tSelector));
        }

        private static void GenerateSpecialSpecial(string rowName, string columnName, ModuleDefinition mainModule, MethodDefinition method)
        {
            var (key, keyEqualityComparer, T) = Prepare(method, mainModule);
            var (Element0, baseEnumerable0, Enumerable0, Enumerator0) = DefineSpecial(rowName, method, 0);
            var TOuterKeySelector = InternalOuterRoutine(method, Element0, key);
            var (Element1, baseEnumerable1, Enumerable1, Enumerator1) = DefineSpecial(columnName, method, 1);
            var TInnerKeySelector = InternalInnerRoutine(Element1, key, mainModule, keyEqualityComparer, out var WhereIndexEnumerable);

            var TSelector = Epilogue(method, mainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, key, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.None, baseEnumerable0);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.None, baseEnumerable1);
            method.Parameters.Add(inner);

            DefineParameters(Element0, Element1, key, method, WhereIndexEnumerable, T);

            var body = method.Body;
            DefineVariables(body, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, TSelector);

            body.GetILProcessor()
                .LdConvArg(Enumerable0, 0)
                .LdConvArg(Enumerable1, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                .LoadFuncArgumentAndStoreToLocalVariableField(3, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(4, 2)
                .LoadFuncArgumentAndStoreToLocalVariableField(5, 3)
                .LdLocAs(4)
                .LdArg(6)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private static void GenerateSpecialNormal(string specialName, TypeDefinition type, ModuleDefinition mainModule, MethodDefinition method, int specialIndex)
        {
            var (key, keyEqualityComparer, T) = Prepare(method, mainModule);


            var body = method.Body;
            if (specialIndex == 0)
            {
                var (Element0, baseEnumerable, Enumerable0, Enumerator0) = DefineSpecial(specialName, method, specialIndex);
                var TOuterKeySelector = InternalOuterRoutine(method, Element0, key);

                var Enumerable1 = InnerRoutine(type, method, key, mainModule, keyEqualityComparer, out var Enumerator1, out var Element1, out var TInnerKeySelector, out var WhereIndexEnumerable);

                var TSelector = Epilogue(method, mainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, key, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, out var @return);

                var outer = new ParameterDefinition("outer", ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(outer);

                var inner = new ParameterDefinition("inner", ParameterAttributes.In, new ByReferenceType(Enumerable1));
                inner.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
                method.Parameters.Add(inner);

                DefineParameters(Element0, Element1, key, method, WhereIndexEnumerable, T);

                DefineVariables(body, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, TSelector);

                body.GetILProcessor()
                    .LdConvArg(Enumerable0, 0)
                    .LdArg(1)
                    .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                    .LoadFuncArgumentAndStoreToLocalVariableField(3, 1)
                    .LoadFuncArgumentAndStoreToLocalVariableField(4, 2)
                    .LoadFuncArgumentAndStoreToLocalVariableField(5, 3)
                    .LdLocAs(4)
                    .LdArg(6)
                    .NewObj(@return.FindMethod(".ctor"))
                    .Ret();
            }
            else
            {
                var Enumerable0 = OuterRoutine(type, method, key, out var Enumerator0, out var Element0, out var TOuterKeySelector);

                var (Element1, baseEnumerable, Enumerable1, Enumerator1) = DefineSpecial(specialName, method, specialIndex);
                var TInnerKeySelector = InternalInnerRoutine(Element1, key, mainModule, keyEqualityComparer, out var WhereIndexEnumerable);

                var TSelector = Epilogue(method, mainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, key, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, out var @return);

                var outer = new ParameterDefinition("outer", ParameterAttributes.In, new ByReferenceType(Enumerable0));
                outer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
                method.Parameters.Add(outer);

                var inner = new ParameterDefinition("inner", ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(inner);

                DefineParameters(Element0, Element1, key, method, WhereIndexEnumerable, T);

                DefineVariables(body, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, TSelector);

                body.GetILProcessor()
                    .LdArg(0)
                    .LdConvArg(Enumerable1, 1)
                    .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                    .LoadFuncArgumentAndStoreToLocalVariableField(3, 1)
                    .LoadFuncArgumentAndStoreToLocalVariableField(4, 2)
                    .LoadFuncArgumentAndStoreToLocalVariableField(5, 3)
                    .LdLocAs(4)
                    .LdArg(6)
                    .NewObj(@return.FindMethod(".ctor"))
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

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, ModuleDefinition mainModule, MethodDefinition method)
        {
            var (key, keyEqualityComparer, T) = Prepare(method, mainModule);

            var Enumerable0 = OuterRoutine(type0, method, key, out var Enumerator0, out var Element0, out var TOuterKeySelector);

            var Enumerable1 = InnerRoutine(type1, method, key, mainModule, keyEqualityComparer, out var Enumerator1, out var Element1, out var TInnerKeySelector, out var WhereIndexEnumerable);

            var TSelector = Epilogue(method, mainModule, Element0, WhereIndexEnumerable, T, Enumerable0, Enumerator0, Enumerable1, Enumerator1, Element1, key, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, out var @return);

            var outer = new ParameterDefinition("outer", ParameterAttributes.In, new ByReferenceType(Enumerable0));
            outer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            var inner = new ParameterDefinition("inner", ParameterAttributes.In, new ByReferenceType(Enumerable1));
            inner.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(inner);

            DefineParameters(Element0, Element1, key, method, WhereIndexEnumerable, T);

            var body = method.Body;

            DefineVariables(body, TOuterKeySelector, TInnerKeySelector, keyEqualityComparer, TSelector);

            body.GetILProcessor()
                .LdArgs(0, 2)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                .LoadFuncArgumentAndStoreToLocalVariableField(3, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(4, 2)
                .LoadFuncArgumentAndStoreToLocalVariableField(5, 3)
                .LdLocAs(4)
                .LdArg(6)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private static void DefineParameters(TypeReference TOuterElement, TypeReference TInnerElement, TypeReference TKey, MethodDefinition method, TypeReference WhereIndexEnumerable, TypeReference T)
        {
            var mainModule = method.Module;
            var outerSelector = new ParameterDefinition("outerSelector", ParameterAttributes.None, new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefFunc`2"))
            {
                GenericArguments = { TOuterElement, TKey }
            });
            method.Parameters.Add(outerSelector);

            var innerSelector = new ParameterDefinition("innerSelector", ParameterAttributes.None, new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefFunc`2"))
            {
                GenericArguments = { TInnerElement, TKey }
            });
            method.Parameters.Add(innerSelector);

            var comparer = new ParameterDefinition("comparer", ParameterAttributes.None, new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefFunc`3"))
            {
                GenericArguments = { TKey, TKey, mainModule.TypeSystem.Boolean }
            });
            method.Parameters.Add(comparer);

            var selector = new ParameterDefinition("selector", ParameterAttributes.None, new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefFunc`3"))
            {
                GenericArguments = { TOuterElement, WhereIndexEnumerable, T }
            });
            method.Parameters.Add(selector);

            var allocator = new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Helper.Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
        }

        private static TypeReference Epilogue(MethodDefinition method, ModuleDefinition MainModule, TypeReference Element0, TypeReference WhereIndexEnumerable, TypeReference T, TypeReference Enumerable0, TypeReference Enumerator0, GenericInstanceType Enumerable1, TypeReference Enumerator1, TypeReference Element1, TypeReference TKey, TypeReference TOuterKeySelector, TypeReference TInnerKeySelector, TypeReference TKeyEqualityComparer, out GenericInstanceType @return)
        {
            var TSelector = new GenericInstanceType(method.Module.GetType("UniNativeLinq", "DelegateRefFuncToStructOperatorFunc`3"))
            {
                GenericArguments =
                {
                    Element0,
                    WhereIndexEnumerable,
                    T,
                }
            };

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

        private static GenericInstanceType InnerRoutine(TypeReference type1, MethodDefinition method, GenericParameter TKey, ModuleDefinition MainModule, TypeReference TKeyEqualityComparer, out TypeReference Enumerator1, out TypeReference Element1, out TypeReference TInnerKeySelector, out GenericInstanceType WhereIndexEnumerable)
        {
            const string suffix1 = "1";
            var added1 = method.FromTypeToMethodParam(type1.GenericParameters, suffix1);
            var Enumerable1 = type1.MakeGenericInstanceType(added1);
            Enumerator1 = Enumerable1.GetEnumeratorTypeOfCollectionType().Replace(added1, suffix1);
            Element1 = Enumerable1.GetElementTypeOfCollectionType().Replace(added1, suffix1);

            TInnerKeySelector = InternalInnerRoutine(Element1, TKey, MainModule, TKeyEqualityComparer, out WhereIndexEnumerable);
            return Enumerable1;
        }

        private static GenericInstanceType OuterRoutine(TypeReference type0, MethodDefinition method, GenericParameter TKey, out TypeReference Enumerator0, out TypeReference Element0, out TypeReference TOuterKeySelector)
        {
            const string suffix0 = "0";
            var added0 = method.FromTypeToMethodParam(type0.GenericParameters, suffix0);
            var Enumerable0 = type0.MakeGenericInstanceType(added0);
            Enumerator0 = Enumerable0.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix0);
            Element0 = Enumerable0.GetElementTypeOfCollectionType().Replace(added0, suffix0);

            TOuterKeySelector = InternalOuterRoutine(method, Element0, TKey);
            return Enumerable0;
        }

        private static TypeReference InternalOuterRoutine(MethodDefinition method, TypeReference Element0, TypeReference TKey)
        {
            return new GenericInstanceType(method.Module.GetType("UniNativeLinq", "DelegateRefFuncToStructOperatorFunc`2"))
            {
                GenericArguments = { Element0, TKey }
            };
        }

        private static TypeReference InternalInnerRoutine(TypeReference Element1, GenericParameter TKey, ModuleDefinition MainModule, TypeReference TKeyEqualityComparer, out GenericInstanceType WhereIndexEnumerable)
        {
            var TInnerKeySelector = new GenericInstanceType(MainModule.GetType("UniNativeLinq", "DelegateRefFuncToStructOperatorFunc`2"))
            {
                GenericArguments = { Element1, TKey }
            };

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

        private static (GenericParameter TKey, TypeReference TKeyEqualityComparer, GenericParameter T) Prepare(MethodDefinition method, ModuleDefinition mainModule)
        {
            GenericParameter TKey = new GenericParameter(nameof(TKey), method) { HasNotNullableValueTypeConstraint = true };
            TKey.CustomAttributes.Add(Helper.UnManagedAttribute);
            method.GenericParameters.Add(TKey);

            var TKeyEqualityComparer = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateRefFuncToStructOperatorFunc`3"))
            {
                GenericArguments = { TKey, TKey, mainModule.TypeSystem.Boolean }
            };

            GenericParameter T = new GenericParameter(nameof(T), method) { HasNotNullableValueTypeConstraint = true };
            T.CustomAttributes.Add(Helper.UnManagedAttribute);
            method.GenericParameters.Add(T);

            return (TKey, TKeyEqualityComparer, T);
        }
    }
}