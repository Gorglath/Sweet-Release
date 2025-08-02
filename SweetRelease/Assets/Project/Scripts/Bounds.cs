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
            (Vector3 min, Vector3 max)? intersection = GetIntersection(other);
            if (!intersection.HasValue)
            {
                return Vector3.zero;
            }

            Vector3 minPoint = intersection.Value.min;
            Vector3 maxPoint = intersection.Value.max;

            Vector3 clamped = new(
                Mathf.Clamp(position.x, minPoint.x, maxPoint.x),
                Mathf.Clamp(position.y, minPoint.y, maxPoint.y),
                Mathf.Clamp(position.z, minPoint.z, maxPoint.z)
            );

            return position != clamped ? clamped : GetClosestPointOnSurface(position, other.min, other.max);
        }

        public Vector3 GetClosestPointOnSurface(Vector3 point, Vector3 boxMin, Vector3 boxMax)
        {
            float distMinX = Mathf.Abs(point.x - boxMin.x);
            float distMaxX = Mathf.Abs(boxMax.x - point.x);
            float distMinY = Mathf.Abs(point.y - boxMin.y);
            float distMaxY = Mathf.Abs(boxMax.y - point.y);
            float distMinZ = Mathf.Abs(point.z - boxMin.z);
            float distMaxZ = Mathf.Abs(boxMax.z - point.z);

            float minDist = distMinX;
            int axis = 0;

            if (distMaxX < minDist)
            {
                minDist = distMaxX;
                axis = 1;
            }
            if (distMinY < minDist)
            {
                minDist = distMinY;
                axis = 2;
            }
            if (distMaxY < minDist)
            {
                minDist = distMaxY;
                axis = 3;
            }
            if (distMinZ < minDist)
            {
                minDist = distMinZ;
                axis = 4;
            }
            if (distMaxZ < minDist)
            {
                axis = 5;
            }

            Vector3 projected = point;

            switch (axis)
            {
                case 0: projected.x = boxMin.x; break;
                case 1: projected.x = boxMax.x; break;
                case 2: projected.y = boxMin.y; break;
                case 3: projected.y = boxMax.y; break;
                case 4: projected.z = boxMin.z; break;
                case 5: projected.z = boxMax.z; break;
            }

            return projected;
        }

        public (Vector3 min, Vector3 max)? GetIntersection(Bounds other)
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

        public bool IsTopSliceIntersecting(Bounds other, float topPercentage = 0.75f)
        {
            float myMaxY = max.y;
            float myHeight = extents.y;

            // Define the top slice range of "my" bounds
            float topSliceMinY = myMaxY - (myHeight * topPercentage);
            float topSliceMaxY = myMaxY;

            float otherMinY = other.min.y;
            float otherMaxY = other.max.y;

            // Check if ANY part of other intersects the top slice
            bool intersectsTopSlice = otherMaxY > topSliceMinY && otherMinY < topSliceMaxY;

            return intersectsTopSlice;
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
