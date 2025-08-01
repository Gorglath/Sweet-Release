using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts
{
    public class GameRestartView : UIView
    {
        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Button levelSelectionButton;

        public event Action OnRestartRequestedEvent;
        public event Action OnLevelSelectRequestedEvent;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            levelSelectionButton.onClick.AddListener(OnLevelSelectionButtonClicked);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            levelSelectionButton.onClick.RemoveListener(OnLevelSelectionButtonClicked);
        }

        private void OnLevelSelectionButtonClicked()
        {
            OnLevelSelectRequestedEvent?.Invoke();
        }

        private void OnRestartButtonClicked()
        {
            OnRestartRequestedEvent?.Invoke();
        }
    }
}
