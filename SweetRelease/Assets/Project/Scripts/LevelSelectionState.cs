using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class LevelSelectionState : AppState
    {
        private const string LevelSelectionConfigPath = "Configs/LevelSelectionConfig";
        private LevelSelectionConfig config;

        private const string LevelSelectionViewPath = "Prefabs/LevelSelectionView";
        private LevelSelectionView levelSelectionViewPrefab;
        private LevelSelectionView levelSelectionView;

        public override async UniTask PreTransitionIn()
        {
            Object loadedConfig = await Resources.LoadAsync<LevelSelectionConfig>(LevelSelectionConfigPath);
            config = (LevelSelectionConfig)loadedConfig;

            Object loadedLevelSelectionView = await Resources.LoadAsync<LevelSelectionView>(LevelSelectionViewPath);
            levelSelectionViewPrefab = (LevelSelectionView)loadedLevelSelectionView;
        }

        public override UniTask DuringTransitionIn()
        {
            levelSelectionView = Object.Instantiate(levelSelectionViewPrefab);
            levelSelectionView.Set(config.levelConfigs);
            MusicManager.Instance.TransitionToMainMenu();
            return UniTask.CompletedTask;
        }

        public override UniTask PreTransitionOut()
        {
            UnsubscribeListeners();
            return base.PreTransitionOut();
        }

        public override UniTask DuringTransitionOut()
        {
            DisposeView();
            return UniTask.CompletedTask;
        }

        public override UniTask PostTransitionOut()
        {
            Resources.UnloadAsset(config);
            return UniTask.CompletedTask;
        }

        public override void OnStateEnter()
        {
            SubscribeListeners();
        }

        private void SubscribeListeners()
        {
            levelSelectionView.OnLevelSelectRequestedEvent += OnLevelSelectRequested;
            levelSelectionView.OnReturnRequestedEvent += OnReturnRequested;
        }

        private void UnsubscribeListeners()
        {
            if (levelSelectionView == null)
            {
                return;
            }

            levelSelectionView.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
            levelSelectionView.OnReturnRequestedEvent -= OnReturnRequested;
        }

        private void DisposeView()
        {
            if (levelSelectionView == null)
            {
                return;
            }

            levelSelectionView.Dispose();
            Object.Destroy(levelSelectionView.gameObject);
        }

        private void OnReturnRequested()
        {
            UnsubscribeListeners();
            fsm.TransitionToState(new MainMenuState()).Forget();
        }

        private void OnLevelSelectRequested(int id)
        {
            UnsubscribeListeners();
            LevelConfig selectedLevel = config.levelConfigs.First(c => c.Id == id);
            fsm.TransitionToState(new GameplayState(selectedLevel)).Forget();
        }

        public override void Dispose()
        {
            UnsubscribeListeners();
            DisposeView();
        }
    }
}
