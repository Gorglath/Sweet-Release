using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private Entity playerEntity;

        [SerializeField]
        private Entity goalEntity;

        [SerializeField]
        private CharacterCamera playerCamera;

        [SerializeField]
        private CinematicCamera cinematicCamera;

        [SerializeField]
        private TrailManager trailManager;

        [SerializeField]
        private EntityCollsiionManager entityCollsiionManager;

        public event Action OnPlayerCharacterDeath;
        public event Action OnWonLevelRequestedEvent;
        public event Action OnStarCollected;
        private void Awake()
        {
            Entity[] levelEntities = GetComponentsInChildren<Entity>();
            foreach (Entity entity in levelEntities)
            {
                entity.Init(trailManager, entityCollsiionManager);
            }
        }

        public async UniTask StartLevel(Func<UniTask> playCountdownAnimation)
        {
            playerCamera.gameObject.SetActive(false);
            cinematicCamera.gameObject.SetActive(true);
            await cinematicCamera.Play();
            playerCamera.gameObject.SetActive(true);
            await playCountdownAnimation.Invoke();

            Entity[] levelEntities = GetComponentsInChildren<Entity>();
            foreach (Entity entity in levelEntities)
            {
                entity.OnEntityDiedEvent += OnEntityDied;
                entity.Activate();
            }
        }

        private void OnEntityDied(Entity entity)
        {
            entity.OnEntityDiedEvent -= OnEntityDied;

            if (entity.Config.IsCollectable)
            {
                if (entity == goalEntity)
                {
                    Entity[] levelEntities = GetComponentsInChildren<Entity>();
                    foreach (Entity targetEntity in levelEntities)
                    {
                        targetEntity.SetState(EntityState.CELEBRATE);
                    }
                    trailManager.Clear();
                    entityCollsiionManager.Clear();
                    OnWonLevelRequestedEvent?.Invoke();
                    SFXManager.instance.PlaySFX(Constants.SFXIds.WinEat);
                    return;
                }

                SFXManager.instance.PlaySFX(Constants.SFXIds.Collect);
                OnStarCollected?.Invoke();
                return;
            }

            if (entity == playerEntity)
            {
                trailManager.Clear();
                entityCollsiionManager.Clear();
                OnPlayerCharacterDeath?.Invoke();
                return;
            }
        }

        public void Dispose()
        {
            Entity[] levelEntities = GetComponentsInChildren<Entity>();
            foreach (Entity entity in levelEntities)
            {
                entity.OnEntityDiedEvent -= OnEntityDied;
                entity.Deactivate();
            }
        }
    }
}
