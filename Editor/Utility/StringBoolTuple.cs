using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class StringBoolTuple : ScriptableObject
    {
        public string Enumerable;
        public bool Enabled;
        public bool IsSpecial;
        public uint GenericParameterCount;
        public string[] RelatedEnumerableArray;
        public string ElementType;
    }
}
