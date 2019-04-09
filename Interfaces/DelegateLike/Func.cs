namespace pcysl5edgo
{
    public interface IFunc<out T>
    {
        T Calc();
    }
    public interface IFunc<in T0, out TResult>
    {
        TResult Calc(T0 arg0);
    }
    public interface IFunc<in T0, in T1, out TResult>
    {
        TResult Calc(T0 arg0, T1 arg1);
    }
    public interface IFuncRefResult<T>
    {
        ref T Calc();
    }
    public interface IFuncRefResult<in T0, TResult>
    {
        ref TResult Calc(T0 arg0);
    }
    public interface IFuncRefResult<in T0, in T1, TResult>
    {
        ref TResult Calc(T0 arg0, T1 arg1);
    }
    public interface IRefFunc<T0, out TResult>
    {
        TResult Calc(ref T0 arg0);
    }
    public interface IRefFunc<T0, T1, out TResult>
    {
        TResult Calc(ref T0 arg0, ref T1 arg1);
    }
    public interface IRefFuncRefResult<T0, TResult>
    {
        ref TResult Calc(ref T0 arg0);
    }
    public interface IRefFuncRefResult<T0, T1, TResult>
    {
        ref TResult Calc(ref T0 arg0, ref T1 arg1);
    }
}
