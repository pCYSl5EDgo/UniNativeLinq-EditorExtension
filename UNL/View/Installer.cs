using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace UniNativeLinq.Editor
{
    internal static class GetViewFolderHelper
    {
        public static string GetFolder() => InternalGetFolder();

        private static string InternalGetFolder([CallerFilePath]string path = "") => path.Substring(0, path.Length - 3 - nameof(Installer).Length);
    }
    internal static class Installer
    {
        [MenuItem("UniNativeLinq/Import/Import UniNativeLinq Essential Resources", false, 2050)]
        public static void ImportProjectResourcesMenu()
        {
            var packageFullPath = Path.Combine(GetViewFolderHelper.GetFolder(), "../Packages/UniNativeLinq-Settings.unitypackage");
            AssetDatabase.ImportPackage(packageFullPath, true);
        }
    }
}