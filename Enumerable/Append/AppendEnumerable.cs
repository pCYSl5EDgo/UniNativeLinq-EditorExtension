using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public struct AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> : IRefEnumerable<AppendEnumerator<TPrevEnumerator, TSource>, TSource>
        where TPrevEnumerable : struct, IRefEnumerable<TPrevEnumerator, TSource>
        where TPrevEnumerator : struct, IRefEnumerator<TSource>
        where TSource : unmanaged
#if STRICT_EQUALITY
        , IEquatable<TSource> 
#endif
    {
        private TPrevEnumerable enumerable;
        private readonly TSource element;
        private readonly Allocator alloc;
        internal AppendEnumerable(in TPrevEnumerable enumerable, in TSource element, Allocator alloc)
        {
            this.enumerable = enumerable;
            this.element = element;
            this.alloc = alloc;
        }

        public AppendEnumerator<TPrevEnumerator, TSource> GetEnumerator() => new AppendEnumerator<TPrevEnumerator, TSource>(enumerable.GetEnumerator(), element, alloc);
        IEnumerator<TSource> IEnumerable<TSource>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Any() => true;
        public bool Any<TPredicate>(TPredicate predicate)
        where TPredicate : struct, IRefFunc<TSource, bool>
        => this.Any<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate>(predicate);
        public bool All<TPredicate>(TPredicate predicate)
        where TPredicate : struct, IRefFunc<TSource, bool>
        => this.All<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TPredicate>(predicate);

        public void Aggregate<TAccumulate, TResult, TFunc, TResultFunc>(ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
        where TFunc : struct, IRefAction<TAccumulate, TSource>
        where TResultFunc : struct, IRefFunc<TAccumulate, TResult>
        => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref seed, func, resultFunc);
        public void Aggregate<TAccumulate, TFunc>(ref TAccumulate seed, TFunc func)
        where TFunc : struct, IRefAction<TAccumulate, TSource>
        => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TAccumulate, TFunc>(ref seed, func);
        public void Aggregate<TFunc>(ref TSource seed, TFunc func)
        where TFunc : struct, IRefAction<TSource, TSource>
        => this.Aggregate<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TFunc>(ref seed, func);

        public bool Contains(in TSource value)
        => this.Contains<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(value);

        public bool Contains<TEqualityComparer>(in TSource value, TEqualityComparer comparer)
        where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
        => this.Contains<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource, TEqualityComparer>(value, comparer);

        public int Count() => enumerable.Count<TPrevEnumerable, TPrevEnumerator, TSource>() + 1;

        public AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource> AsRefEnumerable() => this;

        public unsafe AppendPointerEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(TSource* item)
        => new AppendPointerEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item);

        public AppendEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource> Append(in TSource item, Allocator allocator = Allocator.Temp)
        => new AppendEnumerable<AppendEnumerable<TPrevEnumerable, TPrevEnumerator, TSource>, AppendEnumerator<TPrevEnumerator, TSource>, TSource>(this, item, allocator);
    }
}
