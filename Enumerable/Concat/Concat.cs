using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class ConcatEnumerable
    {
        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TSource>(
                this NativeArray<TSource> first,
                NativeArray<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first.AsRefEnumerable(), second.AsRefEnumerable());

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>(
                this NativeArray<TSource> first,
                in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(first.AsRefEnumerable(), second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource>(
                this NativeArray<TSource> first,
                in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(first.AsRefEnumerable(), second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource>(
                this NativeArray<TSource> first,
                in AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(first.AsRefEnumerable(), second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPrevSource, TAction>(
                this NativeArray<TSource> first,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction> second)
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource>
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>.Enumerator, TSource>(first.AsRefEnumerable(), second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate>(
                this NativeArray<TSource> first,
                in WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>.Enumerator, TSource>(first.AsRefEnumerable(), second);
    }
}