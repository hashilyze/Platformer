using UnityEngine;

namespace UPhys2D
{
    public static class UPhysUtility2D
    {
        /// <summary>Surface tanget for direction</summary>
        public static Vector2 GetTangent(Vector2 direction, Vector2 surfaceNormal)
        {
            Vector3 rightVector = Vector3.Cross(direction, surfaceNormal);
            return Vector3.Cross(surfaceNormal, rightVector).normalized;
        }

        public static Vector2 GetPointVelocity (Vector2 linearVelocity, float angularVelocity, Vector2 point)
        {
            return linearVelocity + (Vector2)Vector3.Cross(new Vector3(0.0f, 0.0f, angularVelocity * Mathf.Deg2Rad), point);
        }

        public static Vector2 RotatePoint(Vector2 point, float angle)
        {
            float sin = Mathf.Sin(angle), cos = Mathf.Cos(angle);
            return new Vector2(point.x * cos - point.y * sin, point.y * cos + point.x * sin);
        }

        public static void GetPosAndRot(Collider2D col, out Vector2 pos, out float angle)
        {
            Transform colTransform = col.transform;
            pos = colTransform.position;
            angle = colTransform.rotation.z;
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