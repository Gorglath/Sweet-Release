using Cysharp.Threading.Tasks;

namespace Assets.Project.Scripts
{

    public abstract class AppState
    {
        protected FSM fsm;
        public void SetFSM(FSM fSM)
        {
            fsm = fSM;
        }

        public virtual UniTask PreTransitionOut() { return UniTask.CompletedTask; }
        public virtual UniTask PreTransitionIn() { return UniTask.CompletedTask; }
        public virtual UniTask PostTransitionOut() { return UniTask.CompletedTask; }
        public virtual UniTask PostTransitionIn() { return UniTask.CompletedTask; }
        public virtual UniTask DuringTransitionOut() { return UniTask.CompletedTask; }
        public virtual UniTask DuringTransitionIn() { return UniTask.CompletedTask; }
        public virtual void OnStateEnter() { }
        public virtual void OnStateExit() { }

        public abstract void Dispose();
    }
}
