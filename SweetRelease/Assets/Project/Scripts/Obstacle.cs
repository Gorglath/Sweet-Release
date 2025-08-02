using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Obstacle : Entity
    {
        [SerializeField]
        private bool killOnCollision = false;

        [SerializeField]
        private GameObject killEffectPrefab;

        public override void OnCollisionObstacle(Bounds myBounds, Entity other, Bounds otherBounds, Vector3 collisionPoint)
        {
            if (!killOnCollision)
            {
                return;
            }

            if (EntityState == EntityState.DEAD)
            {
                return;
            }

            var killEffect = Instantiate(killEffectPrefab, Position, Quaternion.identity);
            SetState(EntityState.DEAD);
            Kill();
        }

        public override void OnCollisionTrail(Vector3 overlapPosition)
        {

        }

        public override void OnEntityAirbound()
        {

        }

        public override void OnEntityDead()
        {

        }

        public override void OnEntityGliding()
        {

        }

        public override void OnEntityStatic()
        {

        }
    }
}
