// this code from lilycalInventory
// https://github.com/lilxyzw/lilycalInventory/blob/cfc380f97a6032211f0568c4b86d6dc11b8a12a2/Editor/VRChat/VRChatHelper.cs

#if LNU_VRCSDK3
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using Control = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control;
using ControlType = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType;
using Parameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter;
using ValueType = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.ValueType;

using nadena.dev.ndmf;

namespace lilToonNDMFUtility
{
    internal static class VRChatHelper
    {
        internal static int costMax = VRCExpressionParameters.MAX_PARAMETER_COST;
        internal static int costBool = VRCExpressionParameters.TypeCost(ValueType.Bool);
        internal static int costInt = VRCExpressionParameters.TypeCost(ValueType.Int);
        internal static int costFloat = VRCExpressionParameters.TypeCost(ValueType.Float);

        internal static void GetAnimatorControllers(VRCAvatarDescriptor descriptor, HashSet<RuntimeAnimatorController> controllers)
        {
            controllers.UnionWith(descriptor.specialAnimationLayers.Where(l => l.animatorController).Select(l => l.animatorController));
            if(descriptor.customizeAnimationLayers) controllers.UnionWith(descriptor.baseAnimationLayers.Where(l => l.animatorController).Select(l => l.animatorController));
        }

        internal static void GetAnimatorControllers(GameObject gameObject, HashSet<RuntimeAnimatorController> controllers)
        {
            var descriptor = gameObject.GetComponent<VRCAvatarDescriptor>();
            if(descriptor) GetAnimatorControllers(descriptor, controllers);
        }
    }
}
#endif
