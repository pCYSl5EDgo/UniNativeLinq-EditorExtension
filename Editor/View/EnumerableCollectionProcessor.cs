using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace UniNativeLinq.Editor
{
    internal sealed class EnumerableCollectionProcessor : IEnumerableCollectionProcessor
    {
        private SerializedObject @object;
        private SerializedProperty specialTypeArrayProperty;
        private SerializedProperty enumerableArrayProperty;

        public EnumerableCollectionProcessor()
        {
            var paths = AssetDatabase.FindAssets("t:" + nameof(EnumerableCollectionObject));
            switch (paths.Length)
            {
                case 1:
                    @object = new SerializedObject(AssetDatabase.LoadAssetAtPath<EnumerableCollectionObject>(AssetDatabase.GUIDToAssetPath(paths[0])));
                    break;
                case 0:
                    throw new Exception($"{nameof(EnumerableCollectionObject)}'s count is 0!");
                default:
                    throw new Exception($"{nameof(EnumerableCollectionObject)}'s count is greater than 1!");
            }
            specialTypeArrayProperty = @object.FindProperty(nameof(EnumerableCollectionObject.SpecialTypeArray));
            enumerableArrayProperty = @object.FindProperty(nameof(EnumerableCollectionObject.EnumerableArray));
        }

        public int Count => specialTypeArrayProperty.arraySize + enumerableArrayProperty.arraySize;

        public IEnumerable<string> NameCollection
        {
            get
            {
                @object.Update();

                IEnumerable<string> Enumerable(SerializedProperty typeArrayProperty)
                {
                    var length = typeArrayProperty.arraySize;
                    for (var i = 0; i < length; i++)
                    {
                        var element = new SerializedObject(typeArrayProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                        element.Update();
                        yield return element.FindProperty(nameof(StringBoolTuple.Enumerable)).stringValue;
                    }
                }

                var first = Enumerable(specialTypeArrayProperty);
                var second = Enumerable(enumerableArrayProperty);
                return first.Concat(second);
            }
        }

        public IEnumerable<(string Name, bool Enabled)> NameEnabledTupleCollection
        {
            get
            {
                @object.Update();

                IEnumerable<(string Name, bool Enabled)> Enumerable(SerializedProperty typeArrayProperty)
                {
                    var length = typeArrayProperty.arraySize;
                    for (var i = 0; i < length; i++)
                    {
                        var element = new SerializedObject(typeArrayProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                        element.Update();
                        yield return (element.FindProperty(nameof(StringBoolTuple.Enumerable)).stringValue, element.FindProperty(nameof(StringBoolTuple.Enabled)).boolValue);
                    }
                }

                var first = Enumerable(specialTypeArrayProperty);
                var second = Enumerable(enumerableArrayProperty);
                return first.Concat(second);
            }
        }

        public IEnumerable<string> EnabledNameCollection
        {
            get
            {
                @object.Update();

                IEnumerable<string> Enumerable(SerializedProperty typeArrayProperty)
                {
                    var length = typeArrayProperty.arraySize;
                    for (var i = 0; i < length; i++)
                    {
                        var element = new SerializedObject(typeArrayProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                        element.Update();
                        if (element.FindProperty(nameof(StringBoolTuple.Enabled)).boolValue)
                            yield return element.FindProperty(nameof(StringBoolTuple.Enumerable)).stringValue;
                    }
                }

                var first = Enumerable(specialTypeArrayProperty);
                var second = Enumerable(enumerableArrayProperty);
                return first.Concat(second);
            }
        }

        public bool TryGetEnabled(string name, out bool value)
        {
            bool Inner(SerializedProperty typeArrayProperty, out bool enabled)
            {
                var length = typeArrayProperty.arraySize;
                for (var i = 0; i < length; i++)
                {
                    var element = new SerializedObject(typeArrayProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                    element.Update();
                    if (element.FindProperty(nameof(StringBoolTuple.Enumerable)).stringValue != name)
                        continue;
                    enabled = element.FindProperty(nameof(StringBoolTuple.Enabled)).boolValue;
                    return true;
                }
                enabled = default;
                return false;
            }

            @object.Update();
            return Inner(specialTypeArrayProperty, out value) || Inner(enumerableArrayProperty, out value);
        }

        public bool TrySetEnabled(string name, bool value)
        {
            bool Inner(SerializedProperty typeArrayProperty)
            {
                var length = typeArrayProperty.arraySize;
                for (var i = 0; i < length; i++)
                {
                    var element = new SerializedObject(typeArrayProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                    element.Update();
                    if (element.FindProperty(nameof(StringBoolTuple.Enumerable)).stringValue != name)
                        continue;
                    var enabledProperty = element.FindProperty(nameof(StringBoolTuple.Enabled));
                    if (value == enabledProperty.boolValue)
                        return true;
                    enabledProperty.boolValue = value;
                    element.ApplyModifiedProperties();
                    return true;
                }
                return false;
            }
            return Inner(specialTypeArrayProperty) || Inner(enumerableArrayProperty);
        }

        public bool HasChanged => @object.hasModifiedProperties;
        public void Apply()
        {
            @object.ApplyModifiedProperties();
        }
    }
}
