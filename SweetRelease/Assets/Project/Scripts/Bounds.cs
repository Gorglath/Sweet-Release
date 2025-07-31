using UnityEngine;

namespace Assets.Project.Scripts
{
    public class Bounds
    {
        private Vector3 offset;
        private Vector3 extents;
        private readonly Entity owner;

        public Vector3 min => Center - extents;
        public Vector3 max => Center + extents;
        private Vector3 Center => owner.Position + offset;
        public Bounds(Entity owner, BoundsConfig config)
        {
            this.owner = owner;
            extents = config.Size * 0.5f;
            offset = config.Offset;
        }

        public bool Intersects(Entity other, Bounds otherBounds)
        {
            return other != owner && min.x <= otherBounds.max.x && max.x >= otherBounds.min.x &&
            min.y <= otherBounds.max.y && max.y >= otherBounds.min.y &&
            min.z <= otherBounds.max.z && max.z >= otherBounds.min.z;
        }

        public Vector3 GetClosestIntersectionPoint(Bounds other, Vector3 position)
        {
            Vector3 closestPoint = Vector3.zero;
            (Vector3 min, Vector3 max)? intersection = GetIntersection(other);
            if (intersection.HasValue)
            {
                Vector3 intersectionMin = intersection.Value.min;
                Vector3 intersectionMax = intersection.Value.max;

                closestPoint = new Vector3(
                    Mathf.Clamp(position.x, intersectionMin.x, intersectionMax.x),
                    Mathf.Clamp(position.y, intersectionMin.y, intersectionMax.y),
                    Mathf.Clamp(position.z, intersectionMin.z, intersectionMax.z)
                );
            }

            return closestPoint;
        }

        private (Vector3 min, Vector3 max)? GetIntersection(Bounds other)
        {
            if (!Intersects(other.owner, other))
            {
                return null;
            }

            Vector3 intersectionMin = Vector3.Max(min, other.min);
            Vector3 intersectionMax = Vector3.Min(max, other.max);

            return (intersectionMin, intersectionMax);
        }

        public Vector3[] GetCorners()
        {
            Vector3[] corners = new Vector3[8];
            Vector3 ex = new(extents.x, 0, 0);
            Vector3 ey = new(0, extents.y, 0);
            Vector3 ez = new(0, 0, extents.z);

            Vector3[] offsets = new Vector3[]
            {
            -ex - ey - ez,  ex - ey - ez,
            -ex + ey - ez,  ex + ey - ez,
            -ex - ey + ez,  ex - ey + ez,
            -ex + ey + ez,  ex + ey + ez
            };

            for (int i = 0; i < 8; i++)
            {
                corners[i] = Center + offsets[i];
            }

            return corners;
        }

        public Vector3 GetSurfaceNormalAtPoint(Vector3 point)
        {
            Vector3 localPoint = point - Center;

            float dx = Mathf.Abs(extents.x - Mathf.Abs(localPoint.x));
            float dy = Mathf.Abs(extents.y - Mathf.Abs(localPoint.y));
            float dz = Mathf.Abs(extents.z - Mathf.Abs(localPoint.z));

            // The smallest delta tells us which face the point is closest to
            return dy <= dx && dy <= dz
                ? Mathf.Sign(localPoint.y) * Vector3.up
                : dx <= dy && dx <= dz ? Mathf.Sign(localPoint.x) * Vector3.right : Mathf.Sign(localPoint.z) * Vector3.forward;
        }

        public bool IsLowerSliceIntersecting(Bounds other, float threshold = 0.15f)
        {
            float myMinY = min.y;
            float myMaxY = max.y;
            float otherMinY = other.min.y;
            float otherMaxY = other.max.y;

            // Calculate vertical intersection range
            float intersectionMinY = Mathf.Max(myMinY, otherMinY);
            float intersectionMaxY = Mathf.Min(myMaxY, otherMaxY);
            float intersectionHeight = Mathf.Max(0f, intersectionMaxY - intersectionMinY);

            // Your total height
            float myHeight = extents.y * 2f;

            // What percentage of your height is intersecting?
            float overlapPercentage = intersectionHeight / myHeight;

            // Optional: only count overlap that starts from the bottom
            bool startsFromBottom = intersectionMinY <= myMinY + Mathf.Epsilon;

            return startsFromBottom && overlapPercentage >= threshold;
        }

        public void OnDrawGizmos()
        {
            Vector3[] corners = GetCorners();
            Debug.DrawLine(corners[0], corners[1], Color.green);
            Debug.DrawLine(corners[0], corners[2], Color.green);
            Debug.DrawLine(corners[0], corners[4], Color.green);
            Debug.DrawLine(corners[1], corners[3], Color.green);
            Debug.DrawLine(corners[1], corners[5], Color.green);
            Debug.DrawLine(corners[2], corners[3], Color.green);
            Debug.DrawLine(corners[2], corners[6], Color.green);
            Debug.DrawLine(corners[3], corners[7], Color.green);
            Debug.DrawLine(corners[4], corners[5], Color.green);
            Debug.DrawLine(corners[4], corners[6], Color.green);
            Debug.DrawLine(corners[5], corners[7], Color.green);
            Debug.DrawLine(corners[6], corners[7], Color.green);
        }
    }
}
