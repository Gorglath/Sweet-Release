using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts
{
    public class StarsBadge : MonoBehaviour
    {
        [SerializeField]
        private Image[] starImages;

        private int nextActiveStarImage;

        private void Awake()
        {
            foreach (Image image in starImages) { image.gameObject.SetActive(false); }
        }

        public void Init(int activeStars)
        {
            for (int i = 0; i < activeStars; i++)
            {
                starImages[i].gameObject.SetActive(true);
            }
            nextActiveStarImage = activeStars - 1;
        }

        public void AddStar()
        {
            starImages[nextActiveStarImage].gameObject.SetActive(true);
            nextActiveStarImage++;
        }
    }
}
