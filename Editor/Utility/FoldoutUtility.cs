using UnityEditor;

namespace UniNativeLinq.Editor
{
    internal static class FoldoutUtility
    {
        public static bool Draw(ref bool flag, string text) => flag = EditorGUILayout.Foldout(flag, text);
    }
}
