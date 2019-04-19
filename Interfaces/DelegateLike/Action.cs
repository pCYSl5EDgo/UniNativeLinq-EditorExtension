namespace pcysl5edgo
{
    public interface IRefAction<T0>
    {
        void Execute(ref T0 arg0);
    }
    public interface IRefAction<T0, T1>
    {
        void Execute(ref T0 arg0, ref T1 arg1);
    }
    
    public interface IRefAction<T0, T1, T2>
    {
        void Execute(ref T0 arg0, ref T1 arg1, ref T2 arg2);
    }
}