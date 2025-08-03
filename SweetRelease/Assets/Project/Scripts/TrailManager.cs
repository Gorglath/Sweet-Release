using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Project.Scripts
{
    public class TrailManager : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRendererTemplate;

        [SerializeField]
        private GameObject collisionEffectPrefab;

        public readonly Dictionary<Entity, Trail> entityTrails = new();
        public readonly List<Entity> pendingRemovalEntities = new();

        private bool isActive = true;

        public void RegisterEntity(Entity entity)
        {
            if (entityTrails.ContainsKey(entity))
            {
                if (pendingRemovalEntities.Contains(entity))
                {
                    pendingRemovalEntities.Remove(entity);
                }
                return;
            }

            Trail trail = new(entity, entity.Config.TrailConfig, lineRendererTemplate);
            entityTrails.Add(entity, trail);
        }

        public void UnregisterEntity(Entity entity)
        {
            if (!entityTrails.ContainsKey(entity))
            {
                return;
            }

            if (!pendingRemovalEntities.Contains(entity))
            {
                pendingRemovalEntities.Add(entity);
            }
        }

        public void Update()
        {
            if (!isActive)
            {
                return;
            }

            foreach (Trail trail in entityTrails.Values)
            {
                trail.Update();
            }

            foreach (Entity entity in entityTrails.Keys)
            {
                if (!entity.IsAlive)
                {
                    continue;
                }

                Trail entityTrail = entityTrails[entity];
                foreach (Trail trail in entityTrails.Values)
                {
                    if (entityTrail == trail && entity.Config.IgnoreOwnTrail)
                    {
                        continue;
                    }

                    if (!trail.Overlaps(entity, out Vector3 overlapPosition))
                    {
                        continue;
                    }

                    trail.Clear();
                    entity.OnCollisionTrail(overlapPosition);

                    var trailEffect = Instantiate(collisionEffectPrefab, entity.TrailAnchor.position, Quaternion.identity);
                    Destroy(trailEffect.gameObject, 5.0f);
                    break;
                }
            }

            foreach (Entity pendingEntity in pendingRemovalEntities)
            {
                entityTrails[pendingEntity].Dispose();
                _ = entityTrails.Remove(pendingEntity);
            }

            pendingRemovalEntities.Clear();
        }

        public void Clear()
        {
            isActive = false;
            ClearNextFrame().Forget();
        }

        private async UniTask ClearNextFrame()
        {
            await UniTask.NextFrame();
            foreach (Trail trail in entityTrails.Values) { trail.Dispose(); }
        }
    }
}
