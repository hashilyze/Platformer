using UnityEngine;

namespace UPhys2D
{
    public class CollisionHandler2D : MonoBehaviour
    {
        public void Setup (BoxCollider2D collision)
        {
            _collision = collision;
        }

        public int Sweep (Vector2 pos, Vector2 dir, float dist, RaycastHit2D[] results, out RaycastHit2D closestHit, int layerMask = -1, System.Func<RaycastHit2D, bool> filter = null)
        {
            int ret = 0;
            closestHit = default;
            float closestDist = float.PositiveInfinity;

            int count = Physics2D.BoxCastNonAlloc(pos, _collision.bounds.size, 0.0f, dir, results, dist, layerMask);
            for (int cur = 0; cur < count; ++cur)
            {
                ref RaycastHit2D hit = ref results[cur];
                if (filter != null && !filter(hit))
                {
                    continue;
                }
                // Recalculate overlaped collider's hitInfo because it has reversed normal
                if (hit.distance <= 0.0f)
                {
                    UPhysUtility2D.GetPosAndRot(hit.collider, out Vector2 hitPos, out float hitRot);
                    if (UPhysUtility2D.ComputePenetration(_collision, pos, 0.0f,
                        hit.collider, hitPos, hitRot,
                        out Vector2 direction, out float _s))
                    {
                        hit.normal = direction;
                    }
                }
                // Ignore reversed normal 
                if (Vector3.Dot(dir, hit.normal) >= 0.0f)
                {
                    continue;
                }
                // Update to closer hitInfo
                if (hit.distance < closestDist)
                {
                    closestHit = hit;
                    closestDist = hit.distance;
                }
                results[ret++] = hit;
            }
            return ret;
        }

        public int Overlap (Vector2 pos, Collider2D[] results, int layerMask = -1, System.Func<Collider2D, bool> filter = null)
        {
            int count = Physics2D.OverlapBoxNonAlloc(pos, _collision.bounds.size, 0.0f, results, layerMask);
            int ret = 0;
            for (int cur = 0; cur < count; ++cur)
            {
                if(filter != null && !filter(results[cur]))
                {
                    continue;
                }
                results[ret++] = results[cur];
            }
            return ret;
        }


        private BoxCollider2D _collision;
    }
}