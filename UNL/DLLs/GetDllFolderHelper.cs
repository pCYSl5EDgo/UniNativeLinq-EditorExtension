using System.Runtime.CompilerServices;

namespace UniNativeLinq.Editor
{
    public static class GetDllFolderHelper
    {
        public static string GetFolder() => InternalGetFolder();

        private static string InternalGetFolder([CallerFilePath]string path = "") => path.Substring(0, path.Length - 3 - nameof(GetDllFolderHelper).Length);
    }
}
