using System.Linq;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class EnumerableCollectionObject : ScriptableObject
    {
        public EnumerableDependencyObject[] Dependencies;

        public StringBoolTuple[] SpecialTypeArray;

        public StringBoolTuple[] EnumerableArray;

        public ref StringBoolTuple this[int index]
        {
            get
            {
                if (index < SpecialTypeArray.Length)
                    return ref SpecialTypeArray[index];
                index -= SpecialTypeArray.Length;
                return ref EnumerableArray[index];
            }
        }

        public int TypeLength => SpecialTypeArray.Length + EnumerableArray.Length;
        public int TypeEnabledCount => SpecialTypeArray.Count(x => x.Enabled) + EnumerableArray.Count(x => x.Enabled);
    }
}