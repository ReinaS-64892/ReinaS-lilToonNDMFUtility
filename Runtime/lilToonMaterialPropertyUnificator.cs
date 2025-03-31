using System.Collections.Generic;
using UnityEngine;

namespace lilToonNDMFUtility
{
    [AddComponentMenu(LNUConsts.ADD_MENU_BASE + nameof(lilToonMaterialPropertyUnificator))]
    public sealed class lilToonMaterialPropertyUnificator : LNUAvatarTagComponent , ITargetMaterialRange
    {
        public MaterialRange UnifiedRange;
        public Material UnificationReferenceMaterial;
        public List<string> UnificationTargetProperties = new();

        public MaterialRange TargetMaterialRange => UnifiedRange;
    }
}
