using UnityEngine;

namespace Assets.Project.Scripts
{

    [CreateAssetMenu(menuName = "SweetRelease/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int Id;
        public string Name;
        public Level LevelPrefab;
    }
}
