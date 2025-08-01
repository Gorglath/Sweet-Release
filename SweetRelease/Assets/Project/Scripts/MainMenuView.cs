using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts
{
    public class MainMenuView : UIView
    {
        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Button creditsButton;

        public event Action OnStartRequestedEvent;
        public event Action OnCreditsRequestedEvent;

        public void OnEnable()
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
            creditsButton.onClick.AddListener(OnCreditsButtonClicked);
        }

        public void OnDisable()
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
            creditsButton.onClick.RemoveListener(OnCreditsButtonClicked);
        }

        private void OnCreditsButtonClicked()
        {
            OnCreditsRequestedEvent?.Invoke();
        }

        public void OnStartButtonClicked()
        {
            OnStartRequestedEvent?.Invoke();
        }
    }
}
