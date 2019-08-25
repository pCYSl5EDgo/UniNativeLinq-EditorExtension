using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

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

        static Helper()
        {
            Initialize();
        }

        public static CustomAttribute GetSystemRuntimeInteropServicesUnmanagedTypeConstraintTypeReference()
        {
            return MainModule.GetType("UniNativeLinq", "NativeEnumerable`1").GenericParameters.First().CustomAttributes[0];
        }

        public static CustomAttribute GetSystemRuntimeCompilerServicesReadonlyAttributeTypeReference()
        {
            return MainModule.GetType("UniNativeLinq.NegatePredicate`2").GetConstructors().First().Parameters.First().CustomAttributes.First();
        }

        public static (TypeReference baseEnumerable, GenericInstanceType specialEnumerable, GenericInstanceType specialEnumerator) MakeSpecialTypePair(this GenericParameter genericParameter, string specialName)
        {
            TypeDefinition typeDefinition;
            switch (specialName)
            {
                case "T[]":
                    typeDefinition = MainModule.GetType("UniNativeLinq", "ArrayEnumerable`1");
                    return (
                        new ArrayType(genericParameter),
                        new GenericInstanceType(typeDefinition)
                        {
                            GenericArguments = { genericParameter }
                        },
                        new GenericInstanceType(typeDefinition.NestedTypes.First(x => x.Name == "Enumerator"))
                        {
                            GenericArguments = { genericParameter }
                        }
                    );
                case "NativeArray<T>":
                    typeDefinition = MainModule.GetType("UniNativeLinq", "NativeEnumerable`1");
                    return (
                        new GenericInstanceType(MainModule.ImportReference(UnityCoreModule.GetType("Unity.Collections", "NativeArray`1")))
                        {
                            GenericArguments = { genericParameter }
                        },
                        new GenericInstanceType(typeDefinition)
                        {
                            GenericArguments = { genericParameter }
                        },
                        new GenericInstanceType(typeDefinition.NestedTypes.First(x => x.Name == "Enumerator"))
                        {
                            GenericArguments = { genericParameter }
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
            var nativeEnumerable = MainModule.GetType("UniNativeLinq.NativeEnumerable");
            ExtensionAttribute = nativeEnumerable.CustomAttributes.Single();
            var negateMethodDefinition = MainModule.GetType("UniNativeLinq.NegatePredicate`2").GetConstructors().First();
            IsReadOnlyAttribute = negateMethodDefinition.Parameters.First().CustomAttributes.First();
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
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            foreach (var genericParameter in self.GenericParameters)
                reference.GenericParameters.Add(new GenericParameter(genericParameter.Name, reference));
            return reference;
        }

        public static FieldReference MakeHostInstanceGeneric(this FieldReference self, IEnumerable<TypeReference> arguments)
            => new FieldReference(self.Name, self.FieldType, self.DeclaringType.MakeGenericInstanceType(arguments));

        public static MethodReference FindMethod(this TypeReference type, string name)
        {
            var typeDefinition = type.ToDefinition();
            var methodDefinition = typeDefinition.Methods.Single(x => x.Name == name);
            var imported = type.Module.ImportReference(methodDefinition);
            return type is GenericInstanceType genericInstanceType ? imported.MakeHostInstanceGeneric(genericInstanceType.GenericArguments) : imported;
        }

        public static MethodReference FindMethod(this TypeReference type, string name, int parameterCount)
        {
            var typeDefinition = type.ToDefinition();
            var methodDefinition = typeDefinition.Methods.Single(x => x.Name == name && x.Parameters.Count == parameterCount);
            var imported = type.Module.ImportReference(methodDefinition);
            return type is GenericInstanceType genericInstanceType ? imported.MakeHostInstanceGeneric(genericInstanceType.GenericArguments) : imported;
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

        public static GenericInstanceType FindNested(this GenericInstanceType type, string name)
        {
            var nestedType = new GenericInstanceType(((TypeDefinition)type.ElementType).NestedTypes.First(x => x.Name.EndsWith(name)));
            foreach (var argument in type.GenericArguments)
                nestedType.GenericArguments.Add(argument);
            return nestedType;
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