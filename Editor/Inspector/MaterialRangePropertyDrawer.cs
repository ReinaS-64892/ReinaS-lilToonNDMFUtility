using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace lilToonNDMFUtility
{
    [CustomPropertyDrawer(typeof(MaterialRange))]
    public sealed class MaterialRangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sRangeMode = property.FindPropertyRelative(nameof(MaterialRange.RangeMode));
            var drawHeight = position.height;
            position.height = 18f;
            EditorGUI.PropertyField(position, sRangeMode, label);
            position.y += position.height;

            using var indent = new EditorGUI.IndentLevelScope(1);

            switch ((MaterialRange.Mode)sRangeMode.enumValueIndex)
            {
                default:
                case MaterialRange.Mode.All: { break; }
                case MaterialRange.Mode.ParentObject:
                    {
                        var sParentObject = property.FindPropertyRelative(nameof(MaterialRange.ParentObjects));
                        position.height = drawHeight - 18f;
                        EditorGUI.PropertyField(position, sParentObject);
                        break;
                    }
                case MaterialRange.Mode.Manual:
                    {
                        var sManualSelections = property.FindPropertyRelative(nameof(MaterialRange.ManualSelections));
                        position.height = drawHeight - 18f;
                        EditorGUI.PropertyField(position, sManualSelections);
                        break;
                    }
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var sRangeMode = property.FindPropertyRelative(nameof(MaterialRange.RangeMode));
            switch ((MaterialRange.Mode)sRangeMode.enumValueIndex)
            {
                default:
                case MaterialRange.Mode.All: return 18f;
                case MaterialRange.Mode.ParentObject: return 18f + EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(MaterialRange.ParentObjects)));
                case MaterialRange.Mode.Manual: return 18f + EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(MaterialRange.ManualSelections)));
            }
        }
    }
}
