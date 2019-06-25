using UnityEditor;

// ReSharper disable VariableHidesOuterVariable

namespace UniNativeLinq.Editor
{
    [CustomEditor(typeof(EnumerableCollectionObject))]
    internal sealed class EnumerableObjectInspector : UnityEditor.Editor
    {
        private EnumerableCollectionObject _enumerableCollectionExistsSettings;
        private IDllGenerator DllGenerator;
        private bool hasChanged;

        //static void CreateInstance()
        //{
        //    var assetsFolder = UnityEditorInternal.InternalEditorUtility.GetAssetsFolder();
        //    var directorySeparatorChar = System.IO.Path.DirectorySeparatorChar;
        //    var editorFolder = assetsFolder + directorySeparatorChar + "Editor";
        //    var uniNativeLinqFolder = editorFolder + directorySeparatorChar + "UniNativeLinq";
        //    const string extension = ".asset";
        //    var name = uniNativeLinqFolder + directorySeparatorChar + "Master" + extension;

        //    if (!AssetDatabase.IsValidFolder(editorFolder))
        //        AssetDatabase.CreateFolder(assetsFolder, "Editor");
        //    if (!AssetDatabase.IsValidFolder(uniNativeLinqFolder))
        //        AssetDatabase.CreateFolder(editorFolder, "UniNativeLinq");

        //    var obj = CreateInstance<EnumerableCollectionObject>();

        //    {
        //        var guids = AssetDatabase.FindAssets("t:" + nameof(StringBoolTuple));
        //        var special = new StringBoolTuple[2];
        //        var enumerable = new List<StringBoolTuple>(guids.Length - 2);
        //        foreach (var guid in guids)
        //        {
        //            var tuple = AssetDatabase.LoadAssetAtPath<StringBoolTuple>(AssetDatabase.GUIDToAssetPath(guid));
        //            switch (tuple.Enumerable)
        //            {
        //                case "NativeArray<T>":
        //                    special[0] = tuple;
        //                    break;
        //                case "T[]":
        //                    special[1] = tuple;
        //                    break;
        //                default:
        //                    enumerable.Add(tuple);
        //                    break;
        //            }
        //        }
        //        obj.EnumerableArray = enumerable.ToArray();
        //        obj.SpecialTypeArray = special;
        //    }

        //    AssetDatabase.CreateAsset(obj, name);
        //}
    }
}

//    void OnEnable()
//    {
//        _enumerableExistsSettings = (EnumerableCollectionObject)target;
//        _enumerableExistsSettings.CollectDependencies();
//        hasChanged = false;
//        if (DllGenerator is null)
//            DllGenerator = new DllGenerator();
//    }


//    void OnDisable()
//    {
//        if (!hasChanged) return;
//        Apply();
//    }

//    public override void OnInspectorGUI()
//    {
//        WhetherToIncludeOrNot();
//        WhetherToUseApiOrNot();
//        ApplyGUI();
//    }

//    private void ApplyGUI()
//    {
//        using (new EditorGUI.DisabledScope(!hasChanged))
//        {
//            if (!GUILayout.Button("Apply")) return;
//            Apply();
//        }
//    }

//    private void Apply()
//    {
//        hasChanged = false;
//        DllGenerator.Execute(_enumerableExistsSettings);
//    }

//    private void WhetherToUseApiOrNot()
//    {
//        if (!ShowFoldout(nameof(UniNativeLinq) + nameof(WhetherToUseApiOrNot), "Whether to use each API or NOT")) return;
//        using (IndentScope.Create())
//        {
//            ShowArrayArray("MinBy", _enumerableExistsSettings.MinByNoneOperatorFuncRefFuncArray);
//            ShowArrayArray("MaxBy", _enumerableExistsSettings.MaxByNoneOperatorFuncRefFuncArray);
//            ShowMatrix(ref _enumerableExistsSettings.Concat);
//            ShowMatrixArray("Zip", ref _enumerableExistsSettings.Zip);
//        }
//    }

//    private void ShowMatrixArray(string apiName, ref String2BoolMatrixTuple[] tupleArray)
//    {
//        if (!ShowFoldout(nameof(UniNativeLinq) + nameof(ShowMatrixArray) + apiName, apiName)) return;
//        using (IndentScope.Create())
//        {
//            for (var i = 0; i < tupleArray.Length; i++)
//            {
//                ref var tuple = ref tupleArray[i];
//                ShowMatrix(ref tuple);
//            }
//        }
//    }

//    private void ShowMatrix(ref String2BoolMatrixTuple tuple)
//    {
//        if (!ShowFoldout(nameof(UniNativeLinq) + nameof(ShowMatrix) + tuple.Name, tuple.Name)) return;
//        using (IndentScope.Create())
//        using (new EditorGUILayout.VerticalScope())
//        {
//            const float labelSize = 140f;
//            const float indent = 30f;
//            const float checkboxSize = 16f;
//            const float buttonSize = 60f;

//            var enabledCount = _enumerableExistsSettings.TypeEnabledCount;
//            var lastRect = GUILayoutUtility.GetRect(labelSize + indent + checkboxSize * enabledCount, labelSize + indent);
//            var pivotPoint = new Vector2(lastRect.xMin, lastRect.yMin);
//            var scroll = ScrollHelper.GetCurrentScroll();
//            using (new EditorGUI.DisabledScope(tuple.EnabledArray.All(x => x)))
//            {
//                if (GUI.Button(new Rect(lastRect.xMin, checkboxSize + lastRect.yMin, labelSize, labelSize * 0.5f), "Select All"))
//                {
//                    hasChanged = true;
//                    for (var i = 0; i < tuple.EnabledArray.Length; i++)
//                        tuple.EnabledArray[i] = true;
//                }
//            }
//            using (new EditorGUI.DisabledScope(tuple.EnabledArray.All(x => !x)))
//            {
//                if (GUI.Button(new Rect(lastRect.xMin, checkboxSize + lastRect.yMin + labelSize * 0.5f, labelSize, labelSize * 0.5f), "Deselect All"))
//                {
//                    hasChanged = true;
//                    for (var i = 0; i < tuple.EnabledArray.Length; i++)
//                        tuple.EnabledArray[i] = false;
//                }
//            }
//            Rotate(pivotPoint, enabledCount, checkboxSize, lastRect, indent + labelSize + scroll.scrollPosition.y);
//            var typeLength = _enumerableExistsSettings.TypeLength;
//            for (int column = 0, dispColumn = 0; column < typeLength; ++column)
//            {
//                var (enumerable, enabled) = _enumerableExistsSettings[column];
//                if (!enabled) continue;
//                GUI.Label(new Rect(checkboxSize * 0.5f, dispColumn * checkboxSize + scroll.scrollPosition.y, labelSize + indent, checkboxSize), enumerable, "RightLabel");
//                if (GUI.Button(new Rect(labelSize + enabledCount * checkboxSize + indent * 2, dispColumn * checkboxSize + scroll.scrollPosition.y, buttonSize, checkboxSize), new GUIContent("Select All", enumerable), "minibutton"))
//                {
//                    hasChanged = true;
//                    for (var row = typeLength; --row >= 0;)
//                        tuple[row, column] = true;
//                }
//                else if (GUI.Button(new Rect(labelSize + enabledCount * checkboxSize + indent * 2 + buttonSize, dispColumn * checkboxSize + scroll.scrollPosition.y, buttonSize, checkboxSize), new GUIContent("Deselect All", enumerable), "minibutton"))
//                {
//                    hasChanged = true;
//                    for (var row = typeLength; --row >= 0;)
//                        tuple[row, column] = false;
//                }
//                dispColumn++;
//            }
//            GUI.matrix = Matrix4x4.identity;
//            for (var row = 0; row < tuple.Width; row++)
//            {
//                var (enumerableFirst, enabledFirst) = _enumerableExistsSettings[row];
//                if (!enabledFirst) continue;
//                var r = GUILayoutUtility.GetRect(indent + checkboxSize * enabledCount + labelSize * 3 + checkboxSize, checkboxSize);
//                GUI.Label(new Rect(r.xMin, r.yMin, labelSize, checkboxSize), enumerableFirst, "Label");
//                var x = 0;
//                for (var column = _enumerableExistsSettings.TypeLength; --column >= 0;)
//                {
//                    var (enumerableSecond, enabledSecond) = _enumerableExistsSettings[column];
//                    if (!enabledSecond) continue;
//                    var tooltip = new GUIContent("", enumerableFirst + " :: " + enumerableSecond);
//                    ref var flag = ref tuple[row, column];
//                    var tmp = flag;
//                    flag = GUI.Toggle(new Rect(labelSize + indent + r.x + x * checkboxSize, r.y, checkboxSize, checkboxSize), flag, tooltip);
//                    hasChanged |= tmp ^ flag;
//                    x++;
//                }
//                var restWidth = (r.xMax - (labelSize + indent + r.x + enabledCount * checkboxSize)) * 0.5f;
//                if (GUI.Button(new Rect(r.xMin + labelSize + indent + r.x + x * checkboxSize, r.yMin, restWidth, checkboxSize), new GUIContent("Select All", enumerableFirst)))
//                {
//                    hasChanged = true;
//                    for (var column = _enumerableExistsSettings.TypeLength; --column >= 0;)
//                        tuple[row, column] = true;
//                }
//                else if (GUI.Button(new Rect(r.xMin + labelSize + indent + r.x + x * checkboxSize + restWidth, r.yMin, restWidth, checkboxSize), new GUIContent("Deselect All", enumerableFirst)))
//                {
//                    hasChanged = true;
//                    for (var column = _enumerableExistsSettings.TypeLength; --column >= 0;)
//                        tuple[row, column] = false;
//                }
//            }
//            GUILayout.Space(indent + buttonSize * 2);
//        }
//    }

//    private static void Rotate(Vector2 pivotPoint, int enabledCount, float checkboxSize, Rect lastRect, float offset)
//    {
//        GUIUtility.RotateAroundPivot(90f, pivotPoint);
//        var mat = GUI.matrix;
//        mat.m03 += offset + enabledCount * checkboxSize - lastRect.yMin;
//        GUI.matrix = mat;
//    }

//    private static bool ShowFoldout(string configName, string title)
//    {
//        if (!bool.TryParse(EditorUserSettings.GetConfigValue(configName), out var foldout))
//            foldout = true;
//        foldout = EditorGUILayout.Foldout(foldout, title);
//        EditorUserSettings.SetConfigValue(configName, foldout.ToString());
//        return foldout;
//    }

//    private void ShowArrayArray(string apiName, String2BoolArrayTuple[] arrayArray)
//    {
//        if (!ShowFoldout(nameof(UniNativeLinq) + nameof(ShowArrayArray) + apiName, apiName)) return;
//        using (IndentScope.Create())
//        {
//            for (var i = 0; i < arrayArray.Length; i++)
//                ShowEachArray(apiName, ref arrayArray[i]);
//        }
//    }

//    private void ShowEachArray(string apiName, ref String2BoolArrayTuple tuple)
//    {
//        if (!ShowFoldout(nameof(UniNativeLinq) + nameof(ShowArrayArray) + apiName + tuple.Name, tuple.Name)) return;

//        void ProcessBool0(ref bool arg0, ref StringBoolTuple boolTuple)
//        {
//            if (!boolTuple.Enabled) return;
//            var tmp = arg0;
//            arg0 = !GUILayout.Toggle(!arg0, "MinBy / " + boolTuple.Enumerable + " : " + arg0.ToString(), "button");
//            hasChanged |= arg0 ^ tmp;
//        }

//        void ProcessBool1(ref bool arg0, ref StringBoolTuple boolTuple)
//        {
//            if (!boolTuple.Enabled) return;
//            var tmp = arg0;
//            arg0 = !GUILayout.Toggle(!arg0, "MaxBy / " + boolTuple.Enumerable + " : " + arg0.ToString(), "button");
//            hasChanged |= arg0 ^ tmp;
//        }

//        void ProcessBool2(ref bool arg0, ref String2BoolTuple boolTuple)
//        {
//            if (!boolTuple.Enabled) return;
//            var tmp = arg0;
//            arg0 = !GUILayout.Toggle(!arg0, boolTuple.Base + " : " + arg0.ToString(), "button");
//            hasChanged |= arg0 ^ tmp;
//        }

//        void ProcessBool3(ref bool arg0, ref StringBoolTuple boolTuple)
//        {
//            if (!boolTuple.Enabled) return;
//            var tmp = arg0;
//            arg0 = !GUILayout.Toggle(!arg0, boolTuple.Enumerable + " : " + arg0.ToString(), "button");
//            hasChanged |= arg0 ^ tmp;
//        }
//        tuple.EnabledArray.SelectDeselectAll(x => x, (xs, value) =>
//        {
//            hasChanged = true;
//            for (var i = 0; i < xs.Length; i++)
//                xs[i] = value;
//        });
//        Process(tuple.EnabledArray, ProcessBool0, ProcessBool1, ProcessBool2, ProcessBool3);
//    }

//    private void Process<T>(T[] array, RefAction<T, StringBoolTuple> action0, RefAction<T, StringBoolTuple> action1, RefAction<T, String2BoolTuple> action2, RefAction<T, StringBoolTuple> action3)
//    {
//        if (array is null) throw new ArgumentNullException();
//        if (array.Length != _enumerableExistsSettings.TypeLength) throw new ArgumentException($"{array.Length} not equals to {_enumerableExistsSettings.TypeLength}");

//        var index = 0;

//        var minByEnumerableNumberVariants = _enumerableExistsSettings.MinByEnumerableNumberVariants;
//        for (var i = 0; i < minByEnumerableNumberVariants.Length; i++, index++)
//            action0(ref array[index], ref minByEnumerableNumberVariants[i]);

//        var maxByEnumerableNumberVariants = _enumerableExistsSettings.MaxByEnumerableNumberVariants;
//        for (var i = 0; i < maxByEnumerableNumberVariants.Length; i++, index++)
//            action1(ref array[index], ref maxByEnumerableNumberVariants[i]);

//        var specialTypeArray = _enumerableExistsSettings.SpecialTypeArray;
//        for (var i = 0; i < specialTypeArray.Length; i++, index++)
//            action2(ref array[index], ref specialTypeArray[i]);

//        var enumerableArray = _enumerableExistsSettings.EnumerableArray;
//        for (var i = 0; i < enumerableArray.Length; i++, index++)
//            action3(ref array[index], ref enumerableArray[i]);
//    }

//    private void WhetherToIncludeOrNot()
//    {
//        if (!ShowFoldout(nameof(UniNativeLinq) + nameof(WhetherToIncludeOrNot), "Whether to include each enumerable type or NOT")) return;
//        using (var indentScope = IndentScope.Create())
//        {
//            XxxByEnumerableSelect("MinBy", _enumerableExistsSettings.MinByEnumerableNumberVariants);
//            XxxByEnumerableSelect("MaxBy", _enumerableExistsSettings.MaxByEnumerableNumberVariants);
//            SpecialEnumerableSelect(_enumerableExistsSettings.SpecialTypeArray);
//            EnumerableSelect(_enumerableExistsSettings.EnumerableArray);
//        }
//    }

//    private void EnumerableSelect(StringBoolTuple[] settings)
//    {
//        void SetAll(StringBoolTuple[] settingCollection, bool enabled)
//        {
//            hasChanged = true;
//            for (var i = 0; i < settingCollection.Length; i++)
//                settingCollection[i].Enabled = enabled;
//        }

//        void ShowSetting(ref StringBoolTuple setting)
//        {
//            var tmp = setting.Enabled;
//            setting.Enabled = !GUILayout.Toggle(!setting.Enabled, setting.Enumerable + " : " + setting.Enabled, "button");
//            hasChanged |= tmp ^ setting.Enabled;
//        }

//        if (!ShowFoldout(nameof(UniNativeLinq) + nameof(EnumerableSelect), "Enumerables")) return;

//        bool Predicate(StringBoolTuple x) => x.Enabled;

//        settings.SelectDeselectAll(Predicate, SetAll);
//        for (var i = 0; i < settings.Length; i++)
//            ShowSetting(ref settings[i]);

//        if (hasChanged)
//            CheckAndRestoreDependencyToNativeEnumerable(settings);
//    }

//    private void SpecialEnumerableSelect(String2BoolTuple[] settings)
//    {
//        if (!ShowFoldout(nameof(UniNativeLinq) + nameof(SpecialEnumerableSelect), nameof(SpecialEnumerableSelect))) return;
//        void ShowSetting(ref String2BoolTuple setting)
//        {
//            var tmp = setting.Enabled;
//            setting.Enabled = !GUILayout.Toggle(!setting.Enabled, setting.Base + " -> " + setting.Enumerable + " : " + setting.Enabled, "button");
//            hasChanged |= tmp ^ setting.Enabled;
//        }

//        bool Predicate(String2BoolTuple x) => x.Enabled;
//        void SetAll(String2BoolTuple[] array, bool enabled)
//        {
//            hasChanged = true;
//            for (var i = 0; i < array.Length; i++)
//                array[i].Enabled = enabled;
//        }
//        settings.SelectDeselectAll(Predicate, SetAll);
//        for (var i = 0; i < settings.Length; i++)
//            ShowSetting(ref settings[i]);

//    }

//    private void CheckAndRestoreDependencyToNativeEnumerable(StringBoolTuple[] settings)
//    {
//        foreach (var element in settings)
//        {
//            if (element.Enumerable != "Native") continue;
//            if (element.Enabled) return;
//            var dependencies = _enumerableExistsSettings.Dependencies;
//            foreach (var dependency in dependencies)
//            {
//                foreach (var t in dependency.Destination)
//                {
//                    if (t != "Native") continue;
//                    _enumerableExistsSettings.SetAllFalse(dependency.Source);
//                }
//            }
//        }
//    }

//    private void XxxByEnumerableSelect(string apiName, StringBoolTuple[] settings)
//    {
//        if (!ShowFoldout(string.Intern(nameof(UniNativeLinq) + nameof(XxxByEnumerableSelect) + apiName), apiName)) return;

//        void SetAll(StringBoolTuple[] array, bool value)
//        {
//            hasChanged = true;
//            for (var i = 0; i < settings.Length; i++)
//                array[i].Enabled = value;
//        }

//        bool Predicate(StringBoolTuple x) => x.Enabled;

//        settings.SelectDeselectAll(Predicate, SetAll);
//        for (var i = 0; i < settings.Length; i++)
//        {
//            ref var setting = ref settings[i];
//            var tmp = setting.Enabled;
//            setting.Enabled = !GUILayout.Toggle(!setting.Enabled, apiName + "/" + setting.Enumerable + " : " + setting.Enabled, "button");
//            hasChanged |= tmp ^ setting.Enabled;
//        }
//    }
//}
//}
