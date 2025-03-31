using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToonNDMFUtility
{
    [CustomEditor(typeof(lilToonMaterialNormalizer))]
    public sealed class lilToonMaterialNormalizerEditor : Editor
    {
        private SerializedProperty sNormalizeRange;
        private SerializedProperty sNormalizeTargetDetectionMode;
        // private SerializedProperty sAutoNormalizingMode;
        // private SerializedProperty sNormalizeTargetManualSelections;
        // private SerializedProperty sForceOverrideSomeReference;
        // private SerializedProperty sNormalizeUseProperty;
        // private Dictionary<NormalizeTarget, SerializedProperty> sBlockProperties;

        void OnEnable()
        {
            sNormalizeRange = serializedObject.FindProperty(nameof(lilToonMaterialNormalizer.NormalizeRange));
            sNormalizeTargetDetectionMode = serializedObject.FindProperty(nameof(lilToonMaterialNormalizer.NormalizeTargetDetectionMode));
            // sAutoNormalizingMode = serializedObject.FindProperty(nameof(lilToonMaterialNormalizer.AutoNormalizingMode));
            // sNormalizeTargetManualSelections = serializedObject.FindProperty(nameof(lilToonMaterialNormalizer.NormalizeTargetManualSelections));
            // sForceOverrideSomeReference = serializedObject.FindProperty(nameof(lilToonMaterialNormalizer.ForceOverrideSomeReference));
            // sNormalizeUseProperty = serializedObject.FindProperty(nameof(lilToonMaterialNormalizer.NormalizeUseProperty));

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(sNormalizeRange);
            EditorGUILayout.PropertyField(sNormalizeTargetDetectionMode);

            // var isManualSelectMode = ((NormalizeTargetDetectionMode)sNormalizeTargetDetectionMode.enumValueIndex) is NormalizeTargetDetectionMode.Manual;
            // if (isManualSelectMode)
            // {
            //     EditorGUILayout.PropertyField(sNormalizeTargetManualSelections);
            //     if (sNormalizeTargetManualSelections.isExpanded is false) { DrawNormalizeBocks(sNormalizeTargetManualSelections); }
            //     else { sBlockProperties = null; }
            // }
            // else { EditorGUILayout.PropertyField(sAutoNormalizingMode); }

            // EditorGUILayout.PropertyField(sForceOverrideSomeReference);
            // EditorGUILayout.PropertyField(sNormalizeUseProperty, new GUIContent("Normalize _Use*** Property"));
            serializedObject.ApplyModifiedProperties();
        }

        // private void DrawNormalizeBocks(SerializedProperty sNormalizeTargetManualSelections)
        // {
        //     if (sBlockProperties is null)
        //     {
        //         sBlockProperties = new();
        //         for (var i = 0; sNormalizeTargetManualSelections.arraySize > i; i += 1)
        //         {
        //             var p = sNormalizeTargetManualSelections.GetArrayElementAtIndex(i);
        //             sBlockProperties[(NormalizeTarget)p.FindPropertyRelative(nameof(NormalizeOption.Target)).enumValueIndex] = p;
        //         }
        //     }

        //     using (var hs = new EditorGUILayout.HorizontalScope())
        //     {
        //         EditorGUILayout.LabelField($"--- Properties ---");
        //         var allBlocks = Blocks().SelectMany(i => i.targets);
        //         using (var hsI = new EditorGUILayout.HorizontalScope())
        //         {
        //             if (GUILayout.Button("EnableAll")) { EnableAll(allBlocks); }
        //             if (GUILayout.Button("Invert")) { Invert(allBlocks); }
        //             // if (GUILayout.Button("ModeChange")) { ModeChange(allBlocks); }
        //         }
        //     }
        //     using var indentS = new EditorGUI.IndentLevelScope(1);
        //     foreach (var (name, blocks) in Blocks())
        //     {
        //         using (var hs = new EditorGUILayout.HorizontalScope())
        //         {
        //             EditorGUILayout.LabelField($"--- {name} ---");
        //             using (var hsI = new EditorGUILayout.HorizontalScope())
        //             {
        //                 if (GUILayout.Button("EnableAll")) { EnableAll(blocks); }
        //                 if (GUILayout.Button("Invert")) { Invert(blocks); }
        //                 // if (GUILayout.Button("ModeChange")) { ModeChange(blocks); }
        //             }
        //         }
        //         using var indentI = new EditorGUI.IndentLevelScope(1);
        //         foreach (var b in blocks)
        //         {
        //             DrawBlock(b);
        //         }
        //     }

        // }


        // private void DrawBlock(NormalizeTarget b)
        // {
        //     if (sBlockProperties is null) { return; }
        //     using var hs = new EditorGUILayout.HorizontalScope();
        //     if (sBlockProperties.ContainsKey(b) is false)
        //     {
        //         var newIndex = sNormalizeTargetManualSelections.arraySize;
        //         sNormalizeTargetManualSelections.arraySize += 1;
        //         var newBp = sNormalizeTargetManualSelections.GetArrayElementAtIndex(newIndex);
        //         newBp.FindPropertyRelative(nameof(NormalizeOption.Target)).intValue = (int)b;
        //         newBp.FindPropertyRelative(nameof(NormalizeOption.EnableNormalize)).boolValue = false;

        //         sBlockProperties[b] = newBp;
        //     }
        //     var bp = sBlockProperties[b];
        //     EditorGUILayout.PropertyField(bp.FindPropertyRelative(nameof(NormalizeOption.EnableNormalize)), new GUIContent(b.ToString()));
        //     // EditorGUILayout.PropertyField(bp.FindPropertyRelative(nameof(NormalizeOption.NormalizingMode)), GUIContent.none);
        // }
        // private void EnableAll(IEnumerable<NormalizeTarget> bs)
        // {
        //     if (sBlockProperties is null) { return; }
        //     foreach (var b in bs)
        //     {
        //         var bp = sBlockProperties[b];
        //         bp.FindPropertyRelative(nameof(NormalizeOption.EnableNormalize)).boolValue = true;
        //     }
        // }
        // private void Invert(IEnumerable<NormalizeTarget> bs)
        // {
        //     if (sBlockProperties is null) { return; }
        //     foreach (var b in bs)
        //     {
        //         var bp = sBlockProperties[b];
        //         var bpEnable = bp.FindPropertyRelative(nameof(NormalizeOption.EnableNormalize));
        //         bpEnable.boolValue = !bpEnable.boolValue;
        //     }
        // }
        // private void ModeChange(IEnumerable<NormalizeTarget> blocks)
        // {
        //     if (sBlockProperties is null) { return; }
        //     if (blocks.Any() is false) { return; }
        //     var refMode = sBlockProperties[blocks.First()].FindPropertyRelative(nameof(NormalizeOption.NormalizingMode)).enumValueIndex;
        //     refMode += 1;
        //     refMode %= 2;
        //     foreach (var b in blocks)
        //     {
        //         sBlockProperties[b].FindPropertyRelative(nameof(NormalizeOption.NormalizingMode)).enumValueIndex = refMode;
        //     }
        // }

        internal static IEnumerable<(string name, IEnumerable<NormalizeTarget> targets)> Blocks()
        {
            yield return (
                "Color"
                , new NormalizeTarget[]{

                    NormalizeTarget.MainTexture,
                    // NormalizeTarget.MainColorAdjustMask,

                    NormalizeTarget.MainTexture2nd,
                    // NormalizeTarget.MainTexture2ndBlendMask,
                    // NormalizeTarget.MainTexture2ndDissolveMask,

                    NormalizeTarget.MainTexture3rd,
                    // NormalizeTarget.MainTexture3rdBlendMask,
                    // NormalizeTarget.MainTexture3rdDissolveMask,

                    NormalizeTarget.AlphaMask,
                }
            );

            yield return (
                "Normal Map & Reflection"
                , new NormalizeTarget[]{
                    // NormalizeTarget.NormalMap,
                    // NormalizeTarget.NormalMap2nd,
                    // NormalizeTarget.NormalMap2ndScaleMask,

                    // NormalizeTarget.AnisotropyTangentMap,
                    // NormalizeTarget.AnisotropyScaleMask,

                    NormalizeTarget.BacklightColorTexture,

                    NormalizeTarget.ShadowStrengthMask,

                    // NormalizeTarget.ShadowBorderMask,
                    // NormalizeTarget.ShadowBlurMask,

                    // NormalizeTarget.ShadowColorTexture,
                    // NormalizeTarget.Shadow2ndColorTexture,
                    // NormalizeTarget.Shadow3rdColorTexture,

                    NormalizeTarget.RimShadeMask,
                    NormalizeTarget.SmoothnessTexture,
                    NormalizeTarget.MetallicGlossMap,
                    NormalizeTarget.ReflectionColorTexture,

                    NormalizeTarget.MatCapBlendMask,
                    // NormalizeTarget.MatCapBumpMap,

                    NormalizeTarget.MatCap2ndBlendMask,
                    // NormalizeTarget.MatCap2ndBumpMap,

                    NormalizeTarget.RimColorTexture,

                    NormalizeTarget.GlitterColorTexture,

                    NormalizeTarget.EmissionMap,
                    NormalizeTarget.EmissionBlendMask,

                    NormalizeTarget.Emission2ndMap,
                    NormalizeTarget.Emission2ndBlendMask,

                    // NormalizeTarget.ParallaxMap,
                }
            );

            yield return (
                "Advanced"
                , new NormalizeTarget[]{
                    NormalizeTarget.OutlineTexture,
                    NormalizeTarget.OutlineWidthMask,
                    // NormalizeTarget.OutlineVectorTexture,
                }
            );
        }
    }
}
