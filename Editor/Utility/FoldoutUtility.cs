using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    internal static class FoldoutUtility
    {
        public static bool Draw(ref bool flag, string text) => flag = EditorGUILayout.Foldout(flag, text);
        public static bool Draw(ref bool flag, GUIContent content) => flag = EditorGUILayout.Foldout(flag, content);
    }
}
