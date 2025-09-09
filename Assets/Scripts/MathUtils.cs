using UnityEngine;

public class MathUtils : MonoBehaviour
{
    public static Vector3 GetNearestPointOnSegment(Vector3 a, Vector3 b, Vector3 target)
    {
        Vector3 n = (b - a).normalized;
        Vector3 AC = (target - a);
        float DotTarget = Vector3.Dot(n, AC);

        DotTarget = Mathf.Clamp(DotTarget, 0, Vector3.Distance(a,b));

        return a + n * DotTarget;
    }
}
