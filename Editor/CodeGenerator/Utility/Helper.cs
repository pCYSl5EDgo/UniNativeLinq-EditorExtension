using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace UniNativeLinq.Editor.CodeGenerator
{
    internal static class Helper
    {
        internal static CustomAttribute ExtensionAttribute;
        internal static CustomAttribute IsReadOnlyAttribute;
        internal static CustomAttribute UnManagedAttribute;
        internal static TypeReference Allocator;
        internal static ModuleDefinition MainModule;
        internal static ModuleDefinition SystemModule;
        internal static ModuleDefinition UnityCoreModule;
        internal const TypeAttributes StaticExtensionClassTypeAttributes = TypeAttributes.AnsiClass | TypeAttributes.AutoLayout | TypeAttributes.BeforeFieldInit | TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.Abstract;
        internal const MethodAttributes StaticMethodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
        internal static Dictionary<string, (TypeDefinition, Func<TypeReference, TypeReference>)> SpecialTypeDictionary;
        internal static GlobalSettings Settings;

        static Helper()
        {
            Initialize();
        }

        public static ILProcessor GetEnumeratorEnumerable(this ILProcessor processor, GenericParameter TEnumerable) => processor.Constrained(TEnumerable).CallVirtual(TEnumerable.Constraints.First(x => x.ToDefinition().Name == "IRefEnumerable`2").FindMethod("GetEnumerator"));
        public static ILProcessor MoveNextEnumerator(this ILProcessor processor, GenericParameter TEnumerator) => processor.Constrained(TEnumerator).CallVirtual(MainModule.ImportReference(SystemModule.GetType("System.Collections", "IEnumerator")).FindMethod("MoveNext"));
        public static ILProcessor DisposeEnumerator(this ILProcessor processor, GenericParameter TEnumerator) => processor.Constrained(TEnumerator).CallVirtual(MainModule.ImportReference(SystemModule.GetType("System", "IDisposable")).FindMethod("Dispose"));
        public static ILProcessor TryGetNextEnumerator(this ILProcessor processor, GenericParameter TEnumerator) => processor.Constrained(TEnumerator).CallVirtual(TEnumerator.Constraints.First(x => x.ToDefinition().Name == "IRefEnumerator`1").FindMethod("TryGetNext"));
        public static ILProcessor TryMoveNextEnumerator(this ILProcessor processor, GenericParameter TEnumerator) => processor.Constrained(TEnumerator).CallVirtual(TEnumerator.Constraints.First(x => x.ToDefinition().Name == "IRefEnumerator`1").FindMethod("TryMoveNext"));
        public static ILProcessor GetCurrentEnumerator(this ILProcessor processor, GenericParameter TEnumerator) => processor.Constrained(TEnumerator).CallVirtual(TEnumerator.Constraints.First(x => x.ToDefinition().Name == "IRefEnumerator`1").FindMethod("get_Current"));

        public static (GenericParameter T, GenericParameter TEnumerator, GenericParameter TEnumerable) Define3GenericParameters(this MethodDefinition method)
        {
            var genericParameters = method.GenericParameters;

            var T = method.DefineUnmanagedGenericParameter();
            genericParameters.Add(T);

            var IRefEnumerator = new GenericInstanceType(method.Module.GetType("UniNativeLinq", "IRefEnumerator`1"))
            {
                GenericArguments = { T }
            };
            var TEnumerator = new GenericParameter("TEnumerator", method)
            {
                HasNotNullableValueTypeConstraint = true,
                Constraints = { IRefEnumerator }
            };
            method.GenericParameters.Add(TEnumerator);

            var IRefEnumerable = new GenericInstanceType(method.Module.GetType("UniNativeLinq", "IRefEnumerable`2"))
            {
                GenericArguments = { TEnumerator, T }
            };
            var TEnumerable = new GenericParameter("TEnumerable", method)
            {
                HasNotNullableValueTypeConstraint = true,
                Constraints = { IRefEnumerable }
            };
            method.GenericParameters.Add(TEnumerable);
            return (T, TEnumerator, TEnumerable);
        }

        public static ILProcessor LoadFuncArgumentAndStoreToLocalVariableField(this ILProcessor processor, int argumentIndex, int variableIndex)
        {
            var success = Instruction.Create(variableIndex <= 255 ? OpCodes.Ldloca_S : OpCodes.Ldloca, processor.Body.Variables[variableIndex]);
            if (Settings.EnableNullCheckOnRuntime)
            {
                processor = processor
                    .LdArg(argumentIndex)
                    .BrTrueS(success)
                    .NewObj(processor.Body.Method.Module.ImportReference(SystemModule.GetType("System", "ArgumentNullException")).FindMethod(".ctor", 0))
                    .Throw();
            }
            return processor
                .Add(success)
                .LdArg(argumentIndex)
                .StFld(processor.Body.Variables[variableIndex].VariableType.FindField("Func"));
        }

        public static void DefineAllocatorParam(this MethodDefinition method)
        {
            method.Parameters.Add(new ParameterDefinition("allocator", ParameterAttributes.HasDefault | ParameterAttributes.Optional, Allocator)
            {
                Constant = 2,
            });
        }

        public static GenericParameter DefineUnmanagedGenericParameter(this MethodDefinition method, string name = "T")
        {
            var x = method.Module.GetType("UniNativeLinq", "NativeEnumerable`1").GenericParameters[0];
            return new GenericParameter(name, method)
            {
                HasNotNullableValueTypeConstraint = true,
                HasDefaultConstructorConstraint = true,
                HasReferenceTypeConstraint = false,
                Constraints = { x.Constraints[0] },
                CustomAttributes = { x.CustomAttributes[0] }
            };
        }

        public static CustomAttribute GetSystemRuntimeCompilerServicesIsReadOnlyAttributeTypeReference()
        {
            return IsReadOnlyAttribute;
        }

        public static bool? CanIndexAccess(this TypeDefinition type)
        {
            var opCode = type.Methods.First(x => x.Name == "CanIndexAccess").Body.Instructions[0].OpCode.Code;
            switch (opCode)
            {
                case Code.Ldc_I4_1:
                    return true;
                case Code.Ldc_I4_0:
                    return false;
                default:
                    return null;
            }
        }

        public static bool? CanFastCount(this TypeDefinition type)
        {
            var opCode = type.Methods.First(x => x.Name == "CanFastCount").Body.Instructions[0].OpCode.Code;
            switch (opCode)
            {
                case Code.Ldc_I4_1:
                    return true;
                case Code.Ldc_I4_0:
                    return false;
                default:
                    return null;
            }
        }

        public static void GenerateSingleNoEnumerable(this ISingleApi Api, IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, Action<string, bool, TypeDefinition, ModuleDefinition, ModuleDefinition> generateEachAction, Action<TypeDefinition, ModuleDefinition, ModuleDefinition> GenerateGeneric)
        {
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(Api.Name + Api.Description + "Helper"));

            if (Api.TryGetEnabled("TEnumerable", out var apiEnabled) && apiEnabled)
                GenerateGeneric(@static, mainModule, systemModule);

            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out apiEnabled) || !apiEnabled) continue;
                generateEachAction(name, isSpecial, @static, mainModule, systemModule);
            }
        }

        public static void GenerateSingleNoEnumerable(this ISingleApi Api, IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, Action<string, bool, TypeDefinition, ModuleDefinition> generateEachAction, Action<TypeDefinition, ModuleDefinition> GenerateGeneric)
        {
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(Api.Name + Api.Description + "Helper"));

            GenerateGeneric(@static, mainModule);

            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                generateEachAction(name, isSpecial, @static, mainModule);
            }
        }

        public static void GenerateSingleWithEnumerable(this ISingleApi Api, IEnumerableCollectionProcessor processor, ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityModule, Action<string, bool, TypeDefinition, ModuleDefinition, ModuleDefinition> generateEachAction)
        {
            if (!processor.TryGetEnabled(Api.Name, out var enabled) || !enabled) return;
            var array = processor.EnabledNameCollection.Intersect(Api.NameCollection).ToArray();
            if (!Api.ShouldDefine(array)) return;
            TypeDefinition @static;
            mainModule.Types.Add(@static = mainModule.DefineStatic(Api.Name + Api.Description + "Helper"));

            foreach (var name in array)
            {
                if (!processor.IsSpecialType(name, out var isSpecial)) throw new KeyNotFoundException();
                if (!Api.TryGetEnabled(name, out var apiEnabled) || !apiEnabled) continue;
                generateEachAction(name, isSpecial, @static, mainModule, systemModule);
            }
        }

        public static (TypeReference element, TypeReference enumerable, TypeReference enumerator) MakeGenericInstanceVariant(this TypeDefinition type, string suffix, MethodDefinition method)
        {
            var added0 = method.FromTypeToMethodParam(type.GenericParameters, suffix);
            var enumerable = type.MakeGenericInstanceType(added0);
            var enumerator = enumerable.GetEnumeratorTypeOfCollectionType().Replace(added0, suffix);
            var element = enumerable.GetElementTypeOfCollectionType().Replace(added0, suffix);
            return (element, enumerable, enumerator);
        }

        public static (TypeReference enumerable, TypeReference enumerator, TypeReference element) MakeFromCommonType(this TypeReference T, MethodDefinition method, TypeReference type, string suffix)
        {
            var index = 0;
            foreach (var genericParameter in type.GenericParameters)
            {
                if (genericParameter.Name == nameof(T))
                    break;
                index++;
            }
            var added0 = method.FromTypeToMethodParam(type.GenericParameters, nameof(T), T, suffix);
            foreach (var parameter in added0)
            {
                parameter.RewriteConstraints(nameof(T), T);
            }

            var enumerable = type.MakeGenericInstanceType(added0.Take(index).Append(T).Concat(added0.Skip(index)));
            var enumerator = enumerable.GetEnumeratorTypeOfCollectionType().Replace(added0, nameof(T), T, suffix);
            var element = enumerable.GetElementTypeOfCollectionType().Replace(added0, nameof(T), T, suffix);

            return (enumerable, enumerator, element);
        }

        public static (TypeReference baseEnumerable, GenericInstanceType specialEnumerable, GenericInstanceType specialEnumerator) MakeSpecialTypePair(this TypeReference type, string specialName)
        {
            TypeDefinition typeDefinition;
            switch (specialName)
            {
                case "T[]":
                    typeDefinition = MainModule.GetType("UniNativeLinq", "ArrayEnumerable`1");
                    return (
                        new ArrayType(type),
                        new GenericInstanceType(typeDefinition)
                        {
                            GenericArguments = { type }
                        },
                        new GenericInstanceType(typeDefinition.NestedTypes.First(x => x.Name == "Enumerator"))
                        {
                            GenericArguments = { type }
                        }
                    );
                case "NativeArray<T>":
                    typeDefinition = MainModule.GetType("UniNativeLinq", "NativeEnumerable`1");
                    return (
                        new GenericInstanceType(MainModule.ImportReference(UnityCoreModule.GetType("Unity.Collections", "NativeArray`1")))
                        {
                            GenericArguments = { type }
                        },
                        new GenericInstanceType(typeDefinition)
                        {
                            GenericArguments = { type }
                        },
                        new GenericInstanceType(typeDefinition.NestedTypes.First(x => x.Name == "Enumerator"))
                        {
                            GenericArguments = { type }
                        }
                    );
                default: throw new ArgumentException(specialName);
            }
        }

        internal static void Initialize()
        {
            var defaultAssemblyResolver = new DefaultAssemblyResolver();
            defaultAssemblyResolver.AddSearchDirectory(Path.GetDirectoryName(UnityEditorInternal.InternalEditorUtility.GetEngineAssemblyPath()));
            MainModule = AssemblyDefinition.ReadAssembly(GetDllFolderHelper.GetFolder() + "UniNativeLinq.bytes", new ReaderParameters(ReadingMode.Deferred)
            {
                AssemblyResolver = defaultAssemblyResolver
            }).MainModule;
            SystemModule = AssemblyDefinition.ReadAssembly(GetDllFolderHelper.GetFolder() + "netstandard.bytes", new ReaderParameters(ReadingMode.Deferred)
            {
                AssemblyResolver = defaultAssemblyResolver,
            }).MainModule;
            UnityCoreModule = AssemblyDefinition.ReadAssembly(UnityEditorInternal.InternalEditorUtility.GetEngineCoreModuleAssemblyPath(), new ReaderParameters(ReadingMode.Deferred)
            {
                AssemblyResolver = defaultAssemblyResolver,
            }).MainModule;
            ExtensionAttribute = new CustomAttribute(MainModule.ImportReference(SystemModule.GetType("System.Runtime.CompilerServices", "ExtensionAttribute")).FindMethod(".ctor"));
            IsReadOnlyAttribute = MainModule.GetType("UniNativeLinq", "ZipValueTuple`2").CustomAttributes[2];
            var nativeEnumerable1 = MainModule.GetType("UniNativeLinq", "NativeEnumerable`1");
            var t = nativeEnumerable1.GenericParameters.First();
            UnManagedAttribute = t.CustomAttributes[0];
            SpecialTypeDictionary = new Dictionary<string, (TypeDefinition, Func<TypeReference, TypeReference>)>()
            {
                { "NativeArray<T>", (MainModule.GetType("UniNativeLinq", "NativeEnumerable`1"),
                    tx => MainModule.ImportReference(UnityCoreModule.GetType("Unity.Collections", "NativeArray`1").MakeGenericInstanceType(tx))) },
                {"T[]", (MainModule.GetType("UniNativeLinq", "ArrayEnumerable`1"),
                    tx => tx.MakeArrayType())}
            };
            Allocator = nativeEnumerable1.Methods.First(x => x.Name == "ToNativeArray").Parameters[0].ParameterType;

            Settings = AssetDatabase.LoadAssetAtPath<GlobalSettings>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:" + nameof(GlobalSettings))[0]));
        }

        public static GenericInstanceType MakeGenericInstanceType(this TypeReference self, IEnumerable<TypeReference> arguments)
        {
            var instance = new GenericInstanceType(self);
            foreach (var argument in arguments)
                instance.GenericArguments.Add(argument);
            return instance;
        }

        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, IEnumerable<TypeReference> arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericInstanceType(arguments))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };
            foreach (var parameter in self.Parameters)
            {
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType)
                {
                    Constant = parameter.Constant,
                });
            }
            foreach (var genericParameter in self.GenericParameters)
            {
                reference.GenericParameters.Add(new GenericParameter(genericParameter.Name, reference));
            }
            return reference;
        }

        public static FieldReference MakeHostInstanceGeneric(this FieldReference self, IEnumerable<TypeReference> arguments)
            => new FieldReference(self.Name, self.FieldType, self.DeclaringType.MakeGenericInstanceType(arguments));

        public static MethodReference FindMethod(this TypeReference type, string name)
        {
            var typeDefinition = type.ToDefinition();
            var methodDefinition = typeDefinition.Methods.Single(x => x.Name == name);
            if (methodDefinition.Module == type.Module)
            {
                return type is GenericInstanceType genericInstanceType ? methodDefinition.MakeHostInstanceGeneric(genericInstanceType.GenericArguments) : methodDefinition;
            }
            else
            {
                var imported = type.Module.ImportReference(methodDefinition);
                return type is GenericInstanceType genericInstanceType ? imported.MakeHostInstanceGeneric(genericInstanceType.GenericArguments) : imported;
            }
        }

        public static MethodReference FindMethod(this TypeReference type, string name, int parameterCount)
        {
            var typeDefinition = type.ToDefinition();
            var methodDefinition = typeDefinition.Methods.Single(x => x.Name == name && x.Parameters.Count == parameterCount);

            if (type.Module == typeDefinition.Module)
            {
                return type is GenericInstanceType genericInstanceType ? methodDefinition.MakeHostInstanceGeneric(genericInstanceType.GenericArguments) : methodDefinition;
            }
            else
            {
                var imported = type.Module.ImportReference(methodDefinition);
                return type is GenericInstanceType genericInstanceType ? imported.MakeHostInstanceGeneric(genericInstanceType.GenericArguments) : imported;
            }
        }

        public static FieldReference FindField(this TypeReference type, string name)
        {
            if (type is TypeDefinition definition)
                return definition.FindField(name);
            if (type is GenericInstanceType genericInstanceType)
                return genericInstanceType.FindField(name);
            var typeDefinition = type.ToDefinition();
            var fieldDefinition = typeDefinition.Fields.Single(x => x.Name == name);
            if (fieldDefinition.Module == type.Module)
                return fieldDefinition;
            return type.Module.ImportReference(fieldDefinition);
        }

        public static FieldReference FindField(this TypeDefinition type, string name)
        {
            return type.Fields.Single(x => x.Name == name);
        }

        public static FieldReference FindField(this GenericInstanceType type, string name)
        {
            var typeDefinition = type.ToDefinition();
            var definition = typeDefinition.Fields.Single(x => x.Name == name);
            return definition.MakeHostInstanceGeneric(type.GenericArguments);
        }

        public static MethodReference FindMethod(this TypeReference type, string name, Func<MethodDefinition, bool> predicate)
        {
            var methodDefinitions = type.ToDefinition().Methods;
            var methodDefinition = methodDefinitions.Single(x => x.Name == name && predicate(x));
            var imported = type.Module.ImportReference(methodDefinition);
            return type is GenericInstanceType genericInstanceType ? imported.MakeHostInstanceGeneric(genericInstanceType.GenericArguments) : imported;
        }

        public static List<GenericParameter> FromTypeToMethodParam(this MethodDefinition method, Collection<GenericParameter> typeGenericParameters, string suffix = "")
        {
            var methodGenericParameters = method.GenericParameters;
            var initialLength = methodGenericParameters.Count;
            var answer = new List<GenericParameter>(typeGenericParameters.Count);
            foreach (var typeGenericParameter in typeGenericParameters)
            {
                var methodGenericParameter = new GenericParameter(string.IsNullOrEmpty(suffix) ? typeGenericParameter.Name : typeGenericParameter.Name + suffix, method)
                {
                    HasNotNullableValueTypeConstraint = typeGenericParameter.HasNotNullableValueTypeConstraint,
                    HasDefaultConstructorConstraint = typeGenericParameter.HasDefaultConstructorConstraint,
                    IsContravariant = typeGenericParameter.IsContravariant,
                    IsValueType = typeGenericParameter.IsValueType,
                };
                foreach (var customAttribute in typeGenericParameter.CustomAttributes)
                    methodGenericParameter.CustomAttributes.Add(customAttribute);
                methodGenericParameters.Add(methodGenericParameter);
                answer.Add(methodGenericParameter);
            }
            for (var j = 0; j < answer.Count; j++)
            {
                var methodGenericParameter = methodGenericParameters[j + initialLength];
                foreach (var constraint in typeGenericParameters[j].Constraints)
                    methodGenericParameter.Constraints.Add(constraint.Replace(answer, suffix));
            }
            return answer;
        }

        public static void RewriteConstraints(this GenericParameter genericParameter, string specialName, TypeReference specialType)
        {
            var constraints = genericParameter.Constraints;
            foreach (var constraint in constraints)
            {
                switch (constraint)
                {
                    case GenericInstanceType genericInstanceType:
                        {
                            var genericArguments = genericInstanceType.GenericArguments;
                            for (var i = genericArguments.Count; --i >= 0;)
                            {
                                if (genericArguments[i].Name == specialName)
                                    genericArguments[i] = specialType;
                            }
                        }
                        break;
                }
            }
        }

        public static List<GenericParameter> FromTypeToMethodParam(this MethodDefinition method, Collection<GenericParameter> typeGenericParameters, string specialReplaceName, TypeReference specialReplaceType, string suffix = "")
        {
            var initialLength = method.GenericParameters.Count;
            var answer = new List<GenericParameter>(typeGenericParameters.Count);
            var skipIndex = -1;
            foreach (var (typeGenericParameter, i) in typeGenericParameters.Select((x, i) => (x, i)))
            {
                if (typeGenericParameter.Name == specialReplaceName)
                {
                    skipIndex = i;
                    continue;
                }
                var methodGenericParameter = new GenericParameter(typeGenericParameter.Name + suffix, method)
                {
                    HasNotNullableValueTypeConstraint = typeGenericParameter.HasNotNullableValueTypeConstraint,
                    HasDefaultConstructorConstraint = typeGenericParameter.HasDefaultConstructorConstraint,
                    IsContravariant = typeGenericParameter.IsContravariant,
                    IsValueType = typeGenericParameter.IsValueType,
                };
                foreach (var customAttribute in typeGenericParameter.CustomAttributes)
                {
                    methodGenericParameter.CustomAttributes.Add(customAttribute);
                }
                method.GenericParameters.Add(methodGenericParameter);
                answer.Add(methodGenericParameter);
            }
            for (var j = 0; j < answer.Count; j++)
            {
                var methodGenericParameter = method.GenericParameters[j + initialLength];
                foreach (var constraint in typeGenericParameters[j < skipIndex ? j : j + 1].Constraints)
                {
                    if (constraint.Module.Name == methodGenericParameter.Module.Name)
                        methodGenericParameter.Constraints.Add(constraint.Replace(answer, specialReplaceName, specialReplaceType));
                }
            }
            return answer;
        }

        internal static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters, string specialName, TypeReference specialType)
        {
            var genericParameters = methodGenericParameters as GenericParameter[] ?? methodGenericParameters.ToArray();
            switch (constraint)
            {
                case GenericInstanceType genericConstraint:
                    {
                        var newConstraint = (GenericInstanceType)constraint.Module.ImportReference(new GenericInstanceType(constraint.ToDefinition()));
                        foreach (var argument in genericConstraint.GenericArguments)
                            newConstraint.GenericArguments.Add(argument.Replace(genericParameters, specialName, specialType));
                        return newConstraint;
                    }
                case GenericParameter genericParameter when genericParameter.Name == specialName:
                    return specialType;
                case GenericParameter genericParameter:
                    var singleOrDefault = genericParameters.SingleOrDefault(x => x.Name == genericParameter.Name);
                    switch (singleOrDefault)
                    {
                        case null:
                            return constraint;
                        default:
                            var constraints = singleOrDefault.Constraints;
                            for (var i = constraints.Count; --i >= 0;)
                            {
                                constraints[i] = constraints[i].Replace(genericParameters, specialName, specialType);
                            }
                            return singleOrDefault;
                    }
                default:
                    return constraint;
            }
        }

        internal static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters, string specialName, TypeReference specialType, string suffix)
        {
            var genericParameters = methodGenericParameters as GenericParameter[] ?? methodGenericParameters.ToArray();
            switch (constraint)
            {
                case GenericInstanceType genericConstraint:
                    {
                        var newConstraint = (GenericInstanceType)constraint.Module.ImportReference(new GenericInstanceType(constraint.ToDefinition()));
                        foreach (var argument in genericConstraint.GenericArguments)
                            newConstraint.GenericArguments.Add(argument.Replace(genericParameters, specialName, specialType, suffix));
                        return newConstraint;
                    }
                case GenericParameter genericParameter when genericParameter.Name == specialName:
                    return specialType;
                case GenericParameter genericParameter:
                    var singleOrDefault = genericParameters.SingleOrDefault(x => x.Name == genericParameter.Name + suffix);
                    switch (singleOrDefault)
                    {
                        case null:
                            return constraint;
                        default:
                            var constraints = singleOrDefault.Constraints;
                            for (var i = constraints.Count; --i >= 0;)
                            {
                                constraints[i] = constraints[i].Replace(genericParameters, specialName, specialType, suffix);
                            }
                            return singleOrDefault;
                    }
                default:
                    return constraint;
            }
        }

        internal static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters)
        {
            switch (constraint)
            {
                case GenericInstanceType genericConstraint:
                    {
                        var newConstraint = (GenericInstanceType)constraint.Module.ImportReference(new GenericInstanceType(constraint.ToDefinition()));
                        var genericParameters = methodGenericParameters as GenericParameter[] ?? methodGenericParameters.ToArray();
                        foreach (var argument in genericConstraint.GenericArguments)
                            newConstraint.GenericArguments.Add(argument.Replace(genericParameters));
                        return newConstraint;
                    }
                case GenericParameter genericParameter:
                    var singleOrDefault = methodGenericParameters.SingleOrDefault(x => x.Name == genericParameter.Name);
                    switch (singleOrDefault)
                    {
                        case null:
                            return constraint;
                        default:
                            return singleOrDefault;
                    }
                default:
                    return constraint;
            }
        }

        internal static TypeReference Replace(this TypeReference constraint, IEnumerable<GenericParameter> methodGenericParameters, string suffix)
        {
            var genericParameters = methodGenericParameters as GenericParameter[] ?? methodGenericParameters.ToArray();
            switch (constraint)
            {
                case GenericInstanceType genericConstraint:
                    {
                        var newConstraint = (GenericInstanceType)constraint.Module.ImportReference(new GenericInstanceType(constraint.ToDefinition()));
                        foreach (var argument in genericConstraint.GenericArguments)
                            newConstraint.GenericArguments.Add(argument.Replace(genericParameters, suffix));
                        return newConstraint;
                    }
                case GenericParameter genericParameter:
                    var singleOrDefault = genericParameters.SingleOrDefault(x => x.Name == genericParameter.Name + suffix);
                    switch (singleOrDefault)
                    {
                        case null:
                            return constraint;
                        default:
                            var constraints = singleOrDefault.Constraints;
                            for (var i = constraints.Count; --i >= 0;)
                            {
                                constraints[i] = constraints[i].Replace(genericParameters, suffix);
                            }
                            return singleOrDefault;
                    }
                default:
                    return constraint;
            }
        }

        public static TypeReference GetElementTypeOfCollectionType(this TypeReference @this)
        {
            var enumeratorType = @this.GetEnumeratorTypeOfCollectionType().ToDefinition();
            var propertyDefinitions = enumeratorType.Properties;
            var propertyDefinition = propertyDefinitions.Single(x => x.Name == "Current");
            switch (propertyDefinition.PropertyType)
            {
                case ByReferenceType byref:
                    return byref.ElementType;
                default:
                    return propertyDefinition.PropertyType;
            }
        }

        public static TypeReference GetEnumeratorTypeOfCollectionType(this TypeReference @this)
        {
            var methodDefinitions = @this.ToDefinition().Methods;
            var methodDefinition = methodDefinitions.Single(x => x.Name == "GetEnumerator" && x.Parameters.Count == 0);
            var enumeratorType = methodDefinition.ReturnType;
            return enumeratorType;
        }
    }
}