using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class GameplayView : UIView
    {
        [SerializeField]
        private UIAnimation countdownAnimation;

        [SerializeField]
        private Timer timer;

        public void StartTimer()
        {
            timer.StartTimer();
        }

        public void StopTimer()
        {
            timer.StopTimer();
        }

        public void ClearTimer()
        {
            timer.ClearTimer();
        }

        public void Dispose()
        {
            ClearTimer();
        }

        public async UniTask PlayCountdownAnimation()
        {
            await countdownAnimation.Play();
        }
    }
}
