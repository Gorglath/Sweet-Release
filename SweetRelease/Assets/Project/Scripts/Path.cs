using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Path : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;
        
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

            lineRenderer.positionCount = m_cachedPathPoints.Length;
            lineRenderer.SetPositions(m_cachedPathPoints);
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
        public float GetClosestPositionIgnoringY(Vector3 referencePosition)
        {
            if (m_cachedPathPoints == null || m_cachedPathPoints.Length < 2)
                return 0f;

            Vector2 posXZ = new Vector2(referencePosition.x, referencePosition.z);

            float totalLength = 0f;
            float closestPathDistance = 0f;
            float shortestSqrDistance = float.MaxValue;

            float accumulatedLength = 0f;

            for (int i = 0; i < m_cachedPathPoints.Length - 1; i++)
            {
                Vector3 a = m_cachedPathPoints[i];
                Vector3 b = m_cachedPathPoints[i + 1];

                Vector2 aXZ = new Vector2(a.x, a.z);
                Vector2 bXZ = new Vector2(b.x, b.z);
                Vector2 segment = bXZ - aXZ;

                float segmentLength = segment.magnitude;

                // Skip degenerate segments
                if (segmentLength < 0.001f)
                    continue;

                Vector2 ap = posXZ - aXZ;
                float t = Mathf.Clamp01(Vector2.Dot(ap, segment) / (segmentLength * segmentLength));

                Vector2 closestPointXZ = aXZ + t * segment;
                float sqrDistance = (posXZ - closestPointXZ).sqrMagnitude;

                if (sqrDistance < shortestSqrDistance)
                {
                    shortestSqrDistance = sqrDistance;
                    closestPathDistance = accumulatedLength + t * segmentLength;
                }

                accumulatedLength += segmentLength;
            }

            totalLength = accumulatedLength;

            return totalLength > 0f ? closestPathDistance / totalLength : 0f;
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
