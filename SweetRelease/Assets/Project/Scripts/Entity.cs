using System;
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

        public bool IsAlive => m_cachedTransform != null;

        public EntityState EntityState { get; private set; }

        protected Transform m_cachedTransform;
        protected TrailManager trailManager;
        protected EntityCollsiionManager entityCollisionManager;

        public event Action<Entity> OnEntityDiedEvent;
        //protected event Action<Entity, collectable> OnEntityCollectedCollectableEvent;

        public void Init(TrailManager trailManager, EntityCollsiionManager entityCollisionManager)
        {
            this.trailManager = trailManager;
            this.entityCollisionManager = entityCollisionManager;
        }

        private void OnEnable()
        {
            m_cachedTransform = transform;
            OnCreated();
            entityCollisionManager.RegisterEntity(this);
            SetState(initialState);
        }

        private void OnDisable()
        {
            OnDisposed();
            entityCollisionManager.UnregisterEntity(this);
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

        public void SetState(EntityState state)
        {
            if (EntityState != state)
            {
                EntityState previousState = EntityState;
                EntityState = state;
                OnStateChanged(state, previousState);

                if (EntityState == EntityState.DEAD)
                {
                    OnEntityDiedEvent?.Invoke(this);
                }
            }
        }

        public virtual void OnStateChanged(EntityState newState, EntityState previousState) { }
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
        public virtual void OnKill() { }
        public void Kill()
        {
            entityCollisionManager.UnregisterEntity(this);
            OnKill();
            Destroy(gameObject);
        }
    }
}
