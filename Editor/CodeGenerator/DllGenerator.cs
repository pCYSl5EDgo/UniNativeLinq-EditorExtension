using System;
using Mono.Cecil;
using UnityEditor;

namespace UniNativeLinq.Editor
{
    sealed class DllGenerator : IDllGenerator, IDisposable
    {
        private ModuleDefinition _mainModule;
        private ModuleDefinition _systemModule;
        private ModuleDefinition _unityCoreModule;
        public void Execute(EnumerableCollectionObject setting)
        {
            var defaultAssemblyResolver = new DefaultAssemblyResolver();
            _mainModule = AssemblyDefinition.ReadAssembly(GetDllFolderHelper.GetFolder() + "UniNativeLinq.bytes", new ReaderParameters(ReadingMode.Deferred)
            {
                AssemblyResolver = defaultAssemblyResolver
            }).MainModule;
            _systemModule = AssemblyDefinition.ReadAssembly(GetDllFolderHelper.GetFolder() + "netstandard.bytes", new ReaderParameters(ReadingMode.Deferred)
            {
                AssemblyResolver = defaultAssemblyResolver,
            }).MainModule;
            _unityCoreModule = AssemblyDefinition.ReadAssembly(UnityEditorInternal.InternalEditorUtility.GetEngineCoreModuleAssemblyPath(), new ReaderParameters(ReadingMode.Deferred)
            {
                AssemblyResolver = defaultAssemblyResolver,
            }).MainModule;
            var destination = new Uri(new Uri(GetDllFolderHelper.GetFolder()), @"../../UniNativeLinq.dll").LocalPath;
            EditorApplication.LockReloadAssemblies();
            try
            {
                _mainModule.Write(destination);
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        public void Dispose()
        {
            _mainModule?.Dispose();
            _systemModule?.Dispose();
        }
    }

    //internal sealed class GetUnityCore : AssetPostprocessor
    //{
    //    internal static string UnityCorePath = null;
    //    // ReSharper disable once InconsistentNaming
    //    private static string OnGeneratedCSProject(string path, string content)
    //    {
    //        var document = XDocument.Parse(content);
    //        var root = document.Root;
    //        var element = root.Descendants()
    //            .First(x => x.Name.LocalName == "Reference" && (string)x.Attribute("Include") == "UnityEngine.CoreModule");
    //        Debug.Log(element.ToString());
    //        Debug.Log(element.Value.ToString());
    //        UnityCorePath = element.Value;
    //        return document.Declaration + Environment.NewLine + document.Root;
    //    }
    //}
}