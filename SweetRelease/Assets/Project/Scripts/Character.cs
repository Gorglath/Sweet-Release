using System.Collections.Generic;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Character : Entity
    {
        private const float IsOnBoundCheckIntervals = 0.1f;

        [SerializeField]
        protected CharacterConfig config;

        [SerializeField]
        protected Animator characterAnimator;

        private float m_jumpVelocity;
        private float m_isOnBoundIntervalCounter;
        private int m_isOnBoundsFrames;

        public override void OnCreated()
        {
            string waveSelected = Random.Range(0, 2) == 1 ? Constants.CharacterAnimationParameters.Wave_1 : Constants.CharacterAnimationParameters.Wave_2;
            characterAnimator.SetTrigger(waveSelected);
        }

        public override void OnActivated()
        {
            characterAnimator.SetTrigger(Constants.CharacterAnimationParameters.Start);
            SetState(EntityState.GLIDING);
        }

        public override void OnDeactivated()
        {
            SetState(EntityState.STATIC);
        }

        public override void OnStateChanged(EntityState newState, EntityState previousState)
        {
            switch (newState)
            {
                case EntityState.GLIDING:
                    if (previousState == EntityState.AIRBOUND)
                    {
                        characterAnimator.SetTrigger(Constants.CharacterAnimationParameters.Land_Good);
                        SFXManager.instance.PlaySFX(Constants.SFXIds.Land);
                    }
                    trailManager.RegisterEntity(this);
                    break;
                case EntityState.AIRBOUND:
                    characterAnimator.SetTrigger(Constants.CharacterAnimationParameters.Jump);
                    trailManager.UnregisterEntity(this);
                    break;
                case EntityState.DEAD:
                    trailManager.UnregisterEntity(this);
                    SFXManager.instance.PlaySFX(Constants.SFXIds.Death);
                    break;
                case EntityState.CELEBRATE:
                    characterAnimator.SetTrigger(Constants.CharacterAnimationParameters.Win);
                    break;
                case EntityState.STATIC:
                case EntityState.NONE:
                    break;
            }
        }
        public override void OnCollisionObstacle(Bounds myBounds, Entity other, Bounds otherBounds, Vector3 collisionPoint)
        {
            if (EntityState is EntityState.DEAD or EntityState.STATIC or EntityState.CELEBRATE)
            {
                return;
            }

            if (other.Config.IsCollectable)
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
            if (!myBounds.IsLowerSliceIntersecting(otherBounds) && !myBounds.IsTopSliceIntersecting(otherBounds))
            {
                return;
            }

            bool isOnBounds = entityCollisionManager.IsOnBounds(this);
            string deathAnimation =
                isOnBounds ?
                (other.Config.IsHeavy ? Constants.CharacterAnimationParameters.Dead_Wall : Constants.CharacterAnimationParameters.Dead_Collision)
                : (other.Config.IsHeavy ? Constants.CharacterAnimationParameters.Dead_Wall : Constants.CharacterAnimationParameters.Dead_Fall);
            characterAnimator.SetTrigger(deathAnimation);
            // We ran into a wall or something, launch it if it isn't heavy.
            SetState(EntityState.DEAD);
        }

        public override void OnCollisionTrail(Vector3 overlapPosition)
        {
            if (EntityState == EntityState.GLIDING)
            {
                m_jumpVelocity = Mathf.Sqrt(config.JumpHeight * -2 * Physics.gravity.y * config.GravityScale);
                SetState(EntityState.AIRBOUND);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingLeft, false);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingRight, false);
                SFXManager.instance.PlaySFX(Constants.SFXIds.Jump);
                return;
            }
        }

        public override void OnEntityStatic()
        {
            // Do nothing.
        }

        public override void OnEntityGliding()
        {
            DoGlide();
        }

        protected virtual void DoGlide()
        {
            // Check if floor is valid.
            if (!IsOnBounds())
            {
                m_isOnBoundsFrames++;
                if(m_isOnBoundsFrames < 3)
                {
                    return;
                }

                m_isOnBoundsFrames = 0;
                m_jumpVelocity = Mathf.Sqrt(config.FallJumpHeight * -2 * Physics.gravity.y * config.GravityScale);
                SetState(EntityState.AIRBOUND);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingLeft, false);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingRight, false);
                SFXManager.instance.PlaySFX(Constants.SFXIds.Jump);
                return;
            }

            // Apply rotation
            bool isMovingLeft = Input.GetKey(KeyCode.LeftArrow);
            bool isMovingRight = Input.GetKey(KeyCode.RightArrow);
            int rotationDirection = 0;
            if (isMovingLeft)
            {
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingLeft, true);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingRight, false);
                rotationDirection = -1;
            }
            else if (isMovingRight)
            {
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingLeft, false);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingRight, true);
                rotationDirection = 1;
            }
            else
            {
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingLeft, false);
                characterAnimator.SetBool(Constants.CharacterAnimationParameters.SlidingRight, false);
            }

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
