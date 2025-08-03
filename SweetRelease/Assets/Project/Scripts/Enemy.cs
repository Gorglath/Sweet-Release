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

        public override void OnCreated()
        {
            m_position01 = path.GetClosestPositionIgnoringY(Position);
        }

        protected override void DoGlide()
        {
            float delta = GetNormalizedSpeed(config.MovementSpeed) * Time.deltaTime;
            m_position01 += delta;

            if (m_position01 > 1f)
            {
                m_position01 = 0f;
            }

            Vector3 positionOnPath = path.GetPositionOnPath(m_position01);

            _ = m_cachedTransform.forward.normalized;
            m_cachedTransform.localPosition = Vector3.Slerp(m_cachedTransform.localPosition, positionOnPath, Time.deltaTime * m_movementSmoothness);

            float lookAtPos01 = Mathf.Clamp01(m_position01 + 0.01f);
            Vector3 lookTarget = path.GetPositionOnPath(lookAtPos01);

            Vector3 directionToLook = (lookTarget - Position).normalized;
            directionToLook.y = 0f;

            HandleAnimator(directionToLook);
            if (directionToLook.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
                m_cachedTransform.rotation = Quaternion.Slerp(m_cachedTransform.rotation, targetRotation, Time.deltaTime * m_rotationSmoothness);
            }
        }

        private void HandleAnimator(Vector3 targetDirection)
        {
            // Calculate the cross product
            Vector3 cross = Vector3.Cross(m_cachedTransform.forward, targetDirection);

            // Check the sign of the dot product with the 'up' vector
            float direction = Vector3.Dot(cross, Vector3.up);

            if (direction > 0.2)
            {
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingLeft, false);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingRight, true);
            }
            else if (direction < -0.2)
            {
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingLeft, true);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingRight, false);
            }
            else
            {
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingLeft, false);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingRight, false);
            }
        }

        // Converts world speed to normalized path speed
        private float GetNormalizedSpeed(float worldSpeed)
        {
            return path.Length > 0f ? worldSpeed / path.WorldLegth : 0f;
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
                characterAnimator.SetTrigger(Constants.CharacterAnimationParameters.Dead_Fall);
            }

            base.OnCollisionObstacle(myBounds, other, otherBounds, collisionPoint);
        }
    }
}
