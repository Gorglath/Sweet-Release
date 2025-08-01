using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class SimpleUITransitionAnimation : UIAnimation
    {
        [SerializeField]
        private GameObject rootObject;

        [SerializeField]
        private bool showObject = true;

        public override UniTask Play()
        {
            rootObject.SetActive(showObject);
            return UniTask.CompletedTask;
        }
    }
}
