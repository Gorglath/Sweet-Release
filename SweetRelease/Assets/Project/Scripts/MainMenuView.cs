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

        [SerializeField]
        private Button snotButton;

        public event Action OnStartRequestedEvent;
        public event Action OnCreditsRequestedEvent;
        public event Action OnSnotBoopRequestedEvent;

        public void OnEnable()
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
            creditsButton.onClick.AddListener(OnCreditsButtonClicked);
            snotButton.onClick.AddListener(OnSnotBooped);
        }

        private void OnSnotBooped()
        {
            OnSnotBoopRequestedEvent?.Invoke();
        }

        public void OnDisable()
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
            creditsButton.onClick.RemoveListener(OnCreditsButtonClicked);
            snotButton.onClick.RemoveListener(OnSnotBooped);
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
