namespace UniNativeLinq.Editor.CodeGenerator
{
    public static class SingleApiHelper
    {
        public static bool ShouldDefine(this ISingleApi api, string[] array)
        {
            foreach (var name in array)
            {
                if (api.TryGetEnabled(name, out var apiEnabled) && apiEnabled)
                {
                    return true;
                }
            }
            return false;
        }
    }
}