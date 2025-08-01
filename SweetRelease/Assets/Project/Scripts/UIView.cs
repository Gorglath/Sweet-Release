using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public abstract class UIView : MonoBehaviour
    {
        [SerializeField]
        protected UITransitionAnimation transitionInAnimation;

        [SerializeField]
        protected UITransitionAnimation transitionOutAnimation;

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
