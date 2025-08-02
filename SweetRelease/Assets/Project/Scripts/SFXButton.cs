using UnityEngine;

namespace Assets.Project.Scripts
{
    public class SFXButton : MonoBehaviour
    {
        public void OnHover()
        {
            SFXManager.instance.PlaySFX(Constants.SFXIds.ButtonHover);
        }

        public void OnClicked()
        {
            SFXManager.instance.PlaySFX(Constants.SFXIds.ButtonClick);
        }
    }
}
