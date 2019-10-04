using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public abstract class String2BoolArrayTuple : ScriptableObject, IComparable<String2BoolArrayTuple>
    {
        public string Name;
        public string Description;
        public StringBoolValueTuple[] EnabledArray;

        protected static T[] Create<T>(IEnumerableCollectionProcessor processor, string name, params string[] descriptions)
            where T : String2BoolArrayTuple, ISingleApi
        {
            var array = processor.NameCollection.ToArray();
            var answer = descriptions.Length == 0 ? Array.Empty<T>() : new T[descriptions.Length];
            var index = 0;
            foreach (var description in descriptions)
            {
                answer[index] = CreateInstance<T>();
                ref var api = ref answer[index++];
                api.EnabledArray = new StringBoolValueTuple[array.Length];
                for (var i = 0; i < array.Length; i++)
                {
                    ref var tuple = ref api.EnabledArray[i];
                    tuple.Enumerable = array[i];
                    tuple.Enabled = true;
                }
                ((ISingleApi)api).Name = name;
                api.Description = description;
                AssetDatabase.CreateAsset(api, "Assets/UniNativeLinq/Editor/ApiSingle/" + name + "___" + (description.Length <= 10 ? description : description.GetHashCode().ToString("X")) + ".asset");
            }
            return answer;
        }

        protected static T Create<T>(IEnumerableCollectionProcessor processor, string name)
            where T : String2BoolArrayTuple, ISingleApi
        {
            var api = CreateInstance<T>();
            var array = processor.NameCollection.ToArray();
            api.EnabledArray = new StringBoolValueTuple[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                ref var tuple = ref api.EnabledArray[i];
                tuple.Enumerable = array[i];
                tuple.Enabled = true;
            }
            ((ISingleApi)api).Name = name;
            AssetDatabase.CreateAsset(api, "Assets/UniNativeLinq/Editor/ApiSingle/" + name + ".asset");
            return api;
        }

        public int CompareTo(String2BoolArrayTuple other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
            return nameComparison != 0 ? nameComparison : string.Compare(Description, other.Description, StringComparison.Ordinal);
        }
    }
}
