using UnityEngine;

namespace UPhys2D
{
    public static class UPhysUtility2D
    {
        public static Vector2 RotatePoint(Vector2 point, float angle)
        {
            float sin = Mathf.Sin(angle), cos = Mathf.Cos(angle);
            return new Vector2(point.x * cos - point.y * sin, point.y * cos + point.x * sin);
        }

        public static void GetPosAndRot(Collider2D col, out Vector2 pos, out float angle)
        {
            {
                Transform colTransform = col.transform;
                pos = colTransform.position;
                angle = colTransform.rotation.z;
                return;
            }
            //Rigidbody2D colRb = col.attachedRigidbody;
            //if (colRb == null)
            //{
            //    Transform transform = col.transform;
            //    pos = transform.position;
            //    rot = transform.rotation.z;
            //    return;
            //}

            //if (!colRb.isKinematic)
            //{
            //    pos = colRb.position;
            //    rot = colRb.rotation;
            //    return;
            //}

            //pos = colRb.position;
            //rot = colRb.rotation;
            //if(colRb.gameObject != col.gameObject)
            //{
            //    pos += RotatePoint(col.transform.localPosition, rot);
            //}
        }

        public static bool ComputePenetration(Collider2D colA, Vector2 posA, float angleA, Collider2D colB, Vector2 posB, float angleB, out Vector2 direction, out float distance)
        {
            Vector2 tmpPosA = colA.transform.position;
            Vector2 tmpPosB = colB.transform.position;
            float tmpAngleA = colA.transform.rotation.eulerAngles.z;
            float tmpAngleB = colB.transform.rotation.eulerAngles.z;

            colA.transform.position = posA;
            colB.transform.position = posB;
            colA.transform.rotation = Quaternion.Euler(.0f, .0f, angleA);
            colB.transform.rotation = Quaternion.Euler(.0f, .0f, angleB);

            var result = Physics2D.Distance(colA, colB);
            direction = (result.pointB - result.pointA).normalized;
            distance = (result.distance += Physics2D.defaultContactOffset * 2.0f) * -1.0f;

            colA.transform.position = tmpPosA;
            colB.transform.position = tmpPosB;
            colA.transform.rotation = Quaternion.Euler(.0f, .0f, tmpAngleA);
            colB.transform.rotation = Quaternion.Euler(.0f, .0f, tmpAngleB);

            return distance >= 0.0f;
        }
    }
}