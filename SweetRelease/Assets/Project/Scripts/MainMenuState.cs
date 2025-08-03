using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class MainMenuState : AppState
    {
        private const string mainMenuViewPath = "Prefabs/MainMenuView";
        private const string creditsViewPath = "Prefabs/CreditsView";
        private const string memeViewPath = "Prefabs/MemeView";

        private CreditsView creditsViewPrefab;
        private CreditsView creditsView;

        private MainMenuView mainMenuViewPrefab;
        private MainMenuView mainMenuView;

        private MemeView memeViewPrefab;

        private int snotBoops;

        public override async UniTask PreTransitionIn()
        {
            Object loadedObjectMainMenuView = await Resources.LoadAsync<MainMenuView>(mainMenuViewPath);
            Object loadedCreditsObject = await Resources.LoadAsync<CreditsView>(creditsViewPath);
            Object loadedMemeObject = await Resources.LoadAsync<MemeView>(memeViewPath);

            mainMenuViewPrefab = (MainMenuView)loadedObjectMainMenuView;
            creditsViewPrefab = (CreditsView)loadedCreditsObject;
            memeViewPrefab = (MemeView)loadedMemeObject;
        }

        public override UniTask DuringTransitionIn()
        {
            mainMenuView = Object.Instantiate(mainMenuViewPrefab);
            MusicManager.Instance.TransitionToMainMenu();
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
            mainMenuView.OnSnotBoopRequestedEvent += OnSnotBoopedRequested
                ;
        }

        private void OnSnotBoopedRequested()
        {
            this.snotBoops++;
            SFXManager.instance.PlaySFX(Constants.SFXIds.Death);
            if(snotBoops >= 10)
            {
                ShowTheMeme().Forget();
            }
        }

        private async UniTask ShowTheMeme()
        {
            var memeView = Object.Instantiate(memeViewPrefab);

            await memeView.Show();
            await memeView.Play();

            Object.Destroy(memeView.gameObject);
            snotBoops = 0;
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
            Object.Destroy(creditsView.gameObject);
        }

        private void DisposeViews()
        {
            if (mainMenuView != null)
            {
                Object.Destroy(mainMenuView.gameObject);
            }

            if (creditsView != null)
            {
                creditsView.OnCloseCreditsRequestedEvent -= OnCloseCreditsRequested;
                Object.Destroy(creditsView.gameObject);
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
