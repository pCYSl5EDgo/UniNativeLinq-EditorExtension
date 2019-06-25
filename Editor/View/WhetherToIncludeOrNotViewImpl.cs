using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    internal sealed class WhetherToIncludeOrNotViewImpl : ScriptableObject, IWhetherToIncludeOrNotView
    {
        public bool FoldoutTopLevel;
        private IEnumerableCollectionProcessor processor;

        public WhetherToIncludeOrNotViewImpl(IEnumerableCollectionProcessor processor)
        {
            this.processor = processor;
        }

        public void Draw()
        {
            if (!FoldoutUtility.Draw(ref FoldoutTopLevel, "Whether to include each enumerable type or NOT")) return;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select All"))
                {
                    foreach (var name in processor.NameCollection)
                    {
                        if (name == "Native") continue;
                        processor.TrySetEnabled(name, true);
                    }
                }
                if (GUILayout.Button("Deselect All"))
                {
                    foreach (var name in processor.NameCollection)
                    {
                        if (name == "Native") continue;
                        processor.TrySetEnabled(name, false);
                    }
                }
            }

            foreach (var (name, enabled) in processor.NameEnabledTupleCollection)
            {
                using (IndentScope.Create())
                using (new EditorGUILayout.HorizontalScope())
                using (new EditorGUI.DisabledScope(name == "Native"))
                {
                    EditorGUILayout.LabelField(name);
                    processor.TrySetEnabled(name, EditorGUILayout.ToggleLeft(name + " : " + enabled, enabled, (GUIStyle)"button"));
                }
            }
        }
    }
}
