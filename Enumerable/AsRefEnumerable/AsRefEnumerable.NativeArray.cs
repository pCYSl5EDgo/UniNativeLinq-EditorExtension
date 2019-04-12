using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class NativeEnumerableHelper
    {
        public static NativeEnumerable<T> AsRefEnumerable<T>(this NativeArray<T> array)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T> 
#endif
            => new NativeEnumerable<T>(array);
    }
}
