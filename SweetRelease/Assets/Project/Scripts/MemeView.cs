using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class MemeView : UIView
    {
        [SerializeField]
        private int animationTime = 7;

        public async UniTask Play()
        {
           await MusicManager.Instance.PlayMemeMusic(animationTime);
        }
    }
}
