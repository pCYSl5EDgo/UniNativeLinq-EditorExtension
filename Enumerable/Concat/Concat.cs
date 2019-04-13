using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static class ConcatEnumerable
    {
        #region Concat
        public static
            ConcatEnumerable<
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource>,
                ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource>(
                in this ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> first,
                in ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            where TEnumerator3 : struct, IRefEnumerator<TSource>
            where TEnumerable3 : struct, IRefEnumerable<TEnumerator3, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource>, ConcatEnumerable<TEnumerable2, TEnumerator2, TEnumerable3, TEnumerator3, TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>(
                in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> first,
                NativeArray<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second.AsRefEnumerable());

        public static
            ConcatEnumerable<
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                AppendEnumerable<TEnumerable2, TEnumerator2, TSource>,
                AppendEnumerator<TEnumerator2, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>(
                in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> first,
                in AppendEnumerable<TEnumerable2, TEnumerator2, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, AppendEnumerable<TEnumerable2, TEnumerator2, TSource>, AppendEnumerator<TEnumerator2, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                AppendPointerEnumerable<TEnumerable2, TEnumerator2, TSource>,
                AppendEnumerator<TEnumerator2, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>(
                in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> first,
                in AppendPointerEnumerable<TEnumerable2, TEnumerator2, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, AppendPointerEnumerable<TEnumerable2, TEnumerator2, TSource>, AppendEnumerator<TEnumerator2, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                WhereEnumerable<TEnumerable2, TEnumerator2, TSource, TPredicate>,
                WhereEnumerator<TEnumerator2, TSource, TPredicate>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource, TPredicate>(
                in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> first,
                in WhereEnumerable<TEnumerable2, TEnumerator2, TSource, TPredicate> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, WhereEnumerable<TEnumerable2, TEnumerator2, TSource, TPredicate>, WhereEnumerator<TEnumerator2, TSource, TPredicate>, TSource>(first, second);

        public static
            ConcatEnumerable<
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                SelectEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>,
                SelectEnumerator<TEnumerator2, TPrevSource, TSource, TAction>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource, TPrevSource, TAction>(
                in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> first,
                in SelectEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TEnumerator2 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TPrevSource>
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, SelectEnumerable<TEnumerable2, TEnumerator2, TPrevSource, TSource, TAction>, SelectEnumerator<TEnumerator2, TPrevSource, TSource, TAction>, TSource>(first, second);
        #endregion

        #region NativeArray
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
                SelectEnumerator<TEnumerator0, TPrevSource, TSource, TAction>,
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
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>, SelectEnumerator<TEnumerator0, TPrevSource, TSource, TAction>, TSource>(first.AsRefEnumerable(), second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>,
                WhereEnumerator<TEnumerator0, TSource, TPredicate>,
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
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>, WhereEnumerator<TEnumerator0, TSource, TPredicate>, TSource>(first.AsRefEnumerable(), second);
        #endregion

        #region Append -
        public static
            ConcatEnumerable<
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>(
                in this AppendEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource>(
                in this AppendEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                NativeArray<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second.AsRefEnumerable());

        public static
            ConcatEnumerable<
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource>(
                in this AppendEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in NativeEnumerable<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>(
                in this AppendEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in AppendEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, AppendEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>(
                in this AppendEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>,
                SelectEnumerator<TEnumerator1, TPrevSource, TSource, TAction>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TPrevSource, TAction>(
                in this AppendEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectEnumerator<TEnumerator1, TPrevSource, TSource, TAction>, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>,
                WhereEnumerator<TEnumerator1, TSource, TPredicate>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TPredicate>(
                this in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>, WhereEnumerator<TEnumerator1, TSource, TPredicate>, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>(
                in this AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource>(
                in this AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                NativeArray<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second.AsRefEnumerable());

        public static
            ConcatEnumerable<
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource>(
                in this AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in NativeEnumerable<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>(
                in this AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in AppendEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, AppendEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>(
                in this AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>,
                SelectEnumerator<TEnumerator1, TPrevSource, TSource, TAction>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TPrevSource, TAction>(
                in this AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectEnumerator<TEnumerator1, TPrevSource, TSource, TAction>, TSource>(first, second);

        public static
            ConcatEnumerable<
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>,
                WhereEnumerator<TEnumerator1, TSource, TPredicate>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TPredicate>(
                this in AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> first,
                in WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>, WhereEnumerator<TEnumerator1, TSource, TPredicate>, TSource>(first, second);
        #endregion

        #region NativeEnumerable
        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TSource>(in this NativeEnumerable<TSource> first, in NativeEnumerable<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TSource>(in this NativeEnumerable<TSource> first,
                NativeArray<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second.AsRefEnumerable());

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>,
                ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>(
                in this NativeEnumerable<TSource> first,
                in ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>, ConcatEnumerable<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                AppendEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource>(
                in this NativeEnumerable<TSource> first,
                in AppendEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, AppendEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>,
                AppendEnumerator<TEnumerator0, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource>(
                in this NativeEnumerable<TSource> first,
                in AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, AppendPointerEnumerable<TEnumerable0, TEnumerator0, TSource>, AppendEnumerator<TEnumerator0, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>,
                SelectEnumerator<TEnumerator0, TPrevSource, TSource, TAction>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPrevSource, TAction>(
                in this NativeEnumerable<TSource> first,
                in SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource, TSource, TAction>, SelectEnumerator<TEnumerator0, TPrevSource, TSource, TAction>, TSource>(first, second);

        public static
            ConcatEnumerable<
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>,
                WhereEnumerator<TEnumerator0, TSource, TPredicate>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate>(
                in this NativeEnumerable<TSource> first,
                in WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate>, WhereEnumerator<TEnumerator0, TSource, TPredicate>, TSource>(first, second);
        #endregion

        #region Select
        public static
            ConcatEnumerable<
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>,
                SelectEnumerator<TEnumerator1, TPrevSource1, TSource, TAction1>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TSource, TPrevSource0, TPrevSource1, TAction0, TAction1>(
                in this SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> first,
                in SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TAction0 : unmanaged, IRefAction<TPrevSource0, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource1>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource1>
            where TAction1 : unmanaged, IRefAction<TPrevSource1, TSource>
            where TPrevSource1 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource1>
#endif
            => new ConcatEnumerable<SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource1, TSource, TAction1>, SelectEnumerator<TEnumerator1, TPrevSource1, TSource, TAction1>, TSource>(first, second);

        public static
            ConcatEnumerable<
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource, TPrevSource0, TAction0>(
                in this SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> first,
                in ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TAction0 : unmanaged, IRefAction<TPrevSource0, TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPrevSource0, TAction0>(
                in this SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> first,
                NativeArray<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TAction0 : unmanaged, IRefAction<TPrevSource0, TSource>
            => new ConcatEnumerable<SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second.AsRefEnumerable());

        public static
            ConcatEnumerable<
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPrevSource0, TAction0>(
                in this SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> first,
                in NativeEnumerable<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TAction0 : unmanaged, IRefAction<TPrevSource0, TSource>
            => new ConcatEnumerable<SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>,
                AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPrevSource0, TAction0, TEnumerable1, TEnumerator1>(
                in this SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> first,
                in AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TAction0 : unmanaged, IRefAction<TPrevSource0, TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>, AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPrevSource0, TAction0, TEnumerable1, TEnumerator1>(
                in this SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> first,
                in AppendEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TAction0 : unmanaged, IRefAction<TPrevSource0, TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>, AppendEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>,
                SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>,
                WhereEnumerator<TEnumerator1, TSource, TPredicate>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPrevSource0, TAction0, TEnumerable1, TEnumerator1, TPredicate>(
                in this SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0> first,
                in WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TPrevSource0 : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource0>
#endif
            where TEnumerator0 : struct, IRefEnumerator<TPrevSource0>
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TPrevSource0>
            where TAction0 : unmanaged, IRefAction<TPrevSource0, TSource>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TPredicate : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<SelectEnumerable<TEnumerable0, TEnumerator0, TPrevSource0, TSource, TAction0>, SelectEnumerator<TEnumerator0, TPrevSource0, TSource, TAction0>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate>, WhereEnumerator<TEnumerator1, TSource, TPredicate>, TSource>(first, second);
        #endregion

        #region Where
        public static
            ConcatEnumerable<
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>,
                WhereEnumerator<TEnumerator0, TSource, TPredicate0>,
                WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate1>,
                WhereEnumerator<TEnumerator1, TSource, TPredicate1>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate0, TEnumerable1, TEnumerator1, TPredicate1>(
                in this WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0> first,
                in WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate1> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate0 : unmanaged, IRefFunc<TSource, bool>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TPredicate1 : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>, WhereEnumerator<TEnumerator0, TSource, TPredicate0>, WhereEnumerable<TEnumerable1, TEnumerator1, TSource, TPredicate1>, WhereEnumerator<TEnumerator1, TSource, TPredicate1>, TSource>(first, second);

        public static
            ConcatEnumerable<
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>,
                WhereEnumerator<TEnumerator0, TSource, TPredicate0>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate0>(
                in this WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0> first,
                NativeArray<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate0 : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>, WhereEnumerator<TEnumerator0, TSource, TPredicate0>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second.AsRefEnumerable());

        public static
            ConcatEnumerable<
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>,
                WhereEnumerator<TEnumerator0, TSource, TPredicate0>,
                NativeEnumerable<TSource>,
                NativeEnumerable<TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate0>(
                in this WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0> first,
                in NativeEnumerable<TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate0 : unmanaged, IRefFunc<TSource, bool>
            => new ConcatEnumerable<WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>, WhereEnumerator<TEnumerator0, TSource, TPredicate0>, NativeEnumerable<TSource>, NativeEnumerable<TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>,
                WhereEnumerator<TEnumerator0, TSource, TPredicate0>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>,
                ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate0, TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2>(
                in this WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0> first,
                in ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate0 : unmanaged, IRefFunc<TSource, bool>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            where TEnumerable2 : struct, IRefEnumerable<TEnumerator2, TSource>
            where TEnumerator2 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>, WhereEnumerator<TEnumerator0, TSource, TPredicate0>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>, ConcatEnumerable<TEnumerable1, TEnumerator1, TEnumerable2, TEnumerator2, TSource>.Enumerator, TSource>(first, second);

        public static
            ConcatEnumerable<
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>,
                WhereEnumerator<TEnumerator0, TSource, TPredicate0>,
                AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate0, TEnumerable1, TEnumerator1>(
                in this WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0> first,
                in AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate0 : unmanaged, IRefFunc<TSource, bool>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>, WhereEnumerator<TEnumerator0, TSource, TPredicate0>, AppendPointerEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>,
                WhereEnumerator<TEnumerator0, TSource, TPredicate0>,
                AppendEnumerable<TEnumerable1, TEnumerator1, TSource>,
                AppendEnumerator<TEnumerator1, TSource>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate0, TEnumerable1, TEnumerator1>(
                in this WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0> first,
                in AppendEnumerable<TEnumerable1, TEnumerator1, TSource> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate0 : unmanaged, IRefFunc<TSource, bool>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TSource>
            where TEnumerator1 : struct, IRefEnumerator<TSource>
            => new ConcatEnumerable<WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>, WhereEnumerator<TEnumerator0, TSource, TPredicate0>, AppendEnumerable<TEnumerable1, TEnumerator1, TSource>, AppendEnumerator<TEnumerator1, TSource>, TSource>(first, second);

        public static
            ConcatEnumerable<
                WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>,
                WhereEnumerator<TEnumerator0, TSource, TPredicate0>,
                SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>,
                SelectEnumerator<TEnumerator1, TPrevSource, TSource, TAction>,
                TSource
            >
            Concat<TEnumerable0, TEnumerator0, TSource, TPredicate0, TEnumerable1, TEnumerator1, TPrevSource, TAction>(
                in this WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0> first,
                in SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction> second)
            where TSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TSource>
#endif
            where TEnumerable0 : struct, IRefEnumerable<TEnumerator0, TSource>
            where TEnumerator0 : struct, IRefEnumerator<TSource>
            where TPredicate0 : unmanaged, IRefFunc<TSource, bool>
            where TEnumerable1 : struct, IRefEnumerable<TEnumerator1, TPrevSource>
            where TEnumerator1 : struct, IRefEnumerator<TPrevSource>
            where TPrevSource : unmanaged
#if STRICT_EQUALITY
            , IEquatable<TPrevSource>
#endif
            where TAction : unmanaged, IRefAction<TPrevSource, TSource>
            => new ConcatEnumerable<WhereEnumerable<TEnumerable0, TEnumerator0, TSource, TPredicate0>, WhereEnumerator<TEnumerator0, TSource, TPredicate0>, SelectEnumerable<TEnumerable1, TEnumerator1, TPrevSource, TSource, TAction>, SelectEnumerator<TEnumerator1, TPrevSource, TSource, TAction>, TSource>(first, second);
        #endregion
    }
}