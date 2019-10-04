namespace UniNativeLinq.Editor
{
    public interface IDependency
    {
        string Enumerable { get; }
        string[] Types { get; }
        string[] Methods { get; }
    }
}
