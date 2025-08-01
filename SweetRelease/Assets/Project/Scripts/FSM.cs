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
                return;
            }

            await currentState.PreTransitionOut();
            await newState.PreTransitionIn();

            await currentState.DuringTransitionOut();
            await newState.DuringTransitionIn();

            await currentState.PostTransitionOut();
            await newState.PostTransitionIn();

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
