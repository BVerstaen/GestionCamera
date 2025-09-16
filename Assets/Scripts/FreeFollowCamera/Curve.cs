using System;
using UnityEngine;

[Serializable]
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
        Gizmos.color = a_color;

        Gizmos.DrawSphere(a_localToWorldMatrix.MultiplyPoint(A), 0.1f);
        Gizmos.DrawSphere(a_localToWorldMatrix.MultiplyPoint(B), 0.1f);
        Gizmos.DrawSphere(a_localToWorldMatrix.MultiplyPoint(C), 0.1f);
        Gizmos.DrawSphere(a_localToWorldMatrix.MultiplyPoint(D), 0.1f);

        for (int i = 0; i < 30; i++)
        {
            Gizmos.DrawLine(GetPosition((i / 30f), a_localToWorldMatrix), GetPosition((i + 1) / 30f, a_localToWorldMatrix));
        }

        Gizmos.color = Color.Lerp(a_color, Color.black, 0.5f);

        Gizmos.DrawLine(a_localToWorldMatrix.MultiplyPoint(A), a_localToWorldMatrix.MultiplyPoint(B));
        Gizmos.DrawLine(a_localToWorldMatrix.MultiplyPoint(B), a_localToWorldMatrix.MultiplyPoint(C));
        Gizmos.DrawLine(a_localToWorldMatrix.MultiplyPoint(C), a_localToWorldMatrix.MultiplyPoint(D));
    }
}
