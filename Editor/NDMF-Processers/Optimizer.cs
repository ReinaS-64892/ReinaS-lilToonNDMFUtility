// this code from lilycalInventory
// https://github.com/lilxyzw/lilycalInventory/blob/cfc380f97a6032211f0568c4b86d6dc11b8a12a2/Editor/Processor/Optimizer.cs

using System.Collections.Generic;
using System.Linq;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace lilToonNDMFUtility
{
    internal static class lilycalProcessor
    {
        internal class Optimizer
        {
            internal static void OptimizeMaterials(BuildContext ctx, Material[] materials, bool optimizeUnusedPropertyRemove = true)
            {
                var propMap = materials.Select(m => m.shader).Distinct().Where(s => s).ToDictionary(s => s, s => new ShaderPropertyContainer(s));

                var controllers = new HashSet<RuntimeAnimatorController>();
                controllers.UnionWith(ctx.AvatarRootObject.GetComponentsInChildren<Animator>(true).Where(a => a.runtimeAnimatorController).Select(a => a.runtimeAnimatorController));
#if LNU_VRCSDK3
                VRChatHelper.GetAnimatorControllers(ctx.AvatarDescriptor, controllers);
#endif
                var props = controllers.SelectMany(c => c.animationClips).SelectMany(c => AnimationUtility.GetCurveBindings(c)).Select(b => b.propertyName).Where(n => n.Contains("material."))
                .Select(n => n = n.Substring("material.".Length))
                .Select(n => { if (n.Contains(".")) n = n.Substring(0, n.IndexOf(".")); return n; }).Distinct().ToArray();

                foreach (var m in materials)
                {
                    if (optimizeUnusedPropertyRemove) RemoveUnusedProperties(m, propMap);
                    if (lilToon.lilMaterialUtils.CheckShaderIslilToon(m)) lilToon.lilMaterialUtils.RemoveUnusedTextureOnly(m, m.shader.name.Contains("Lite"), props);
                }
            }

            // シェーダーで使われていないプロパティを除去
            private static void RemoveUnusedProperties(Material material, Dictionary<Shader, ShaderPropertyContainer> propMap)
            {
                using var so = new SerializedObject(material);
                so.Update();
                using var savedProps = so.FindProperty("m_SavedProperties");
                if (material.shader)
                {
                    var dic = propMap[material.shader];
                    DeleteUnused(savedProps, "m_TexEnvs", dic.textures);
                    DeleteUnused(savedProps, "m_Floats", dic.floats);
                    DeleteUnused(savedProps, "m_Colors", dic.vectors);
                }
                else
                {
                    DeleteAll(savedProps, "m_TexEnvs");
                    DeleteAll(savedProps, "m_Floats");
                    DeleteAll(savedProps, "m_Colors");
                }
                so.ApplyModifiedProperties();
            }

            private static void DeleteUnused(SerializedProperty prop, string name, HashSet<string> names)
            {
                using var props = prop.FPR(name);
                if (props.arraySize == 0) return;
                int i = 0;
                var size = props.arraySize;
                var p = props.GetArrayElementAtIndex(i);
                void DeleteUnused()
                {
                    if (!names.Contains(p.GetStringInProperty("first")))
                    {
                        p.DeleteCommand();
                        if (i < --size)
                        {
                            p = props.GetArrayElementAtIndex(i);
                            DeleteUnused();
                        }
                    }
                    else if (p.NextVisible(false))
                    {
                        if (++i < size) DeleteUnused();
                    }
                }
                DeleteUnused();
                p.Dispose();
            }

            private static void DeleteAll(SerializedProperty prop, string name)
            {
                using var props = prop.FPR(name);
                props.arraySize = 0;
            }

            // シェーダーのプロパティを検索して保持するクラス
            private class ShaderPropertyContainer
            {
                internal HashSet<string> textures;
                internal HashSet<string> floats;
                internal HashSet<string> vectors;

                internal ShaderPropertyContainer(Shader shader)
                {
                    textures = new HashSet<string>();
                    floats = new HashSet<string>();
                    vectors = new HashSet<string>();

                    var count = shader.GetPropertyCount();

                    for (int i = 0; i < count; i++)
                    {
                        var t = shader.GetPropertyType(i);
                        var name = shader.GetPropertyName(i);
                        if (t == ShaderPropertyType.Texture) textures.Add(name);
                        else if (t == ShaderPropertyType.Color || t == ShaderPropertyType.Vector) vectors.Add(name);
                        else floats.Add(name);
                    }
                }
            }
        }
        // https://github.com/lilxyzw/lilycalInventory/blob/cfc380f97a6032211f0568c4b86d6dc11b8a12a2/Editor/Helper/ObjHelper.SerializedProperty.cs#L11-L14
        internal static SerializedProperty FPR(this SerializedProperty property, string name)
        {
            return property.FindPropertyRelative(name);
        }

        // https://github.com/lilxyzw/lilycalInventory/blob/cfc380f97a6032211f0568c4b86d6dc11b8a12a2/Editor/Helper/ObjHelper.SerializedProperty.cs#L96-L100
        internal static string GetStringInProperty(this SerializedProperty serializedProperty, string name)
        {
            using var prop = serializedProperty.FPR(name);
            return prop.stringValue;
        }

    }
}
