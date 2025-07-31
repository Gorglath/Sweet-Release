using UnityEngine;

namespace Assets.Project.Scripts
{
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField]
        public EntityConfig Config;

        [SerializeField]
        private EntityState initialState;

        public Vector3 Position => m_cachedTransform.position;

        public EntityState EntityState { get; private set; }

        protected Transform m_cachedTransform;

        private void OnEnable()
        {
            m_cachedTransform = transform;
            OnCreated();
            EntityCollsiionManager.instance.RegisterEntity(this);
            SetState(initialState);
        }

        private void OnDisable()
        {
            OnDisposed();
            EntityCollsiionManager.instance.UnregisterEntity(this);
        }

        private void Update()
        {
            switch (EntityState)
            {
                case EntityState.STATIC:
                    OnEntityStatic();
                    break;
                case EntityState.GLIDING:
                    OnEntityGliding();
                    break;
                case EntityState.AIRBOUND:
                    OnEntityAirbound();
                    break;
                case EntityState.DEAD:
                    OnEntityDead();
                    break;
                case EntityState.NONE:
                default:
                    break;
            }
        }

        public void Activate()
        {
            OnActivated();
        }

        public void Deactivate()
        {
            OnDeactivated();
        }

        protected void SetState(EntityState state)
        {
            if (EntityState != state)
            {
                EntityState = state;
                OnStateChanged(state);
            }
        }

        public virtual void OnStateChanged(EntityState newState) { }
        public virtual void OnActivated() { }
        public virtual void OnDeactivated() { }
        public virtual void OnCreated() { }
        public virtual void OnDisposed() { }
        public abstract void OnEntityStatic();
        public abstract void OnEntityGliding();
        public abstract void OnEntityAirbound();
        public abstract void OnEntityDead();
        public abstract void OnCollisionObstacle(Bounds myBounds, Entity other, Bounds otherBounds, Vector3 collisionPoint);
        public abstract void OnCollisionTrail(Vector3 overlapPosition);
    }
}
