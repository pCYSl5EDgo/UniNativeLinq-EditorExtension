using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class GlobalSettings : ScriptableObject
    {
        public bool EnableNullCheckOnRuntime;


        private static GlobalSettings _instance;

        public static GlobalSettings Instance => _instance ?? (_instance = AssetDatabase.LoadAssetAtPath<GlobalSettings>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:" + nameof(GlobalSettings))[0])));
    }
}