using System;

namespace UniNativeLinq.Editor
{
    public interface IDoubleApi : IComparable<IDoubleApi>
    {
        string Name { get; set; }
        string Description { get; }
        string[] RelatedEnumerableArray { get; }
        string[] ExcludeEnumerableArray { get; }

        string[] NameCollection { get; }
        int Count { get; }
        bool TryGetEnabled(string name0, string name1, out bool value);
        bool TrySetEnabled(string name0, string name1, bool value);

        bool IsHided { get; set; }
    }
}
