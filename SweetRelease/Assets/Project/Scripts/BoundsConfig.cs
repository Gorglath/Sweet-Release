using UnityEngine;

namespace Assets.Project.Scripts
{
    [CreateAssetMenu(menuName = "SweetRelease/BoundsConfig")]
    public class BoundsConfig : ScriptableObject
    {
        public Vector3 Size;
        public Vector3 Offset;
    }
}
