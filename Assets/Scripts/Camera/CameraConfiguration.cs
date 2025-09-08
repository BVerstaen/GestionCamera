using UnityEngine;

[System.Serializable]
public struct CameraConfiguration
{
    public float yaw;
    public float pitch;
    public float roll;
    public Vector3 pivot;
    public float distance;
    public float fieldOfView;

    public Quaternion GetRotation() => Quaternion.Euler(pitch, yaw, roll);

    public Vector3 GetPosition()
    {
        Vector3 offset = GetRotation() * (Vector3.back * distance);
        return pivot + offset;
    }

    public void OnClampPitch() => pitch = Mathf.Clamp(pitch, -90f, 90f);

    public void DrawGizmos(Color a_color)
    {
        Gizmos.color = a_color;
        Gizmos.DrawSphere(pivot, 0.25f);
        Vector3 position = GetPosition();
        Gizmos.DrawLine(pivot, position);
        Gizmos.matrix = Matrix4x4.TRS(position, GetRotation(), Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, fieldOfView, 0.5f, 0f, Camera.main.aspect);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
    