using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class DifferentNameEnumerableDoubleApi : String2BoolMatrixTuple, IDoubleApi
    {
        public int CompareTo(IDoubleApi other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
            return nameComparison != 0 ? nameComparison : string.Compare(Description, other.Description, StringComparison.Ordinal);
        }

        string IDoubleApi.Name
        {
            get => Name;
            set => Name = value;
        }

        public string DisplayName;

        string IDoubleApi.Description => Description;
        [field: SerializeField] public string[] RelatedEnumerableArray { get; private set; }
        [field: SerializeField] public string[] ExcludeEnumerableArray { get; private set; }

        public IEnumerable<string> NameCollection => EnumerableArray;

        public int Count => EnumerableArray.Length;

        public bool TryGetEnabled(string name0, string name1, out bool value)
        {
            if (string.IsNullOrWhiteSpace(name0) || string.IsNullOrWhiteSpace(name1))
            {
                value = false;
                return false;
            }
            int index0 = -1, index1 = -1;
            for (var i = 0; i < EnumerableArray.Length; i++)
            {
                if (name0 == EnumerableArray[i])
                    index0 = i;
                if (name1 == EnumerableArray[i])
                    index1 = i;
            }
            if (index0 == -1 || index1 == -1)
            {
                value = false;
                return false;
            }
            value = this[index0, index1];
            return true;
        }

        public bool TrySetEnabled(string name0, string name1, bool value)
        {
            if (string.IsNullOrWhiteSpace(name0) || string.IsNullOrWhiteSpace(name1))
            {
                return false;
            }
            int index0 = -1, index1 = -1;
            for (var i = 0; i < EnumerableArray.Length; i++)
            {
                if (name0 == EnumerableArray[i])
                    index0 = i;
                if (name1 == EnumerableArray[i])
                    index1 = i;
            }
            if (index0 == -1 || index1 == -1)
            {
                return false;
            }
            this[index0, index1] = value;
            return true;
        }

        //[MenuItem("UniNativeLinq/Create Double Api &#d")]
        //public static void Create()
        //{
        //    var three = new[] { "Func", "RefFunc", "Operator" };
        //    var four = new[] { "None", "Func", "RefFunc", "Operator" };

        //    //Create(processor, "GroupJoin", "GroupJoin", new[] { "GroupJoin", "Grouping" }, three);
        //    //Create(processor, "AdjustedZip", "Zip(Adjusted)", new[] { "AdjustedZip" }, four);
        //    //Create(processor, "ExceptionalZip", "Zip(Exceptional)", new[] { "ExceptionalZip" }, four);
        //    //Create(processor, "Except", "Except", new[] { "SetOperation" }, four);
        //    //Create(processor, "Intersect", "Intersect", new[] { "SetOperation" }, four);
        //}

        //[MenuItem("UniNativeLinq/Convert")]
        //public static void Convert()
        //{
        //    Convert<DifferentNameEnumerableDoubleApi>();
        //    Convert<SimpleEnumerableDoubleApi>();
        //    Convert<WithFuncEnumerableDoubleApi>();
        //}

        //private static void Convert<T>() where T : String2BoolMatrixTuple, IDoubleApi
        //{
        //    var a = AssetDatabase.FindAssets("t:" + typeof(T).Name).Select(_ => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(_)));
        //    foreach (var api in a)

        //    {

        //        if ((api.ExcludeEnumerableArray?.Length ?? 0) != 0) continue;
        //        var old = api.EnumerableArray;
        //        api.EnabledArray = new bool[(old.Length + 1) * (old.Length + 1)];
        //        for (int i = 0; i < api.EnabledArray.Length; i++)
        //        {
        //            api.EnabledArray[i] = true;
        //        }
        //        api.EnumerableArray = new string[old.Length + 1];
        //        Array.Copy(old, api.EnumerableArray, old.Length);
        //        api.EnumerableArray[old.Length] = "GroupBy";
        //        for (var i = old.Length - 1; i >= 0; i--)
        //        {
        //            if (string.CompareOrdinal("GroupBy", api.EnumerableArray[i]) >= 0) break;
        //            (api.EnumerableArray[i], api.EnumerableArray[i + 1]) = (api.EnumerableArray[i + 1], api.EnumerableArray[i]);
        //        }
        //    }
        //}

        private static DifferentNameEnumerableDoubleApi[] Create(IEnumerableCollectionProcessor processor, string name, string displayName, string[] relatedEnumerableArray, string[] descriptions)
        {
            var array = processor.NameCollection.ToArray();
            var answer = new DifferentNameEnumerableDoubleApi[descriptions.Length];
            var index = 0;
            foreach (var description in descriptions)
            {
                answer[index] = CreateInstance<DifferentNameEnumerableDoubleApi>();
                ref var api = ref answer[index++];
                ((IDoubleApi)api).Name = name;
                api.DisplayName = displayName;
                api.RelatedEnumerableArray = relatedEnumerableArray;
                api.Description = description;
                api.EnumerableArray = new string[array.Length];
                for (var i = 0; i < array.Length; i++)
                    api.EnumerableArray[i] = array[i];
                api.EnabledArray = new bool[api.Count * api.Count];
                for (var i = 0; i < api.EnabledArray.Length; i++)
                    api.EnabledArray[i] = true;
                AssetDatabase.CreateAsset(api, "Assets/UniNativeLinq/Editor/ApiDouble/" + name + "___" + (description.Length <= 10 ? description : description.GetHashCode().ToString("X")) + ".asset");
            }
            return answer;
        }

        [field: SerializeField] public bool IsHided { get; set; }
        [NonSerialized] private bool fold;
        public void Draw(IEnumerableCollectionProcessor processor, ref Vector2 scrollPosition)
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
            if (!FoldoutUtility.Draw(ref fold, Name + "\t\t" + Description)) return;
            ShowMatrix(processor, ref scrollPosition);
        }

        private void ShowMatrix(IEnumerableCollectionProcessor processor, ref Vector2 scrollPosition)
        {
            using (IndentScope.Create())
            using (new EditorGUILayout.VerticalScope())
            {
                const float labelSize = 140f;
                const float indent = 30f;
                const float checkboxSize = 16f;
                const float buttonSize = 60f;

                var enabledEnumerableArray = this.EnumerableArray.Where(x => processor.TryGetEnabled(x, out var xe) && xe).ToArray();
                var lastRect = GUILayoutUtility.GetRect(labelSize + indent + checkboxSize * enabledEnumerableArray.Length, labelSize + indent);
                var pivotPoint = new Vector2(lastRect.xMin, lastRect.yMin);

                if (GUI.Button(new Rect(lastRect.xMin, checkboxSize + lastRect.yMin, labelSize, labelSize / 3f), "Select All"))
                {
                    for (var i = 0; i < EnabledArray.Length; i++)
                        EnabledArray[i] = true;
                }
                else if (GUI.Button(new Rect(lastRect.xMin, checkboxSize + lastRect.yMin + labelSize / 3f, labelSize, labelSize / 3f), "Deselect All"))
                {
                    Array.Clear(EnabledArray, 0, EnabledArray.Length);
                }
                else if (GUI.Button(new Rect(lastRect.xMin, checkboxSize + lastRect.yMin + labelSize * 2f / 3f, labelSize, labelSize / 3f), "Hide All"))
                {
                    IsHided = true;
                    Array.Clear(EnabledArray, 0, EnabledArray.Length);
                }
                DrawRotatedColumnLabels(processor, scrollPosition, pivotPoint, enabledEnumerableArray, checkboxSize, lastRect, labelSize, buttonSize);
                DrawCheckBoxMatrix(enabledEnumerableArray, indent, checkboxSize, labelSize);
            }
        }

        private void DrawRotatedColumnLabels(IEnumerableCollectionProcessor processor, Vector2 scrollPosition, Vector2 pivotPoint, string[] enabledEnumerableArray, float checkboxSize, Rect lastRect, float labelSize, float buttonSize)
        {
            const float indent = 30f;

            Rotate(pivotPoint, enabledEnumerableArray.Length, checkboxSize, lastRect, indent + labelSize + scrollPosition.y);
            var typeLength = processor.Count;
            for (var displayColumn = 0; displayColumn < enabledEnumerableArray.Length; displayColumn++)
            {
                var enumerable = enabledEnumerableArray[displayColumn];
                GUI.Label(new Rect(checkboxSize * 0.5f, displayColumn * checkboxSize + scrollPosition.y, labelSize + indent - checkboxSize, checkboxSize), enumerable, "RightLabel");
                if (GUI.Button(new Rect(labelSize + enabledEnumerableArray.Length * checkboxSize + indent * 2, displayColumn * checkboxSize + scrollPosition.y, buttonSize, checkboxSize), new GUIContent("Select All", enumerable), "minibutton"))
                {
                    var column = FindIndex(enumerable);
                    if (column == -1) throw new KeyNotFoundException();
                    for (var row = typeLength; --row >= 0;)
                        this[row, column] = true;
                }
                else if (GUI.Button(new Rect(labelSize + enabledEnumerableArray.Length * checkboxSize + indent * 2 + buttonSize, displayColumn * checkboxSize + scrollPosition.y, buttonSize, checkboxSize), new GUIContent("Deselect All", enumerable), "minibutton"))
                {
                    var column = FindIndex(enumerable);
                    if (column == -1) throw new KeyNotFoundException();
                    for (var row = typeLength; --row >= 0;)
                        this[row, column] = false;
                }
            }
            GUI.matrix = Matrix4x4.identity;
        }

        private void DrawCheckBoxMatrix(string[] enabledEnumerableArray, float indent, float checkboxSize, float labelSize)
        {
            const float buttonSize = 60f;
            foreach (var rowEnumerable in enabledEnumerableArray)
            {
                var row = FindIndex(rowEnumerable);
                var r = GUILayoutUtility.GetRect(indent + checkboxSize * enabledEnumerableArray.Length + labelSize * 3 + checkboxSize, checkboxSize);
                GUI.Label(new Rect(r.xMin, r.yMin, labelSize, checkboxSize), rowEnumerable, "Label");
                var x = 0;
                for (var j = enabledEnumerableArray.Length; --j >= 0;)
                {
                    var columnEnumerable = enabledEnumerableArray[j];
                    var tooltip = new GUIContent("", rowEnumerable + " :: " + columnEnumerable);
                    ref var flag = ref this[row, FindIndex(columnEnumerable)];
                    flag = GUI.Toggle(new Rect(labelSize + indent + r.x + x * checkboxSize, r.y, checkboxSize, checkboxSize), flag, tooltip);
                    x++;
                }
                var restWidth = (r.xMax - (labelSize + indent + r.x + enabledEnumerableArray.Length * checkboxSize)) * 0.5f;
                if (GUI.Button(new Rect(r.xMin + labelSize + indent + r.x + x * checkboxSize, r.yMin, restWidth, checkboxSize), new GUIContent("Select All", EnumerableArray[row])))
                {
                    for (var column = EnumerableArray.Length; --column >= 0;)
                        this[row, column] = true;
                }
                else if (GUI.Button(new Rect(r.xMin + labelSize + indent + r.x + x * checkboxSize + restWidth, r.yMin, restWidth, checkboxSize), new GUIContent("Deselect All", EnumerableArray[row])))
                {
                    for (var column = EnumerableArray.Length; --column >= 0;)
                        this[row, column] = false;
                }
            }
            GUILayout.Space(indent + buttonSize * 2);
        }

        private static void Rotate(Vector2 pivotPoint, int enabledCount, float checkboxSize, Rect lastRect, float offset)
        {
            GUIUtility.RotateAroundPivot(90f, pivotPoint);
            var mat = GUI.matrix;
            mat.m03 += offset + enabledCount * checkboxSize - lastRect.yMin;
            GUI.matrix = mat;
        }
    }
}
