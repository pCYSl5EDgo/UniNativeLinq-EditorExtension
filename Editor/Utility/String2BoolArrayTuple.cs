using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class String2BoolArrayTuple : ScriptableObject, IComparable<String2BoolArrayTuple>
    {
        public string Name;
        public string Description;
        public StringBoolValueTuple[] EnabledArray;

        //[MenuItem("UniNativeLinq/Concat &#d")]
        public static void Create()
        {
            var obj = AssetDatabase.LoadAssetAtPath<EnumerableCollectionObject>(
                AssetDatabase.GUIDToAssetPath(
                    AssetDatabase.FindAssets("t:" + nameof(EnumerableCollectionObject))[0]
                )
            );
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
            //Create(obj, "Aggregate",
            //    "TNextResult0 Aggregate<TAccumulate0, TNextResult0>(TAccumulate0 seed, Func<TAccumulate0, T, TAccumulate0> func, Func<TAccumulate0, TNextResult0> resultFunc)",
            //    "TAccumulate0 Aggregate<TAccumulate0>(TAccumulate0 seed, Func<TAccumulate0, T, TAccumulate0> func)",
            //    "T Aggregate(Func<T, T, T> func)",
            //    "TNextResult0 Aggregate<TAccumulate0, TNextResult0>(TAccumulate0 seed, RefFunc<TAccumulate0, T, TAccumulate0> func, RefFunc<TAccumulate0, TNextResult0> resultFunc)",
            //    "TAccumulate0 Aggregate<TAccumulate0>(TAccumulate0 seed, RefFunc<TAccumulate0, T, TAccumulate0> func)",
            //    "T Aggregate(RefFunc<T, T, T> func)",
            //    "TNextResult0 Aggregate<TAccumulate0, TNextResult0, TFunc0, TResultFunc0>(ref TAccumulate0 seed, in TFunc0 func, in TResultFunc0 resultFunc)",
            //    "void Aggregate<TAccumulate0, TFunc0>(ref TAccumulate0 seed, in TFunc0 func)"
            //);
            //Create(obj, "Append");
            //Create(obj, "Prepend");
            //Create(obj, "TryGetLast");
            //Create(obj, "TryGetFirst");
            //Create(obj, "TryGetSingle");
            //Create(obj, "TryGetElementAt");
            //var t = numbers.SelectMany(num => four, (num, desc) => desc + " --- " + num).ToArray();
            //Create(obj, "TryGetMin", t);
            //Create(obj, "TryGetMax", t);
            //foreach (var number in numbers)
            //{
            //    Create(obj, "MinBy" + number, three);
            //    Create(obj, "MaxBy" + number, three);
            //}
            //Create(obj, "SelectIndex", three);
            //Create(obj, "Select", three);
            //Create(obj, "WhereIndex", three);
            //Create(obj, "Where", three);
            //Create(obj, "Any", three);
            //Create(obj, "All", three);
            //Create(obj, "Distinct", four);
            //Create(obj, "Contains", four);
            //Create(obj, "OrderBy", four);
            //Create(obj, "DefaultIfEmpty");
            //Create(obj, "Average", numbers);
            //Create(obj, "Sum", numbers);
            //Create(obj, "Skip");
            //Create(obj, "Take");
            //Create(obj, "SkipLast");
            //Create(obj, "TakeLast");
            //Create(obj, "SkipWhile", three);
            //Create(obj, "TakeWhile", three);
            //Create(obj, "Repeat");
            //Create(obj, "Reverse");
        }

        private static void Create(EnumerableCollectionObject obj, string name, params string[] descriptions)
        {
            var objSpecialTypeArray = obj.SpecialTypeArray;
            var length = objSpecialTypeArray.Length;
            foreach (var description in descriptions)
            {
                var api = CreateInstance<String2BoolArrayTuple>();
                api.EnabledArray = new StringBoolValueTuple[obj.EnumerableArray.Length + obj.SpecialTypeArray.Length];
                for (var i = 0; i < objSpecialTypeArray.Length; i++)
                {
                    ref var tuple = ref api.EnabledArray[i];
                    tuple.Enumerable = objSpecialTypeArray[i].Enumerable;
                    tuple.Enabled = true;
                }
                for (var i = length; i < api.EnabledArray.Length; i++)
                {
                    ref var tuple = ref api.EnabledArray[i];
                    tuple.Enumerable = obj.EnumerableArray[i - length].Enumerable;
                    tuple.Enabled = true;
                }
                api.Name = name;
                api.Description = description;
                AssetDatabase.CreateAsset(api, "Assets/UniNativeLinq/Editor/ApiSingle/" + name + "___" + (description.Length <= 10 ? description : description.GetHashCode().ToString("X")) + ".asset");
            }
        }
        private static void Create(EnumerableCollectionObject obj, string name)
        {
            var api = CreateInstance<String2BoolArrayTuple>();
            api.EnabledArray = new StringBoolValueTuple[obj.EnumerableArray.Length + obj.SpecialTypeArray.Length];
            var objSpecialTypeArray = obj.SpecialTypeArray;
            for (var i = 0; i < objSpecialTypeArray.Length; i++)
            {
                ref var tuple = ref api.EnabledArray[i];
                tuple.Enumerable = objSpecialTypeArray[i].Enumerable;
                tuple.Enabled = true;
            }
            for (var i = objSpecialTypeArray.Length; i < api.EnabledArray.Length; i++)
            {
                ref var tuple = ref api.EnabledArray[i];
                tuple.Enumerable = obj.EnumerableArray[i - objSpecialTypeArray.Length].Enumerable;
                tuple.Enabled = true;
            }
            api.Name = name;
            AssetDatabase.CreateAsset(api, "Assets/UniNativeLinq/Editor/ApiSingle/" + name + ".asset");
        }

        public String2BoolArrayTuple(string name, string description, StringBoolValueTuple[] enabledArray)
        {
            Name = name;
            Description = description;
            EnabledArray = enabledArray;
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
