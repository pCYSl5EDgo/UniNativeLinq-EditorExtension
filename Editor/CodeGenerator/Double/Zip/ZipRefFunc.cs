using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    public sealed class ZipRefFunc : IApiExtensionMethodGenerator, ITypeDictionaryHolder
    {
        public readonly IDoubleApi Api;
        public ZipRefFunc(IDoubleApi api) => Api = api;
        public Dictionary<string, TypeDefinition> Dictionary { private get; set; }

        public void Generate(IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule)
        {
            Api.HelpWithGenerate(processor, mainModule, GenerateEachPair);
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
                GenerateSpecialNormal(rowName, Dictionary[columnName], mainModule, method, 0);
            }
            else if (isColumnSpecial)
            {
                GenerateSpecialNormal(columnName, Dictionary[rowName], mainModule, method, 1);
            }
            else
            {
                GenerateNormalNormal(Dictionary[rowName], Dictionary[columnName], mainModule, method);
            }
        }

        private void GenerateSpecialSpecial(string rowName, string columnName, ModuleDefinition mainModule, MethodDefinition method)
        {
            var (element0, enumerable0, enumerator0, baseTypeReference0) = DefineWithSpecial(rowName, method, 0);
            var (element1, enumerable1, enumerator1, baseTypeReference1) = DefineWithSpecial(columnName, method, 1);
            var (T, TAction, Func) = Prepare(element0, element1, mainModule, method);
            var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, element0, enumerable1, enumerator1, element1, T, TAction);
            var param0 = new ParameterDefinition("@this", ParameterAttributes.None, baseTypeReference0);
            method.Parameters.Add(param0);
            var param1 = new ParameterDefinition("second", ParameterAttributes.None, baseTypeReference1);
            method.Parameters.Add(param1);
            var param2 = new ParameterDefinition("action", ParameterAttributes.None, Func);
            method.Parameters.Add(param2);
            method.Body.Variables.Add(new VariableDefinition(TAction));

            method.Body.GetILProcessor()
                .LdConvArg(enumerable0, 0)
                .LdConvArg(enumerable1, 1)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                .LdLocA(0)
                .NewObj(@return.FindMethod(".ctor", 3))
                .Ret();
        }

        private void GenerateSpecialNormal(string specialName, TypeDefinition type0, ModuleDefinition mainModule, MethodDefinition method, int specialIndex)
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
                (element0, enumerable0, enumerator0, baseTypeReference) = DefineWithSpecial(specialName, method, 0);
                (element1, enumerable1, enumerator1) = type0.MakeGenericInstanceVariant("1", method);
            }
            else
            {
                (element0, enumerable0, enumerator0) = type0.MakeGenericInstanceVariant("0", method);
                (element1, enumerable1, enumerator1, baseTypeReference) = DefineWithSpecial(specialName, method, 1);
            }
            var (T, TAction, Func) = Prepare(element0, element1, mainModule, method);

            var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, element0, enumerable1, enumerator1, element1, T, TAction);

            var systemRuntimeCompilerServicesReadonlyAttributeTypeReference = Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference();
            if (specialIndex == 0)
            {
                var param0 = new ParameterDefinition("@this", ParameterAttributes.None, baseTypeReference);
                method.Parameters.Add(param0);

                var param1 = new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1))
                {
                    CustomAttributes = { systemRuntimeCompilerServicesReadonlyAttributeTypeReference }
                };
                method.Parameters.Add(param1);

                var param2 = new ParameterDefinition("action", ParameterAttributes.None, Func);
                method.Parameters.Add(param2);

                method.Body.Variables.Add(new VariableDefinition(TAction));

                method.Body.GetILProcessor()
                    .LdConvArg(enumerable0, 0)
                    .LdArg(1)
                    .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                    .LdLocA(0)
                    .NewObj(@return.FindMethod(".ctor", 3))
                    .Ret();
            }
            else
            {
                var param0 = new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0))
                {
                    CustomAttributes = { systemRuntimeCompilerServicesReadonlyAttributeTypeReference }
                };
                method.Parameters.Add(param0);

                var param1 = new ParameterDefinition("second", ParameterAttributes.None, baseTypeReference);
                method.Parameters.Add(param1);

                var param2 = new ParameterDefinition("action", ParameterAttributes.None, Func);
                method.Parameters.Add(param2);

                method.Body.Variables.Add(new VariableDefinition(TAction));

                method.Body.GetILProcessor()
                    .LdArg(0)
                    .LdConvArg(enumerable1, 1)
                    .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                    .LdLocA(0)
                    .NewObj(@return.FindMethod(".ctor", 3))
                    .Ret();
            }
        }

        private static (TypeReference elemenet, TypeReference enumerable, TypeReference enumerator, TypeReference baseTypeReference) DefineWithSpecial(string specialName, MethodDefinition method, int specialIndex)
        {
            var element = method.DefineUnmanagedGenericParameter("TSpecial" + specialIndex);
            method.GenericParameters.Add(element);
            var (baseEnumerable, enumerable, enumerator) = element.MakeSpecialTypePair(specialName);
            return (element, enumerable, enumerator, baseEnumerable);
        }

        private void GenerateNormalNormal(TypeDefinition type0, TypeDefinition type1, ModuleDefinition mainModule, MethodDefinition method)
        {
            var (element0, enumerable0, enumerator0) = type0.MakeGenericInstanceVariant("0", method);
            var (element1, enumerable1, enumerator1) = type1.MakeGenericInstanceVariant("1", method);

            var (T, TAction, Func) = Prepare(element0, element1, mainModule, method);

            var @return = DefineReturn(mainModule, method, enumerable0, enumerator0, element0, enumerable1, enumerator1, element1, T, TAction);

            var systemRuntimeCompilerServicesReadonlyAttributeTypeReference = Helper.GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference();

            var param0 = new ParameterDefinition("@this", ParameterAttributes.In, new ByReferenceType(enumerable0))
            {
                CustomAttributes = { systemRuntimeCompilerServicesReadonlyAttributeTypeReference }
            };
            method.Parameters.Add(param0);

            var param1 = new ParameterDefinition("second", ParameterAttributes.In, new ByReferenceType(enumerable1))
            {
                CustomAttributes = { systemRuntimeCompilerServicesReadonlyAttributeTypeReference }
            };
            method.Parameters.Add(param1);

            var param2 = new ParameterDefinition("action", ParameterAttributes.None, Func);
            method.Parameters.Add(param2);

            method.Body.Variables.Add(new VariableDefinition(TAction));

            method.Body.GetILProcessor()
                .LdArgs(0, 2)
                .LoadFuncArgumentAndStoreToLocalVariableField(2, 0)
                .LdLocA(0)
                .NewObj(@return.FindMethod(".ctor", 3))
                .Ret();
        }

        private TypeReference DefineReturn(ModuleDefinition mainModule, MethodDefinition method, TypeReference enumerable0, TypeReference enumerator0, TypeReference element0, TypeReference enumerable1, TypeReference enumerator1, TypeReference element1, TypeReference T, TypeReference TAction)
        {
            return method.ReturnType = new GenericInstanceType(mainModule.GetType("UniNativeLinq", Api.Name + "Enumerable`8"))
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

        private static (TypeReference, TypeReference, TypeReference) Prepare(TypeReference element0, TypeReference element1, ModuleDefinition mainModule, MethodDefinition method)
        {
            var T = method.DefineUnmanagedGenericParameter();
            method.GenericParameters.Add(T);

            var TAction = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "DelegateRefActionToStructOperatorAction`3"))
            {
                GenericArguments = { element0, element1, T }
            };

            var Func = new GenericInstanceType(mainModule.GetType("UniNativeLinq", "RefAction`3"))
            {
                GenericArguments = { element0, element1, T }
            };

            return (T, TAction, Func);
        }
    }
}