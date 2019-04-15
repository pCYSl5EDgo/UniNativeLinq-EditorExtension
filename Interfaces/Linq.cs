using System;

namespace pcysl5edgo.Collections.LINQ
{
    internal interface ILinq<TSource> where TSource : unmanaged
    {
        bool Any();
        bool Any<TPredicate>(TPredicate predicate)
        where TPredicate : unmanaged, IRefFunc<TSource, bool>;
        bool Any(Func<TSource, bool> predicate);

        bool All<TPredicate>(TPredicate predicate)
        where TPredicate : unmanaged, IRefFunc<TSource, bool>;
        bool All(Func<TSource, bool> predicate);

        void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
        where TFunc : unmanaged, IRefAction<TAccumulate, TSource>;
        TResult Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
        where TFunc : unmanaged, IRefAction<TAccumulate, TSource>
        where TResultFunc : unmanaged, IRefFunc<TAccumulate, TResult>;

        TSource Aggregate(Func<TSource, TSource, TSource> func);
        TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func);
        TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc);

        bool Contains(TSource value);
        bool Contains(TSource value, System.Collections.Generic.IEqualityComparer<TSource> comparer);
        bool Contains<TComparer>(TSource value, TComparer comparer) where TComparer : unmanaged, IRefFunc<TSource, TSource, bool>;

        int Count();
        int Count(Func<TSource, bool> predicate);
        int Count<TPredicate>(TPredicate predicate) where TPredicate : unmanaged, IRefFunc<TSource, bool>;
        long LongCount();
        long LongCount(Func<TSource, bool> predicate);
        long LongCount<TPredicate>(TPredicate predicate) where TPredicate : unmanaged, IRefFunc<TSource, bool>;

        bool TryGetElementAt(int index, out TSource element);

        bool TryGetFirst(out TSource first);
        bool TryGetLast(out TSource last);

        bool TryGetSingle(out TSource value);
        bool TryGetSingle<TPredicate>(out TSource value, TPredicate predicate) where TPredicate : unmanaged, IRefFunc<TSource, bool>;
        bool TryGetSingle(out TSource value, Func<TSource, bool> predicate);

        TSource[] ToArray();
        Unity.Collections.NativeArray<TSource> ToNativeArray(Unity.Collections.Allocator allocator);

        System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector);
        System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TKey, TElement, TKeyFunc, TElementFunc>(TKeyFunc keySelector, TElementFunc elementSelector)
        where TKeyFunc : unmanaged, IRefFunc<TSource, TKey>
        where TElementFunc : unmanaged, IRefFunc<TSource, TElement>;

        System.Collections.Generic.HashSet<TSource> ToHashSet();
        System.Collections.Generic.HashSet<TSource> ToHashSet(System.Collections.Generic.IEqualityComparer<TSource> comparer);

        System.Collections.Generic.List<TSource> ToList();
    }
}
