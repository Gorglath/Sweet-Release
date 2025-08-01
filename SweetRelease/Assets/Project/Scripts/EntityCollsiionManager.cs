using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class EntityCollsiionManager : MonoBehaviour
    {
        public readonly List<Entity> pendingRemovalEntities = new();
        public readonly Dictionary<Entity, Bounds> registeredEntities = new();

        public void RegisterEntity(Entity entity)
        {
            if (!registeredEntities.ContainsKey(entity))
            {
                registeredEntities.Add(entity, new Bounds(entity, entity.Config.BoundsConfig));
            }
        }

        public void UnregisterEntity(Entity entity)
        {
            if (registeredEntities.ContainsKey(entity) && !pendingRemovalEntities.Contains(entity))
            {
                pendingRemovalEntities.Add(entity);
            }
        }

        private void Update()
        {
            foreach (KeyValuePair<Entity, Bounds> checkedEntityPair in registeredEntities)
            {
                Bounds entity1Bounds = checkedEntityPair.Value;
                foreach (KeyValuePair<Entity, Bounds> targetEntityPair in registeredEntities)
                {
                    Bounds entity2Bounds = targetEntityPair.Value;
                    if (!entity2Bounds.Intersects(checkedEntityPair.Key, checkedEntityPair.Value))
                    {
                        continue;
                    }

                    Vector3 closestCollisionPoint = entity2Bounds.GetClosestIntersectionPoint(entity1Bounds, checkedEntityPair.Key.Position);
                    checkedEntityPair.Key.OnCollisionObstacle(entity1Bounds, targetEntityPair.Key, entity2Bounds, closestCollisionPoint);
                }
            }

            foreach (Entity pendingEntities in pendingRemovalEntities)
            {
                _ = registeredEntities.Remove(pendingEntities);
            }

            pendingRemovalEntities.Clear();
        }

        public bool IsOnBounds(Entity entity)
        {
            Bounds entityBounds = registeredEntities[entity];
            foreach (KeyValuePair<Entity, Bounds> checkedEntityPair in registeredEntities)
            {
                if (entity == checkedEntityPair.Key || !checkedEntityPair.Key.Config.Glidable)
                {
                    continue;
                }

                if (!checkedEntityPair.Value.Intersects(entity, entityBounds))
                {
                    continue;
                }

                Vector3 closestPoint = checkedEntityPair.Value.GetClosestIntersectionPoint(entityBounds, entity.Position);
                Vector3 normal = checkedEntityPair.Value.GetSurfaceNormalAtPoint(closestPoint);
                if (Vector3.Dot(normal, Vector3.up) < 0.9f)
                {
                    continue;
                }

                return true;
            }

            return false;
        }
        private void OnDrawGizmos()
        {
            foreach (Bounds bounds in registeredEntities.Values)
            {
                bounds.OnDrawGizmos();
            }
        }

        public void Clear()
        {
            registeredEntities.Clear();
        }
    }
}
