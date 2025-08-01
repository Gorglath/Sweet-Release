using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts
{
    public class GameWonView : UIView
    {
        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Button levelSelectionButton;

        [SerializeField]
        private Button nextLevelButton;

        [SerializeField]
        private TMP_Text totalTimeLabel;

        [SerializeField]
        private Image[] starsImages;

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

        [SerializeField]
        private string totalTimeFormat;

        public event Action OnRestartRequestedEvent;
        public event Action OnLevelSelectRequestedEvent;
        public event Action OnNextLevelRequestedEvent;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            levelSelectionButton.onClick.AddListener(OnLevelSelectionButtonClicked);
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            levelSelectionButton.onClick.RemoveListener(OnLevelSelectionButtonClicked);
            nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);
        }

        private void OnNextLevelButtonClicked()
        {
            OnNextLevelRequestedEvent?.Invoke();
        }

        private void OnLevelSelectionButtonClicked()
        {
            OnLevelSelectRequestedEvent?.Invoke();
        }

        private void OnRestartButtonClicked()
        {
            OnRestartRequestedEvent?.Invoke();
        }

        public async UniTask PlayWinAnimation(float totalTime, int starsCollected)
        {
            if (starsCollected > 0)
            {
                int elementIndex = 0;
                while (!destroyCancellationToken.IsCancellationRequested)
                {
                    await ShowElement(elementIndex, destroyCancellationToken);
                    elementIndex++;
                    if (elementIndex == starsCollected)
                    {
                        break;
                    }

                    await WaitToShowNextElement(destroyCancellationToken);

                    await UniTask.Yield(destroyCancellationToken);
                }
            }

            await UniTask.Delay((int)timeBetweenElements, cancellationToken: destroyCancellationToken);
            totalTimeLabel.text = string.Format(totalTimeFormat, totalTime.ToString("F1"));
        }

        private void Awake()
        {
            foreach (Image element in starsImages)
            {
                element.gameObject.SetActive(false);
            }
        }

        private async UniTask WaitToShowNextElement(CancellationToken token)
        {
            float timeBetweenElementsCounter = 0.0f;
            while (!token.IsCancellationRequested)
            {
                timeBetweenElementsCounter += Time.deltaTime;
                if (timeBetweenElementsCounter / timeBetweenElements >= 1)
                {
                    break;
                }

                await UniTask.Yield();
            }
        }

        private async UniTask ShowElement(int elementIndex, CancellationToken token)
        {
            Transform selectedElement = starsImages[elementIndex].transform;
            selectedElement.gameObject.SetActive(true);

            float scaleInDuration = showElementDuration * elementScaleInRatio;
            float scaleOutDuration = showElementDuration * elementScaleOutRatio;
            float scaleOutDelay = showElementDuration * elementRevealRatio;

            selectedElement.localScale = Vector3.zero;

            Sequence sequence = DOTween.Sequence();
            _ = sequence.Append(selectedElement.DOScale(Vector3.one, scaleInDuration).SetEase(scaleInEase));
            _ = sequence.Append(selectedElement.DOScale(Vector3.zero, scaleOutDuration).SetDelay(scaleOutDelay).SetEase(scaleOutEase));
            _ = sequence.Play();
            await UniTask.WaitUntil(() => sequence.IsComplete(), cancellationToken: token);
            selectedElement.gameObject.SetActive(false);
        }
    }
}
