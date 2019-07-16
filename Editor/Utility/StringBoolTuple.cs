using UnityEngine;

namespace UniNativeLinq.Editor
{
    [CreateAssetMenu(menuName = "Linq/StringBoolTuple")]
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
