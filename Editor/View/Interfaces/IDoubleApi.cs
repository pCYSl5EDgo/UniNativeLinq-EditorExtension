using System;
using System.Collections.Generic;

namespace UniNativeLinq.Editor
{
    public interface IDoubleApi : IDrawableWithEnumerableAndScrollPosition, IComparable<IDoubleApi>
    {
        string Name { get; set; }
        string Description { get; }
        string[] RelatedEnumerableArray { get; }
        string[] ExcludeEnumerableArray { get; }

        IEnumerable<string> NameCollection { get; }
        int Count { get; }
        bool TryGetEnabled(string name0, string name1, out bool value);
        bool TrySetEnabled(string name0, string name1, bool value);

        bool IsHided { get; set; }
    }
}
