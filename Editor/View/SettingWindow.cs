using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniNativeLinq.Editor
{
    public class SettingWindow : EditorWindow
    {
        private IEnumerableCollectionProcessor enumerableCollectionProcessor;
        private IDrawable whetherToIncludeEnumerableOrNotView;
        private IDrawable whetherToUseApiOrNotView;
        private Vector2 scrollPosition;
        private IDllGenerator dllGenerator;

        [MenuItem("UniNativeLinq/Open Settings &#c")]
        static void Open()
        {
            var window = GetWindow<SettingWindow>(typeof(SceneView));
            window.Initialize();
        }

        private void Initialize()
        {
            T1[] Gets<T0, T1>() where T0 : Object where T1 : class
            {
                var paths = AssetDatabase.FindAssets("t:" + typeof(T0).Name);
                var answer = paths.Length == 0 ? Array.Empty<T1>() : new T1[paths.Length];
                for (var i = 0; i < paths.Length; i++)
                    answer[i] = AssetDatabase.LoadAssetAtPath<T0>(AssetDatabase.GUIDToAssetPath(paths[i])) as T1;
                return answer;
            }
            T0[] Gets0<T0>() where T0 : Object
            {
                var paths = AssetDatabase.FindAssets("t:" + typeof(T0).Name);
                var answer = paths.Length == 0 ? Array.Empty<T0>() : new T0[paths.Length];
                for (var i = 0; i < paths.Length; i++)
                    answer[i] = AssetDatabase.LoadAssetAtPath<T0>(AssetDatabase.GUIDToAssetPath(paths[i]));
                return answer;
            }

            if (enumerableCollectionProcessor == null)
                enumerableCollectionProcessor = new EnumerableCollectionProcessor(Gets0<StringBoolTuple>());
            if (whetherToIncludeEnumerableOrNotView == null)
                whetherToIncludeEnumerableOrNotView = new DrawableImplWhetherToIncludeEnumerable(enumerableCollectionProcessor);
            if (whetherToUseApiOrNotView == null)
                whetherToUseApiOrNotView = new DrawableImplWhetherToUseApiOrNot(enumerableCollectionProcessor, Gets<String2BoolArrayTuple, ISingleApi>(), Gets<String2BoolMatrixTuple, IDoubleApi>());
        }

        void OnGUI()
        {
            if (whetherToIncludeEnumerableOrNotView is null || enumerableCollectionProcessor is null)
                Initialize();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.box);
            whetherToIncludeEnumerableOrNotView.Draw(ref scrollPosition);
            whetherToUseApiOrNotView.Draw(ref scrollPosition);
            EditorGUILayout.EndScrollView();
        }
    }
}