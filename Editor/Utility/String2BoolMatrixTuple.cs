using System;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class String2BoolMatrixTuple : ScriptableObject, IComparable<String2BoolMatrixTuple>
    {
        public string Name;
        public string Description;
        public string[] EnumerableArray;
        public bool[] EnabledArray;
        public int Width;

        public String2BoolMatrixTuple(string name, string description, string[] enumerableArray, bool[] enabledArray, int width)
        {
            Name = name;
            Description = description;
            EnumerableArray = enumerableArray;
            EnabledArray = enabledArray;
            Width = width;
        }

        public int CompareTo(String2BoolMatrixTuple other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var nameComparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
            if (nameComparison != 0) return nameComparison;
            var descriptionComparison = string.Compare(Description, other.Description, StringComparison.Ordinal);
            return descriptionComparison != 0 ? descriptionComparison : Width.CompareTo(other.Width);
        }
    }
}
