using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class TrailManager : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRendererTemplate;

        public readonly Dictionary<Entity, Trail> entityTrails = new();
        public readonly List<Entity> pendingRemovalEntities = new();

        public void RegisterEntity(Entity entity)
        {
            if (entityTrails.ContainsKey(entity))
            {
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
            foreach (Trail trail in entityTrails.Values)
            {
                trail.Update();
            }

            foreach (Entity entity in entityTrails.Keys)
            {
                foreach (Trail trail in entityTrails.Values)
                {
                    if (!trail.Overlaps(entity, out Vector3 overlapPosition))
                    {
                        continue;
                    }

                    trail.Clear();
                    entity.OnCollisionTrail(overlapPosition);
                    break;
                }
            }

            foreach (Entity pendingEntity in pendingRemovalEntities)
            {
                entityTrails[pendingEntity].Clear();
                _ = entityTrails.Remove(pendingEntity);
            }

            pendingRemovalEntities.Clear();
        }

        public void Clear()
        {
            foreach (Trail trail in entityTrails.Values) { trail.Clear(); }
        }
    }
}
