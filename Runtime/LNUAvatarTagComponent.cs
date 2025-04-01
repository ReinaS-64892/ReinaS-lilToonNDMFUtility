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


    /*
        このツールも API はすべて内部APIであり、 SemanticVersioning の対象ではありません。
        ただし、以下の 属性がついた存在に限り、 SemanticVersioning の対象となり、その関数やクラスが削除されないことを保証し、挙動を可能な限り保ちます。
    */

    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class LNUPublicAPIAttribute : System.Attribute
    {
        public LNUPublicAPIAttribute() { }
    }
}
