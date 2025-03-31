using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToonNDMFUtility
{
    [CustomEditor(typeof(lilToonMaterialPropertyUnificator))]
    public sealed class lilToonMaterialPropertyUnificatorEditor : Editor
    {
        private SerializedProperty sUnifiedRange;
        private SerializedProperty sUnificationReferenceMaterial;
        private SerializedProperty sUnificationTargetProperties;

        private lilToonInspector lilToonInspector = new();
        private Dictionary<string, bool> foldOutState = new();

        void OnEnable()
        {
            sUnifiedRange = serializedObject.FindProperty(nameof(lilToonMaterialPropertyUnificator.UnifiedRange));
            sUnificationReferenceMaterial = serializedObject.FindProperty(nameof(lilToonMaterialPropertyUnificator.UnificationReferenceMaterial));
            sUnificationTargetProperties = serializedObject.FindProperty(nameof(lilToonMaterialPropertyUnificator.UnificationTargetProperties));

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(sUnifiedRange);
            EditorGUILayout.PropertyField(sUnificationReferenceMaterial);
            EditorGUILayout.PropertyField(sUnificationTargetProperties);

            if (sUnificationReferenceMaterial.objectReferenceValue != null && sUnificationTargetProperties.isExpanded is false)
            {
                EditorGUILayout.LabelField("--- Target Preset ---");
                using (new EditorGUI.IndentLevelScope(1))
                    foreach (var brockBlock in BrockBlocks())
                    {
                        foldOutState[brockBlock.Item1] = EditorGUILayout.Foldout(foldOutState.TryGetValue(brockBlock.Item1, out var v) ? v : false, brockBlock.Item1);
                        using var bi = new EditorGUI.IndentLevelScope(1);
                        if (foldOutState[brockBlock.Item1])
                            foreach (var block in brockBlock.Item2)
                            {
                                DrawBrockUtility(block.Item1);
                                using var bni = new EditorGUI.IndentLevelScope(1);
                                foreach (var nestedBlock in block.Item2)
                                    DrawBrockUtility(nestedBlock);
                            }
                    }
            }



            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBrockUtility(PropertyBlock brock)
        {
            using var hs = new EditorGUILayout.HorizontalScope();
            EditorGUILayout.LabelField(brock.ToString());
            if (GUILayout.Button("Include"))
                AddTarget(lilToonInspector.GetBlock2Properties()[brock].Where(i => i.isTexture is false).Select(p => p.propertyName));
            if (GUILayout.Button("+Tex"))
                AddTarget(lilToonInspector.GetBlock2Properties()[brock].Where(i => i.isTexture is true).Select(p => p.propertyName));
            if (GUILayout.Button("Remove"))
                RemoveTarget(lilToonInspector.GetBlock2Properties()[brock].Select(p => p.propertyName));
        }

        internal static IEnumerable<(string, IEnumerable<(PropertyBlock, IEnumerable<PropertyBlock>)>)> BrockBlocks()
        {
            yield return (
                "Base Settings"
                , new (PropertyBlock, IEnumerable<PropertyBlock>)[]{
                    (PropertyBlock.Base,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Lighting,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.UV,Array.Empty<PropertyBlock>()),
                }
            );
            yield return (
                "Color"
                , new (PropertyBlock, IEnumerable<PropertyBlock>)[]{
                    (PropertyBlock.MainColor,new PropertyBlock[]{
                        PropertyBlock.MainColor1st,
                        PropertyBlock.MainColor2nd,
                        PropertyBlock.MainColor3rd,
                        PropertyBlock.AlphaMask,
                    }),
                    (PropertyBlock.Shadow,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.RimShade,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Emission,new PropertyBlock[]{
                        PropertyBlock.Emission1st,
                        PropertyBlock.Emission2nd,
                    }),
                }
            );

            yield return (
                "Normal Map & Reflection"
                , new (PropertyBlock, IEnumerable<PropertyBlock>)[]{
                    (PropertyBlock.NormalMap,new PropertyBlock[]{
                        PropertyBlock.NormalMap1st,
                        PropertyBlock.NormalMap2nd,
                        PropertyBlock.Anisotropy,
                    }),
                    // (PropertyBlock.Reflections,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Reflection,new PropertyBlock[]{
                        // PropertyBlock.Gem,
                    }),
                    (PropertyBlock.MatCaps,new PropertyBlock[]{
                        PropertyBlock.MatCap1st,
                        PropertyBlock.MatCap2nd,
                    }),
                    (PropertyBlock.RimLight,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Glitter,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Backlight,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Gem,Array.Empty<PropertyBlock>()),

                }
            );
            yield return (
                "Advanced"
                , new (PropertyBlock, IEnumerable<PropertyBlock>)[]{
                    (PropertyBlock.Outline,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Parallax,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.DistanceFade,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.AudioLink,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Dissolve,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.IDMask,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.UDIMDiscard,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Refraction,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Fur,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Encryption,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Stencil,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Rendering,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Tessellation,Array.Empty<PropertyBlock>()),
                    (PropertyBlock.Other,Array.Empty<PropertyBlock>()),
                }
            );
        }

        private void AddTarget(IEnumerable<string> enumerable)
        {
            var obj = (lilToonMaterialPropertyUnificator)target;
            Undo.RecordObject(obj, "Add targets");
            obj.UnificationTargetProperties.AddRange(enumerable);
            obj.UnificationTargetProperties = obj.UnificationTargetProperties.Distinct().ToList();
        }
        private void RemoveTarget(IEnumerable<string> enumerable)
        {
            var obj = (lilToonMaterialPropertyUnificator)target;
            var removeSet = new HashSet<string>(enumerable);
            Undo.RecordObject(obj, "Add targets");
            obj.UnificationTargetProperties.RemoveAll(removeSet.Contains);
        }

    }
}
