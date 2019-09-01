using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class JustGenericSingleApi : String2BoolArrayTuple, ISingleApi
    {
        string ISingleApi.Name
        {
            get => Name;
            set => Name = value;
        }
        string ISingleApi.Description => Description;

        [field: SerializeField] public bool IsHided { get; set; }
        [field: SerializeField] public string[] RelatedEnumerableArray { get; internal set; }
        [field: SerializeField] public string[] ExcludeEnumerableArray { get; internal set; }

        public IEnumerable<string> NameCollection => EnabledArray.Select(x => x.Enumerable);
        public int Count => EnabledArray.Length;
        public IEnumerable<(string Name, bool Enabled)> NameEnabledTupleCollection => EnabledArray.Select(x => (x.Enumerable, x.Enabled));
        public IEnumerable<string> EnabledNameCollection => EnabledArray.Where(x => x.Enabled).Select(x => x.Enumerable);

        public bool TryGetEnabled(string name, out bool value)
        {
            if (name != "TEnumerable")
            {
                value = default;
                return false;
            }
            value = EnabledArray[0].Enabled;
            return true;
        }

        public bool TrySetEnabled(string name, bool value)
        {
            if (name != "TEnumerable")
                return false;
            if (EnabledArray[0].Enabled ^ value)
                EditorUtility.SetDirty(this);
            EnabledArray[0].Enabled = value;
            return true;
        }

        public int CompareTo(ISingleApi other)
        {
            if (other is null)
                return 1;
            var c = string.Compare(Name, other.Name, StringComparison.Ordinal);
            return c == 0 ? string.Compare(Description, other.Description, StringComparison.Ordinal) : c;
        }

        public void Draw(IEnumerableCollectionProcessor processor)
        {
            if (IsHided) return;
            foreach (var relatedEnumerable in RelatedEnumerableArray)
            {
                if (!processor.TryGetEnabled(relatedEnumerable, out var enabled))
                {
                    Debug.LogError(relatedEnumerable + " of " + Name);
                    throw new KeyNotFoundException();
                }
                if (!enabled) return;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                ref var enabled = ref EnabledArray[0].Enabled;
                if (EditorGUILayout.ToggleLeft(new GUIContent(Name + " " + Description + " Generic"), enabled, GUI.skin.button) ^ enabled)
                {
                    enabled = !enabled;
                    EditorUtility.SetDirty(this);
                }
                if (GUILayout.Button("Hide and Deselect"))
                {
                    IsHided = true;
                    enabled = false;
                    EditorUtility.SetDirty(this);
                }
            }
        }
    }
}