using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public static class SelectDeselectAllHelper
    {
        private const string SelectAll = "Select All";
        private const string DeselectAll = "Deselect All";

        public static void SelectDeselectAll<T>(this T[] array, Func<T, bool> predicate, Action<T[], bool> action)
        {
            using (var horizontalScope = new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(array.All(predicate)))
                    if (GUILayout.Button(SelectAll))
                        action(array, true);

                using (var disabledScope = new EditorGUI.DisabledScope(!array.Any(predicate)))
                    if (GUILayout.Button(DeselectAll))
                        action(array, false);
            }
        }
    }
}
