using System;
using Mono.Cecil;
using UnityEditor;

namespace UniNativeLinq.Editor
{
    public sealed class DllGenerator : IDllGenerator, IDisposable
    {
        private ModuleDefinition mainModule;
        private ModuleDefinition systemModule;
        private ModuleDefinition unityCoreModule;
        public void Execute(IEnumerableCollectionProcessor processor, ISingleApi[] singleApis, IDoubleApi[] doubleApis)
        {
            var defaultAssemblyResolver = new DefaultAssemblyResolver();
            mainModule = AssemblyDefinition.ReadAssembly(GetDllFolderHelper.GetFolder() + "UniNativeLinq.bytes", new ReaderParameters(ReadingMode.Deferred)
            {
                AssemblyResolver = defaultAssemblyResolver
            }).MainModule;
            systemModule = AssemblyDefinition.ReadAssembly(GetDllFolderHelper.GetFolder() + "netstandard.bytes", new ReaderParameters(ReadingMode.Deferred)
            {
                AssemblyResolver = defaultAssemblyResolver,
            }).MainModule;
            unityCoreModule = AssemblyDefinition.ReadAssembly(UnityEditorInternal.InternalEditorUtility.GetEngineCoreModuleAssemblyPath(), new ReaderParameters(ReadingMode.Deferred)
            {
                AssemblyResolver = defaultAssemblyResolver,
            }).MainModule;
            var destination = new Uri(new Uri(GetDllFolderHelper.GetFolder()), @"../../UniNativeLinq.dll").LocalPath;
            EditorApplication.LockReloadAssemblies();
            try
            {
                mainModule.Write(destination);
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