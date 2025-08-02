using DG.Tweening;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class DotweenFloatAnimation : MonoBehaviour
    {
        [SerializeField]
        private float duration;

        [SerializeField]
        private float targetLocation;

        [SerializeField]
        private float delay;

        [SerializeField]
        private Ease ease;

        [SerializeField]
        private bool rotate;

        [SerializeField]
        private float rotationDuration;

        private void OnEnable()
        {
            Sequence sequence = DOTween.Sequence();
            _ = sequence.Append(transform.DOLocalMoveY(targetLocation, duration));
            if (rotate)
            {
                Vector3 rotation = new(transform.eulerAngles.x, 360.0f, transform.eulerAngles.z);
                _ = sequence.Join(transform.DOLocalRotate(rotation, rotationDuration, RotateMode.FastBeyond360));
            }

            _ = sequence.SetLoops(-1, LoopType.Yoyo).SetDelay(delay).SetEase(ease).Play();
        }

        private void OnDisable()
        {
            _ = transform.DOKill();
        }
    }
}
