using System.Collections.Generic;

namespace pcysl5edgo.Collections.LINQ
{
    public struct DefaultOrderByAscending<TSource>
        : IRefFunc<TSource, TSource, int>
    {
        public int Calc(ref TSource arg0, ref TSource arg1) => Comparer<TSource>.Default.Compare(arg0, arg1);
    }
}