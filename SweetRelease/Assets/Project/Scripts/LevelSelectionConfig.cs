using UnityEngine;

namespace Assets.Project.Scripts
{
    [CreateAssetMenu(menuName = "SweetRelease/LevelSelectionConfig")]
    public class LevelSelectionConfig : ScriptableObject
    {
        public LevelConfig[] levelConfigs;
    }
}
