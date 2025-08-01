using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts
{
    public class LevelSelectionView : UIView
    {
        [SerializeField]
        private Button returnButton;

        [SerializeField]
        private RectTransform levelsContainer;

        [SerializeField]
        private LevelSelectionButton levelSelectionButtonTemplate;
        private readonly List<LevelSelectionButton> levelSelectionButtons = new();

        public event Action<int> OnLevelSelectRequestedEvent;
        public event Action OnReturnRequestedEvent;

        private void Awake()
        {
            levelSelectionButtonTemplate.gameObject.SetActive(false);
        }

        public void OnEnable()
        {
            returnButton.onClick.AddListener(OnReturnButtonClicked);
        }

        public void OnDisable()
        {
            returnButton.onClick.RemoveListener(OnReturnButtonClicked);
        }

        private void OnReturnButtonClicked()
        {
            OnReturnRequestedEvent?.Invoke();
        }

        public void Set(LevelConfig[] configs)
        {
            CreateLevelSelectionButtons(configs);
        }

        public void Dispose()
        {
            foreach (LevelSelectionButton levelSelectionButton in levelSelectionButtons)
            {
                levelSelectionButton.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
                Destroy(levelSelectionButton.gameObject);
            }

            levelSelectionButtons.Clear();
        }

        private void CreateLevelSelectionButtons(LevelConfig[] configs)
        {
            for (int i = 0; i < configs.Length; i++)
            {
                LevelSelectionButton createdButton = Instantiate(levelSelectionButtonTemplate, levelsContainer);
                createdButton.gameObject.SetActive(true);
                createdButton.Set(configs[i], i + 1);
                createdButton.OnLevelSelectRequestedEvent += OnLevelSelectRequested;
                levelSelectionButtons.Add(createdButton);
            }
        }

        private void OnLevelSelectRequested(int id)
        {
            OnLevelSelectRequestedEvent?.Invoke(id);
        }
    }
}
