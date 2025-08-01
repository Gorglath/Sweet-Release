using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Character : Entity
    {
        private const float IsOnBoundCheckIntervals = 0.1f;

        [SerializeField]
        protected CharacterConfig config;

        private float m_jumpVelocity;
        private float m_isOnBoundIntervalCounter;

        public override void OnCreated()
        {

        }

        public override void OnActivated()
        {
            SetState(EntityState.GLIDING);
        }

        public override void OnDeactivated()
        {
            SetState(EntityState.STATIC);
        }

        public override void OnStateChanged(EntityState newState)
        {
            switch (newState)
            {
                case EntityState.GLIDING:
                    trailManager.RegisterEntity(this);
                    break;
                case EntityState.AIRBOUND:
                case EntityState.STATIC:
                case EntityState.DEAD:
                    trailManager.UnregisterEntity(this);
                    break;
                case EntityState.NONE:
                    break;
            }
        }
        public override void OnCollisionObstacle(Bounds myBounds, Entity other, Bounds otherBounds, Vector3 collisionPoint)
        {
            if (EntityState is EntityState.DEAD or EntityState.STATIC)
            {
                return;
            }

            // If we landed on a glidable position, snap and glide on it.
            Vector3 normal = otherBounds.GetSurfaceNormalAtPoint(collisionPoint);
            if (other.Config.Glidable && Vector3.Dot(normal, Vector3.up) > 0.9f)
            {
                // Snap to the collision point and start gliding.
                m_cachedTransform.position = new Vector3(m_cachedTransform.position.x, collisionPoint.y, m_cachedTransform.position.z);
                SetState(EntityState.GLIDING);
                return;
            }

            //Ignore if we're moving away from the collision. 
            if (Vector3.Dot(normal, m_cachedTransform.forward) > 0)
            {
                return;
            }

            // Ignore if the other bounds is lower then us.
            if (!myBounds.IsLowerSliceIntersecting(otherBounds))
            {
                return;
            }
            // We ran into a wall or something, launch it if it isn't heavy.
            SetState(EntityState.DEAD);
        }

        public override void OnCollisionTrail(Vector3 overlapPosition)
        {
            if (EntityState == EntityState.GLIDING)
            {
                m_jumpVelocity = Mathf.Sqrt(config.JumpHeight * -2 * Physics.gravity.y * config.GravityScale);
                SetState(EntityState.AIRBOUND);
                return;
            }
        }

        public override void OnEntityStatic()
        {
            // Do nothing.
        }

        public override void OnEntityGliding()
        {
            // Check if floor is valid.
            if (!IsOnBounds())
            {
                m_jumpVelocity = Mathf.Sqrt(config.FallJumpHeight * -2 * Physics.gravity.y * config.GravityScale);
                SetState(EntityState.AIRBOUND);
                return;
            }

            DoGlide();
        }

        protected virtual void DoGlide()
        {
            // Apply rotation
            int rotationDirection = Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
            rotationDirection = Input.GetKey(KeyCode.RightArrow) ? 1 : rotationDirection;

            if (rotationDirection != 0)
            {
                float rotationAngle = rotationDirection * config.RotationAngle;
                Quaternion rotationStep = Quaternion.Euler(0, rotationAngle * config.RotationSpeed * Time.deltaTime, 0);
                m_cachedTransform.localRotation *= rotationStep;
            }

            // Add the normal movement.
            Vector3 moveDirection = m_cachedTransform.forward.normalized;
            m_cachedTransform.localPosition += moveDirection * config.MovementSpeed * Time.deltaTime;
        }

        private bool IsOnBounds()
        {
            m_isOnBoundIntervalCounter += Time.deltaTime;
            if (m_isOnBoundIntervalCounter / IsOnBoundCheckIntervals < 1)
            {
                return true;
            }

            m_isOnBoundIntervalCounter = 0;
            return entityCollisionManager.IsOnBounds(this);
        }

        public override void OnEntityAirbound()
        {
            // Add gravity
            m_jumpVelocity += Physics.gravity.y * config.GravityScale * Time.deltaTime;
            m_cachedTransform.localPosition += Vector3.up * m_jumpVelocity * Time.deltaTime;

            // Add the normal movement.
            Vector3 moveDirection = m_cachedTransform.forward.normalized;
            m_cachedTransform.localPosition += moveDirection * config.JumpMovementSpeed * Time.deltaTime;
        }

        public override void OnEntityDead()
        {
            // Spin right round.
        }
    }
}
