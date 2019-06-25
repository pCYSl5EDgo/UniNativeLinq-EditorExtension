using System.Collections.Generic;

namespace UniNativeLinq.Editor
{
    public interface IEnumerableCollectionProcessor
    {
        IEnumerable<string> NameCollection { get; }
        int Count { get; }
        IEnumerable<(string Name, bool Enabled)> NameEnabledTupleCollection { get; }
        IEnumerable<string> EnabledNameCollection { get; }
        bool TryGetEnabled(string name, out bool value);
        bool TrySetEnabled(string name, bool value);
        bool HasChanged { get; }
        void Apply();
    }
}
