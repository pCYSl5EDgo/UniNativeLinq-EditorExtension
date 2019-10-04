using System;
using System.Collections.Generic;

namespace UniNativeLinq.Editor
{
    public interface ISingleApi : IDrawableWithEnumerable, IComparable<ISingleApi>
    {
        string Name { get; set; }
        string Description { get; }
        string[] RelatedEnumerableArray { get; }
        string[] ExcludeEnumerableArray { get; }

        IEnumerable<string> NameCollection { get; }
        int Count { get; }
        bool TryGetEnabled(string name, out bool value);
        bool TrySetEnabled(string name, bool value);

        bool IsHided { get; set; }
    }
}
