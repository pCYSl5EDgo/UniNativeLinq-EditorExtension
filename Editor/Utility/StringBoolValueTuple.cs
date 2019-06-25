using System;

namespace UniNativeLinq.Editor
{
    [Serializable]
    public struct StringBoolValueTuple
    {
        public string Enumerable;
        public bool Enabled;

        public StringBoolValueTuple(string enumerable, bool enabled)
        {
            Enumerable = enumerable;
            Enabled = enabled;
        }
    }
}
