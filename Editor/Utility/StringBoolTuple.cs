using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class StringBoolTuple : ScriptableObject
    {
        public string Enumerable;
        public bool Enabled;

        public StringBoolTuple(string enumerable, bool enabled)
        {
            Enumerable = enumerable;
            Enabled = enabled;
        }
    }
}
