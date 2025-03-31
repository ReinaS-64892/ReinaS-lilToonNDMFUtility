using UnityEngine;

namespace lilToonNDMFUtility
{
    public abstract class LNUAvatarTagComponent : MonoBehaviour
#if LNU_VRCSDK3
    , VRC.SDKBase.IEditorOnly
#endif
    {
        void Start() { }
    }
}
