using UnityEngine;

namespace Assets.Project.Scripts
{
    [CreateAssetMenu(menuName = "SweetRelease/LevelSelectionConfig")]
    public class LevelSelectionConfig : ScriptableObject
    {
        public LevelConfig[] levelConfigs;
    }

    [CreateAssetMenu(menuName = "SweetRelease/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int Id;
        public string Name;
        public GameObject LevelPrefab;
    }
}
