using UnityEngine;

namespace UniNativeLinq.Editor
{
    [CreateAssetMenu(menuName = "Linq/StringBoolTuple")]
    public sealed class StringBoolTuple : ScriptableObject
    {
        public string Enumerable;
        public bool Enabled;
        public bool IsSpecial;

        public StringBoolTuple(string enumerable, bool enabled)
        {
            Enumerable = enumerable;
            Enabled = enabled;
        }
    }
}
