using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class GameplayState : AppState
    {
        private const string LevelSelectionConfigPath = "Configs/LevelSelectionConfig";
        private const string GameplayViewPath = "Prefabs/GameplayView";
        private const string GameWonViewPath = "Prefabs/GameWonView";
        private const string GameRestartView = "Prefabs/GameRestartView";

        private readonly LevelConfig levelConfig;

        private Level activeLevel;
        private GameplayView gameplayViewPrefab;
        private GameplayView gameplayView;

        private GameWonView gameWonViewPrefab;
        private GameWonView gameWonView;

        private GameRestartView gameRestartViewPrefab;
        private GameRestartView gameRestartView;

        private int starsCollected;
        private LevelSelectionConfig levelSelectionConfig;

        public GameplayState(LevelConfig config)
        {
            levelConfig = config;
        }

        public override async UniTask PreTransitionIn()
        {
            Object loadedGameplayView = await Resources.LoadAsync<GameplayView>(GameplayViewPath);
            gameplayViewPrefab = (GameplayView)loadedGameplayView;

            Object loadedGameWonView = await Resources.LoadAsync<GameWonView>(GameWonViewPath);
            gameWonViewPrefab = (GameWonView)loadedGameWonView;

            Object loadedGameRestartView = await Resources.LoadAsync<GameRestartView>(GameRestartView);
            gameRestartViewPrefab = (GameRestartView)loadedGameRestartView;

            Object loadedConfig = await Resources.LoadAsync<LevelSelectionConfig>(LevelSelectionConfigPath);
            levelSelectionConfig = (LevelSelectionConfig)loadedConfig;
        }

        public override UniTask DuringTransitionIn()
        {
            gameplayView = Object.Instantiate(gameplayViewPrefab);
            CreateLevel();
            return UniTask.CompletedTask;
        }

        public override UniTask DuringTransitionOut()
        {
            TryDisposeLevel();
            TryDisposeView();
            TryDisposeGameOverViews();
            return UniTask.CompletedTask;
        }

        public override void OnStateEnter()
        {
            StartLevel(true).Forget();
        }

        private async UniTask StartLevel(bool showCinematic)
        {
            await activeLevel.StartLevel(showCinematic, gameplayView.PlayCountdownAnimation);
            gameplayView.StartTimer();
        }

        private void TryDisposeView()
        {
            if (gameplayView == null)
            {
                return;
            }

            Object.Destroy(gameplayView.gameObject);
        }

        private void OnStarCollected()
        {
            gameplayView.AddStar();
            starsCollected++;
        }

        private void OnWonLevelRequested()
        {
            gameplayView.StopTimer();
            ShowGameWonView().Forget();
        }

        private async UniTask ShowGameWonView()
        {
            if (PlayerPrefs.GetInt(levelConfig.Id.ToString()) < starsCollected)
            {
                PlayerPrefs.SetInt(levelConfig.Id.ToString(), starsCollected);
            }

            gameWonView = GameObject.Instantiate(gameWonViewPrefab);

            bool hasNextLevel = System.Array.IndexOf(levelSelectionConfig.levelConfigs, levelConfig) != levelSelectionConfig.levelConfigs.Length - 1;
            gameWonView.Init(hasNextLevel);

            await gameWonView.Show();
            await gameWonView.PlayWinAnimation(gameplayView.TotalTime, starsCollected);
            SubscribeGameWonListeners();
        }

        private void SubscribeGameWonListeners()
        {
            gameWonView.OnLevelSelectRequestedEvent += OnLevelSelectRequested;
            gameWonView.OnRestartRequestedEvent += OnRestartLevelRequested;
            gameWonView.OnNextLevelRequestEvent += OnNextLevelRequested;
        }
        private void UnsubscribeGameWonListeners()
        {
            gameWonView.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
            gameWonView.OnRestartRequestedEvent -= OnRestartLevelRequested;
            gameWonView.OnNextLevelRequestEvent -= OnNextLevelRequested;
        }
        private void OnNextLevelRequested()
        {
            UnsubscribeGameWonListeners();
            int currentLevelIndex = System.Array.IndexOf(levelSelectionConfig.levelConfigs, levelConfig);
            fsm.TransitionToState(new GameplayState(levelSelectionConfig.levelConfigs[currentLevelIndex + 1])).Forget();
        }

        private void OnLevelSelectRequested()
        {
            if (gameWonView != null)
            {
                UnsubscribeGameWonListeners();
            }

            if (gameRestartView != null)
            {
                UnsubscribeGameRestartListeners();
            }

            fsm.TransitionToState(new LevelSelectionState()).Forget();
        }

        private void OnRestartLevelRequested()
        {
            if (gameWonView != null)
            {
                UnsubscribeGameWonListeners();
            }

            if (gameRestartView != null)
            {
                UnsubscribeGameRestartListeners();
            }

            TryDisposeLevel();
            TryDisposeGameOverViews();
            CreateLevel();
            gameplayView.Clear();
            starsCollected = 0;
            StartLevel(false).Forget();
        }

        private void TryDisposeGameOverViews()
        {
            if (gameWonView != null)
            {
                UnsubscribeGameWonListeners();
                Object.Destroy(gameWonView.gameObject);
            }

            if (gameRestartView != null)
            {
                UnsubscribeGameRestartListeners();
                Object.Destroy(gameRestartView.gameObject);
            }
        }

        private void UnsubscribeGameRestartListeners()
        {
            gameRestartView.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
            gameRestartView.OnRestartRequestedEvent -= OnRestartLevelRequested;
        }

        private void CreateLevel()
        {
            activeLevel = Object.Instantiate(levelConfig.LevelPrefab);
            SubscribeListeners();
        }

        private void TryDisposeLevel()
        {
            if (activeLevel == null)
            {
                return;
            }

            UnsubscribeListeners();
            activeLevel.Dispose();
            Object.Destroy(activeLevel.gameObject);
        }

        private void SubscribeListeners()
        {
            activeLevel.OnPlayerCharacterDeath += OnPlayerCharacterDied;
            activeLevel.OnWonLevelRequestedEvent += OnWonLevelRequested;
            activeLevel.OnStarCollected += OnStarCollected;
        }

        private void OnPlayerCharacterDied()
        {
            gameplayView.StopTimer();
            gameRestartView = GameObject.Instantiate(gameRestartViewPrefab);

            gameRestartView.Show().Forget();

            gameRestartView.OnLevelSelectRequestedEvent += OnLevelSelectRequested;
            gameRestartView.OnRestartRequestedEvent += OnRestartLevelRequested;
        }

        private void UnsubscribeListeners()
        {
            activeLevel.OnPlayerCharacterDeath -= OnPlayerCharacterDied;
            activeLevel.OnWonLevelRequestedEvent -= OnWonLevelRequested;
            activeLevel.OnStarCollected -= OnStarCollected;
        }

        public override void Dispose()
        {
            TryDisposeLevel();
            TryDisposeView();
            TryDisposeGameOverViews();
        }
    }
}
