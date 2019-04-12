using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public unsafe struct AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> : IRefEnumerable<AppendEnumerator<TPrevEnumerator, TSource>, TSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource> 
#endif
    {
        private TPrevEnumerable enumerable;
        private readonly TSource* element;

        internal AppendPointerEnumerable(in TPrevEnumerable enumerable, TSource* element)
        {
            this.enumerable = enumerable;
            this.element = element;
        }

        public AppendEnumerator<TPrevEnumerator, TSource> GetEnumerator() => new AppendEnumerator<TPrevEnumerator, TSource>(enumerable.GetEnumerator(), element);
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Any() => true;
        public bool Any<TPredicate>(TPredicate predicate)
        where TPredicate : struct, IRefFunc<TSource, bool>
        => Enumerable.Any<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate>(ref this, predicate);
        public bool All<TPredicate>(TPredicate predicate)
        where TPredicate : struct, IRefFunc<TSource, bool>
        => Enumerable.All<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate>(ref this, predicate);

        public void Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
        where TFunc : struct, IRefAction<TAccumulate, TSource>
        where TResultFunc : struct, IRefFunc<TAccumulate, TResult>
        => Enumerable.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref this, ref seed, func, resultFunc);
        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
        where TFunc : struct, IRefAction<TAccumulate, TSource>
        => Enumerable.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TFunc>(ref this, ref seed, func);
        public void Aggregate<TFunc>(ref TSource seed, TFunc func)
        where TFunc : struct, IRefAction<TSource, TSource>
        => Enumerable.Aggregate<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TFunc>(ref this, ref seed, func);

        public bool Contains(in TSource value)
        => Enumerable.Contains<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(ref this, value);

        public bool Contains<TEqualityComparer>(in TSource value, TEqualityComparer comparer)
        where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
        => Enumerable.Contains<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TEqualityComparer>(ref this, value, comparer);

        public int Count() => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource>() + 1;

        public AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> AsRefEnumerable() => this;

        public AppendPointerEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(TSource* item)
        => new AppendPointerEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item);

        public AppendEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(in TSource item, Allocator allocator = Allocator.Temp)
        => new AppendEnumerable<AppendPointerEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item, allocator);
    }
}
