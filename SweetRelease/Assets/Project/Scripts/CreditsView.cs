using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts
{
    public class CreditsView : UIView
    {
        [SerializeField]
        private Button closeCreditsButton;

        public event Action OnCloseCreditsRequestedEvent;

        private void OnEnable()
        {
            closeCreditsButton.onClick.AddListener(OnCreditsCloseButtonClicked);
        }
        private void OnDisable()
        {
            closeCreditsButton.onClick.RemoveListener(OnCreditsCloseButtonClicked);
        }

        private void OnCreditsCloseButtonClicked()
        {
            OnCloseCreditsRequestedEvent?.Invoke();
        }
    }
}
