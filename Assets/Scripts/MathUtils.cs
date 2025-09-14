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

    public static Vector3 LinearBezier(Vector3 a, Vector3 b, float t)
    {
        return (1 - t) * a + t * b;
    }
    public static Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return (1 - t) * LinearBezier(a, b, t) + t * LinearBezier(b, c, t);
    }
    public static Vector3 CubicBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        return (1 - t) * QuadraticBezier(a, b, c, t) + t * QuadraticBezier(b, c, d, t);
    }
}
