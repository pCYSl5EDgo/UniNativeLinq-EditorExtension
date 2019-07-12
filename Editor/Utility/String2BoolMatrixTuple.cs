using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public abstract class String2BoolMatrixTuple : ScriptableObject, IComparable<String2BoolMatrixTuple>
    {
        public string Name;
        public string Description;
        public string[] EnumerableArray;
        public bool[] EnabledArray;

        protected static T[] Create<T>(IEnumerableCollectionProcessor processor, string name, params string[] descriptions)
            where T : String2BoolMatrixTuple, IDoubleApi
        {
            var array = processor.NameCollection.ToArray();
            var answer = descriptions.Length == 0 ? Array.Empty<T>() : new T[descriptions.Length];
            var index = 0;
            foreach (var description in descriptions)
            {
                answer[index] = CreateInstance<T>();
                ref var api = ref answer[index++];
                ((IDoubleApi)api).Name = name;
                api.Description = description;
                api.EnumerableArray = new string[array.Length];
                for (var i = 0; i < array.Length; i++)
                    api.EnumerableArray[i] = array[i];
                api.EnabledArray = new bool[api.Count * api.Count];
                for (int i = 0; i < api.EnabledArray.Length; i++)
                    api.EnabledArray[i] = true;
                AssetDatabase.CreateAsset(api, "Assets/UniNativeLinq/Editor/ApiDouble/" + name + "___" + (description.Length <= 10 ? description : description.GetHashCode().ToString("X")) + ".asset");
            }
            return answer;
        }

        protected static T Create<T>(IEnumerableCollectionProcessor processor, string name)
            where T : String2BoolMatrixTuple, IDoubleApi
        {
            var array = processor.NameCollection.ToArray();
            var api = CreateInstance<T>();
            ((IDoubleApi)api).Name = name;
            api.Description = "";
            api.EnumerableArray = new string[array.Length];
            for (var i = 0; i < array.Length; i++)
                api.EnumerableArray[i] = array[i];
            api.EnabledArray = new bool[api.Count * api.Count];
            for (int i = 0; i < api.EnabledArray.Length; i++)
                api.EnabledArray[i] = true;
            AssetDatabase.CreateAsset(api, "Assets/UniNativeLinq/Editor/ApiDouble/" + name + ".asset");
            return api;
        }

        public int CompareTo(String2BoolMatrixTuple other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
            if (nameComparison != 0) return nameComparison;
            var descriptionComparison = string.Compare(Description, other.Description, StringComparison.Ordinal);
            return descriptionComparison != 0 ? descriptionComparison : EnumerableArray.Length.CompareTo(other.EnumerableArray.Length);
        }

        public ref bool this[int index0, int index1] => ref EnabledArray[index0 * EnumerableArray.Length + index1];

        public int FindIndex(string name1)
        {
            if (string.IsNullOrEmpty(name1)) return -1;
            for (var i = 0; i < EnumerableArray.Length; i++)
            {
                if (EnumerableArray[i] == name1)
                    return i;
            }
            return -1;
        }
    }
}
