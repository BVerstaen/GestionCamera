using UnityEngine;

public class Curve
{
    public Vector3 A;
    public Vector3 B;
    public Vector3 C;
    public Vector3 D;


    public Vector3 GetPosition(float t)
    {
        return MathUtils.CubicBezier(A, B, C, D, t);
    }

    public Vector3 GetPosition(float t, Matrix4x4 a_localToWorldMatrix)
    {
        return a_localToWorldMatrix.MultiplyPoint(GetPosition(t));
    }

    public void DrawGizmo(Color a_color, Matrix4x4 a_localToWorldMatrix)
    {
        
    }
}
