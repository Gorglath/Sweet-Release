using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class GameplayState : AppState
    {
        private const string GameplayView = "Prefabs/GameplayView";
        private const string GameWonView = "Prefabs/GameWonView";
        private const string GameRestartView = "Prefabs/GameRestartView";

        private readonly LevelConfig levelConfig;

        private Level activeLevel;
        private GameplayView gameplayViewPrefab;
        private GameplayView gameplayView;

        public GameplayState(LevelConfig config)
        {
            levelConfig = config;
        }

        public override async UniTask PreTransitionIn()
        {
            UnityEngine.Object loadedGameplayView = await Resources.LoadAsync<GameplayView>(GameplayView);
            gameplayViewPrefab = (GameplayView)loadedGameplayView;
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
        }

        private void OnWonLevelRequested()
        {
            gameplayView.StopTimer();

        }

        private void OnRestartLevelRequested()
        {
            TryDisposeLevel();
            CreateLevel();
            gameplayView.ClearTimer();
            StartLevel().Forget();
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
            activeLevel.OnRestartLevelRequestedEvent += OnRestartLevelRequested;
            activeLevel.OnWonLevelRequestedEvent += OnWonLevelRequested;
            activeLevel.OnStarCollected += OnStarCollected;
        }

        private void UnsubscribeListeners()
        {
            activeLevel.OnRestartLevelRequestedEvent -= OnRestartLevelRequested;
            activeLevel.OnWonLevelRequestedEvent -= OnWonLevelRequested;
            activeLevel.OnStarCollected -= OnStarCollected;
        }

        public override void Dispose()
        {
            TryDisposeLevel();
            TryDisposeView();
        }
    }
}
