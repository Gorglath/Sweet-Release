using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class GameplayView : UIView
    {
        [SerializeField]
        private UIAnimation countdownAnimation;

        [SerializeField]
        private StarsBadge starsBadge;

        [SerializeField]
        private Timer timer;

        public float TotalTime => timer.TotalTime;
        public void StartTimer()
        {
            timer.StartTimer();
        }

        public void StopTimer()
        {
            timer.StopTimer();
        }

        public void Clear()
        {
            timer.ClearTimer();
            starsBadge.Clear();
        }

        public void Dispose()
        {
            Clear();
        }

        public void AddStar()
        {
            starsBadge.AddStar();
        }

        public async UniTask PlayCountdownAnimation()
        {
            await countdownAnimation.Play();
        }
    }
}
