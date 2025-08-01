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
        private TMP_Text levelNumberLabel;

        [SerializeField]
        private StarsBadge starsBadge;

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

        public void Set(LevelConfig config, int levelNumber)
        {
            id = config.Id;
            levelNumberLabel.text = levelNumber.ToString();
            NameLabel.text = config.Name;
            starsBadge.Init(PlayerPrefs.GetInt(config.Id.ToString(), 0));
        }
    }
}
