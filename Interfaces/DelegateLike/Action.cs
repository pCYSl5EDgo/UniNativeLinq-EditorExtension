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
}