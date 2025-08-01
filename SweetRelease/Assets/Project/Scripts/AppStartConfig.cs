using UnityEngine;

namespace Assets.Project.Scripts
{
    public enum AppStateType
    {
        NONE = 0,
        MAINMENU = 1,
        LEVELSELECTION = 2,
        GAMEPLAY = 3
    }

    [CreateAssetMenu(menuName = "SweetRelease/AppStartupConfig")]
    public class AppStartConfig : ScriptableObject
    {
        public AppStateType InitialState;
    }
}
