using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Main : MonoBehaviour
    {
        [SerializeField]
        private AppStartConfig startConfig;

        private FSM fsm;

        private void Awake()
        {
            fsm = new FSM();
        }

        private void Start()
        {
            switch (startConfig.InitialState)
            {
                case AppStateType.MAINMENU:
                    fsm.TransitionToState(new MainMenuState()).Forget();
                    break;
                case AppStateType.LEVELSELECTION:
                    fsm.TransitionToState(new LevelSelectionState()).Forget();
                    break;
                case AppStateType.GAMEPLAY:
                    fsm.TransitionToState(new GameplayState(startConfig.gameplayLevelConfig)).Forget();
                    break;
                case AppStateType.NONE:
                default:
                    break;
            }
        }
    }
}
