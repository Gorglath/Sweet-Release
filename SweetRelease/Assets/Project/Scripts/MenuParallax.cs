using UnityEngine;

namespace Assets.Project.Scripts
{
    public class MenuParallax : MonoBehaviour
    {
        [SerializeField]
        private float offsetMultiplier = 1f;

        [SerializeField]
        private float smoothTime = .3f;

        private Vector2 startPosition;
        private Transform m_cachedTransform;
        private Vector3 velocity;

        private void Start()
        {
            startPosition = transform.position;
            m_cachedTransform = transform;
        }

        private void Update()
        {
            Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            m_cachedTransform.position = Vector3.SmoothDamp(m_cachedTransform.position, startPosition + (offset * offsetMultiplier), ref velocity, smoothTime);
        }
    }
}
