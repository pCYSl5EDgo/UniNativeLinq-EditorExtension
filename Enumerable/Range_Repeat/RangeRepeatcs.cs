using System;

namespace pcysl5edgo.Collections.LINQ
{
    public struct SingleIncrement : IRefAction<Single>
    {
        public void Execute(ref float arg0) => ++arg0;
    }

    public struct DoubleIncrement : IRefAction<Double>
    {
        public void Execute(ref Double arg0) => ++arg0;
    }

    public struct DecimalIncrement : IRefAction<Decimal>
    {
        public void Execute(ref Decimal arg0) => ++arg0;
    }

    public struct Int32Increment : IRefAction<Int32>
    {
        public void Execute(ref Int32 arg0) => ++arg0;
    }

    public struct Int64Increment : IRefAction<Int64>
    {
        public void Execute(ref Int64 arg0) => ++arg0;
    }

    public struct UInt64Increment : IRefAction<UInt64>
    {
        public void Execute(ref UInt64 arg0) => ++arg0;
    }

    public struct UInt32Increment : IRefAction<UInt32>
    {
        public void Execute(ref UInt32 arg0) => ++arg0;
    }

    public struct NoAction<T> : IRefAction<T>
        where T : unmanaged
#if STRICT_EQUALITY
        , IEquatable<T>
#endif
    {
        public void Execute(ref T arg0) { }
    }
}