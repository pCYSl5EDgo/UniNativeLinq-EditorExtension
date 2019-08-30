using System;
using System.Collections.Generic;
using UniNativeLinq.Editor.CodeGenerator;
using UniNativeLinq.Editor.CodeGenerator.ForEach;
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
            foreach (var api in singleApis)
            {
                RegisterEachSingleApi(api, list);
            }
            extensionMethodGenerators = list.ToArray();
        }

        private static void RegisterEachSingleApi(ISingleApi api, List<IApiExtensionMethodGenerator> list)
        {
            switch (api.Name)
            {
                case "ForEach":
                    switch (api.Description)
                    {
                        case "Action":
                            list.Add(new ForEachAction(api));
                            break;
                        case "RefAction":
                            list.Add(new ForEachRefAction(api));
                            break;
                        case "Operator":
                            list.Add(new ForEachOperator(api));
                            break;
                    }
                    break;
                case "OrderBy":
                    switch (api.Description)
                    {
                        case "NoneDouble":
                        case "NoneSingle":
                        case "NoneInt32":
                        case "NoneUInt32":
                        case "NoneInt64":
                        case "NoneUInt64":
                            list.Add(new OrderByNoneNumber(api));
                            break;
                        case "None":
                            list.Add(new OrderByNone(api));
                            break;
                        case "IComparer":
                            list.Add(new OrderByIComparer(api));
                            break;
                        case "Func":
                            list.Add(new OrderByFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new OrderByRefFunc(api));
                            break;
                        case "Operator":
                            list.Add(new OrderByOperator(api));
                            break;
                    }
                    break;
                case "SelectIndex":
                    switch (api.Description)
                    {
                        case "Operator":
                            list.Add(new SelectIndexOperator(api));
                            break;
                        case "Func":
                            list.Add(new SelectIndexFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new SelectIndexRefFunc(api));
                            break;
                        case "WithIndex":
                            list.Add(new SelectIndexWithIndex(api));
                            break;
                    }
                    break;
                case "WhereIndex":
                    switch (api.Description)
                    {
                        case "Operator":
                            list.Add(new WhereIndexOperator(api));
                            break;
                        case "Func":
                            list.Add(new WhereIndexFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new WhereIndexRefFunc(api));
                            break;
                    }
                    break;
                case "Reverse":
                    list.Add(new Reverse(api));
                    break;
                case "Distinct":
                    switch (api.Description)
                    {
                        case "Operator":
                            list.Add(new DistinctOperator(api));
                            break;
                        case "Func":
                            list.Add(new DistinctFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new DistinctRefFunc(api));
                            break;
                        case "None":
                            list.Add(new DistinctNone(api));
                            break;
                    }
                    break;
                case "Append":
                case "Prepend":
                case "DefaultIfEmpty":
                    list.Add(new AppendPrependDefaultIfEmpty(api));
                    break;
                case "Skip":
                case "SkipLast":
                case "Take":
                case "TakeLast":
                case "Repeat":
                    list.Add(new SkipSkipLastTakeTakeLastRepeat(api));
                    break;
                case "Select":
                    switch (api.Description)
                    {
                        case "Operator":
                            list.Add(new SelectOperator(api));
                            break;
                        case "Func":
                            list.Add(new SelectFunc(api));
                            break;
                        case "RefAction":
                            list.Add(new SelectRefAction(api));
                            break;
                    }
                    break;
                case "Where":
                case "SkipWhile":
                case "TakeWhile":
                    switch (api.Description)
                    {
                        case "Operator":
                            list.Add(new WhereSkipWhileTakeWhileOperator(api));
                            break;
                        case "Func":
                            list.Add(new WhereSkipWhileTakeWhileFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new WhereSkipWhileTakeWhileRefFunc(api));
                            break;
                    }
                    break;
                case "MaxByDouble":
                case "MaxBySingle":
                case "MaxByInt32":
                case "MaxByInt64":
                case "MaxByUInt32":
                case "MaxByUInt64":
                case "MinByDouble":
                case "MinBySingle":
                case "MinByInt32":
                case "MinByInt64":
                case "MinByUInt32":
                case "MinByUInt64":
                    {
                        var keyName = api.Name.Substring(5);
                        var isMax = api.Name[1] == 'a';
                        switch (api.Description)
                        {
                            case "Operator":
                                list.Add(new MinMaxByOperator(api, isMax, keyName));
                                break;
                            case "Func":
                                list.Add(new MinMaxByFunc(api, isMax, keyName));
                                break;
                            case "RefFunc":
                                list.Add(new MinMaxByRefFunc(api, isMax, keyName));
                                break;
                        }
                    }
                    break;
                case "Contains":
                    switch (api.Description)
                    {
                        case "Func":
                            list.Add(new ContainsFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new ContainsRefFunc(api));
                            break;
                        case "Operator":
                            list.Add(new ContainsOperator(api));
                            break;
                        case "None":
                            list.Add(new ContainsNone(api));
                            break;
                    }
                    break;
                case "TryGetFirst":
                    switch (api.Description)
                    {
                        case "Func":
                            list.Add(new TryGetFirstFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new TryGetFirstRefFunc(api));
                            break;
                        case "Operator":
                            list.Add(new TryGetFirstOperator(api));
                            break;
                        case "None":
                            list.Add(new TryGetFirstNone(api));
                            break;
                    }
                    break;
                case "TryGetLast":
                    switch (api.Description)
                    {
                        case "Func":
                            list.Add(new TryGetLastFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new TryGetLastRefFunc(api));
                            break;
                        case "Operator":
                            list.Add(new TryGetLastOperator(api));
                            break;
                        case "None":
                            list.Add(new TryGetLastNone(api));
                            break;
                    }
                    break;
                case "TryGetElementAt":
                    list.Add(new TryGetElementAt(api));
                    break;
                case "TryGetSingle":
                    switch (api.Description)
                    {
                        case "Func":
                            list.Add(new TryGetSingleFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new TryGetSingleRefFunc(api));
                            break;
                        case "Operator":
                            list.Add(new TryGetSingleOperator(api));
                            break;
                        case "None":
                            list.Add(new TryGetSingleNone(api));
                            break;
                    }
                    break;
                case "TryGetAverage":
                    list.Add(new TryGetAverageNone(api));
                    break;
                case "Aggregate":
                    switch (api.Description)
                    {
                        case "void Aggregate(ref TAccumulate seed, RefAction<T, TAccumulate> func)":
                            list.Add(new AggregateRefValue1Ref(api));
                            break;
                        case "TAccumulate Aggregate(TAccumulate seed, RefAction<T, TAccumulate> func)":
                            list.Add(new AggregateValue1Ref(api));
                            break;
                        case "TResult Aggregate(TAccumulate seed, RefAction<TAccumulate, T, TAccumulate> func, RefFunc<TAccumulate, TResult> resultFunc)":
                            list.Add(new AggregateValue2Refs(api));
                            break;
                        case "TResult Aggregate(ref TAccumulate seed, in TFunc func, in TResultFunc resultFunc)":
                            list.Add(new AggregateRefValue2Operators(api));
                            break;
                        case "TResult Aggregate(ref TAccumulate seed, RefAction<T, TAccumulate> func, RefFunc<TAccumulate, TResult> resultFunc)":
                            list.Add(new AggregateRefValue2Refs(api));
                            break;
                        case "TResult Aggregate(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)":
                            list.Add(new AggregateValue2Functions(api));
                            break;
                        case "void Aggregate(ref TAccumulate seed, in TFunc func)":
                            list.Add(new AggregateRefValue1Operator(api));
                            break;
                        case "TAccumulate Aggregate(TAccumulate seed, in TFunc func)":
                            list.Add(new AggregateValue1Operator(api));
                            break;
                        case "TAccumulate Aggregate(TAccumulate seed, Func<TAccumulate, T, TAccumulate> func)":
                            list.Add(new AggregateValue1Function(api));
                            break;
                    }
                    break;
                case "TryGetMax":
                case "TryGetMin":
                    {
                        var result = api.Description.Split(new[] { " --- " }, StringSplitOptions.RemoveEmptyEntries);
                        if (result.Length != 2)
                        {
                            Debug.LogWarning(api.Name + "\n" + api.Description);
                            return;
                        }
                        switch (result[0])
                        {
                            case "Func":
                                list.Add(new TryGetMinMaxFunc(api));
                                break;
                            case "RefFunc":
                                list.Add(new TryGetMinMaxRefFunc(api));
                                break;
                            case "Operator":
                                list.Add(new TryGetMinMaxOperator(api));
                                break;
                            case "None":
                                list.Add(new TryGetMinMaxNone(api));
                                break;
                        }
                    }
                    break;
                case "All":
                    switch (api.Description)
                    {
                        case "Operator":
                            list.Add(new AllOperator(api));
                            break;
                        case "Func":
                            list.Add(new AllFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new AllRefFunc(api));
                            break;
                    }
                    break;
                case "Any":
                    switch (api.Description)
                    {
                        case "Operator":
                            list.Add(new AnyOperator(api));
                            break;
                        case "Func":
                            list.Add(new AnyFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new AnyRefFunc(api));
                            break;
                    }
                    break;
                case "Sum":
                    switch (api.Description)
                    {
                        case "Double":
                            list.Add(new SumDouble(api));
                            break;
                        case "Single":
                            list.Add(new SumSingle(api));
                            break;
                        case "Int32":
                            list.Add(new SumInt32(api));
                            break;
                        case "UInt32":
                            list.Add(new SumUInt32(api));
                            break;
                        case "Int64":
                            list.Add(new SumInt64(api));
                            break;
                        case "UInt64":
                            list.Add(new SumUInt64(api));
                            break;
                    }
                    break;
            }
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
                            list.Add(new ExceptRefFunc(api));
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
                            list.Add(new IntersectRefFunc(api));
                            break;
                    }
                    break;
                case "Union":
                    switch (api.Description)
                    {
                        case "None":
                            list.Add(new UnionNone(api));
                            break;
                        case "Operator":
                            list.Add(new UnionOperator(api));
                            break;
                        case "Func":
                            list.Add(new UnionFunc(api));
                            break;
                        case "RefFunc":
                            list.Add(new UnionRefFunc(api));
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
                AssetDatabase.SaveAssets();
                dllGenerator.Execute(enumerableCollectionProcessor, extensionMethodGenerators, dependencies);
                Close();
            }
            whetherToIncludeEnumerableOrNotView.Draw(ref scrollPosition);
            whetherToUseApiOrNotView.Draw(ref scrollPosition);
            EditorGUILayout.EndScrollView();
        }
    }
}