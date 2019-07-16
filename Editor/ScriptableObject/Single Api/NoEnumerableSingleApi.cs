using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public class NoEnumerableSingleApi : String2BoolArrayTuple, ISingleApi
    {
        string ISingleApi.Name
        {
            get => Name;
            set => Name = value;
        }
        string ISingleApi.Description => Description;
        [field: SerializeField] public bool IsHided { get; set; }
        public string[] RelatedEnumerableArray => Array.Empty<string>();
        [field: SerializeField] public string[] ExcludeEnumerableArray { get; internal set; }
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

        //[MenuItem("UniNativeLinq/Convert")]
        public static void Convert()
        {
            var n0 = AssetDatabase.FindAssets("t:" + nameof(DifferentNameEnumerableDoubleApi));
            var n1 = AssetDatabase.FindAssets("t:" + nameof(SimpleEnumerableDoubleApi));
            var n2 = AssetDatabase.FindAssets("t:" + nameof(WithFuncEnumerableDoubleApi));
            var a0 = n0.Select(_ => AssetDatabase.LoadAssetAtPath<DifferentNameEnumerableDoubleApi>(AssetDatabase.GUIDToAssetPath(_)));
            var a1 = n1.Select(_ => AssetDatabase.LoadAssetAtPath<SimpleEnumerableDoubleApi>(AssetDatabase.GUIDToAssetPath(_)));
            var a2 = n2.Select(_ => AssetDatabase.LoadAssetAtPath<WithFuncEnumerableDoubleApi>(AssetDatabase.GUIDToAssetPath(_)));
            T0[] Gets0<T0>() where T0 : UnityEngine.Object
            {
                var paths = AssetDatabase.FindAssets("t:" + typeof(T0).Name);
                var answer = paths.Length == 0 ? Array.Empty<T0>() : new T0[paths.Length];
                for (var i = 0; i < paths.Length; i++)
                    answer[i] = AssetDatabase.LoadAssetAtPath<T0>(AssetDatabase.GUIDToAssetPath(paths[i]));
                return answer;
            }
            var c = new Comparer { Processor = new EnumerableCollectionProcessor(Gets0<StringBoolTuple>()) };
            //foreach (var api in a0)
            //{
            //    if ((api.ExcludeEnumerableArray?.Length ?? 0) != 0) continue;
            //    api.ExcludeEnumerableArray = Array.Empty<string>();
            //    var old = api.EnabledArray;
            //    api.EnabledArray = new StringBoolValueTuple[old.Length + 1];
            //    Array.Copy(old, api.EnabledArray, old.Length);
            //    api.EnabledArray[old.Length] = new StringBoolValueTuple { Enabled = true, Enumerable = "GroupBy" };
            //    Array.Sort(api.EnabledArray, c);
            //}
            //foreach (var api in a1)
            //{
            //    if ((api.ExcludeEnumerableArray?.Length ?? 0) != 0) continue;
            //    api.ExcludeEnumerableArray = Array.Empty<string>();
            //    var old = api.EnabledArray;
            //    api.EnabledArray = new StringBoolValueTuple[old.Length + 1];
            //    Array.Copy(old, api.EnabledArray, old.Length);
            //    api.EnabledArray[old.Length] = new StringBoolValueTuple { Enabled = true, Enumerable = "GroupBy" };
            //    Array.Sort(api.EnabledArray, c);
            //}
            //foreach (var api in a2)
            //{
            //    if ((api.ExcludeEnumerableArray?.Length ?? 0) != 0) continue;
            //    api.ExcludeEnumerableArray = Array.Empty<string>();
            //    var old = api.EnabledArray;
            //    api.EnabledArray = new StringBoolValueTuple[old.Length + 1];
            //    Array.Copy(old, api.EnabledArray, old.Length);
            //    api.EnabledArray[old.Length] = new StringBoolValueTuple { Enabled = true, Enumerable = "GroupBy" };
            //    Array.Sort(api.EnabledArray, c);
            //}
        }

        private sealed class Comparer : IComparer<StringBoolValueTuple>
        {
            public IEnumerableCollectionProcessor Processor;
            public int Compare(StringBoolValueTuple x, StringBoolValueTuple y)
            {
                var xn = x.Enumerable;
                var yn = y.Enumerable;
                Processor.IsSpecialType(xn, out var xs);
                Processor.IsSpecialType(yn, out var ys);
                return xs ? !ys ? -1 : string.Compare(xn, yn, StringComparison.Ordinal) : ys ? 1 : string.Compare(xn, yn, StringComparison.Ordinal);
            }
        }

        //[MenuItem("UniNativeLinq/Create Single Apis &#e")]
        public static void Create()
        {
            //var three = new[] { "Operator", "Func", "RefFunc" };
            //var four = new[] { "None", "Operator", "Func", "RefFunc" };
            //var numbers = new[]
            //{
            //    "Int32",
            //    "UInt32",
            //    "Int64",
            //    "UInt64",
            //    "Single",
            //    "Double",
            //};
            //Create<NoEnumerableSingleApi>(obj, "Aggregate",
            //    "TNextResult0 Aggregate<TAccumulate0, TNextResult0>(TAccumulate0 seed, Func<TAccumulate0, T, TAccumulate0> func, Func<TAccumulate0, TNextResult0> resultFunc)",
            //    "TAccumulate0 Aggregate<TAccumulate0>(TAccumulate0 seed, Func<TAccumulate0, T, TAccumulate0> func)",
            //    "T Aggregate(Func<T, T, T> func)",
            //    "TNextResult0 Aggregate<TAccumulate0, TNextResult0>(TAccumulate0 seed, RefFunc<TAccumulate0, T, TAccumulate0> func, RefFunc<TAccumulate0, TNextResult0> resultFunc)",
            //    "TAccumulate0 Aggregate<TAccumulate0>(TAccumulate0 seed, RefFunc<TAccumulate0, T, TAccumulate0> func)",
            //    "T Aggregate(RefFunc<T, T, T> func)",
            //    "TNextResult0 Aggregate<TAccumulate0, TNextResult0, TFunc0, TResultFunc0>(ref TAccumulate0 seed, in TFunc0 func, in TResultFunc0 resultFunc)",
            //    "void Aggregate<TAccumulate0, TFunc0>(ref TAccumulate0 seed, in TFunc0 func)"
            //);
            //Create<NoEnumerableSingleApi>(obj, "TryGetLast");
            //Create<NoEnumerableSingleApi>(obj, "TryGetFirst");
            //Create<NoEnumerableSingleApi>(obj, "TryGetSingle");
            //Create<NoEnumerableSingleApi>(obj, "TryGetElementAt");
            //var t = numbers.SelectMany(num => four, (num, desc) => desc + " --- " + num).ToArray();
            //Create<NoEnumerableSingleApi>(obj, "TryGetMin", t);
            //Create<NoEnumerableSingleApi>(obj, "TryGetMax", t);
            //Create<NoEnumerableSingleApi>(obj, "Any", three);
            //Create<NoEnumerableSingleApi>(obj, "All", three);
            //Create<NoEnumerableSingleApi>(obj, "Contains", four);
            //Create<NoEnumerableSingleApi>(obj, "Average", numbers);
            //Create<NoEnumerableSingleApi>(obj, "Sum", numbers);
        }

        private bool fold;
        public void Draw(IEnumerableCollectionProcessor processor)
        {
            if (IsHided) return;
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
                if (!processor.TryGetEnabled(tuple.Enumerable, out var targetEnabled) || !targetEnabled || (ExcludeEnumerableArray?.Contains(tuple.Enumerable) ?? false)) continue;

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
