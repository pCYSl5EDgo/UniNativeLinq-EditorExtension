using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    internal static class Installer
    {
        [MenuItem("UniNativeLinq/Import/Import UniNativeLinq Essential Resources", false, 2050)]
        public static void ImportProjectResourcesMenu()
        {
            var packageFullPath = Path.Combine(GetDllFolderHelper.GetFolder(), "../Packages/UniNativeLinq-Settings.unitypackage");
            AssetDatabase.ImportPackage(packageFullPath, true);
        }
    }
}