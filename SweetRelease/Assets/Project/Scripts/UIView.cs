using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public abstract class UIView : MonoBehaviour
    {
        [SerializeField]
        protected UIAnimation transitionInAnimation;

        [SerializeField]
        protected UIAnimation transitionOutAnimation;

        public virtual async UniTask Show()
        {
            await transitionInAnimation.Play();
        }

        public virtual async UniTask Hide()
        {
            await transitionOutAnimation.Play();
        }
    }
}
