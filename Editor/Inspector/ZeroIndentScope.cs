// this code from Modular Avatar
// https://github.com/bdunderscore/modular-avatar/blob/b75e74ef849e02235022080d82942347d01f5691/Editor/Inspector/ZeroIndentScope.cs

using System;
using UnityEditor;

namespace lilToonNDMFUtility
{
    internal class ZeroIndentScope : IDisposable
    {
        private int oldIndentLevel;

        public ZeroIndentScope()
        {
            oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
        }

        public void Dispose()
        {
            EditorGUI.indentLevel = oldIndentLevel;
        }
    }
}
