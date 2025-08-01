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
        private CharacterCamera playerCamera;

        [SerializeField]
        private CinematicCamera cinematicCamera;

        [SerializeField]
        private TrailManager trailManager;

        [SerializeField]
        private EntityCollsiionManager entityCollsiionManager;

        public event Action OnRestartLevelRequestedEvent;
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

        public async UniTask StartLevel(bool showCinematicCamera, Func<UniTask> playCountdownAnimation)
        {
            playerCamera.gameObject.SetActive(false);
            cinematicCamera.gameObject.SetActive(showCinematicCamera);
            if (showCinematicCamera)
            {
                await cinematicCamera.Play();
            }

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

            if (entity != playerEntity)
            {
                return;
            }

            OnRestartLevelRequestedEvent?.Invoke();
        }

        public void Dispose()
        {
            Entity[] levelEntities = GetComponentsInChildren<Entity>();
            foreach (Entity entity in levelEntities)
            {
                entity.OnEntityDiedEvent -= OnEntityDied;
                entity.Activate();
            }
        }
    }
}
