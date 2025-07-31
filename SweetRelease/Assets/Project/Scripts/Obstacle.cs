using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Obstacle : Entity
    {
        public override void OnCollisionObstacle(Bounds myBounds, Entity other, Bounds otherBounds, Vector3 collisionPoint)
        {

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
