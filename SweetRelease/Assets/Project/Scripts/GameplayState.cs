using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class GameplayState : AppState
    {
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
            StartLevel().Forget();
        }

        private async UniTask StartLevel()
        {
            await activeLevel.StartLevel(true, gameplayView.PlayCountdownAnimation);
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
            gameWonView = GameObject.Instantiate(gameWonViewPrefab);

            await gameWonView.Show();
            await gameWonView.PlayWinAnimation(gameplayView.TotalTime, starsCollected);

            gameWonView.OnLevelSelectRequestedEvent += OnLevelSelectRequested;
            gameWonView.OnRestartRequestedEvent += OnRestartLevelRequested;
        }


        private void OnLevelSelectRequested()
        {
            if (gameWonView != null)
            {
                gameWonView.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
            }

            if (gameRestartView != null)
            {
                gameRestartView.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
            }

            fsm.TransitionToState(new LevelSelectionState()).Forget();
        }

        private void OnRestartLevelRequested()
        {
            if (gameWonView != null)
            {
                gameWonView.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
            }

            if (gameRestartView != null)
            {
                gameRestartView.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
            }

            TryDisposeLevel();
            TryDisposeGameOverViews();
            CreateLevel();
            gameplayView.Clear();
            StartLevel().Forget();
        }

        private void TryDisposeGameOverViews()
        {
            if (gameWonView != null)
            {
                gameWonView.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
                gameWonView.OnRestartRequestedEvent -= OnRestartLevelRequested;
                Object.Destroy(gameWonView.gameObject);
            }

            if (gameRestartView != null)
            {
                gameRestartView.OnLevelSelectRequestedEvent -= OnLevelSelectRequested;
                gameRestartView.OnRestartRequestedEvent -= OnRestartLevelRequested;
                Object.Destroy(gameRestartView.gameObject);
            }
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
