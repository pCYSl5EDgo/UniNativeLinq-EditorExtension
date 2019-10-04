using System;
using UnityEditor;

namespace UniNativeLinq.Editor
{
    internal readonly struct IndentScope : IDisposable
    {
        public readonly int OriginalIndent;

        public IndentScope(int originalIndent)
        {
            OriginalIndent = originalIndent;
        }

        public static IndentScope Create(int value = 1)
        {
            var indentScope = new IndentScope(EditorGUI.indentLevel);
            EditorGUI.indentLevel += value;
            return indentScope;
        }

        public void Dispose() => EditorGUI.indentLevel = OriginalIndent;
    }
}
