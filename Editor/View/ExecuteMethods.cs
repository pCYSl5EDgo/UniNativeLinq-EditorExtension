using UniNativeLinq.Editor;
using UnityEditor;

internal class ExecuteMethod
{
    public static void PrepareTest()
    {
        var settingWindow = EditorWindow.GetWindow<SettingWindow>();
        settingWindow.Initialize();

        ref var processor = ref settingWindow.EnumerableCollectionProcessor;

        foreach (var name in processor.NameCollection)
        {
            if (name == "Native" || name == "Array") continue;
            processor.TrySetEnabled(name, true);
        }

        ref var doubleApis = ref settingWindow.DoubleApis;
        for (var i = 0; i < doubleApis.Length; i++)
        {
            ref var api = ref doubleApis[i];
            foreach (var name0 in api.NameCollection)
                foreach (var name1 in api.NameCollection)
                    api.TrySetEnabled(name0, name1, true);
        }

        ref var singleApis = ref settingWindow.SingleApis;
        for (var i = 0; i < singleApis.Length; i++)
        {
            ref var api = ref singleApis[i];
            foreach (var name in api.NameCollection)
            {
                api.TrySetEnabled(name, true);
            }
        }

        settingWindow.DllGenerator.Execute(settingWindow.EnumerableCollectionProcessor, settingWindow.ExtensionMethodGenerators, settingWindow.Dependencies);
    }
}