using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class CountdownUIAnimation : UIAnimation
    {
        [SerializeField]
        private Transform[] countdownElements;

        [SerializeField]
        private float timeBetweenElements;

        [SerializeField]
        private float showElementDuration;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float elementScaleInRatio;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float elementScaleOutRatio;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float elementRevealRatio;

        [SerializeField]
        private Ease scaleInEase;

        [SerializeField]
        private Ease scaleOutEase;

        private void Awake()
        {
            foreach (Transform element in countdownElements)
            {
                element.gameObject.SetActive(false);
            }
        }

        public override async UniTask Play()
        {
            int elementIndex = 0;
            while (true)
            {
                SFXManager.instance.PlaySFX(Constants.SFXIds.Countdown);
                await ShowElement(elementIndex);
                elementIndex++;
                if (elementIndex == countdownElements.Length)
                {
                    break;
                }

                await WaitToShowNextElement();

                await UniTask.Yield();
            }

            SFXManager.instance.PlaySFX(Constants.SFXIds.CountdownEnd);
        }

        private async System.Threading.Tasks.Task WaitToShowNextElement()
        {
            float timeBetweenElementsCounter = 0.0f;
            while (true)
            {
                timeBetweenElementsCounter += Time.deltaTime;
                if (timeBetweenElementsCounter / timeBetweenElements >= 1)
                {
                    break;
                }

                await UniTask.Yield();
            }
        }

        private async UniTask ShowElement(int elementIndex)
        {
            Transform selectedElement = countdownElements[elementIndex];
            selectedElement.gameObject.SetActive(true);

            float scaleInDuration = showElementDuration * elementScaleInRatio;
            float scaleOutDuration = showElementDuration * elementScaleOutRatio;
            float scaleOutDelay = showElementDuration * elementRevealRatio;

            selectedElement.localScale = Vector3.zero;

            Sequence sequence = DOTween.Sequence();
            _ = sequence.Append(selectedElement.DOScale(Vector3.one, scaleInDuration).SetEase(scaleInEase));
            _ = sequence.Append(selectedElement.DOScale(Vector3.zero, scaleOutDuration).SetDelay(scaleOutDelay).SetEase(scaleOutEase));
            await sequence.Play().AsyncWaitForCompletion();
            selectedElement.gameObject.SetActive(false);
        }
    }
}
