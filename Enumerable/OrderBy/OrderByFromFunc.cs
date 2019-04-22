using System;

namespace pcysl5edgo.Collections.LINQ
{
    public struct OrderByFromFunc<TSource>
        : IRefFunc<TSource, TSource, int>
    {
        private readonly Func<TSource, TSource, int> comparer;
        
        public OrderByFromFunc(Func<TSource, TSource, int> comparer) => this.comparer = comparer;
        public int Calc(ref TSource arg0, ref TSource arg1) => comparer(arg0, arg1);
    }
}