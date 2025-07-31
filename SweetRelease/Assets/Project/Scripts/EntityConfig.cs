using UnityEngine;

namespace Assets.Project.Scripts
{
    [CreateAssetMenu(menuName = "SweetRelease/EntityConfig")]
    public class EntityConfig : ScriptableObject
    {
        public bool IsHeavy;
        public bool Glidable;
        public TrailConfig TrailConfig;
        public BoundsConfig BoundsConfig;
    }
}
