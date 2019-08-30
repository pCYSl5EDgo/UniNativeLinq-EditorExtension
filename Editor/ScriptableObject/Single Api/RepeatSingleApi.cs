using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    [CreateAssetMenu(menuName = "Linq/a")]
    public sealed class RepeatSingleApi : String2BoolArrayTuple, ISingleApi
    {
        string ISingleApi.Name
        {
            get => "RangeRepeat";
            set { }
        }
        string ISingleApi.Description => "Repeat";
        [field: SerializeField] public bool IsHided { get; set; }
        private static readonly string[] Related = { "RangeRepeat" };
        public string[] RelatedEnumerableArray => Related;
        public string[] ExcludeEnumerableArray => Array.Empty<string>();
        public IEnumerable<string> NameCollection => EnabledArray.Select(x => x.Enumerable);
        public int Count => EnabledArray.Length;
        public IEnumerable<(string Name, bool Enabled)> NameEnabledTupleCollection => EnabledArray.Select(x => (x.Enumerable, x.Enabled));
        public IEnumerable<string> EnabledNameCollection => EnabledArray.Where(x => x.Enabled).Select(x => x.Enumerable);

        public bool TryGetEnabled(string name, out bool value)
        {
            if (name != "")
            {
                value = false;
                return false;
            }
            value = EnabledArray[0].Enabled;
            return true;
        }

        public bool TrySetEnabled(string name, bool value)
        {
            if (name != "")
            {
                return false;
            }
            if (!(EnabledArray[0].Enabled ^ value))
            {
                return true;
            }
            EditorUtility.SetDirty(this);
            EnabledArray[0].Enabled = value;
            return true;
        }

        public void Draw(IEnumerableCollectionProcessor processor)
        {
            if (IsHided) return;
            ref var enabled = ref EnabledArray[0].Enabled;
            using (new EditorGUILayout.HorizontalScope())
            {
                if (EditorGUILayout.ToggleLeft(new GUIContent("Enumerable.Repeat"), enabled, GUI.skin.button) ^ enabled)
                {
                    enabled = !enabled;
                    EditorUtility.SetDirty(this);
                }
                if (GUILayout.Button("Hide and Deselect All"))
                {
                    IsHided = true;
                    enabled = false;
                    EditorUtility.SetDirty(this);
                }
            }
        }

        public int CompareTo(ISingleApi other)
        {
            if (other is null)
                return 1;
            var c = string.Compare(Name, other.Name, StringComparison.Ordinal);
            if (c == 0)
                return string.Compare(Description, other.Description, StringComparison.Ordinal);
            return c;
        }
    }
}
