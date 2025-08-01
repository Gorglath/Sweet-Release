using Cysharp.Threading.Tasks;

namespace Assets.Project.Scripts
{
    public class FSM
    {
        private AppState currentState;

        public async UniTask TransitionToState(AppState newState)
        {
            newState.SetFSM(this);
            if (currentState == null)
            {
                await newState.PreTransitionIn();
                await newState.DuringTransitionIn();
                await newState.PostTransitionIn();
                newState.OnStateEnter();
                currentState = newState;
                return;
            }

            await UniTask.WhenAll(currentState.PreTransitionOut(), newState.PreTransitionIn());

            await UniTask.WhenAll(currentState.DuringTransitionOut(), newState.DuringTransitionIn());

            await UniTask.WhenAll(currentState.PostTransitionOut(), newState.PostTransitionIn());

            currentState.OnStateExit();
            newState.OnStateEnter();

            currentState = newState;
        }

        public UniTask Dispose()
        {
            currentState?.OnStateExit();

            return UniTask.CompletedTask;
        }
    }
}
