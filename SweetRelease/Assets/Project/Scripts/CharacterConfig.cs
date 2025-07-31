using UnityEngine;

namespace Assets.Project.Scripts
{
    [CreateAssetMenu(menuName = "SweetRelease/CharacterConfig")]
    public class CharacterConfig : ScriptableObject
    {
        public float MovementSpeed;
        public float JumpMovementSpeed;
        public float RotationAngle;
        public float RotationSpeed;
        public float JumpHeight;
        public float FallJumpHeight;
        public float GravityScale;
    }
}
