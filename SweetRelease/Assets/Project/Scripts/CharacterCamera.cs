using UnityEngine;

namespace Assets.Project.Scripts
{
    public class CharacterCamera : MonoBehaviour
    {
        [SerializeField]
        private Entity targetEntity;

        [SerializeField]
        public Vector2 screenEdgeThreshold = new(0.3f, 0.5f);

        [SerializeField]
        public float followSpeed = 3f;

        [SerializeField]
        private Camera cam;

        private void LateUpdate()
        {
            if (targetEntity == null)
            {
                return;
            }

            Vector3 viewportPos = cam.WorldToViewportPoint(targetEntity.Position);
            Vector3 cameraMove = Vector3.zero;

            // Check horizontal bounds
            if (viewportPos.x < screenEdgeThreshold.x)
            {
                cameraMove.x = viewportPos.x - screenEdgeThreshold.x;
            }
            else if (viewportPos.x > 1 - screenEdgeThreshold.x)
            {
                cameraMove.x = viewportPos.x - (1 - screenEdgeThreshold.x);
            }

            // Check vertical bounds
            if (viewportPos.y < screenEdgeThreshold.y)
            {
                cameraMove.z = viewportPos.y - screenEdgeThreshold.y;
            }
            else if (viewportPos.y > 1 - screenEdgeThreshold.y)
            {
                cameraMove.z = viewportPos.y - (1 - screenEdgeThreshold.y);
            }

            // Convert movement in viewport space to world space
            if (cameraMove != Vector3.zero)
            {
                Vector3 moveDirection = (cam.transform.right * cameraMove.x) + (cam.transform.forward * cameraMove.z);
                moveDirection.y = 0;

                transform.position += moveDirection * followSpeed * Time.deltaTime;
            }
        }
    }
}
