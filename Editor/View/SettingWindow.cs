using System;
using System.Collections.Generic;
using UniNativeLinq.Editor.CodeGenerator;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniNativeLinq.Editor
{
    public sealed class SettingWindow : EditorWindow
    {
        private IEnumerableCollectionProcessor enumerableCollectionProcessor;
        private IDrawable whetherToIncludeEnumerableOrNotView;
        private IDrawable whetherToUseApiOrNotView;
        private Vector2 scrollPosition;
        private IDllGenerator dllGenerator;
        private ISingleApi[] singleApis;
        private IDoubleApi[] doubleApis;
        private IDependency[] dependencies;
        private IApiExtensionMethodGenerator[] extensionMethodGenerators;

        private bool AnyNull
            => enumerableCollectionProcessor is null ||
               dllGenerator is null ||
               singleApis is null ||
               doubleApis is null ||
               whetherToUseApiOrNotView is null ||
               whetherToIncludeEnumerableOrNotView is null;

        [MenuItem("UniNativeLinq/Open Settings &#c")]
        // ReSharper disable once UnusedMember.Local
        private static void Open()
        {
            var window = GetWindow<SettingWindow>(typeof(SceneView));
            window.Initialize();
        }

        private void Initialize()
        {
            Helper.Initialize();
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

            if (enumerableCollectionProcessor is null)
                enumerableCollectionProcessor = new EnumerableCollectionProcessor(Gets0<StringBoolTuple>());
            if (whetherToIncludeEnumerableOrNotView is null)
                whetherToIncludeEnumerableOrNotView = new DrawableImplWhetherToIncludeEnumerable(enumerableCollectionProcessor);
            if (singleApis is null)
                singleApis = Gets<String2BoolArrayTuple, ISingleApi>();
            if (doubleApis is null)
                doubleApis = Gets<String2BoolMatrixTuple, IDoubleApi>();
            if (dependencies is null)
                dependencies = Gets<DependencyObject, IDependency>();
            if (whetherToUseApiOrNotView is null)
                whetherToUseApiOrNotView = new DrawableImplWhetherToUseApiOrNot(enumerableCollectionProcessor, singleApis, doubleApis);
            dllGenerator?.Dispose();
            dllGenerator = new DllGenerator(Helper.MainModule, Helper.SystemModule, Helper.UnityCoreModule);

            InitializeExtensionMethodsGenerator();
        }

        private void InitializeExtensionMethodsGenerator()
        {
            var list = new List<IApiExtensionMethodGenerator>();
            foreach (var api in doubleApis)
            {
                RegisterEachDoubleApi(api, list);
            }
            extensionMethodGenerators = list.ToArray();
        }

        private static void RegisterEachDoubleApi(IDoubleApi api, List<IApiExtensionMethodGenerator> list)
        {
            switch (api.Name)
            {
                case "Concat":
                    list.Add(new Concat(api));
                    break;
                case "Join":
                    switch (api.Description)
                    {
                        case "Operator":
                            list.Add(new JoinOperator(api));
                            break;
                        case "Func":
                            list.Add(new JoinFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new JoinRefFunc(api));
                            break;
                    }
                    break;
                case "GroupJoin":
                    switch (api.Description)
                    {
                        case "Operator":
                            list.Add(new GroupJoinOperator(api));
                            break;
                        case "Func":
                            list.Add(new GroupJoinFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new GroupJoinRefFunc(api));
                            break;
                    }
                    break;
                case "AdjustedZip":
                    switch (api.Description)
                    {
                        case "None":
                            list.Add(new AdjustedZipNone(api));
                            break;
                        case "Operator":
                            list.Add(new AdjustedZipOperator(api));
                            break;
                        case "Func":
                            list.Add(new AdjustedZipFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new AdjustedZipRefFunc(api));
                            break;
                    }
                    break;
                case "ExceptionalZip":
                    switch (api.Description)
                    {
                        case "None":
                            list.Add(new ExceptionalZipNone(api));
                            break;
                        case "Operator":
                            list.Add(new ExceptionalZipOperator(api));
                            break;
                        case "Func":
                            list.Add(new ExceptionalZipFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new ExceptionalZipRefFunc(api));
                            break;
                    }
                    break;
                case "Except":
                    switch (api.Description)
                    {
                        case "None":
                            list.Add(new ExceptNone(api));
                            break;
                        case "Operator":
                            list.Add(new ExceptOperator(api));
                            break;
                        case "Func":
                            list.Add(new ExceptFunc(api));
                            break;
                        case "RefFunc":
                            break;
                    }
                    break;
                case "Intersect":
                    switch (api.Description)
                    {
                        case "None":
                            list.Add(new IntersectNone(api));
                            break;
                        case "Operator":
                            list.Add(new IntersectOperator(api));
                            break;
                        case "Func":
                            list.Add(new IntersectFunc(api));
                            break;
                        case "RefFunc":
                            break;
                    }
                    break;
            }
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        private void OnGUI()
        {
            if (AnyNull)
                Initialize();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUI.skin.box);
            if (GUILayout.Button("Generate DLL"))
            {
                AssetDatabase.Refresh();
                dllGenerator.Execute(enumerableCollectionProcessor, extensionMethodGenerators, dependencies);
                Close();
            }
            whetherToIncludeEnumerableOrNotView.Draw(ref scrollPosition);
            whetherToUseApiOrNotView.Draw(ref scrollPosition);
            EditorGUILayout.EndScrollView();
        }
    }
}