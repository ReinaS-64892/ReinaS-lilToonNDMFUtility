using UnityEngine;

namespace lilToonNDMFUtility
{
    [AddComponentMenu(LNUConsts.ADD_MENU_BASE + nameof(lilToonMaterialOptimizer))]
    public sealed class lilToonMaterialOptimizer : LNUAvatarTagComponent , ITargetMaterialRange
    {
        public MaterialRange OptimizeRange;

        public MaterialRange TargetMaterialRange => OptimizeRange;
    }
}
