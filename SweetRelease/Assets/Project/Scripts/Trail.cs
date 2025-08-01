using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Trail
    {
        private const float timeBetweenPoints = 0.1f;

        private float timeBetweenPointsCounter = 0;
        private readonly Queue<Vector3> points = new();
        private readonly Entity owner;
        private readonly int length;
        public Trail(Entity owner, TrailConfig config)
        {
            this.owner = owner;
            length = config.TrailLength;
        }

        public void Clear()
        {
            points.Clear();
        }

        public void Update()
        {
            timeBetweenPointsCounter += Time.deltaTime;
            if (timeBetweenPointsCounter / timeBetweenPoints < 1)
            {
                return;
            }

            timeBetweenPointsCounter = 0;

            if (points.Count == length)
            {
                _ = points.Dequeue();
            }

            points.Enqueue(owner.Position);
        }

        public void OnDrawGizmos()
        {
            if (points.Count == 0) { return; }
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 currentPoint = points.ElementAt(i);
                Vector3 nextPoint = points.ElementAt(i + 1);
                Gizmos.DrawSphere(currentPoint, 0.2f);
                Gizmos.DrawLine(currentPoint, nextPoint);
            }

            Gizmos.DrawSphere(points.Last(), 0.2f);
        }

        public bool Overlaps(Entity entity, out Vector3 overlapPosition)
        {
            Vector3 entityPosition = entity.Position;
            entityPosition.y = 0;

            int length = entity == owner ? points.Count - 2 : points.Count - 1;
            for (int i = 0; i < length; i++)
            {
                Vector3 a = points.ElementAt(i);
                a.y = 0;
                Vector3 b = points.ElementAt(i + 1);
                b.y = 0;
                if (IsPointNearLineSegment(entityPosition, a, b, out overlapPosition))
                {
                    return true;
                }
            }

            overlapPosition = Vector3.zero;
            return false;
        }

        private bool IsPointNearLineSegment(Vector3 point, Vector3 a, Vector3 b, out Vector3 overlapPosition)
        {
            const float threshold = 0.1f; // Hardcoded collision radius

            Vector3 ab = b - a;
            Vector3 ap = point - a;

            float abLengthSq = ab.sqrMagnitude;

            // Handle degenerate segment (a == b)
            if (abLengthSq == 0f)
            {
                overlapPosition = a;
                return Vector3.Distance(point, a) <= threshold;
            }

            // Project point onto segment
            float t = Vector3.Dot(ap, ab) / abLengthSq;
            t = Mathf.Clamp01(t); // Clamp to segment

            // Compute closest point
            overlapPosition = a + (t * ab);

            // Check if the distance is within threshold
            float distanceSq = (point - overlapPosition).sqrMagnitude;
            return distanceSq <= threshold * threshold;
        }
    }
}
