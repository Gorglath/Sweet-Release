using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Enemy : Character
    {
        [SerializeField]
        private float m_rotationSmoothness;

        [SerializeField]
        private float m_movementSmoothness;

        [SerializeField]
        private Path path;

        private float m_position01 = 0f;
        private bool m_pendingDeath;

        protected override void DoGlide()
        {
            // Move along path based on actual path length
            float delta = GetNormalizedSpeed(config.MovementSpeed) * Time.deltaTime;
            m_position01 += delta;

            if (m_position01 > 1f)
            {
                m_position01 = 0f;
            }

            Vector3 positionOnPath = path.GetPositionOnPath(m_position01);

            // Add the normal movement.
            _ = m_cachedTransform.forward.normalized;
            m_cachedTransform.localPosition = Vector3.Slerp(m_cachedTransform.localPosition, positionOnPath, Time.deltaTime * m_movementSmoothness);

            float lookAtPos01 = Mathf.Clamp01(m_position01 + 0.01f);
            Vector3 lookTarget = path.GetPositionOnPath(lookAtPos01);

            Vector3 directionToLook = (lookTarget - Position).normalized;
            directionToLook.y = 0f; // Lock to horizontal plane

            if (directionToLook.sqrMagnitude > 0.001f) // Avoid zero direction
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_rotationSmoothness);
            }
        }

        // Converts world speed to normalized path speed
        private float GetNormalizedSpeed(float worldSpeed)
        {
            float totalLength = 0f;
            for (int i = 0; i < path.Length - 1; i++)
            {
                totalLength += Vector3.Distance(path.PathPoints[i], path.PathPoints[i + 1]);
            }

            return totalLength > 0f ? worldSpeed / totalLength : 0f;
        }

        public override void OnStateChanged(EntityState newState, EntityState previousState)
        {
            base.OnStateChanged(newState, previousState);
            // Once airborne enemy dies.
            if (newState == EntityState.AIRBOUND)
            {
                m_pendingDeath = true;
            }
        }

        public override void OnCollisionObstacle(Bounds myBounds, Entity other, Bounds otherBounds, Vector3 collisionPoint)
        {
            if (m_pendingDeath)
            {
                m_pendingDeath = false;
                SetState(EntityState.DEAD);
            }

            base.OnCollisionObstacle(myBounds, other, otherBounds, collisionPoint);
        }
    }
}
