using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class WithFuncEnumerableSingleApi : String2BoolArrayTuple, ISingleApi
    {
        string ISingleApi.Name
        {
            get => Name;
            set
            {
                if (RelatedEnumerableArray?.Length != 1)
                    RelatedEnumerableArray = new string[1];
                Name = RelatedEnumerableArray[0] = value;
            }
        }

        string ISingleApi.Description => Description;
        [field: SerializeField] public string[] RelatedEnumerableArray { get; private set; }
        [field: SerializeField] public bool IsHided { get; set; }
        public IEnumerable<string> NameCollection => EnabledArray.Select(x => x.Enumerable);
        public int Count => EnabledArray.Length;
        public IEnumerable<(string Name, bool Enabled)> NameEnabledTupleCollection => EnabledArray.Select(x => (x.Enumerable, x.Enabled));
        public IEnumerable<string> EnabledNameCollection => EnabledArray.Where(x => x.Enabled).Select(x => x.Enumerable);
        public bool TryGetEnabled(string name, out bool value)
        {
            for (var i = 0; i < EnabledArray.Length; i++)
            {
                ref var tuple = ref EnabledArray[i];
                if (tuple.Enumerable != name) continue;
                value = tuple.Enabled;
                return true;
            }
            value = default;
            return false;
        }

        public bool TrySetEnabled(string name, bool value)
        {
            for (var i = 0; i < EnabledArray.Length; i++)
            {
                ref var tuple = ref EnabledArray[i];
                if (tuple.Enumerable != name) continue;
                tuple.Enabled = value;
                return true;
            }
            return false;
        }

        //[MenuItem("UniNativeLinq/Create Single Apis &#e")]
        public static void Create()
        {
            var three = new[] { "Operator", "Func", "RefFunc" };
            var four = new[] { "None", "Operator", "Func", "RefFunc" };
            var numbers = new[]
            {
                "Int32",
                "UInt32",
                "Int64",
                "UInt64",
                "Single",
                "Double",
            };
            //foreach (var number in numbers)
            //{
            //    Create<WithFuncEnumerableSingleApi>(obj, "MinBy" + number, three);
            //    Create<WithFuncEnumerableSingleApi>(obj, "MaxBy" + number, three);
            //}
            //Create<WithFuncEnumerableSingleApi>(obj, "SelectIndex", three);
            //Create<WithFuncEnumerableSingleApi>(obj, "Select", three);
            //Create<WithFuncEnumerableSingleApi>(obj, "WhereIndex", three);
            //Create<WithFuncEnumerableSingleApi>(obj, "Where", three);
            //Create<WithFuncEnumerableSingleApi>(obj, "Distinct", four);
            //Create<WithFuncEnumerableSingleApi>(obj, "OrderBy", four);
            //Create<WithFuncEnumerableSingleApi>(obj, "SkipWhile", three);
            //Create<WithFuncEnumerableSingleApi>(obj, "TakeWhile", three);
        }

        private bool fold;
        public void Draw(IEnumerableCollectionProcessor processor)
        {
            if (IsHided) return;
            foreach (var relatedEnumerable in RelatedEnumerableArray)
            {
                if (!processor.TryGetEnabled(relatedEnumerable, out var enabled)) throw new KeyNotFoundException(relatedEnumerable);
                if (!enabled) return;
            }
            if (!FoldoutUtility.Draw(ref fold, new GUIContent(Name + "\t\t" + Description, Description))) return;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select All"))
                {
                    for (var i = 0; i < EnabledArray.Length; i++)
                    {
                        ref var tuple = ref EnabledArray[i];
                        tuple.Enabled = true;
                    }
                }
                else if (GUILayout.Button("Deselect All"))
                {
                    for (var i = 0; i < EnabledArray.Length; i++)
                    {
                        ref var tuple = ref EnabledArray[i];
                        tuple.Enabled = false;
                    }
                }
                else if (GUILayout.Button("Hide and Deselect All"))
                {
                    IsHided = true;
                    for (var i = 0; i < EnabledArray.Length; i++)
                    {
                        ref var tuple = ref EnabledArray[i];
                        tuple.Enabled = false;
                    }
                }
            }

            for (var i = 0; i < EnabledArray.Length; i++)
            {
                ref var tuple = ref EnabledArray[i];
                if (!processor.TryGetEnabled(tuple.Enumerable, out var targetEnabled) || !targetEnabled) continue;
                using (IndentScope.Create())
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent(tuple.Enumerable, Description));
                    var guiContent = new GUIContent(tuple.Enumerable + " : " + tuple.Enabled, Description);
                    TrySetEnabled(tuple.Enumerable, EditorGUILayout.ToggleLeft(guiContent, tuple.Enabled, GUI.skin.button));
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
