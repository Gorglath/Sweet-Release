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
        private float scaleInDuration;

        [SerializeField]
        private Ease scaleInEase;

        [SerializeField]
        private string totalTimeFormat = "Time : {0}";

        [SerializeField]
        private TMP_Text levelLabel;

        public event Action OnRestartRequestedEvent;
        public event Action OnLevelSelectRequestedEvent;
        public event Action OnNextLevelRequestEvent;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            levelSelectionButton.onClick.AddListener(OnLevelSelectionButtonClicked);
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }

        private void OnNextLevelButtonClicked()
        {
            OnNextLevelRequestEvent?.Invoke();
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            levelSelectionButton.onClick.RemoveListener(OnLevelSelectionButtonClicked);
            nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);
        }

        private void OnLevelSelectionButtonClicked()
        {
            OnLevelSelectRequestedEvent?.Invoke();
        }

        private void OnRestartButtonClicked()
        {
            OnRestartRequestedEvent?.Invoke();
        }

        public void Init(bool hasNextLevel)
        {
            nextLevelButton.gameObject.SetActive(hasNextLevel);
        }

        public async UniTask PlayWinAnimation(float totalTime, int starsCollected, int currentLevel)
        {
            levelLabel.text = currentLevel.ToString();
            await UniTask.Delay(500);

            if (starsCollected > 0)
            {
                int elementIndex = 0;
                while (!destroyCancellationToken.IsCancellationRequested)
                {
                    string starSfxName = GetStarSfxName(elementIndex);
                    SFXManager.instance.PlaySFX(starSfxName);

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

        private string GetStarSfxName(int elementIndex)
        {
            return elementIndex switch
            {
                0 => Constants.SFXIds.StarsUI_1,
                1 => Constants.SFXIds.StarsUI_2,
                2 => Constants.SFXIds.StarsUI_3,
                _ => Constants.SFXIds.StarsUI_3,
            };
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
            selectedElement.localScale = Vector3.zero;

            DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> tween = selectedElement.DOScale(Vector3.one, scaleInDuration).SetEase(scaleInEase);
            while (!token.IsCancellationRequested && tween.IsActive() && !tween.IsComplete())
            {
                await UniTask.Yield();
            }
        }
    }
}
