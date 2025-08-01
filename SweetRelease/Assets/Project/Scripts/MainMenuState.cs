using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class MainMenuState : AppState
    {
        private const string mainMenuViewPath = "Prefabs/MainMenuView";
        private const string creditsViewPath = "Prefabs/CreditsView";

        private CreditsView creditsViewPrefab;
        private CreditsView creditsView;

        private MainMenuView mainMenuViewPrefab;
        private MainMenuView mainMenuView;

        public override async UniTask PreTransitionIn()
        {
            Object loadedObjectMainMenuView = await Resources.LoadAsync<MainMenuView>(mainMenuViewPath);
            Object loadedCreditsObject = await Resources.LoadAsync<CreditsView>(mainMenuViewPath);

            mainMenuViewPrefab = (MainMenuView)loadedObjectMainMenuView;
            creditsViewPrefab = (CreditsView)loadedCreditsObject;
        }

        public override UniTask DuringTransitionIn()
        {
            mainMenuView = Object.Instantiate(mainMenuViewPrefab);
            return UniTask.CompletedTask;
        }

        public override UniTask PostTransitionIn()
        {
            mainMenuViewPrefab = null;
            Resources.UnloadAsset(mainMenuViewPrefab);
            return UniTask.CompletedTask;
        }

        public override UniTask PreTransitionOut()
        {
            UnsubscribeListeners();
            return UniTask.CompletedTask;
        }

        public override UniTask DuringTransitionOut()
        {
            DisposeViews();
            return UniTask.CompletedTask;
        }

        public override void OnStateEnter()
        {
            mainMenuView.OnStartRequestedEvent += OnStartRequested;
            mainMenuView.OnCreditsRequestedEvent += OnCreditsRequested;
        }

        public override void OnStateExit()
        {
        }

        private void OnStartRequested()
        {
            mainMenuView.OnStartRequestedEvent -= OnStartRequested;
            fsm.TransitionToState(new LevelSelectionState()).Forget();
        }

        private void OnCreditsRequested()
        {
            if (creditsView != null)
            {
                return;
            }

            OpenCredits().Forget();
        }

        private async UniTask OpenCredits()
        {
            creditsView = Object.Instantiate(creditsViewPrefab);

            await creditsView.Show();

            creditsView.OnCloseCreditsRequestedEvent += OnCloseCreditsRequested;
        }

        private void OnCloseCreditsRequested()
        {
            CloseCredits().Forget();
        }

        private async UniTask CloseCredits()
        {
            creditsView.OnCloseCreditsRequestedEvent -= OnCloseCreditsRequested;
            await creditsView.Hide();
            Object.Destroy(creditsView);
        }

        private void DisposeViews()
        {
            if (mainMenuView != null)
            {
                Object.Destroy(mainMenuView);
            }

            if (creditsView != null)
            {
                creditsView.OnCloseCreditsRequestedEvent -= OnCloseCreditsRequested;
                Object.Destroy(creditsView);
            }
        }

        public void SubscribeListeners()
        {
            mainMenuView.OnStartRequestedEvent += OnStartRequested;
            mainMenuView.OnCreditsRequestedEvent += OnCreditsRequested;
        }

        public void UnsubscribeListeners()
        {
            if (mainMenuView == null)
            {
                return;
            }

            mainMenuView.OnStartRequestedEvent -= OnStartRequested;
            mainMenuView.OnCreditsRequestedEvent -= OnCreditsRequested;
        }

        public override void Dispose()
        {
            UnsubscribeListeners();
            DisposeViews();
        }
    }
}
