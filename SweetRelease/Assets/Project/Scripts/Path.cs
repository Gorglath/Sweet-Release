using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Path : MonoBehaviour
    {
        private Vector3[] m_cachedPathPoints;

        public int Length => PathPoints != null ? PathPoints.Length : 0;

        public Vector3[] PathPoints => m_cachedPathPoints;
        public float WorldLegth { get; private set; }

        private void Awake()
        {
            m_cachedPathPoints = new Vector3[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                m_cachedPathPoints[i] = transform.GetChild(i).position;
            }

            for (int i = 0; i < PathPoints.Length - 1; i++)
            {
                WorldLegth += Vector3.Distance(PathPoints[i], PathPoints[i + 1]);
            }
        }

        public Vector3 GetPositionOnPath(float position01)
        {
            if (PathPoints == null || PathPoints.Length < 2)
            {
                return Vector3.zero;
            }

            position01 = Mathf.Clamp01(position01);

            float[] segmentLengths = new float[PathPoints.Length - 1];
            float totalLength = 0f;

            for (int i = 0; i < PathPoints.Length - 1; i++)
            {
                float segmentLength = Vector3.Distance(PathPoints[i], PathPoints[i + 1]);
                segmentLengths[i] = segmentLength;
                totalLength += segmentLength;
            }

            float targetDistance = totalLength * position01;

            float accumulated = 0f;

            for (int i = 0; i < segmentLengths.Length; i++)
            {
                float segLen = segmentLengths[i];

                if (accumulated + segLen >= targetDistance)
                {
                    float remaining = targetDistance - accumulated;
                    float t = segLen > 0 ? remaining / segLen : 0f;

                    return Vector3.Lerp(PathPoints[i], PathPoints[i + 1], t);
                }

                accumulated += segLen;
            }

            // If we reach here, return last point
            return PathPoints[^1];
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (transform.childCount == 0) { return; }
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                Vector3 currentPoint = transform.GetChild(i).position;
                Vector3 nextPoint = transform.GetChild(i + 1).position;
                Gizmos.DrawSphere(currentPoint, 0.2f);
                Gizmos.DrawLine(currentPoint, nextPoint);
            }

            Gizmos.DrawSphere(transform.GetChild(transform.childCount - 1).position, 0.2f);
        }
    }
}
