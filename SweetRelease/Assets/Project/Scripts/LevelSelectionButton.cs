using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts
{
    public class LevelSelectionButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private TMP_Text NameLabel;

        [SerializeField]
        private StarsBadge starsBadge;

        [SerializeField]
        private Image cherryImage;

        [SerializeField]
        private Image backgroundImage;

        public event Action<int> OnLevelSelectRequestedEvent;
        private int id;

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            OnLevelSelectRequestedEvent?.Invoke(id);
        }

        public void Set(LevelConfig config)
        {
            id = config.Id;
            NameLabel.text = config.Name;
            int totalStars = PlayerPrefs.GetInt(config.Id.ToString(), 0);
            cherryImage.gameObject.SetActive(totalStars == 3);
            backgroundImage.sprite = config.backgroundSprite;
            starsBadge.Init(totalStars);
        }
    }
}
