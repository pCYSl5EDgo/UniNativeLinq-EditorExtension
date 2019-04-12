using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class ConcatEnumerable
    {
        public static ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource> Concat<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>(ref this TFirstEnumerable first, in TSecondEnumerable second)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, TSource>
            where TFirstEnumerator : struct, IRefEnumerator<TSource>
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, TSource>
            where TSecondEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            => new ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, TSecondEnumerable, TSecondEnumerator, TSource>(first, second);

        public static ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T> Concat<T>(this NativeArray<T> first, NativeArray<T> second)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            => new ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>(new NativeEnumerable<T>(first), new NativeEnumerable<T>(second));

        public static ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, TSecondEnumerable, TSecondEnumerator, T> Concat<TSecondEnumerable, TSecondEnumerator, T>(this NativeArray<T> first, in TSecondEnumerable second)
            where TSecondEnumerable : struct, IRefEnumerable<TSecondEnumerator, T>
            where TSecondEnumerator : struct, IRefEnumerator<T>
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            => new ConcatEnumerable<NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, TSecondEnumerable, TSecondEnumerator, T>(new NativeEnumerable<T>(first), second);

        public static ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T> Concat<TFirstEnumerable, TFirstEnumerator, T>(ref this TFirstEnumerable first, NativeArray<T> second)
            where TFirstEnumerable : struct, IRefEnumerable<TFirstEnumerator, T>
            where TFirstEnumerator : struct, IRefEnumerator<T>
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            => new ConcatEnumerable<TFirstEnumerable, TFirstEnumerator, NativeEnumerable<T>, NativeEnumerable<T>.Enumerator, T>(first, new NativeEnumerable<T>(second));
    }
}