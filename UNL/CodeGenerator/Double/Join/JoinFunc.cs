using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class JoinFunc : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public readonly IDoubleApi Api;
        public JoinFunc(IDoubleApi api) => Api = api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.HelpWithGenerate(processor, mainModule, systemModule, GenerateEachPair);
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
                GenerateNormalNormal(Dictionary[rowName], Dictionary[columnName], mainModule, systemModule, method);
            }
        }

        private void GenerateSpecialSpecial(string rowName, string columnName, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var (key, keyEqualityComparer, T) = Prepare(method, mainModule, systemModule);

            var (element0, baseEnumerable0, enumerable0, enumerator0, keySelector0) = DefineWithSpecial(rowName, method, key, 0);
            var (element1, baseEnumerable1, enumerable1, enumerator1, keySelector1) = DefineWithSpecial(columnName, method, key, 1);

            ParameterDefinition outer = new ParameterDefinition(nameof(outer), ParameterAttributes.None, baseEnumerable0);
            method.Parameters.Add(outer);

            ParameterDefinition inner = new ParameterDefinition(nameof(inner), ParameterAttributes.None, baseEnumerable1);
            method.Parameters.Add(inner);

            Epilogue(method, mainModule, systemModule, element0, element1, T, enumerable0, enumerator0, enumerable1, enumerator1, key, keySelector0, keySelector1, keyEqualityComparer, out var @return, out var tSelector);

            var body = method.Body;
            DefineVariables(body, keySelector0, keySelector1, keyEqualityComparer, tSelector);

            body.GetILProcessor()
                .LdConvArg(enumerable0, 0)
                .LdConvArg(enumerable1, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                .LoadFuncArgumentAndStoreToLocalVariableField(3, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(4, 2)
                .LoadFuncArgumentAndStoreToLocalVariableField(5, 3)
                .LdLocAs(4)
                .LdArg(6)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private static void DefineVariables(MethodBody body, TypeReference keySelector0, TypeReference keySelector1, TypeReference keyEqualityComparer, TypeReference tSelector)
        {
            var variableDefinitions = body.Variables;
            variableDefinitions.Add(new VariableDefinition(keySelector0));
            variableDefinitions.Add(new VariableDefinition(keySelector1));
            variableDefinitions.Add(new VariableDefinition(keyEqualityComparer));
            variableDefinitions.Add(new VariableDefinition(tSelector));
        }

        private void GenerateSpecialNormal(string specialName, TypeDefinition type0, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method, int specialIndex)
        {
            var (key, keyEqualityComparer, T) = Prepare(method, mainModule, systemModule);

            TypeReference enumerable0, enumerator0, element0, keySelector0, element1, baseEnumerable, enumerable1, enumerator1, keySelector1;

            if (specialIndex == 0)
            {
                (element0, baseEnumerable, enumerable0, enumerator0, keySelector0) = DefineWithSpecial(specialName, method, key, specialIndex);
                Routine(type0, method, "1", key, out enumerable1, out enumerator1, out element1, out keySelector1);

                ParameterDefinition outer = new ParameterDefinition(nameof(outer), ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(outer);

                ParameterDefinition inner = new ParameterDefinition(nameof(inner), ParameterAttributes.In, new ByReferenceType(enumerable1));
                inner.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
                method.Parameters.Add(inner);
            }
            else
            {
                Routine(type0, method, "0", key, out enumerable0, out enumerator0, out element0, out keySelector0);
                (element1, baseEnumerable, enumerable1, enumerator1, keySelector1) = DefineWithSpecial(specialName, method, key, specialIndex);

                ParameterDefinition outer = new ParameterDefinition(nameof(outer), ParameterAttributes.In, new ByReferenceType(enumerable0));
                outer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
                method.Parameters.Add(outer);

                ParameterDefinition inner = new ParameterDefinition(nameof(inner), ParameterAttributes.None, baseEnumerable);
                method.Parameters.Add(inner);
            }

            Epilogue(method, mainModule, systemModule, element0, element1, T, enumerable0, enumerator0, enumerable1, enumerator1, key, keySelector0, keySelector1, keyEqualityComparer, out var @return, out var tSelector);

            var body = method.Body;
            var processor = body.GetILProcessor();

            DefineVariables(body, keySelector0, keySelector1, keyEqualityComparer, tSelector);

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
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                .LoadFuncArgumentAndStoreToLocalVariableField(3, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(4, 2)
                .LoadFuncArgumentAndStoreToLocalVariableField(5, 3)
                .LdLocAs(4)
                .LdArg(6)
                .NewObj(@return.FindMethod(".ctor"))
                .Ret();
        }

        private static (TypeReference element0, TypeReference baseEnumerable, GenericInstanceType enumerable0, TypeReference enumerator0, GenericInstanceType keySelector0) DefineWithSpecial(string specialName, MethodDefinition method, GenericParameter key, int specialIndex)
        {
            TypeReference element = method.DefineUnmanagedGenericParameter("TSpecial" + specialIndex);
            method.GenericParameters.Add((GenericParameter)element);
            var (baseEnumerable, enumerable, enumerator) = ((GenericParameter)element).MakeSpecialTypePair(specialName);
            var keySelector = new GenericInstanceType(method.Module.GetType("UniNativeLinq", "DelegateFuncToStructOperatorFunc`2"))
            {
                GenericArguments =
                {
                    element,
                    key
                }
            };
            return (element, baseEnumerable, enumerable, enumerator, keySelector);
        }

        private static void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, ModuleDefinition mainModule, ModuleDefinition systemModule, MethodDefinition method)
        {
            var (key, keyEqualityComparer, T) = Prepare(method, mainModule, systemModule);

            Routine(type0, method, "0", key, out var enumerable0, out var enumerator0, out var element0, out var keySelector0);

            Routine(type1, method, "1", key, out var enumerable1, out var enumerator1, out var element1, out var keySelector1);

            DefineOuterInner(method, enumerable0, enumerable1);

            Epilogue(method, mainModule, systemModule, element0, element1, T, enumerable0, enumerator0, enumerable1, enumerator1, key, keySelector0, keySelector1, keyEqualityComparer, out var @return, out var tSelector);

            var body = method.Body;
            DefineVariables(body, keySelector0, keySelector1, keyEqualityComparer, tSelector);
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

        private static void DefineOuterInner(MethodDefinition method, TypeReference enumerable0, TypeReference enumerable1)
        {
            ParameterDefinition outer = new ParameterDefinition(nameof(outer), ParameterAttributes.In, new ByReferenceType(enumerable0));
            outer.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(outer);

            ParameterDefinition inner = new ParameterDefinition(nameof(inner), ParameterAttributes.In, new ByReferenceType(enumerable1));
            inner.CustomAttributes.Add(Helper.IsReadOnlyAttribute);
            method.Parameters.Add(inner);
        }

        private static (GenericParameter TKey, GenericInstanceType TKeyEqualityComparer, GenericParameter T) Prepare(MethodDefinition method, ModuleDefinition mainModule, ModuleDefinition systemModule)
        {
            var TKey = method.DefineUnmanagedGenericParameter("TKey");
            method.GenericParameters.Add(TKey);
            TKey.Constraints.Add(new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "IEquatable`1")))
            {
                GenericArguments = { TKey }
            });

            var TKeyEqualityComparer = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateFuncToStructOperatorFunc`3"))
            {
                GenericArguments = { TKey, TKey, mainModule.TypeSystem.Boolean }
            };

            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);
            return (TKey, TKeyEqualityComparer, T);
        }

        private static void Routine(TypeDefinition type, MethodDefinition method, string suffix, TypeReference key, out TypeReference enumerable, out TypeReference enumerator, out TypeReference element, out TypeReference keySelector)
        {
            (element, enumerable, enumerator) = type.MakeGenericInstanceVariant(suffix, method);

            keySelector = new GenericInstanceType(method.Module.GetType("UniNativeLinq", "DelegateFuncToStructOperatorFunc`2"))
            {
                GenericArguments =
                {
                    element,
                    key
                }
            };
        }

        private static void Epilogue(MethodDefinition method, ModuleDefinition mainModule, ModuleDefinition systemModule,
            TypeReference element0, TypeReference element1, TypeReference T, TypeReference enumerable0,
            TypeReference enumerator0, TypeReference enumerable1, TypeReference enumerator1, TypeReference key,
            TypeReference tKeySelector0, TypeReference tKeySelector1, TypeReference tKeyEqualityComparer,
            out GenericInstanceType @return, out GenericInstanceType tSelector)
        {
            tSelector = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateFuncToStructOperatorFunc`3"))
            {
                GenericArguments =
                {
                    element0,
                    element1,
                    T,
                }
            };

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

            ParameterDefinition outerSelector = new ParameterDefinition(nameof(outerSelector), ParameterAttributes.None, new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { element0, key }
            });
            method.Parameters.Add(outerSelector);

            ParameterDefinition innerSelector = new ParameterDefinition(nameof(innerSelector), ParameterAttributes.None, new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`2")))
            {
                GenericArguments = { element1, key }
            });
            method.Parameters.Add(innerSelector);

            ParameterDefinition comparer = new ParameterDefinition(nameof(comparer), ParameterAttributes.None, new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`3")))
            {
                GenericArguments = { key, key, mainModule.TypeSystem.Boolean }
            });
            method.Parameters.Add(comparer);

            ParameterDefinition selector = new ParameterDefinition(nameof(selector), ParameterAttributes.None, new GenericInstanceType(mainModule.ImportReference(systemModule.GetType("System", "Func`3")))
            {
                GenericArguments = { element0, element1, T }
            });
            method.Parameters.Add(selector);

            ParameterDefinition allocator = new ParameterDefinition(nameof(allocator), ParameterAttributes.HasDefault | ParameterAttributes.Optional, Helper.Allocator)
            {
                Constant = 2,
            };
            method.Parameters.Add(allocator);
        }
    }
}