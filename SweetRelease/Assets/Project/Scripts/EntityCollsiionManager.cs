using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class EntityCollsiionManager : MonoBehaviour
    {
        public readonly List<Entity> pendingRemovalEntities = new();
        public readonly Dictionary<Entity, Bounds> registeredEntities = new();

        private bool isActive = true;
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
            if (!isActive)
            {
                return;
            }

            foreach (KeyValuePair<Entity, Bounds> checkedEntityPair in registeredEntities)
            {
                if (!checkedEntityPair.Key.IsAlive)
                {
                    continue;
                }

                Bounds entity1Bounds = checkedEntityPair.Value;
                foreach (KeyValuePair<Entity, Bounds> targetEntityPair in registeredEntities)
                {
                    if (!targetEntityPair.Key.IsAlive)
                    {
                        continue;
                    }

                    Bounds entity2Bounds = targetEntityPair.Value;
                    if (!entity2Bounds.Intersects(checkedEntityPair.Key, checkedEntityPair.Value))
                    {
                        continue;
                    }
                    (Vector3 min, Vector3 max)? intersection = entity2Bounds.GetIntersection(entity1Bounds);
                    Vector3 referencePoint = intersection.HasValue ? (intersection.Value.min + intersection.Value.max) * 0.5f : checkedEntityPair.Key.Position;
                    Vector3 closestCollisionPoint = entity2Bounds.GetClosestIntersectionPoint(entity1Bounds, referencePoint);
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
            foreach (KeyValuePair<Entity, Bounds> checkedPair in registeredEntities)
            {
                if (checkedPair.Key.IsAlive)
                {
                    checkedPair.Value.OnDrawGizmos();
                }
            }
        }

        public void Clear()
        {
            isActive = false;
            ClearNextFrame().Forget();
        }

        private async UniTask ClearNextFrame()
        {
            await UniTask.NextFrame();
            registeredEntities.Clear();
        }
    }
}
