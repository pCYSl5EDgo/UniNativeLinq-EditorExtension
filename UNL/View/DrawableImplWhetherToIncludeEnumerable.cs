using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    internal sealed class DrawableImplWhetherToIncludeEnumerable : IDrawable
    {
        public bool FoldoutTopLevel;
        private readonly IEnumerableCollectionProcessor processor;

        public DrawableImplWhetherToIncludeEnumerable(IEnumerableCollectionProcessor processor)
        {
            this.processor = processor;
        }

        public void Draw(ref Vector2 scrollPosition)
        {
            if (!FoldoutUtility.Draw(ref FoldoutTopLevel, "Whether to include each enumerable type or NOT")) return;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select All"))
                {
                    foreach (var name in processor.NameCollection)
                    {
                        if (name == "Native" || name == "Array") continue;
                        processor.TrySetEnabled(name, true);
                    }
                }
                if (GUILayout.Button("Deselect All"))
                {
                    foreach (var name in processor.NameCollection)
                    {
                        if (name == "Native" || name == "Array") continue;
                        processor.TrySetEnabled(name, false);
                    }
                }
            }

            foreach (var (name, enabled) in processor.NameEnabledTupleCollection)
            {
                foreach (var related in processor.GetRelatedEnumerable(name))
                {
                    if (!processor.TryGetEnabled(related, out var relatedEnabled)) throw new KeyNotFoundException();
                    if (relatedEnabled) continue;
                    if (enabled)
                        processor.TrySetEnabled(name, false);
                    goto NEXT;
                }
                using (IndentScope.Create())
                using (new EditorGUILayout.HorizontalScope())
                using (new EditorGUI.DisabledScope(name == "Native" || name == "Array"))
                {
                    EditorGUILayout.LabelField(name);
                    processor.TrySetEnabled(name, EditorGUILayout.ToggleLeft(name + " : " + enabled, enabled, (GUIStyle)"button"));
                }
            NEXT:;
            }
        }
    }
}
