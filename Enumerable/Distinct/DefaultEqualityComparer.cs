namespace pcysl5edgo.Collections.LINQ
{
    public struct DefaultEqualityComparer<T> : IRefFunc<T, T, bool>
        where T : unmanaged
    {
        public bool Calc(ref T arg0, ref T arg1) => arg0.Equals(arg1);
    }
}