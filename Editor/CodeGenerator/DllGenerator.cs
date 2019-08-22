using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using UniNativeLinq.Editor.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class DllGenerator : IDllGenerator
    {
        private readonly ModuleDefinition mainModule;
        private readonly ModuleDefinition systemModule;
        private readonly ModuleDefinition unityCoreModule;

        public DllGenerator(ModuleDefinition mainModule, ModuleDefinition systemModule, ModuleDefinition unityCoreModule)
        {
            this.mainModule = mainModule;
            this.systemModule = systemModule;
            this.unityCoreModule = unityCoreModule;
        }

        public void Execute(IEnumerableCollectionProcessor processor, IApiExtensionMethodGenerator[] generators, IDependency[] dependencies)
        {
            RemoveUnnecessaryEnumerable(processor, dependencies, out var enumerableDefinitionDictionary, out var dependencyDictionary);
            GenerateExtensionMethods(processor, generators, enumerableDefinitionDictionary);
            Write();
        }

        private void GenerateExtensionMethods(IEnumerableCollectionProcessor processor, IApiExtensionMethodGenerator[] generators, Dictionary<string, TypeDefinition> definitions)
        {
            foreach (var generator in generators)
            {
                if (generator is ITypeDictionaryHolder holder)
                    holder.Dictionary = definitions;
                generator.Generate(processor, mainModule, systemModule, unityCoreModule);
            }
        }

        private void RemoveUnnecessaryEnumerable(IEnumerableCollectionProcessor processor, IDependency[] dependencies, out Dictionary<string, TypeDefinition> enumerableDefinitionDictionary, out Dictionary<string, IDependency> dependencyDictionary)
        {
            enumerableDefinitionDictionary = new Dictionary<string, TypeDefinition>();
            var names = processor.NameCollection.ToArray();
            foreach (var name in names)
            {
                if (!processor.IsSpecialType(name, out var value) || value) continue;
                enumerableDefinitionDictionary.Add(name, mainModule.GetType("UniNativeLinq", string.Intern(name + "Enumerable`" + processor.GetGenericParameterCount(name))));
            }
            dependencyDictionary = dependencies.ToDictionary(x => x.Enumerable, x => x);

            foreach (var name in names)
            {
                if (!processor.IsSpecialType(name, out var isSpecial) || isSpecial)
                {
                    continue;
                }
                if (!processor.TryGetEnabled(name, out var enabled) || enabled)
                {
                    continue;
                }
                var type = enumerableDefinitionDictionary[name];

                if (dependencyDictionary.TryGetValue(name, out var dependency))
                    RemoveDependency(type, dependency);

                mainModule.Types.Remove(type);
            }
        }

        private void RemoveDependency(TypeDefinition type, IDependency dependency)
        {
            if ((dependency.Types?.Length ?? 0) != 0)
                RemoveDependentTypes(dependency.Types);
            if ((dependency.Methods?.Length ?? 0) != 0)
                RemoveDependentMethods(type, dependency.Methods);
        }

        private void RemoveDependentMethods(TypeDefinition type, string[] dependencyMethods)
        {
            foreach (var method in dependencyMethods)
            {
                var splitIndex = method.IndexOf(':');
                var removeType = mainModule.GetType("UniNativeLinq", method.Substring(0, splitIndex));
                var removeMethod = method.Substring(splitIndex + 1);
                var removes = removeType.Methods.Where(x =>
                {
                    if (x.Name != removeMethod) return false;
                    Debug.Log(x.ReturnType.Name);
                    return true;
                }).ToArray();
            }
        }

        private void RemoveDependentTypes(string[] dependencyTypes)
        {
            foreach (var type in dependencyTypes)
            {
                mainModule.Types.Remove(mainModule.GetType("UniNativeLinq", type));
            }
        }

        private void Write()
        {
            var destination = new Uri(new Uri(GetDllFolderHelper.GetFolder()), @"../../UniNativeLinq.dll").LocalPath;
            EditorApplication.LockReloadAssemblies();
            try
            {
                mainModule.Write(destination);
            }
            catch
            {
                File.Delete(destination);
                throw;
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        public void Dispose()
        {
            mainModule?.Dispose();
            systemModule?.Dispose();
            unityCoreModule?.Dispose();
        }
    }
}